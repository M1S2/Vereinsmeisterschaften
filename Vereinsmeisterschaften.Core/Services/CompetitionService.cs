using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Helpers;
using Vereinsmeisterschaften.Core.Models;
using Vereinsmeisterschaften.Core.Settings;

namespace Vereinsmeisterschaften.Core.Services
{
    /// <summary>
    /// Service used to get and store a list of Competition objects
    /// </summary>
    public class CompetitionService : ObservableObject, ICompetitionService
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
        /// Path to the competition file
        /// </summary>
        public string PersistentPath { get; set; }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// List with all competitions
        /// </summary>
        private ObservableCollection<Competition> _competitionList { get; set; }

        /// <summary>
        /// List with all competitions at the time the <see cref="Load(string, CancellationToken)"/> method was called.
        /// </summary>
        private List<Competition> _competitionListOnLoad { get; set; }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private IFileService _fileService;
        private IPersonService _personService;
        private IWorkspaceService _workspaceService;

        /// <summary>
        /// Constructor
        /// </summary>
        public CompetitionService(IFileService fileService, IPersonService personService)
        {
            _competitionList = new ObservableCollection<Competition>();
            _fileService = fileService;
            _personService = personService;
            _personService.SetCompetitionServiceObj(this);        // Dependency Injection can't be used in the constructor because of circular dependency
        }

