using CommunityToolkit.Mvvm.ComponentModel;
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
        private ICompetitionService _competitionService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fileService"><see cref="IFileService"/> object</param>
        public PersonService(IFileService fileService)
        {
            _personList = new ObservableCollection<Person>();
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
                        foreach (Person person in list)
                        {
                            AddPerson(person);
                        }
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
            if(string.IsNullOrEmpty(path)) { path = PersistentPath; }

            bool saveResult = false;
            Exception exception = null;
            await Task.Run(() =>
            {
                try
                {
                    _fileService.SaveToCsv(path, _personList.ToList(), cancellationToken, OnFileProgress, (data) =>
                    {
                        if (data is bool dataBool)
                        {
                            return dataBool ? "X" : "";
                        }
                        else if (data is Enum dataEnum)
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
            if (_personList == null) { _personList = new ObservableCollection<Person>(); }

            foreach(Person person in _personList)
            {
                person.PropertyChanged -= Person_PropertyChanged;
            }
            _personList.Clear();

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
        }

        /// <summary>
        /// Add a new <see cref="Person"/> to the list of Persons
        /// </summary>
        /// <param name="person"><see cref="Person"/> to add</param>
        public void AddPerson(Person person)
        {
            if(_personList == null) { _personList = new ObservableCollection<Person>(); }
            _personList.Add(person);

            person.PropertyChanged += Person_PropertyChanged;
            
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
            // Update the competitions for the person. They depend on the gender and birth year.
            switch(e.PropertyName)
            {
                case nameof(Person.Gender):
                case nameof(Person.BirthYear):
                    _competitionService.UpdateAllCompetitionsForPerson(sender as Person);
                    break;
                default: break;
            }

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
        /// Find all duplicate <see cref="Person"/> objects.
        /// </summary>
        /// <returns>List with duplicate <see cref="Person"/></returns>
        public List<Person> CheckForDuplicatePerson()
        {
            List<Person> tmpPersonList = new List<Person>();
            List<Person> duplicates = new List<Person>();
            PersonBasicEqualityComparer basicPersonComparer = new PersonBasicEqualityComparer();
            foreach (Person person in _personList)
            {
                if(!tmpPersonList.Contains(person, basicPersonComparer))
                {
                    tmpPersonList.Add(person);
                }
                else
                {
                    duplicates.Add(person);
                }
            }
            return duplicates.Distinct().ToList();
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
        /// <returns>List with <see cref="PersonStart"/> objects</returns>
        public List<PersonStart> GetAllPersonStarts(PersonStartFilters filter = PersonStartFilters.None, object filterParameter = null)
        {
            List<PersonStart> allPersonStarts = new List<PersonStart>();
            foreach (Person person in _personList)
            {
                allPersonStarts.AddRange(person.Starts.Values.Cast<PersonStart>().Where(s => s != null));
            }

            switch (filter)
            {
                case PersonStartFilters.None: return allPersonStarts;
                case PersonStartFilters.Person: return allPersonStarts.Where(s => s.PersonObj == (Person)filterParameter).ToList();
                case PersonStartFilters.SwimmingStyle: return allPersonStarts.Where(s => s.Style == (SwimmingStyles)filterParameter).ToList();
                case PersonStartFilters.CompetitionID: return allPersonStarts.Where(s => s.CompetitionObj != null && s.CompetitionObj.Id == (int)filterParameter).ToList();
                default: return allPersonStarts;
            }
        }
    }
}
