using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Resources;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Helpers;
using Vereinsmeisterschaften.Core.Settings;

namespace Vereinsmeisterschaften.Core.Services
{
    /// <summary>
    /// Service used to manage a workspace
    /// </summary>
    public partial class WorkspaceService : ObservableObject, IWorkspaceService
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

        private ObservableCollection<string> _lastWorkspacePaths = new ObservableCollection<string>();
        /// <inheritdoc/>
        public ObservableCollection<string> LastWorkspacePaths
        {
            get => _lastWorkspacePaths;
            set { _lastWorkspacePaths = value; OnPropertyChanged(); }
        }

        /// <inheritdoc/>
        public void AddLastWorkspacePath(string path)
        {
            if (LastWorkspacePaths.Contains(path))
            {
                LastWorkspacePaths.Remove(path);
            }
            LastWorkspacePaths.Insert(0, path);
        }

        /// <inheritdoc/>
        public void ClearAllLastWorkspacePaths()
            => LastWorkspacePaths?.Clear();

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

        /// <inheritdoc/>
        [ObservableProperty]
        private bool _isCompletelyNewWorkspace;

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Unsaved changes exist in the <see cref="PersonService"/>
        /// </summary>
        public bool HasUnsavedChanges_Persons => _personService?.HasUnsavedChanges ?? false;

        /// <summary>
        /// Unsaved changes exist in the <see cref="CompetitionService" or <see cref="CompetitionDistanceRuleService"/>/>
        /// </summary>
        public bool HasUnsavedChanges_Competitions => (_competitionService?.HasUnsavedChanges ?? false) || (_competitionDistanceRuleService?.HasUnsavedChanges ?? false);

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
                                        (HasUnsavedChanges_Persons || HasUnsavedChanges_Competitions || HasUnsavedChanges_Races || HasUnsavedChanges_Settings || IsCompletelyNewWorkspace);

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
        /// Call the <see cref="ICompetitionService.ResetToLoadedState"/> and <see cref="ICompetitionDistanceRuleService.ResetToLoadedState"/>
        /// </summary>
        public void ResetCompetitionsToLoadedState()
        {
            _competitionService.ResetToLoadedState();
            _competitionDistanceRuleService.ResetToLoadedState();
        }

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
        private ICompetitionDistanceRuleService _competitionDistanceRuleService;
        private IRaceService _raceService;
        private IFileService _fileService;

