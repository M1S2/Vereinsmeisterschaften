using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Helpers;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Services
{
    /// <summary>
    /// Service used to get and store a list of Person objects
    /// </summary>
    public class PersonService : ObservableObject, IPersonService
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
        /// List with all people
        /// </summary>
        private ObservableCollection<Person> _personList { get; set; }

        /// <summary>
        /// List with all people at the time the <see cref="Load(string, CancellationToken)"/> method was called.
        /// </summary>
        private List<Person> _personListOnLoad { get; set; }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private IFileService _fileService;
        private IScoreService _scoreService;
        private IRaceService _raceService;
        private ICompetitionService _competitionService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fileService"><see cref="IFileService"/> object</param>
        public PersonService(IFileService fileService)
        {
            _personList = new ObservableCollection<Person>();
            _personList.CollectionChanged += _personList_CollectionChanged;
            _fileService = fileService;
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
        /// Save the reference to the <see cref="ICompetitionService"/> object.
        /// Dependency Injection in the constructor can't be used here because there would be a circular dependency.
        /// </summary>
        /// <param name="competitionService">Reference to the <see cref="ICompetitionService"/> implementation</param>
        public void SetCompetitionServiceObj(ICompetitionService competitionService)
        {
            _competitionService = competitionService;
        }

        /// <summary>
        /// Save the reference to the <see cref="IRaceService"/> object.
        /// Dependency Injection in the constructor can't be used here because there would be a circular dependency.
        /// </summary>
        /// <param name="raceService">Reference to the <see cref="IRaceService"/> implementation</param>
        public void SetRaceServiceObj(IRaceService raceService)
        {
            _raceService = raceService;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Load a list of Persons to the <see cref="_personList"/>.
        /// This is using a separate Task because the file possibly can be large.
        /// If the file doesn't exist, the <see cref="_personList"/> is cleared and the functions returns loading success.
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
                        List<Person> list = _fileService.LoadFromCsv<Person>(path, cancellationToken, Person.SetPropertyFromString, OnFileProgress, (header) =>
                        {
                            return PropertyNameLocalizedStringHelper.FindProperty(typeof(Person), header);
                        });
                        _personList = new ObservableCollection<Person>();
                        _personList.CollectionChanged += _personList_CollectionChanged;
                        foreach (Person person in list)
                        {
                            AddPerson(person);
                        }
                        UpdateAllFriendReferencesFromFriendGroupIDs();
                    }

                    _personListOnLoad = _personList.ToList().ConvertAll(p => new Person(p));

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
        /// Save the list of Persons to a file
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
                    _fileService.SaveToCsv(path, _personList.ToList(), cancellationToken, OnFileProgress, (data, parentObject, currentProperty) =>
                    {
                        if (data is bool dataBool)
                        {
                            // Get the corresponding IsActive flag for the swimming style and set the file content accordingly
                            bool isActive = true;
                            switch (currentProperty.Name)
                            {
                                case nameof(Person.Breaststroke): isActive = (parentObject as Person)?.Starts[SwimmingStyles.Breaststroke]?.IsActive ?? true; break;
                                case nameof(Person.Freestyle): isActive = (parentObject as Person)?.Starts[SwimmingStyles.Freestyle]?.IsActive ?? true; break;
                                case nameof(Person.Backstroke): isActive = (parentObject as Person)?.Starts[SwimmingStyles.Backstroke]?.IsActive ?? true; break;
                                case nameof(Person.Butterfly): isActive = (parentObject as Person)?.Starts[SwimmingStyles.Butterfly]?.IsActive ?? true; break;
                                case nameof(Person.Medley): isActive = (parentObject as Person)?.Starts[SwimmingStyles.Medley]?.IsActive ?? true; break;
                                case nameof(Person.WaterFlea): isActive = (parentObject as Person)?.Starts[SwimmingStyles.WaterFlea]?.IsActive ?? true; break;
                            }
                            return dataBool ? (isActive ? "X" : Person.START_INACTIVE_MARKER_STRING) : "";
                        }
                        else if (data is Enum dataEnum)
                        {
                            return EnumCoreLocalizedStringHelper.Convert(dataEnum);
                        }
                        else if (data is IList dataList)
                        {
                            return string.Join(",", dataList.Cast<object>().Select(d => d.ToString()));
                        }
                        else
                        {
                            return data.ToString();
                        }
                    },
                    (header, type) =>
                    {
                        return PropertyNameLocalizedStringHelper.Convert(typeof(Person), header);
                    });

                    _personListOnLoad = _personList.ToList().ConvertAll(p => new Person(p));
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
        /// Return all available Persons
        /// </summary>
        /// <returns>List of <see cref="Person"/> objects</returns>
        public ObservableCollection<Person> GetPersons() => _personList;

        /// <summary>
        /// Clear all Persons
        /// </summary>
        public void ClearAll()
        {
            if (_personList == null) 
            { 
                _personList = new ObservableCollection<Person>();
                _personList.CollectionChanged += _personList_CollectionChanged;
            }

            foreach (Person person in _personList)
            {
                person.PropertyChanged -= Person_PropertyChanged;
            }
            if (_personList.Count > 0) { _personList.Clear(); }

            OnPropertyChanged(nameof(PersonCount));
            OnPropertyChanged(nameof(PersonStarts));
            OnPropertyChanged(nameof(HasUnsavedChanges));
        }

        /// <summary>
        /// Reset the list of Persons to the state when the <see cref="Load(string, CancellationToken)"/> method was called.
        /// This will clear all Persons and add the Persons that were loaded at that time.
        /// </summary>
        public void ResetToLoadedState()
        {
            if (_personListOnLoad == null) { return; }
            ClearAll();
            foreach (Person person in _personListOnLoad)
            {
                AddPerson(new Person(person));
            }
            UpdateAllFriendReferencesFromFriendGroupIDs();
            _competitionService.UpdateAllCompetitionsForPerson();
            _raceService.ReassignAllPersonStarts();
        }

        /// <summary>
        /// Add a new <see cref="Person"/> to the list of Persons
        /// </summary>
        /// <param name="person"><see cref="Person"/> to add</param>
        public void AddPerson(Person person)
        {
            if (_personList == null) 
            {
                _personList = new ObservableCollection<Person>();
                _personList.CollectionChanged += _personList_CollectionChanged;
            }
            _personList.Add(person);

            person.PropertyChanged += Person_PropertyChanged;

            UpdateHasDuplicatesForPersons();

            OnPropertyChanged(nameof(PersonCount));
            OnPropertyChanged(nameof(PersonStarts));
            OnPropertyChanged(nameof(HasUnsavedChanges));
        }

        /// <summary>
        /// Remove the given <see cref="Person"/> from the list of Persons
        /// </summary>
        /// <param name="person">Person to remove</param>
        public void RemovePerson(Person person)
        {
            person.PropertyChanged -= Person_PropertyChanged;
            _personList?.Remove(person);

            UpdateHasDuplicatesForPersons();

            OnPropertyChanged(nameof(PersonCount));
            OnPropertyChanged(nameof(PersonStarts));
            OnPropertyChanged(nameof(HasUnsavedChanges));
        }

        private bool _isupdatingScores = false;     // Flag to avoid recursive calls of the Person_PropertyChanged event. During UpdateScoresForPerson the Score of the PersonStart is changed which would raise a property changed event of the Starts property of the Person again.
        /// <summary>
        /// Event that is raised when a property of a <see cref="Person"/> changes.
        /// - Update the scores of the person.
        /// - Update all competitions for the person.
        /// - Raise some further property changed events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Person_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Person.Starts) && sender is Person senderPerson && _scoreService != null && !_isupdatingScores)
            {
                _isupdatingScores = true;
                _scoreService.UpdateScoresForPerson(senderPerson);
                _isupdatingScores = false;
            }

            switch (e.PropertyName)
            {
                case nameof(Person.Name):
                case nameof(Person.FirstName):
                    // Duplicates depends on the basic parameters of the person (First name, Name, Birth year, Gender)
                    UpdateHasDuplicatesForPersons();
                    break;
                case nameof(Person.Gender):
                case nameof(Person.BirthYear):
                    // Duplicates depends on the basic parameters of the person (First name, Name, Birth year, Gender)
                    UpdateHasDuplicatesForPersons();
                    // Update the competitions for the person. They depend on the gender and birth year.
                    _competitionService.UpdateAllCompetitionsForPerson(sender as Person);
                    break;
                case nameof(Person.Starts):
                    OnPropertyChanged(nameof(PersonStarts));
                    break;
                default: break;
            }

            OnPropertyChanged(nameof(HasUnsavedChanges));
        }

        private void _personList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateHasDuplicatesForPersons();
            OnPropertyChanged(nameof(PersonCount));
            OnPropertyChanged(nameof(PersonStarts));
            OnPropertyChanged(nameof(HasUnsavedChanges));
        }

        /// <summary>
        /// Return the number of <see cref="Person"/>
        /// </summary>
        public int PersonCount => _personList?.Count ?? 0;

        /// <summary>
        /// Return the total number of starts of all persons
        /// </summary>
        public int PersonStarts => _personList.Sum(p => p.Starts.Where(s => s.Value != null).Count());

        /// <summary>
        /// Find all duplicate <see cref="Person"/> objects and update the <see cref="Person.HasDuplicates"/> flags.
        /// </summary>
        public void UpdateHasDuplicatesForPersons()
        {
            PersonBasicEqualityComparer basicPersonComparer = new PersonBasicEqualityComparer();
            IEnumerable<IGrouping<Person, Person>> duplicateGroups = _personList.GroupBy(p => p, basicPersonComparer).Where(g => g.Count() > 1);

            // Reset all HasDuplicates flags
            foreach (Person p in _personList) { p.HasDuplicates = false; }

            // Set HasDuplicates flag for all persons that are in a duplicate group
            foreach (IGrouping<Person, Person> duplicateGroup in duplicateGroups)
            {
                foreach (Person person in duplicateGroup) { person.HasDuplicates = true; }
            }
        }

        /// <summary>
        /// Check if the list of <see cref="Person"/> has not saved changed.
        /// True, if unsaved changes exist; otherwise false.
        /// </summary>
        public bool HasUnsavedChanges => (_personList != null && _personListOnLoad != null) ? !_personList.ToList().SequenceEqual(_personListOnLoad) : false;

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Get all <see cref="PersonStart"/> objects for all <see cref="Person"/> objects that are not <see langword="null"/>.
        /// </summary>
        /// <param name="filter">Filter used to only return a subset of all <see cref="PersonStart"/> objects</param>
        /// <param name="filterParameter">Parameter used depending on the selected filter</param>
        /// <param name="onlyValidStarts">Return only the starts that are active and have a competition assigned</param>
        /// <returns>List with <see cref="PersonStart"/> objects</returns>
        public List<PersonStart> GetAllPersonStarts(PersonStartFilters filter = PersonStartFilters.None, object filterParameter = null, bool onlyValidStarts = false)
        {
            List<PersonStart> allPersonStarts = new List<PersonStart>();
            foreach (Person person in _personList)
            {
                allPersonStarts.AddRange(person.Starts.Values.Cast<PersonStart>().Where(s => s != null));
            }

            if(onlyValidStarts)
            {
                allPersonStarts = allPersonStarts.Where(s => s.IsActive && s.IsCompetitionObjAssigned).ToList();
            }

            switch (filter)
            {
                case PersonStartFilters.None:
                    return allPersonStarts;
                case PersonStartFilters.Person:
                    PersonBasicEqualityComparer personBasicComparer = new PersonBasicEqualityComparer();
                    return allPersonStarts.Where(s => personBasicComparer.Equals(s.PersonObj, (Person)filterParameter)).ToList();
                case PersonStartFilters.SwimmingStyle:
                    return allPersonStarts.Where(s => s.Style == (SwimmingStyles)filterParameter).ToList();
                case PersonStartFilters.CompetitionID:
                    return allPersonStarts.Where(s => s.CompetitionObj != null && s.CompetitionObj.Id == (int)filterParameter).ToList();
                default: return allPersonStarts;
            }
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Friend Management

        /// <summary>
        /// Loop all <see cref="Person"/> objects and update their friend references (<see cref="Person.Friends"/>) from the friend group IDs (<see cref="Person.FriendGroupIDs"/>).
        /// </summary>
        public void UpdateAllFriendReferencesFromFriendGroupIDs()
        {
            // For each person: Find all other persons with whom they share a common FriendGroupID
            foreach (Person person in _personList)
            {
                // Make sure the friend group IDs are sorted ascending
                person.FriendGroupIDs.Sort((a, b) => a.CompareTo(b));

                person.Friends?.Clear();
                
                // Skip persons without FriendGroupIDs
                if (person.FriendGroupIDs == null || person.FriendGroupIDs.Count == 0) { continue; }

                List<Person> friends = _personList.Where(p => p != person &&
                                                              p.FriendGroupIDs != null &&
                                                              p.FriendGroupIDs.Intersect(person.FriendGroupIDs).Any()).ToList();
                person.Friends?.AddRange(friends);
            }

            foreach (RacesVariant racesVariant in _raceService.AllRacesVariants)
            {
                racesVariant.CalculateScore();
            }
        }

        /// <summary>
        /// Return the number of friend groups.
        /// </summary>
        //public int NumberFriendGroups => _personList?.Sum(p => p.FriendGroupIDs.Count) ?? 0;
        public int NumberFriendGroups => _personList?.SelectMany(p => p.FriendGroupIDs)?.Distinct().Count() ?? 0;

        #endregion

    }
}
