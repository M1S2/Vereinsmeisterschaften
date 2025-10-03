using CommunityToolkit.Mvvm.ComponentModel;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Settings;

namespace Vereinsmeisterschaften.Core.Services
{
    /// <summary>
    /// Service used to manage a workspace
    /// </summary>
    public partial class WorkspaceService : ObservableObject, IWorkspaceService
    {
        /// <summary>
        /// Name of the workspace settings file
        /// </summary>
        public const string WorkspaceSettingsFileName = "WorkspaceSettings.json";

        /// <summary>
        /// Name of the person file
        /// </summary>
        public const string PersonFileName = "Person.csv";

        /// <summary>
        /// Name of the competitions file
        /// </summary>
        public const string CompetitionsFileName = "Competitions.csv";

        /// <summary>
        /// Name of the best race file
        /// </summary>
        public const string BestRaceFileName = "BestRace.csv";

        /// <summary>
        /// Combined path to the workspace settings file
        /// </summary>
        public string WorkspaceSettingsFilePath => Path.Combine(PersistentPath, WorkspaceSettingsFileName);

        /// <summary>
        /// Combined path to the person file
        /// </summary>
        public string PersonFilePath => Path.Combine(PersistentPath, PersonFileName);

        /// <summary>
        /// Combined path to the competitions file
        /// </summary>
        public string CompetitionsFilePath => Path.Combine(PersistentPath, CompetitionsFileName);

        /// <summary>
        /// Combined path to the best race file
        /// </summary>
        public string BestRaceFilePath => Path.Combine(PersistentPath, BestRaceFileName);

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

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
        /// Path of the current workspace. This is the path where the workspace files are stored.
        /// </summary>
        public string PersistentPath { get; private set; } = string.Empty;

