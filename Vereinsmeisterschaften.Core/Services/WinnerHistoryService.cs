using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Helpers;
using Vereinsmeisterschaften.Core.Models;
using Vereinsmeisterschaften.Core.Settings;

namespace Vereinsmeisterschaften.Core.Services
{
    /// <summary>
    /// Service used to keep the history of all winners
    /// </summary>
    public partial class WinnerHistoryService : ObservableObject, IWinnerHistoryService
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
        /// Path to the person file
        /// </summary>
        public string PersistentPath { get; set; }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// List with all previous <see cref="WinnerHistoryElement"/> objects
        /// </summary>
        private ObservableCollection<WinnerHistoryElement> _previousWinnersList { get; set; }

        /// <summary>
        /// List with all previous <see cref="WinnerHistoryElement"/> objects at the time the <see cref="Load(string, CancellationToken)"/> method was called.
        /// </summary>
        private List<WinnerHistoryElement> _previousWinnersListOnLoad { get; set; }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private IFileService _fileService;
        private IScoreService _scoreService;
        private IWorkspaceService _workspaceService;

        /// <summary>
        /// Constructor for the <see cref="WinnerHistoryService"/>
        /// </summary>      
        /// /// <param name="fileService"><see cref="IFileService"/> object</param>
        public WinnerHistoryService(IFileService fileService)
        {
            _fileService = fileService;
            _previousWinnersList = new ObservableCollection<WinnerHistoryElement>();
        }

        /// <summary>
        /// Save the reference to the <see cref="IScoreService"/> object.
        /// Dependency Injection in the constructor can't be used here because there would be a circular dependency.
        /// </summary>
        /// <param name="scoreService">Reference to the <see cref="IScoreService"/> implementation</param>
        public void SetScoreServiceObj(IScoreService scoreService)
        {
            _scoreService = scoreService;
        }

        /// <summary>
        /// Save the reference to the <see cref="IWorkspaceService"/> object.
        /// Dependency Injection in the constructor can't be used here because there would be a circular dependency.
        /// </summary>
        /// <param name="workspaceService">Reference to the <see cref="IWorkspaceService"/> implementation</param>
        public void SetWorkspaceServiceObj(IWorkspaceService workspaceService)
        {
            _workspaceService = workspaceService;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Load a list of <see cref="WinnerHistoryElement"> to the <see cref="_previousWinnersList"/>.
        /// This is using a separate Task because the file possibly can be large.
        /// If the file doesn't exist, the <see cref="_previousWinnersList"/> is cleared and the functions returns loading success.
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
                        ClearAll();
                        OnFileProgress?.Invoke(this, 100);
                    }
                    else
                    {
                        List<WinnerHistoryElement> list = _fileService.LoadFromCsv<WinnerHistoryElement>(path, cancellationToken, WinnerHistoryElement.SetPropertyFromString, out _, OnFileProgress, (header) =>
                        {
                            return PropertyNameLocalizedStringHelper.FindProperty(typeof(WinnerHistoryElement), header);
                        });
                        _previousWinnersList = new ObservableCollection<WinnerHistoryElement>();
                        foreach (WinnerHistoryElement previousWinner in list)
                        {
                            addPreviousWinnerInternal(previousWinner, false);
                        }
                    }

                    _previousWinnersListOnLoad = _previousWinnersList.ToList().ConvertAll(p => new WinnerHistoryElement(p));

                    sortPreviousWinners();

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
        /// Save the list of previous winners to a file
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
                    _fileService.SaveToCsv(path, _previousWinnersList.ToList(), cancellationToken, null, OnFileProgress, (data, parentObject, currentProperty) =>
                    {
                        if (data is IList dataList)
                        {
                            return string.Join(" | ", dataList.Cast<object>().Select(d => d.ToString()));
                        }
                        else if(data is DateTime dataDate)
                        {
                            return dataDate.ToShortDateString();
                        }
                        else
                        {
                            return data?.ToString();
                        }
                    },
                    (header, type) =>
                    {
                        return PropertyNameLocalizedStringHelper.Convert(typeof(WinnerHistoryElement), header);
                    });

                    _previousWinnersListOnLoad = _previousWinnersList.ToList().ConvertAll(p => new WinnerHistoryElement(p));
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

        [ObservableProperty]
        private WinnerHistoryElement _currentWinners;