        /// <summary>
        /// Save the reference to the <see cref="IWorkspaceService"/> object.
        /// Dependency Injection in the constructor can't be used here because there would be a circular dependency.
        /// </summary>
        /// <param name="workspaceService">Reference to the <see cref="IWorkspaceService"/> implementation</param>
        public void SetWorkspaceServiceObj(IWorkspaceService workspaceService)
        {
            _workspaceService = workspaceService;

            if(_workspaceService != null)
            {
                _workspaceService.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == nameof(IWorkspaceService.Settings))
                    {
                        UpdateAllCompetitionsForPerson();
                    }
                };
            }
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Load a list of Competitions to the <see cref="_competitionList"/>.
        /// This is using a separate Task because the file possibly can be large.
        /// If the file doesn't exist, the <see cref="_competitionList"/> is cleared and the functions returns loading success.
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
                        _competitionList.Clear();
                        OnFileProgress?.Invoke(this, 100);
                    }
                    else
                    {
                        List<Competition> list = _fileService.LoadFromCsv<Competition>(path, cancellationToken, Competition.SetPropertyFromString, OnFileProgress, (header) =>
                        {
                            return PropertyNameLocalizedStringHelper.FindProperty(typeof(Competition), header);
                        });
                        _competitionList = new ObservableCollection<Competition>();
                        foreach (Competition competition in list)
                        {
                            AddCompetition(competition);
                        }
                    }

                    _competitionListOnLoad = _competitionList.ToList().ConvertAll(c => new Competition(c));

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
                    _fileService.SaveToCsv(path, _competitionList.ToList(), cancellationToken, OnFileProgress, (data, parentObject, currentProperty) =>
                    {
                        if (data is Enum dataEnum)
                        {
                            return EnumCoreLocalizedStringHelper.Convert(dataEnum);
                        }
                        else
                        {
                            return data.ToString();
                        }
                    },
                    (header, type) =>
                    {
                        return PropertyNameLocalizedStringHelper.Convert(typeof(Competition), header);
                    });

                    _competitionListOnLoad = _competitionList.ToList().ConvertAll(c => new Competition(c));
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
        /// Return all available Competitions
        /// </summary>
        /// <returns>List of <see cref="Competition"/> objects</returns>
        public ObservableCollection<Competition> GetCompetitions() => _competitionList;

        /// <summary>
        /// Clear all Competitions
        /// </summary>
        public void ClearAll()
        {
            if (_competitionList == null) { _competitionList = new ObservableCollection<Competition>(); }
            foreach (Competition competition in _competitionList)
            {
                competition.PropertyChanged -= Competition_PropertyChanged;
            }
            _competitionList.Clear();

            OnPropertyChanged(nameof(CompetitionCount));
            UpdateAllCompetitionsForPerson();
            OnPropertyChanged(nameof(HasUnsavedChanges));
        }

        /// <summary>
        /// Reset the list of Competitions to the state when the <see cref="Load(string, CancellationToken)"/> method was called.
        /// This will clear all Competitions and add the Competitions that were loaded at that time.
        /// </summary>
        public void ResetToLoadedState()
        {
            if (_competitionListOnLoad == null) { return; }
            ClearAll();
            foreach (Competition competition in _competitionListOnLoad)
            {
                AddCompetition(new Competition(competition));
            }
        }

        /// <summary>
        /// Add a new <see cref="Competition"/> to the list of Competitions.
        /// </summary>
        /// <param name="person"><see cref="Competition"/> to add</param>
        public void AddCompetition(Competition competition)
        {
            if (_competitionList == null) { _competitionList = new ObservableCollection<Competition>(); }
            _competitionList.Add(competition);

            competition.PropertyChanged += Competition_PropertyChanged;

            OnPropertyChanged(nameof(CompetitionCount));
            UpdateAllCompetitionsForPerson();
            OnPropertyChanged(nameof(HasUnsavedChanges));
        }

        /// <summary>
        /// Remove the given <see cref="Competition"/> from the list of Competitions
        /// </summary>
        /// <param name="competition">Competition to remove</param>
        public void RemoveCompetition(Competition competition)
        {
            competition.PropertyChanged -= Competition_PropertyChanged;
            _competitionList?.Remove(competition);
            OnPropertyChanged(nameof(CompetitionCount));
            UpdateAllCompetitionsForPerson();
            OnPropertyChanged(nameof(HasUnsavedChanges));
        }

        private void Competition_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateAllCompetitionsForPerson();
            OnPropertyChanged(nameof(HasUnsavedChanges));
        }

        /// <summary>
        /// Return the number of <see cref="Competition"/>
        /// </summary>
        /// <returns>Number of <see cref="Competition"/></returns>
        public int CompetitionCount => _competitionList?.Count ?? 0;

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Check if the list of <see cref="Competition"/> has not saved changed.
        /// True, if unsaved changes exist; otherwise false.
        /// </summary>
        public bool HasUnsavedChanges => (_competitionList != null && _competitionListOnLoad != null) ? !_competitionList.SequenceEqual(_competitionListOnLoad) : false;

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Return the competition that matches the person and swimming style
        /// </summary>
        /// <param name="person"><see cref="Person"/> used to search the <see cref="Competition"/></param>
        /// <param name="swimmingStyle"><see cref="SwimmingStyles"/> that must match the <see cref="Competition"/></param>
        /// <param name="isUsingMaxAgeCompetition">Flag indicating if this start is using a competition that was found by the max available age.</param>
        /// <param name="isUsingExactAgeCompetition">Flag indicating if this start is using a competition for which the age of the person matches the competition age.</param>
        /// <returns>Found <see cref="Competition"/> or <see langword="null"/></returns>
        public Competition GetCompetitionForPerson(Person person, SwimmingStyles swimmingStyle, out bool isUsingMaxAgeCompetition, out bool isUsingExactAgeCompetition)
        {
            isUsingMaxAgeCompetition = false;
            isUsingExactAgeCompetition = true;

            // Get all competitions that match the gender and swimming style
            List<Competition> competitions = _competitionList.Where(c => c.Gender == person.Gender && c.SwimmingStyle == swimmingStyle)
                                                             .OrderBy(c => c.Age).ToList();
            if (competitions.Count == 0) { return null; }

            CompetitionSearchModes searchMode = _workspaceService?.Settings?.GetSettingValue<CompetitionSearchModes>(WorkspaceSettings.GROUP_GENERAL, WorkspaceSettings.SETTING_GENERAL_COMPETITIONSEARCHMODE) ?? CompetitionSearchModes.ExactOrNextLowerOnlyMaxAge;
            ushort competitionYear = _workspaceService?.Settings?.GetSettingValue<ushort>(WorkspaceSettings.GROUP_GENERAL, WorkspaceSettings.SETTING_GENERAL_COMPETITIONYEAR) ?? 0;
            int personAge = competitionYear - person.BirthYear;
            byte maxAge = competitions.Max(c => c.Age);
            byte minAge = competitions.Min(c => c.Age);

            // Special case WaterFlea
            if (swimmingStyle == SwimmingStyles.WaterFlea)
            {
                return competitions.Where(c => personAge <= c.Age).FirstOrDefault();
            }

            Competition foundCompetition = null;
            switch (searchMode)
            {
                case CompetitionSearchModes.OnlyExactAge:
                    foundCompetition = competitions.FirstOrDefault(c => c.Age == personAge);
                    break;
                case CompetitionSearchModes.ExactOrNextLowerAge:
                    foundCompetition = competitions.LastOrDefault(c => c.Age <= personAge);
                    break;
                case CompetitionSearchModes.ExactOrNextHigherAge:
                    foundCompetition = competitions.FirstOrDefault(c => c.Age >= personAge);
                    break;
                case CompetitionSearchModes.ExactOrNextLowerOnlyMaxAge:
                    foundCompetition = competitions.FirstOrDefault(c => c.Age == personAge);
                    if (foundCompetition == null && personAge > maxAge)    // if no exact match was found and the person age is greater than the max age, use the competition with the max age
                    {
                        foundCompetition = competitions.FirstOrDefault(c => c.Age == maxAge);
                    }
                    break;
                case CompetitionSearchModes.ExactOrNearestPreferLower:
                    foundCompetition = competitions.FirstOrDefault(c => c.Age == personAge);
                    if (foundCompetition == null && personAge >= minAge)
                    {
                        // Calculate age distance and order by minimal distance. Prefer lower age on equal distance.
                        foundCompetition = competitions.OrderBy(c => Math.Abs(c.Age - personAge)).ThenBy(c => c.Age).FirstOrDefault();
                    }
                    break;
                case CompetitionSearchModes.ExactOrNearestPreferHigher:
                    foundCompetition = competitions.FirstOrDefault(c => c.Age == personAge);
                    if (foundCompetition == null && personAge >= minAge)
                    {
                        // Calculate age distance and order by minimal distance. Prefer higher age on equal distance.
                        foundCompetition = competitions.OrderBy(c => Math.Abs(c.Age - personAge)).ThenByDescending(c => c.Age).FirstOrDefault();
                    }
                    break;
            }

            if (foundCompetition != null && foundCompetition.Age == maxAge) { isUsingMaxAgeCompetition = true; }
            if (foundCompetition != null && foundCompetition.Age != personAge) { isUsingExactAgeCompetition = false; }

            return foundCompetition;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Update all <see cref="PersonStart"/> objects and the <see cref="Person.AvailableCompetitions"/> for the given <see cref="Person"/> with the corresponding <see cref="Competition"/> objects
        /// </summary>
        /// <param name="person"><see cref="Person"/> to update</param>
        public void UpdateAllCompetitionsForPerson(Person person)
        {
            // Update the available competitions for the person
            List<SwimmingStyles> _availableSwimmingStyles = Enum.GetValues(typeof(SwimmingStyles)).Cast<SwimmingStyles>().Where(s => s != SwimmingStyles.Unknown).ToList();
            Dictionary<SwimmingStyles, Competition> availableCompetitions = new Dictionary<SwimmingStyles, Competition>();
            Dictionary<SwimmingStyles, bool> isUsingMaxAgeCompetitionDict = new Dictionary<SwimmingStyles, bool>();
            Dictionary<SwimmingStyles, bool> isUsingExactAgeCompetitionDict = new Dictionary<SwimmingStyles, bool>();
            foreach (SwimmingStyles swimmingStyle in _availableSwimmingStyles)
            {
                bool isUsingMaxAgeCompetition, isUsingExactAgeCompetition;
                Competition competition = GetCompetitionForPerson(person, swimmingStyle, out isUsingMaxAgeCompetition, out isUsingExactAgeCompetition);
                
                availableCompetitions[swimmingStyle] = competition;
                isUsingMaxAgeCompetitionDict[swimmingStyle] = isUsingMaxAgeCompetition;
                isUsingExactAgeCompetitionDict[swimmingStyle] = isUsingExactAgeCompetition;
            }
            person.AvailableCompetitions = availableCompetitions;
            person.IsUsingMaxAgeCompetitionDict = isUsingMaxAgeCompetitionDict;
            person.IsUsingExactAgeCompetitionDict = isUsingExactAgeCompetitionDict;

            // Update the competitions for the person starts
            foreach (PersonStart personStart in _personService.GetAllPersonStarts(PersonStartFilters.Person, person))
            {
                personStart.CompetitionObj = person.AvailableCompetitions[personStart.Style];
                personStart.IsUsingMaxAgeCompetition = person.IsUsingMaxAgeCompetitionDict[personStart.Style];
                personStart.IsUsingExactAgeCompetition = person.IsUsingExactAgeCompetitionDict[personStart.Style];
            }
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Update all <see cref="PersonStart"/> and the <see cref="Person.AvailableCompetitions"/> objects with the corresponding <see cref="Competition"/> objects
        /// </summary>
        public void UpdateAllCompetitionsForPerson()
        {
            foreach (Person person in _personService.GetPersons())
            {
                UpdateAllCompetitionsForPerson(person);
            }
        }
    }
}
