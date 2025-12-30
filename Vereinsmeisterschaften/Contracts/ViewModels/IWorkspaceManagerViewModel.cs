using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Vereinsmeisterschaften.ViewModels;

namespace Vereinsmeisterschaften.Contracts.ViewModels
{
    /// <summary>
    /// Interface for the workspace manager
    /// </summary>
    public interface IWorkspaceManagerViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Current workspace folder path.
        /// </summary>
        string CurrentWorkspaceFolder { get; }

        /// <summary>
        /// True if the workspace has unsaved changes, false otherwise.
        /// </summary>
        bool HasUnsavedChanges { get; }

        /// <summary>
        /// List with previous workspace paths
        /// </summary>
        ObservableCollection<string> LastWorkspacePaths { get; }

        /// <summary>
        /// Clear all <see cref="LastWorkspacePaths"/>
        /// </summary>
        ICommand ClearAllLastWorkspacePathsCommand { get; }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Command to close the current workspace.
        /// </summary>
        ICommand CloseWorkspaceCommand { get; }

        /// <summary>
        /// Command to save the current workspace to a folder.
        /// </summary>
        ICommand SaveWorkspaceCommand { get; }

        /// <summary>
        /// Command to load a workspace from a folder.
        /// </summary>
        ICommand LoadWorkspaceCommand { get; }

        /// <summary>
        /// Command to load a previous workspace from the selected folder (command parameter).
        /// </summary>
        ICommand LoadLastWorkspaceCommand { get; }

        /// <summary>
        /// Command to open the current workspace folder in the file explorer.
        /// </summary>
        ICommand OpenWorkspaceFolderCommand { get; }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Delegate void for the <see cref="OnWorkspaceLoaded"/> event
        /// </summary>
        /// <param name="workspaceManagerViewModel">Reference to the <see cref="WorkspaceManagerViewModel"/></param>
        /// <param name="currentWorkspacePath">Path to the current workspace</param>
        delegate void WorkspaceLoadedDelegate(WorkspaceManagerViewModel workspaceManagerViewModel, string currentWorkspacePath);

        /// <summary>
        /// Event that his raised when the <see cref="LoadWorkspaceCommand"/> was executed successfully.
        /// </summary>    
        event WorkspaceLoadedDelegate OnWorkspaceLoaded;
    }
}
