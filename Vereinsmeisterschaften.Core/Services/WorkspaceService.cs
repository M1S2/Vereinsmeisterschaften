using System;
using System.Collections.Generic;
using System.Text;
using Vereinsmeisterschaften.Core.Contracts.Services;

namespace Vereinsmeisterschaften.Core.Services
{
    /// <summary>
    /// Service used to manage a workspace
    /// </summary>
    public class WorkspaceService : IWorkspaceService
    {
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

        private IPersonService _personService;

        public WorkspaceService(IPersonService personService)
        {
            _personService = personService;
            _personService.OnFileProgress += (sender, p) => OnWorkspaceProgress?.Invoke(this, p);
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

            _personService.PersonFilePath = Path.Combine(WorkspaceFolderPath, PersonFileName);
            bool openResult = false;
            Exception exception = null;
            try
            {
                openResult = await _personService.LoadFromFile(cancellationToken);
                if(!openResult) { return openResult; }
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
            bool saveResult = false;
            Exception exception = null;
            try
            {
                saveResult = await _personService.SaveToFile(cancellationToken);
                if (!saveResult) { return saveResult; }
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
