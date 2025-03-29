using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Resources;
using System.Text;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Services
{
    /// <summary>
    /// Service used to manage a workspace
    /// </summary>
    public class WorkspaceService : ObservableObject, IWorkspaceService
    {
        public const string WorkspaceSettingsFileName = "WorkspaceSettings.csv";
        public const string PersonFileName = "Person.csv";
        public const string CompetitionsFileName = "Competitions.csv";

        public string WorkspaceSettingsFilePath => Path.Combine(PersistentPath, WorkspaceSettingsFileName);
        public string PersonFilePath => Path.Combine(PersistentPath, PersonFileName);
        public string CompetitionsFilePath => Path.Combine(PersistentPath, CompetitionsFileName);

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

        public string PersistentPath { get; private set; } = string.Empty;

        private bool _isWorkspaceOpen;
        /// <summary>
        /// If true, a workspace is loaded; if false, not workspace is loaded
        /// </summary>
        public bool IsWorkspaceOpen
        {
            get => _isWorkspaceOpen;
            set { SetProperty(ref _isWorkspaceOpen, value); OnPropertyChanged(nameof(HasUnsavedChanges)); }
        }

        /// <summary>
        /// Check if the list of <see cref="Person"/> and the <see cref="Settings"/> were changed since loading it from the file.
        /// True, if unsaved changes exist; otherwise false.
        /// </summary>
        public bool HasUnsavedChanges => IsWorkspaceOpen && 
                                        ((_personService?.HasUnsavedChanges ?? false) || 
                                         (_competitionService?.HasUnsavedChanges ?? false) ||
                                         (Settings != null && _settingsPersistedInFile != null && (!Settings?.Equals(_settingsPersistedInFile) ?? true)));


        private WorkspaceSettings _settings;
        public WorkspaceSettings Settings
        {
            get => _settings;
            set { SetProperty(ref _settings, value); OnPropertyChanged(nameof(HasUnsavedChanges)); }
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Settings));
            OnPropertyChanged(nameof(HasUnsavedChanges));
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private WorkspaceSettings _settingsPersistedInFile;
        private IPersonService _personService;
        private ICompetitionService _competitionService;
        private IRaceService _raceService;
        private IFileService _fileService;

        public WorkspaceService(IPersonService personService, ICompetitionService competitionService, IRaceService raceService, IFileService fileService)
        {
            _personService = personService;
            _competitionService = competitionService;
            _raceService = raceService;
            _fileService = fileService;
            IsWorkspaceOpen = false;
            _personService.OnFileProgress += (sender, p, currentStep) => OnFileProgress?.Invoke(this, p / 2, "Loading persons...");
            _competitionService.OnFileProgress += (sender, p, currentStep) => OnFileProgress?.Invoke(this, 50 + (p / 2), "Loading competitions...");

            _personService.PropertyChanged += (sender, e) =>
            {
                switch(e.PropertyName)
                {
                    case nameof(PersonService.HasUnsavedChanges): OnPropertyChanged(nameof(HasUnsavedChanges)); break;
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
                Settings = _fileService.LoadFromCsv<WorkspaceSettings>(WorkspaceSettingsFilePath, cancellationToken, WorkspaceSettings.SetPropertyFromString, null)?.FirstOrDefault() ?? new WorkspaceSettings();
                Settings.PropertyChanged += Settings_PropertyChanged;

                // Persons
                openResult = await _personService.Load(PersonFilePath, cancellationToken);
                if(!openResult) { return openResult; }

                // Competitions
                openResult = await _competitionService.Load(CompetitionsFilePath, cancellationToken);

                _settingsPersistedInFile = new WorkspaceSettings(Settings);
                OnPropertyChanged(nameof(HasUnsavedChanges));
                IsWorkspaceOpen = openResult;
            }
            catch(Exception ex)
            {
                PersistentPath = previousPath;
                OnPropertyChanged(nameof(HasUnsavedChanges));
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
                _fileService.SaveToCsv(WorkspaceSettingsFilePath, new List<WorkspaceSettings>() { Settings }, cancellationToken, null);

                // Persons
                saveResult = await _personService.Save(cancellationToken, PersonFilePath);
                if (!saveResult) { return saveResult; }

                // Competitions
                saveResult = await _competitionService.Save(cancellationToken, CompetitionsFilePath);

                _settingsPersistedInFile = new WorkspaceSettings(Settings);
                OnPropertyChanged(nameof(HasUnsavedChanges));
            }
            catch (Exception ex)
            {
                OnPropertyChanged(nameof(HasUnsavedChanges));
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
            _raceService.LastCalculatedCompetitionRaces = null;
            PersistentPath = string.Empty;
            IsWorkspaceOpen = false;
            
            OnFileFinished?.Invoke(this, null);
            return saveResult;
        }
    }
}