        public int IndexOfCurrentWinnersInsidePreviousWinners => _previousWinnersList.IndexOf(CurrentWinners);

        public bool IsCurrentWinnersPartOfPreviousWinners => IndexOfCurrentWinnersInsidePreviousWinners != -1;

        /// <summary>
        /// Check if the list of previous <see cref="WinnerHistoryElement"/> has not saved changed.
        /// True, if unsaved changes exist; otherwise false.
        /// </summary>
        public bool HasUnsavedChanges => (_previousWinnersList != null && _previousWinnersListOnLoad != null) ? !_previousWinnersList.ToList().SequenceEqual(_previousWinnersListOnLoad) : false;

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <inheritdoc/>
        public ObservableCollection<WinnerHistoryElement> GetPreviousWinnersList() => _previousWinnersList;

        /// <inheritdoc/>
        public void ClearAll()
        {
            if (_previousWinnersList == null)
            {
                _previousWinnersList = new ObservableCollection<WinnerHistoryElement>();
            }
            if (_previousWinnersList.Count > 0) { _previousWinnersList.Clear(); }

            OnPropertyChanged(nameof(IndexOfCurrentWinnersInsidePreviousWinners));
            OnPropertyChanged(nameof(IsCurrentWinnersPartOfPreviousWinners));
            OnPropertyChanged(nameof(HasUnsavedChanges));
        }

        /// <inheritdoc/>
        public void ResetToLoadedState()
        {
            if (_previousWinnersListOnLoad == null) { return; }
            ClearAll();
            foreach (WinnerHistoryElement winnerElement in _previousWinnersListOnLoad)
            {
                AddPreviousWinner(new WinnerHistoryElement(winnerElement));
            }
        }

        /// <inheritdoc/>
        public void AddPreviousWinner(WinnerHistoryElement previousWinner)
        {
            addPreviousWinnerInternal(previousWinner, true);
        }

        /// <summary>
        /// Same as the <see cref="AddPreviousWinner(WinnerHistoryElement)"/> but with the possibility to disable the sorting
        /// </summary>
        /// <param name="previousWinner"><see cref="WinnerHistoryElement"/> to add</param>
        /// <param name="sortElements">If <see langword="true"/> sort the <see cref="_previousWinnersList"/></param>
        private void addPreviousWinnerInternal(WinnerHistoryElement previousWinner, bool sortElements = true)
        {
            if (_previousWinnersList == null)
            {
                _previousWinnersList = new ObservableCollection<WinnerHistoryElement>();
            }
            _previousWinnersList.Add(previousWinner);
            if (sortElements) { sortPreviousWinners(); }

            UpdatePreviousWinnersHistoryFlags();
            OnPropertyChanged(nameof(IndexOfCurrentWinnersInsidePreviousWinners));
            OnPropertyChanged(nameof(IsCurrentWinnersPartOfPreviousWinners));
            OnPropertyChanged(nameof(HasUnsavedChanges));
        }

        /// <summary>
        /// Sort the <see cref="_previousWinnersList"/> descending by the <see cref="WinnerHistoryElement.Timestamp"/>
        /// </summary>
        private void sortPreviousWinners()
        {
            _previousWinnersList.Sort((a, b) => b.CompareTo(a));
        }

        /// <inheritdoc/>
        public void RemovePreviousWinner(WinnerHistoryElement previousWinner)
        {
            _previousWinnersList?.Remove(previousWinner);

            UpdatePreviousWinnersHistoryFlags();
            OnPropertyChanged(nameof(IndexOfCurrentWinnersInsidePreviousWinners));
            OnPropertyChanged(nameof(IsCurrentWinnersPartOfPreviousWinners));
            OnPropertyChanged(nameof(HasUnsavedChanges));
        }

        /// <inheritdoc/>
        public void CopyCurrentWinnersToPrevious()
        {
            RemoveCurrentWinnersFromPrevious();
            AddPreviousWinner(CurrentWinners);
        }

