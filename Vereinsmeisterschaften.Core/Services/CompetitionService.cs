using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Helpers;
using Vereinsmeisterschaften.Core.Models;
using Vereinsmeisterschaften.Core.Settings;
using Windows.Devices.PointOfService.Provider;

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
        private ICompetitionDistanceRuleService _competitionDistanceRuleService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fileService"><see cref="IFileService"/> object</param>
        /// <param name="personService"><see cref="IPersonService"/> object</param>
        /// <param name="competitionDistanceRuleService"><see cref="ICompetitionDistanceRuleService"/> object</param>
        public CompetitionService(IFileService fileService, IPersonService personService, ICompetitionDistanceRuleService competitionDistanceRuleService)
        {
            _competitionList = new ObservableCollection<Competition>();
            _competitionList.CollectionChanged += _competitionList_CollectionChanged;
            _fileService = fileService;
            _personService = personService;
            _personService.SetCompetitionServiceObj(this);                          // Dependency Injection can't be used in the constructor because of circular dependency
            _competitionDistanceRuleService = competitionDistanceRuleService;
            _competitionDistanceRuleService.SetCompetitionServiceObj(this);         // Dependency Injection can't be used in the constructor because of circular dependency
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
                        _competitionList.CollectionChanged += _competitionList_CollectionChanged;
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
                        else if (data is bool dataBool)
                        {
                            return dataBool ? "X" : "";
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
            if (_competitionList == null) 
            { 
                _competitionList = new ObservableCollection<Competition>();
                _competitionList.CollectionChanged += _competitionList_CollectionChanged;
            }
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
            if (_competitionList == null) 
            { 
                _competitionList = new ObservableCollection<Competition>(); 
                _competitionList.CollectionChanged += _competitionList_CollectionChanged;
            }
            _competitionList.Add(competition);

            competition.PropertyChanged += Competition_PropertyChanged;

            UpdateHasDuplicatesForCompetitions();

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

            UpdateHasDuplicatesForCompetitions();

            OnPropertyChanged(nameof(CompetitionCount));
            UpdateAllCompetitionsForPerson();
            OnPropertyChanged(nameof(HasUnsavedChanges));
        }

        private void Competition_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Competition.Gender):
                case nameof(Competition.SwimmingStyle):
                case nameof(Competition.Age):
                    UpdateHasDuplicatesForCompetitions();
                    break;
                default: break;
            }

            UpdateAllCompetitionsForPerson();
            UpdateCompetitionDistanceFromDistanceRules(sender as Competition);
            OnPropertyChanged(nameof(HasUnsavedChanges));
        }

        private void _competitionList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateHasDuplicatesForCompetitions();
            UpdateAllCompetitionsForPerson();
            OnPropertyChanged(nameof(HasUnsavedChanges));
        }

        /// <summary>
        /// Return the number of <see cref="Competition"/>
        /// </summary>
        /// <returns>Number of <see cref="Competition"/></returns>
        public int CompetitionCount => _competitionList?.Count ?? 0;

        /// <summary>
        /// Find all duplicate <see cref="Competition"/> objects and update the <see cref="Competition.HasDuplicates"/> flags.
        /// </summary>
        public void UpdateHasDuplicatesForCompetitions()
        {
            CompetitionBasicEqualityComparer basicCompetitionComparer = new CompetitionBasicEqualityComparer();
            IEnumerable<IGrouping<Competition, Competition>> duplicateGroups = _competitionList.GroupBy(c => c, basicCompetitionComparer).Where(g => g.Count() > 1);

            // Reset all HasDuplicates flags
            foreach (Competition c in _competitionList) { c.HasDuplicates = false; }

            // Set HasDuplicates flag for all competitions that are in a duplicate group
            foreach (IGrouping<Competition, Competition> duplicateGroup in duplicateGroups)
            {
                foreach (Competition c in duplicateGroup) { c.HasDuplicates = true; }
            }
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Check if the list of <see cref="Competition"/> has not saved changed.
        /// True, if unsaved changes exist; otherwise false.
        /// </summary>
        public bool HasUnsavedChanges => (_competitionList != null && _competitionListOnLoad != null) ? !_competitionList.SequenceEqual(_competitionListOnLoad) : false;

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <inheritdoc/>
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
                    if (foundCompetition == null)
                    {
                        // Calculate age distance and order by minimal distance. Prefer lower age on equal distance.
                        foundCompetition = competitions.OrderBy(c => Math.Abs(c.Age - personAge)).ThenBy(c => c.Age).FirstOrDefault();
                    }
                    break;
                case CompetitionSearchModes.ExactOrNearestPreferHigher:
                    foundCompetition = competitions.FirstOrDefault(c => c.Age == personAge);
                    if (foundCompetition == null)
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public void UpdateAllCompetitionsForPerson()
        {
            foreach (Person person in _personService.GetPersons())
            {
                UpdateAllCompetitionsForPerson(person);
            }
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <inheritdoc/>
        public void UpdateCompetitionDistanceFromDistanceRules(Competition competition)
        {
            if (competition == null) { return; }
            competition.Distance = _competitionDistanceRuleService.GetCompetitionDistanceFromRules(competition.Age, competition.SwimmingStyle);
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <inheritdoc/>
        public void UpdateAllCompetitionDistancesFromDistanceRules()
        {
            foreach (Competition competition in _competitionList)
            {
                UpdateCompetitionDistanceFromDistanceRules(competition);
            }
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <inheritdoc/>
        public void UpdateAllCompetitionTimesFromRudolphTable(string rudolphTableCsvFile, byte rudolphScore)
        {
            RudolphTable rudolphTable = new RudolphTable(rudolphTableCsvFile);

            foreach(Competition competition in _competitionList)
            {
                RudolphTableEntry foundRudolphTableEntry = rudolphTable.GetEntryByParameters(competition.Gender, competition.Age, competition.SwimmingStyle, competition.Distance, rudolphScore);
                if (foundRudolphTableEntry != null)
                {
                    competition.BestTime = foundRudolphTableEntry.Time;
                    competition.IsTimeFromRudolphTable = true;
                }
            }
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <inheritdoc/>
        public void CreateCompetitionsFromRudolphTable(string rudolphTableCsvFile, byte rudolphScore)
        {
            RudolphTable rudolphTable = new RudolphTable(rudolphTableCsvFile);

            List<Competition> competitions = rudolphTable.Entries
                                                .Where(e => e.RudolphScore == rudolphScore &&
                                                            !e.IsOpenAge &&
                                                            e.Distance == _competitionDistanceRuleService.GetCompetitionDistanceFromRules(e.Age, e.SwimmingStyle))
                                                .Select(e => new Competition
                                                             {
                                                                 Gender = e.Gender,
                                                                 Age = e.Age,
                                                                 SwimmingStyle = e.SwimmingStyle,
                                                                 Distance = e.Distance,
                                                                 BestTime = e.Time,
                                                                 IsTimeFromRudolphTable = true
                                                             }
                                                ).ToList();

            // All competitions should be deleted here that are later added from the competitions list.
            // All competitions that are not in this list are kept.
            CompetitionBasicEqualityComparer basicEqualityComparer = new CompetitionBasicEqualityComparer();
            List<Competition> competitionsToDelete = _competitionList.Intersect(competitions, basicEqualityComparer).ToList();
            foreach (Competition competition in competitionsToDelete)
            {
                competition.PropertyChanged -= Competition_PropertyChanged;
                _competitionList.Remove(competition);
            }
            OnPropertyChanged(nameof(CompetitionCount));
            UpdateAllCompetitionsForPerson();
            OnPropertyChanged(nameof(HasUnsavedChanges));

            // Add all new competitions
            int id = 1;
            foreach (Competition competition in competitions)
            {
                List<int> currentIds = _competitionList.Select(c => c.Id).ToList();
                while (currentIds.Contains(id)) { id++; }

                competition.Id = id;
                AddCompetition(competition);
            }
        }
    }
}