        private bool _isWorkspaceOpen;
        /// <summary>
        /// If true, a workspace is loaded; if false, not workspace is loaded
        /// </summary>
        public bool IsWorkspaceOpen
        {
            get => _isWorkspaceOpen;
            set
            {
                if (SetProperty(ref _isWorkspaceOpen, value))
                {
                    OnPropertyChangedAllHasUnsavedChanges();
                }
            }
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Unsaved changes exist in the <see cref="PersonService"/>
        /// </summary>
        public bool HasUnsavedChanges_Persons => _personService?.HasUnsavedChanges ?? false;

        /// <summary>
        /// Unsaved changes exist in the <see cref="CompetitionService"/>
        /// </summary>
        public bool HasUnsavedChanges_Competitions => _competitionService?.HasUnsavedChanges ?? false;

        /// <summary>
        /// Unsaved changes exist in the <see cref="RaceService"/>
        /// </summary>
        public bool HasUnsavedChanges_Races => _raceService?.HasUnsavedChanges ?? false;

        /// <summary>
        /// Unsaved changes exist in the <see cref="Settings"/>. This is true if the <see cref="Settings"/> was changed since loading it from the file.
        /// </summary>
        public bool HasUnsavedChanges_Settings => _settings != null && SettingsPersistedInFile != null && (!Settings?.Equals(SettingsPersistedInFile) ?? true);

        /// <summary>
        /// Check if there are unsaved changes in the workspace.
        /// </summary>
        public bool HasUnsavedChanges => IsWorkspaceOpen && 
                                        (HasUnsavedChanges_Persons || HasUnsavedChanges_Competitions || HasUnsavedChanges_Races || HasUnsavedChanges_Settings);

        private void OnPropertyChangedAllHasUnsavedChanges()
        {
            OnPropertyChanged(nameof(HasUnsavedChanges_Persons));
            OnPropertyChanged(nameof(HasUnsavedChanges_Competitions));
            OnPropertyChanged(nameof(HasUnsavedChanges_Races));
            OnPropertyChanged(nameof(HasUnsavedChanges_Settings));
            OnPropertyChanged(nameof(HasUnsavedChanges));
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Reset methods of services

        /// <summary>
        /// Call the <see cref="ICompetitionService.ResetToLoadedState"/>
        /// </summary>
        public void ResetCompetitionsToLoadedState() => _competitionService.ResetToLoadedState();

        /// <summary>
        /// Call the <see cref="IPersonService.ResetToLoadedState"/>
        /// </summary>
        public void ResetPersonsToLoadedState() => _personService.ResetToLoadedState();

        /// <summary>
        /// Call the <see cref="IRaceService.ResetToLoadedState"/>
        /// </summary>
        public void ResetRacesToLoadedState() => _raceService.ResetToLoadedState();

        /// <summary>
        /// Reset the <see cref="Settings"> to the state when the <see cref="Load(string, CancellationToken)"/> method was called.
        /// </summary>
        public void ResetSettingsToLoadedState()
        {
            if (SettingsPersistedInFile != null)
            {
                // Unsubscribe from the old settings PropertyChanged event
                if (Settings != null) { Settings.PropertyChanged -= Settings_PropertyChanged; }

                Settings = new WorkspaceSettings(SettingsPersistedInFile);
                Settings.PropertyChanged += Settings_PropertyChanged;
            }
        }

        #endregion

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private WorkspaceSettings _settings;
        /// <summary>
        /// Workspace settings for the current workspace.
        /// </summary>
        public WorkspaceSettings Settings
        {
            get => _settings;
            set
            {
                if (SetProperty(ref _settings, value))
                {
                    OnPropertyChanged(nameof(HasUnsavedChanges_Settings));
                    OnPropertyChanged(nameof(HasUnsavedChanges));
                }
            }
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Settings));
            OnPropertyChanged(nameof(HasUnsavedChanges_Settings));
            OnPropertyChanged(nameof(HasUnsavedChanges));
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Last workspace settings that were loaded from the file.
        /// </summary>
        [ObservableProperty]
        private WorkspaceSettings _settingsPersistedInFile;
        
        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private IPersonService _personService;
        private ICompetitionService _competitionService;
        private IRaceService _raceService;
        private IFileService _fileService;

        /// <summary>
        /// Constructor for the <see cref="WorkspaceService"/>.
        /// </summary>
        /// <param name="personService"><see cref="IPersonService"/> object</param>
        /// <param name="competitionService"><see cref="ICompetitionService"/> object</param>
        /// <param name="raceService"><see cref="IRaceService"/> object</param>
        /// <param name="fileService"><see cref="IFileService"/> object</param>
        public WorkspaceService(IPersonService personService, ICompetitionService competitionService, IRaceService raceService, IFileService fileService)
        {
            _personService = personService;
            _competitionService = competitionService;
            _competitionService.SetWorkspaceServiceObj(this);   // Dependency Injection can't be used in the constructor because of circular dependency
            _raceService = raceService;
            _raceService.SetWorkspaceServiceObj(this);          // Dependency Injection can't be used in the constructor because of circular dependency
            _fileService = fileService;
            IsWorkspaceOpen = false;
            _personService.OnFileProgress += (sender, p, currentStep) => OnFileProgress?.Invoke(this, p / 3, "Loading persons...");
            _competitionService.OnFileProgress += (sender, p, currentStep) => OnFileProgress?.Invoke(this, (100 / 3) + (p / 3), "Loading competitions...");
            _raceService.OnFileProgress += (sender, p, currentStep) => OnFileProgress?.Invoke(this, (2 * (100 / 3)) + (p / 3), "Loading best race...");

            _personService.PropertyChanged += (sender, e) =>
            {
                switch(e.PropertyName)
                {
                    case nameof(PersonService.HasUnsavedChanges): OnPropertyChanged(nameof(HasUnsavedChanges)); break;
                    default: break;
                }
            };
            _competitionService.PropertyChanged += (sender, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(CompetitionService.HasUnsavedChanges): OnPropertyChanged(nameof(HasUnsavedChanges)); break;
                    default: break;
                }
            };
            _raceService.PropertyChanged += (sender, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(RaceService.HasUnsavedChanges): OnPropertyChanged(nameof(HasUnsavedChanges)); break;
                    default: break;
                }
            };
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Open the workspace and load all files
        /// Caution: If there is already a workspace opened that has unsaved changed, the changes are saved! To change this behaviour, call the <see cref="CloseWorkspace(CancellationToken, bool)"/> with the save flag set to false before calling this method.
        /// </summary>
        /// <param name="path">Path from where to load</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>true if loading succeeded; false if importing failed (e.g. canceled)</returns>
        public async Task<bool> Load(string path, CancellationToken cancellationToken)
        {
            if (!Directory.Exists(path)) { return false; }

            if(IsWorkspaceOpen)
            {
                await CloseWorkspace(cancellationToken, true);
            }

            string previousPath = PersistentPath;
            PersistentPath = path;

            bool openResult = false;
            Exception exception = null;
            try
            {
                // Workspace settings
                Settings = new WorkspaceSettings();
                Settings.Load(_fileService, WorkspaceSettingsFilePath);
                Settings.PropertyChanged += Settings_PropertyChanged;

                // Persons
                openResult = await _personService.Load(PersonFilePath, cancellationToken);
                if(!openResult) { return openResult; }

                // Competitions
                openResult = await _competitionService.Load(CompetitionsFilePath, cancellationToken);
                if (!openResult) { return openResult; }

                _competitionService.UpdateAllCompetitionsForPerson();

                // Best Race
                openResult = await _raceService.Load(BestRaceFilePath, cancellationToken);

                SettingsPersistedInFile = new WorkspaceSettings(Settings);
                OnPropertyChangedAllHasUnsavedChanges();
                IsWorkspaceOpen = openResult;
            }
            catch(Exception ex)
            {
                PersistentPath = previousPath;
                OnPropertyChangedAllHasUnsavedChanges();
                OnPropertyChanged(nameof(IsWorkspaceOpen));
                exception = ex;
            }
            OnFileFinished?.Invoke(this, null);
            if (exception != null) { throw exception; }
            return openResult;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Save all workspace files
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="path">Path to which to save</param>
        /// <returns>true if saving succeeded; false if saving failed (e.g. canceled)</returns>
        public async Task<bool> Save(CancellationToken cancellationToken, string path = "")
        {
            if (string.IsNullOrEmpty(path)) { path = PersistentPath; }

            bool saveResult = false;
            Exception exception = null;
            try
            {
                // Workspace settings
                Settings?.Save(_fileService, WorkspaceSettingsFilePath);

                // Persons
                saveResult = await _personService.Save(cancellationToken, PersonFilePath);
                if (!saveResult) { return saveResult; }

                // Competitions
                saveResult = await _competitionService.Save(cancellationToken, CompetitionsFilePath);

                // Best Race
                saveResult = await _raceService.Save(cancellationToken, BestRaceFilePath);

                SettingsPersistedInFile = new WorkspaceSettings(Settings);
                OnPropertyChangedAllHasUnsavedChanges();
            }
            catch (Exception ex)
            {
                OnPropertyChangedAllHasUnsavedChanges();
                exception = ex;
            }
            OnFileFinished?.Invoke(this, null);
            if (exception != null) { throw exception; }
            return saveResult;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Close the current workspace (set the current path to <see cref="string.Empty"/> and the <see cref="Settings"/> to <see langword="null"/>)
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="save">If true, <see cref="Save(CancellationToken, string)"/> is called before</param>
        /// <returns>true if saving during close succeeded; false if saving failed (e.g. canceled)</returns>
        public async Task<bool> CloseWorkspace(CancellationToken cancellationToken, bool save = true)
        {
            bool saveResult = true;
            if (save)
            {
                saveResult = await Save(cancellationToken);
            }

            Settings.PropertyChanged -= Settings_PropertyChanged;
            Settings = null;
            _personService.ClearAll();
            _competitionService.ClearAll();
            _raceService.AllRacesVariants.Clear();
            PersistentPath = string.Empty;
            IsWorkspaceOpen = false;
            
            OnFileFinished?.Invoke(this, null);
            return saveResult;
        }
    }
}
