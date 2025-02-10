using System;
using System.Collections.Generic;
using System.Text;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Contracts.Services
{
    /// <summary>
    /// Interface for a service used to manage a workspace
    /// </summary>
    public interface IWorkspaceService
    {
        /// <summary>
        /// Path to the workspace folder
        /// </summary>
        string WorkspaceFolderPath { get; }

        /// <summary>
        /// Settings for the current workspace
        /// </summary>
        public WorkspaceSettings Settings { get; set; }

        /// <summary>
        /// Open the workspace and load all files
        /// </summary>
        /// <param name="workspacePath">Path to the workspace folder</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>true if opening succeeded; false if opening failed (e.g. canceled)</returns>
        Task<bool> OpenWorkspace(string workspacePath, CancellationToken cancellationToken);

        /// <summary>
        /// Save all workspace files
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>true if saving succeeded; false if saving failed (e.g. canceled)</returns>
        Task<bool> SaveWorkspace(CancellationToken cancellationToken);

        /// <summary>
        /// Event that is raised when the workspace operation progress changes
        /// </summary>
        event ProgressDelegate OnWorkspaceProgress;

        /// <summary>
        /// Event that is raised when the workspace operation is finished.
        /// </summary>
        event EventHandler OnWorkspaceFinished;
    }
}
