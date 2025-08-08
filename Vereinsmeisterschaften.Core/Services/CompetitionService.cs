using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;
using Vereinsmeisterschaften.Core.Settings;

namespace Vereinsmeisterschaften.Core.Services
{
    /// <summary>
    /// Service used to get and store a list of Competition objects
    /// </summary>
    public class CompetitionService : ICompetitionService
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
        private List<Competition> _competitionList { get; set; }

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
            _competitionList = new List<Competition>();
            _fileService = fileService;
            _personService = personService;
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
                        _competitionList = _fileService.LoadFromCsv<Competition>(path, cancellationToken, Competition.SetPropertyFromString, OnFileProgress);
                    }

                    _competitionListOnLoad = _competitionList.ConvertAll(c => new Competition(c));

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
                    _fileService.SaveToCsv(path, _competitionList, cancellationToken, OnFileProgress);

                    _competitionListOnLoad = _competitionList.ConvertAll(c => new Competition(c));
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
        public List<Competition> GetCompetitions() => _competitionList;

        /// <summary>
        /// Clear all Competitions
        /// </summary>
        public void ClearAll()
        {
            if (_competitionList == null) { _competitionList = new List<Competition>(); }
            _competitionList.Clear();
        }

        /// <summary>
        /// Add a new <see cref="Competition"/> to the list of Competitions.
        /// </summary>
        /// <param name="person"><see cref="Competition"/> to add</param>
        public void AddCompetition(Competition competition)
        {
            if (_competitionList == null) { _competitionList = new List<Competition>(); }
            _competitionList.Add(competition);
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
        /// <returns>Found <see cref="Competition"/> or <see langword="null"/></returns>
        public Competition GetCompetitionForPerson(Person person, SwimmingStyles swimmingStyle)
        {
            // Find the competition with the maximum age that matches the swimming style and gender
            byte maxAge = _competitionList.Where(c => c.Gender == person.Gender && c.SwimmingStyle == swimmingStyle).OrderByDescending(c => c.Age).FirstOrDefault()?.Age ?? 0;

            ushort competitionYear = _workspaceService?.Settings?.GetSettingValue<ushort>(WorkspaceSettings.GROUP_GENERAL, WorkspaceSettings.SETTING_GENERAL_COMPETITIONYEAR) ?? 0;
            int personAge = competitionYear - person.BirthYear;
            return _competitionList.Where(c => c.Gender == person.Gender &&
                                               c.SwimmingStyle == swimmingStyle &&
                                               (c.SwimmingStyle == SwimmingStyles.WaterFlea ? personAge <= c.Age : c.Age == (personAge <= maxAge ? personAge : maxAge))).FirstOrDefault();
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
            foreach (SwimmingStyles swimmingStyle in _availableSwimmingStyles)
            {
                Competition competition = GetCompetitionForPerson(person, swimmingStyle);
                if (availableCompetitions.ContainsKey(swimmingStyle))
                {
                    availableCompetitions[swimmingStyle] = competition;
                }
                else
                {
                    availableCompetitions.Add(swimmingStyle, competition);
                }
            }
            person.AvailableCompetitions = availableCompetitions;

            // Update the competitions for the person starts
            foreach (PersonStart personStart in _personService.GetAllPersonStarts(PersonStartFilters.Person, person))
            {
                personStart.CompetitionObj = person.AvailableCompetitions[personStart.Style];
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
