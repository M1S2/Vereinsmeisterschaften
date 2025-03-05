using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Contracts.Services
{
    /// <summary>
    /// Interface for a service used to manage a workspace
    /// </summary>
    public interface IWorkspaceService : INotifyPropertyChanged
    {
        /// <summary>
        /// Path to the workspace folder
        /// </summary>
        string WorkspaceFolderPath { get; }

        /// <summary>
        /// If true, a workspace is loaded; if false, not workspace is loaded
        /// </summary>
        bool IsWorkspaceOpen { get; set; }

        /// <summary>
        /// Check if the list of <see cref="Person"/> and the <see cref="Settings"/> were changed since loading it from the file.
        /// True, if unsaved changes exist; otherwise false.
        /// </summary>
        bool HasUnsavedChanges { get; }

        /// <summary>
        /// Settings for the current workspace
        /// </summary>
        public WorkspaceSettings Settings { get; set; }

        /// <summary>
        /// Open the workspace and load all files.
        /// If there is already an open workspace, save the files first. Then open the new workspace.
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
        /// Close the current workspace (set the current path to <see cref="string.Empty"/> and the <see cref="Settings"/> to <see langword="null"/>)
        /// </summary>
        /// <param name="save">If true, <see cref="SaveWorkspace(CancellationToken)"/> is called before</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>true if saving during close succeeded; false if saving failed (e.g. canceled)</returns>
        Task<bool> CloseWorkspace(bool save, CancellationToken cancellationToken);

        /// <summary>
        /// Event that is raised when the workspace operation progress changes
        /// </summary>
        event ProgressDelegate OnWorkspaceProgress;

        /// <summary>
        /// Event that is raised when the workspace operation is finished.
        /// </summary>
        event EventHandler OnWorkspaceFinished;

        /// <summary>
        /// Event that is called whenever the <see cref="IsWorkspaceOpen"/> changed
        /// </summary>
        event EventHandler OnIsWorkspaceOpenChanged;
    }
}