        /// <inheritdoc/>
        public bool RemoveCurrentWinnersFromPrevious()
        {
            WinnerHistoryElement existingElement = _previousWinnersList.FirstOrDefault(p => p.Timestamp.Equals(CurrentWinners.Timestamp));
            if (existingElement != null)
            {
                RemovePreviousWinner(existingElement);
                return true;
            }
            return false;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <inheritdoc/>
        public void UpdateCurrentWinners()
        {
            WinnerHistoryElement currentWinners = new WinnerHistoryElement(_workspaceService?.Settings?.GetSetting<DateTime>(WorkspaceSettings.GROUP_GENERAL, WorkspaceSettings.SETTING_GENERAL_COMPETITIONDATE)?.Value ?? DateTime.Now,
                                                                            _scoreService?.GetWinnersPodiumStarts(ResultTypes.Overall, ResultPodiumsPlaces.Gold),
                                                                            _scoreService?.GetWinnersPodiumStarts(ResultTypes.Overall, ResultPodiumsPlaces.Silver),
                                                                            _scoreService?.GetWinnersPodiumStarts(ResultTypes.Overall, ResultPodiumsPlaces.Bronze));
            currentWinners.IsCurrentWinnersElement = true;
            currentWinners.MatchesCurrentWinnersElement = false;     // It matches but is not in the list of previous winners

            // Important to assing the local value to the backing field instead of the property (the SetProperty() method of the setter sometimes skips because equality doesn't check the flags used to display)
            _currentWinners = currentWinners;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <inheritdoc/>
        public void UpdatePreviousWinnersHistoryFlags()
        {
            if (_previousWinnersList == null || _previousWinnersList.Count == 0) { return; }

            // Collect the names of all current winners (from all podium places)
            List<WinnerName> currentWinnersCombined = new[] { CurrentWinners?.WinnersGold, CurrentWinners?.WinnersSilver, CurrentWinners?.WinnersBronze }
                .Where(list => list != null)
                .SelectMany(list => list)
                .ToList();

            foreach (WinnerHistoryElement historyElement in _previousWinnersList)
            {
                // Check if the winner history element is the current one.
                historyElement.MatchesCurrentWinnersElement = historyElement.Equals(CurrentWinners);

                // Check for each podium place of the winner history element if the current winner names are contained.
                historyElement.WinnersGold?.ForEach(name => name.IsPartOfCurrentWinners = currentWinnersCombined.Contains(name));
                historyElement.WinnersSilver?.ForEach(name => name.IsPartOfCurrentWinners = currentWinnersCombined.Contains(name));
                historyElement.WinnersBronze?.ForEach(name => name.IsPartOfCurrentWinners = currentWinnersCombined.Contains(name));
            }
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <inheritdoc/>
        public Dictionary<WinnerName, int> GetPreviousWinnersCountPerPerson(ResultPodiumsPlaces podiumPlace = ResultPodiumsPlaces.All, bool ignoreCurrentWinners = false)
        {
            IEnumerable<WinnerHistoryElement> previousWinners = ignoreCurrentWinners ? _previousWinnersList.Except(new List<WinnerHistoryElement>() { CurrentWinners }).Where(p => !p.MatchesCurrentWinnersElement) : _previousWinnersList;

            List<WinnerName> podiumWinnerNames = new List<WinnerName>();
            switch (podiumPlace)
            {
                case ResultPodiumsPlaces.Gold: podiumWinnerNames = previousWinners.Where(h => h.WinnersGold != null).SelectMany(h => h.WinnersGold).ToList(); break;
                case ResultPodiumsPlaces.Silver: podiumWinnerNames = previousWinners.Where(h => h.WinnersSilver != null).SelectMany(h => h.WinnersSilver).ToList(); break;
                case ResultPodiumsPlaces.Bronze: podiumWinnerNames = previousWinners.Where(h => h.WinnersBronze != null).SelectMany(h => h.WinnersBronze).ToList(); break;
                case ResultPodiumsPlaces.All:
                    podiumWinnerNames = previousWinners.SelectMany(h => (h.WinnersGold ?? Enumerable.Empty<WinnerName>())
                                                                        .Concat(h.WinnersSilver ?? Enumerable.Empty<WinnerName>())
                                                                        .Concat(h.WinnersBronze ?? Enumerable.Empty<WinnerName>())).ToList();
                    break;
                default: break;
            }

            Dictionary<WinnerName, int> podiumCounts = podiumWinnerNames.GroupBy(winner => winner)
                                                                        .OrderByDescending(g => g.Count())
                                                                        .ToDictionary(g => g.Key, g => g.Count());
            return podiumCounts;
        }
    }
}
