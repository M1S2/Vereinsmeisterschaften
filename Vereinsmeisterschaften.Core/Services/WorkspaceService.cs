using System;
using System.Collections.Generic;
using System.Text;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Services
{
    /// <summary>
    /// Service used to manage a workspace
    /// </summary>
    public class WorkspaceService : IWorkspaceService
    {
        public const string WorkspaceSettingsFileName = "WorkspaceSettings.csv";
        public const string PersonFileName = "Person.csv";
        public const string CompetitionsFileName = "Competitions.csv";

        /// <summary>
        /// Event that is raised when the workspace operation progress changes
        /// </summary>
        public event ProgressDelegate OnWorkspaceProgress;

        /// <summary>
        /// Event that is raised when the workspace operation is finished.
        /// </summary>
        public event EventHandler OnWorkspaceFinished;

        /// <summary>
        /// Event that is called whenever the <see cref="IsWorkspaceOpen"/> changed
        /// </summary>
        public event EventHandler OnIsWorkspaceOpenChanged;

        public string WorkspaceFolderPath { get; private set; } = string.Empty;

        private bool _isWorkspaceOpen;
        /// <summary>
        /// If true, a workspace is loaded; if false, not workspace is loaded
        /// </summary>
        public bool IsWorkspaceOpen
        {
            get => _isWorkspaceOpen;
            set { _isWorkspaceOpen = value; OnIsWorkspaceOpenChanged?.Invoke(this, null); }
        }

        public WorkspaceSettings Settings { get; set; }

        private IPersonService _personService;
        private ICompetitionService _competitionService;
        private IFileService _fileService;

        public WorkspaceService(IPersonService personService, ICompetitionService competitionService, IFileService fileService)
        {
            _personService = personService;
            _competitionService = competitionService;
            _fileService = fileService;
            IsWorkspaceOpen = false;
            _personService.OnFileProgress += (sender, p, currentStep) => OnWorkspaceProgress?.Invoke(this, p / 2, "Loading persons...");
            _competitionService.OnFileProgress += (sender, p, currentStep) => OnWorkspaceProgress?.Invoke(this, 50 + (p / 2), "Loading competitions...");
        }

        /// <summary>
        /// Open the workspace and load all files
        /// </summary>
        /// <param name="workspacePath">Path to the workspace folder</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>true if opening succeeded; false if opening failed (e.g. canceled)</returns>
        public async Task<bool> OpenWorkspace(string workspacePath, CancellationToken cancellationToken)
        {
            if(IsWorkspaceOpen)
            {
                await CloseWorkspace(true, cancellationToken);
            }

            WorkspaceFolderPath = workspacePath;

            string workspaceSettingsPath = Path.Combine(WorkspaceFolderPath, WorkspaceSettingsFileName);
            _personService.PersonFilePath = Path.Combine(WorkspaceFolderPath, PersonFileName);
            _competitionService.CompetitionFilePath = Path.Combine(WorkspaceFolderPath, CompetitionsFileName);
            bool openResult = false;
            Exception exception = null;
            try
            {
                // Workspace settings
                Settings = _fileService.LoadFromCsv<WorkspaceSettings>(workspaceSettingsPath, cancellationToken, WorkspaceSettings.SetPropertyFromString, null)?.FirstOrDefault() ?? new WorkspaceSettings();

                // Persons
                openResult = await _personService.LoadFromFile(cancellationToken);
                if(!openResult) { return openResult; }

                // Competitions
                openResult = await _competitionService.LoadFromFile(cancellationToken);

                IsWorkspaceOpen = openResult;
            }
            catch(Exception ex)
            {
                exception = ex;
            }
            OnWorkspaceFinished?.Invoke(this, null);
            if (exception != null) { throw exception; }
            return openResult;
        }

        /// <summary>
        /// Save all workspace files
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>true if saving succeeded; false if saving failed (e.g. canceled)</returns>
        public async Task<bool> SaveWorkspace(CancellationToken cancellationToken)
        {
            string workspaceSettingsPath = Path.Combine(WorkspaceFolderPath, WorkspaceSettingsFileName);
            bool saveResult = false;
            Exception exception = null;
            try
            {
                // Workspace settings
                _fileService.SaveToCsv(workspaceSettingsPath, new List<WorkspaceSettings>() { Settings }, cancellationToken, null);

                // Persons
                saveResult = await _personService.SaveToFile(cancellationToken);
                if (!saveResult) { return saveResult; }

                // Competitions
                saveResult = await _competitionService.SaveToFile(cancellationToken);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            OnWorkspaceFinished?.Invoke(this, null);
            if (exception != null) { throw exception; }
            return saveResult;
        }

        /// <summary>
        /// Close the current workspace (set the current path to <see cref="string.Empty"/> and the <see cref="Settings"/> to <see langword="null"/>)
        /// </summary>
        /// <param name="save">If true, <see cref="SaveWorkspace(CancellationToken)"/> is called before</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>true if saving during close succeeded; false if saving failed (e.g. canceled)</returns>
        public async Task<bool> CloseWorkspace(bool save, CancellationToken cancellationToken)
        {
            bool saveResult = true;
            if (save)
            {
                saveResult = await SaveWorkspace(cancellationToken);
            }

            WorkspaceFolderPath = string.Empty;
            Settings = null;
            _personService.ClearAll();
            _competitionService.ClearAll();
            IsWorkspaceOpen = false;
            
            OnWorkspaceFinished?.Invoke(this, null);
            return saveResult;
        }

    }
}
