using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Helpers;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Services
{
    /// <summary>
    /// Service used to get and store a list of <see cref="CompetitionDistanceRule"> objects
    /// </summary>
    public class CompetitionDistanceRuleService : ObservableObject, ICompetitionDistanceRuleService
    {
        /// <summary>
        /// Event that is raised when the file operation progress changes
        /// </summary>
        public event ProgressDelegate OnFileProgress;

        /// <summary>
        /// Event that is raised when the file operation is finished.
        /// </summary>
        public event EventHandler OnFileFinished;

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Path to the competition distance rule file
        /// </summary>
        public string PersistentPath { get; set; }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// List with all <see cref="CompetitionDistanceRule"> objects
        /// </summary>
        private ObservableCollection<CompetitionDistanceRule> _competitionDistanceRules { get; set; }

        /// <summary>
        /// List with all <see cref="CompetitionDistanceRule"> objects at the time the <see cref="Load(string, CancellationToken)"/> method was called.
        /// </summary>
        private List<CompetitionDistanceRule> _competitionDistanceRulesOnLoad { get; set; }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private IFileService _fileService;
        private ICompetitionService _competitionService;

        /// <summary>
        /// Constructor
        /// </summary>
        public CompetitionDistanceRuleService(IFileService fileService)
        {
            _competitionDistanceRules = new ObservableCollection<CompetitionDistanceRule>();
            _competitionDistanceRules.CollectionChanged += _competitionDistanceRules_CollectionChanged;
            _fileService = fileService;
        }

        /// <inheritdoc/>
        public void SetCompetitionServiceObj(ICompetitionService competitionService)
        {
            _competitionService = competitionService;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Load a list of <see cref="CompetitionDistanceRule"> to the <see cref="_competitionDistanceRules"/>.
        /// This is using a separate Task because the file possibly can be large.
        /// If the file doesn't exist, the <see cref="_competitionDistanceRules"/> is cleared and the functions returns loading success.
        /// </summary>
        /// <param name="path">Path from where to load</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>true if importing succeeded; false if importing failed (e.g. canceled)</returns>
        public async Task<bool> Load(string path, CancellationToken cancellationToken)
        {
            bool importingResult = false;
            Exception exception = null;
            await Task.Run(() =>
            {
                try
                {
                    if (!File.Exists(path))
                    {
                        OnFileProgress?.Invoke(this, 0);
                        _competitionDistanceRules.Clear();
                        OnFileProgress?.Invoke(this, 100);
                    }
                    else
                    {
                        List<CompetitionDistanceRule> list = _fileService.LoadFromCsv<CompetitionDistanceRule>(path, cancellationToken, CompetitionDistanceRule.SetPropertyFromString, OnFileProgress, (header) =>
                        {
                            return PropertyNameLocalizedStringHelper.FindProperty(typeof(CompetitionDistanceRule), header);
                        });
                        _competitionDistanceRules = new ObservableCollection<CompetitionDistanceRule>();
                        _competitionDistanceRules.CollectionChanged += _competitionDistanceRules_CollectionChanged;
                        foreach (CompetitionDistanceRule rule in list)
                        {
                            AddDistanceRule(rule);
                        }
                    }

                    _competitionDistanceRulesOnLoad = _competitionDistanceRules.ToList().ConvertAll(cr => new CompetitionDistanceRule(cr));

                    PersistentPath = path;
                    importingResult = true;
                }
                catch (OperationCanceledException)
                {
                    importingResult = false;
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            });
            OnFileFinished?.Invoke(this, null);
            if (exception != null) { throw exception; }

            return importingResult;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Save the list of Competitions to a file
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="path">Path to which to save</param>
        /// <returns>true if saving succeeded; false if saving failed (e.g. canceled)</returns>
        public async Task<bool> Save(CancellationToken cancellationToken, string path = "")
        {
            if (string.IsNullOrEmpty(path)) { path = PersistentPath; }

            bool saveResult = false;
            Exception exception = null;
            await Task.Run(() =>
            {
                try
                {
                    _fileService.SaveToCsv(path, _competitionDistanceRules.ToList(), cancellationToken, OnFileProgress, (data, parentObject, currentProperty) =>
                    {
                        if (data is Enum dataEnum)
                        {
                            return EnumCoreLocalizedStringHelper.Convert(dataEnum);
                        }
                        else if(currentProperty.Name == nameof(CompetitionDistanceRule.SwimmingStyle) && data == null)
                        {
                            return Properties.Resources.AllString;
                        }
                        else
                        {
                            return data.ToString();
                        }
                    },
                    (header, type) =>
                    {
                        return PropertyNameLocalizedStringHelper.Convert(typeof(CompetitionDistanceRule), header);
                    });

                    _competitionDistanceRulesOnLoad = _competitionDistanceRules.ToList().ConvertAll(cr => new CompetitionDistanceRule(cr));
                    saveResult = true;
                }
                catch (OperationCanceledException)
                {
                    saveResult = false;
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            });
            OnFileFinished?.Invoke(this, null);
            if (exception != null) { throw exception; }
            return saveResult;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Check if the list of <see cref="CompetitionDistanceRule"/> has not saved changed.
        /// True, if unsaved changes exist; otherwise false.
        /// </summary>
        public bool HasUnsavedChanges => (_competitionDistanceRules != null && _competitionDistanceRulesOnLoad != null) ? !_competitionDistanceRules.SequenceEqual(_competitionDistanceRulesOnLoad) : false;

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <inheritdoc/>
        public ObservableCollection<CompetitionDistanceRule> GetCompetitionDistanceRules() => _competitionDistanceRules;

        /// <inheritdoc/>
        public void ClearAll()
        {
            if (_competitionDistanceRules == null)
            {
                _competitionDistanceRules = new ObservableCollection<CompetitionDistanceRule>();
                _competitionDistanceRules.CollectionChanged += _competitionDistanceRules_CollectionChanged;
            }
            foreach (CompetitionDistanceRule rule in _competitionDistanceRules)
            {
                rule.PropertyChanged -= DistanceRule_PropertyChanged;
            }
            _competitionDistanceRules.Clear();

            OnPropertyChanged(nameof(HasUnsavedChanges));
        }

        /// <inheritdoc/>
        public void ResetToLoadedState()
        {
            if (_competitionDistanceRulesOnLoad == null) { return; }
            ClearAll();
            foreach (CompetitionDistanceRule rule in _competitionDistanceRulesOnLoad)
            {
                AddDistanceRule(new CompetitionDistanceRule(rule));
            }
        }

        /// <inheritdoc/>
        public void AddDistanceRule(CompetitionDistanceRule distanceRule)
        {
            if (_competitionDistanceRules == null)
            {
                _competitionDistanceRules = new ObservableCollection<CompetitionDistanceRule>();
                _competitionDistanceRules.CollectionChanged += _competitionDistanceRules_CollectionChanged;
            }
            _competitionDistanceRules.Add(distanceRule);
            distanceRule.PropertyChanged += DistanceRule_PropertyChanged;

            OnPropertyChanged(nameof(HasUnsavedChanges));
        }

        /// <inheritdoc/>
        public void RemoveDistanceRule(CompetitionDistanceRule distanceRule)
        {
            distanceRule.PropertyChanged -= DistanceRule_PropertyChanged;
            _competitionDistanceRules?.Remove(distanceRule);

            OnPropertyChanged(nameof(HasUnsavedChanges));
        }

        private void DistanceRule_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _competitionService.UpdateAllCompetitionDistancesFromDistanceRules();
            OnPropertyChanged(nameof(HasUnsavedChanges));
        }

        private void _competitionDistanceRules_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _competitionService.UpdateAllCompetitionDistancesFromDistanceRules();
            OnPropertyChanged(nameof(HasUnsavedChanges));
        }

        /// <inheritdoc/>
        public ushort GetCompetitionDistanceFromRules(byte age, SwimmingStyles swimmingStyle)
        {
            CompetitionDistanceRule rule = _competitionDistanceRules.FirstOrDefault(r => age >= r.MinAge &&
                                                                                         age <= r.MaxAge &&
                                                                                         (r.SwimmingStyle == null || r.SwimmingStyle == swimmingStyle));
            if (rule == null)
            {
                return 0;   // No distance rule found
            }
            return rule.Distance;
        }
    }
}
