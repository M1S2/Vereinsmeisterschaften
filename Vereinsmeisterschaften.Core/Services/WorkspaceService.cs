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

        public string WorkspaceFolderPath { get; private set; } = string.Empty;

        public WorkspaceSettings Settings { get; set; }

        private IPersonService _personService;
        private ICompetitionService _competitionService;
        private IFileService _fileService;

        public WorkspaceService(IPersonService personService, ICompetitionService competitionService, IFileService fileService)
        {
            _personService = personService;
            _competitionService = competitionService;
            _fileService = fileService;
            _personService.OnFileProgress += (sender, p) => OnWorkspaceProgress?.Invoke(this, p / 2);
            _competitionService.OnFileProgress += (sender, p) => OnWorkspaceProgress?.Invoke(this, 50 + (p / 2));
        }

        /// <summary>
        /// Open the workspace and load all files
        /// </summary>
        /// <param name="workspacePath">Path to the workspace folder</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>true if opening succeeded; false if opening failed (e.g. canceled)</returns>
        public async Task<bool> OpenWorkspace(string workspacePath, CancellationToken cancellationToken)
        {
            WorkspaceFolderPath = workspacePath;

            string workspaceSettingsPath = Path.Combine(WorkspaceFolderPath, WorkspaceSettingsFileName);
            _personService.PersonFilePath = Path.Combine(WorkspaceFolderPath, PersonFileName);
            _competitionService.CompetitionFilePath = Path.Combine(WorkspaceFolderPath, CompetitionsFileName);
            bool openResult = false;
            Exception exception = null;
            try
            {
                Settings = _fileService.LoadFromCsv<WorkspaceSettings>(workspaceSettingsPath, cancellationToken, WorkspaceSettings.SetPropertyFromString, null)?.FirstOrDefault() ?? new WorkspaceSettings();

                openResult = await _personService.LoadFromFile(cancellationToken);
                if(!openResult) { return openResult; }

                openResult = await _competitionService.LoadFromFile(cancellationToken);
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
                _fileService.SaveToCsv(workspaceSettingsPath, new List<WorkspaceSettings>() { Settings }, cancellationToken, null);

                saveResult = await _personService.SaveToFile(cancellationToken);
                if (!saveResult) { return saveResult; }

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

    }
}