        /// <summary>
        /// Constructor for the <see cref="WorkspaceService"/>.
        /// </summary>
        /// <param name="personService"><see cref="IPersonService"/> object</param>
        /// <param name="competitionService"><see cref="ICompetitionService"/> object</param>
        /// <param name="competitionDistanceRuleService"><see cref="ICompetitionDistanceRuleService"/> object</param>
        /// <param name="raceService"><see cref="IRaceService"/> object</param>
        /// <param name="fileService"><see cref="IFileService"/> object</param>
        public WorkspaceService(IPersonService personService, ICompetitionService competitionService, ICompetitionDistanceRuleService competitionDistanceRuleService, IRaceService raceService, IFileService fileService)
        {
            _personService = personService;
            _competitionService = competitionService;
            _competitionService.SetWorkspaceServiceObj(this);   // Dependency Injection can't be used in the constructor because of circular dependency
            _competitionDistanceRuleService = competitionDistanceRuleService;
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
            _competitionDistanceRuleService.PropertyChanged += (sender, e) =>
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
                string filePathCompetitions = getFilePathToLoadFrom(KEY_FILENAME_COMPETITIONS);
                string filePathCompetitionDistanceRules = getFilePathToLoadFrom(KEY_FILENAME_COMPETITIONDISTANCERULES);
                string filePathPersons = getFilePathToLoadFrom(KEY_FILENAME_PERSON);
                string filePathBestRace = getFilePathToLoadFrom(KEY_FILENAME_BESTRACE);
                if (!File.Exists(WorkspaceSettingsFilePath) && !File.Exists(filePathCompetitions) && !File.Exists(filePathPersons) && !File.Exists(filePathBestRace))
                {
                    IsCompletelyNewWorkspace = true;
                }
                else
                {
                    IsCompletelyNewWorkspace = false;
                }

                // Workspace settings
                Settings = new WorkspaceSettings();
                Settings.Load(_fileService, WorkspaceSettingsFilePath);
                Settings.PropertyChanged += Settings_PropertyChanged;
                
                // Competitions
                openResult = await _competitionService.Load(filePathCompetitions, cancellationToken);
                if (!openResult) { return openResult; }

                // Competition Distance Rules
                openResult = await _competitionDistanceRuleService.Load(filePathCompetitionDistanceRules, cancellationToken);
                if (!openResult) { return openResult; }

                // Persons
                openResult = await _personService.Load(filePathPersons, cancellationToken);
                if(!openResult) { return openResult; }
                
                _competitionService.UpdateAllCompetitionsForPerson();
                _competitionService.UpdateAllCompetitionDistancesFromDistanceRules();

                // Best Race
                openResult = await _raceService.Load(filePathBestRace, cancellationToken);

                SettingsPersistedInFile = new WorkspaceSettings(Settings);
                OnPropertyChangedAllHasUnsavedChanges();
                IsWorkspaceOpen = openResult;

                AddLastWorkspacePath(path);
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

                // Competitions (delete old file if the file name changed)
                if (!await saveServiceAsync(KEY_FILENAME_COMPETITIONS, _competitionService, cancellationToken))
                    return false;

                // Competition Distance Rules
                if (!await saveServiceAsync(KEY_FILENAME_COMPETITIONDISTANCERULES, _competitionDistanceRuleService, cancellationToken))
                    return false;

                // Persons
                if (!await saveServiceAsync(KEY_FILENAME_PERSON, _personService, cancellationToken))
                    return false;

                // Best Race
                if (!await saveServiceAsync(KEY_FILENAME_BESTRACE, _raceService, cancellationToken))
                    return false;

                SettingsPersistedInFile = new WorkspaceSettings(Settings);
                IsCompletelyNewWorkspace = false;
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

        /// <summary>
        /// Helper method to save a service and delete the old file if the file name changed.
        /// </summary>
        /// <param name="fileNameResourceKey">Resource key used to find the file name resource</param>
        /// <param name="saveableService">Service with <see cref="ISaveable"/> implementation</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> for the save operation</param>
        /// <returns>Result of <see cref="ISaveable.Save(CancellationToken, string)"/></returns>
        private async Task<bool> saveServiceAsync(string fileNameResourceKey, ISaveable saveableService, CancellationToken cancellationToken)
        {
            string newPath = getFilePathToSaveTo(fileNameResourceKey);

            if (!string.IsNullOrEmpty(saveableService.PersistentPath) &&
                File.Exists(saveableService.PersistentPath) &&
                !string.Equals(saveableService.PersistentPath, newPath, StringComparison.OrdinalIgnoreCase))
            {
                File.Delete(saveableService.PersistentPath);
            }

            return await saveableService.Save(cancellationToken, newPath);
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
            _competitionDistanceRuleService.ClearAll();
            _raceService.AllRacesVariants.Clear();
            PersistentPath = string.Empty;
            IsWorkspaceOpen = false;
            
            OnFileFinished?.Invoke(this, null);
            return saveResult;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region File name handling

        /// <summary>
        /// Name of the workspace settings file
        /// </summary>
        public const string WorkspaceSettingsFileName = "WorkspaceSettings.json";

        /// <summary>
        /// Combined path to the workspace settings file
        /// </summary>
        public string WorkspaceSettingsFilePath => Path.Combine(PersistentPath, WorkspaceSettingsFileName);

        private readonly ResourceManager _fileNameResources = new ResourceManager(typeof(Properties.Resources));
        private const string KEY_FILENAME_PERSON = "FileName_Person";
        private const string KEY_FILENAME_COMPETITIONS = "FileName_Competitions";
        private const string KEY_FILENAME_COMPETITIONDISTANCERULES = "FileName_CompetitionDistanceRules";
        private const string KEY_FILENAME_BESTRACE = "FileName_BestRace";

        private string getLocalizedFilePath(string resourceKey, CultureInfo culture = null)
        {
            string localizedFileName = _fileNameResources.GetString(resourceKey, culture == null ? CultureInfo.CurrentUICulture : culture)
                                   ?? _fileNameResources.GetString(resourceKey, CultureInfo.InvariantCulture);
            return Path.Combine(PersistentPath, localizedFileName);
        }

        /// <summary>
        /// This method returns the file path to save to. This is always the file for the current culture.
        /// </summary>
        /// <param name="resourceKey">The resource key decides, which Resource is used to find the file name.</param>
        /// <returns>File path to save to</returns>
        private string getFilePathToSaveTo(string resourceKey)
            => getLocalizedFilePath(resourceKey);

        /// <summary>
        /// This method returns the file path to load from. If the file for the current culture doesn't exist, all other cultures are checked and the newest file is returned.
        /// </summary>
        /// <param name="resourceKey">The resource key decides, which Resource is used to find the file name.</param>
        /// <returns>File path to load from</returns>
        private string getFilePathToLoadFrom(string resourceKey)
        {
            if (string.IsNullOrEmpty(resourceKey)) { return string.Empty; }
            List<string> candidates = new List<string>();

            // Test if the file for the current culture exists
            string current = getLocalizedFilePath(resourceKey);
            if (File.Exists(current))
            {
                return current;
            }

            // Check all known cultures
            List<string> localizedFileNames = GeneralLocalizationHelper.GetAllTranslationsForKey(_fileNameResources, resourceKey).Values.ToList();
            List<string> localizedFilePaths = localizedFileNames.Select(f => Path.Combine(PersistentPath, f)).ToList();
            foreach (string localizedFilePath in localizedFilePaths)
            {
                if (File.Exists(localizedFilePath) && !candidates.Contains(localizedFilePath, StringComparer.OrdinalIgnoreCase))
                {
                    candidates.Add(localizedFilePath);
                }
            }

            // Use newest file depending on last write time
            return candidates.Select(f => new FileInfo(f))
                             .OrderByDescending(f => f.LastWriteTimeUtc)
                             .FirstOrDefault()?.FullName;
        }

        #endregion

    }
}
