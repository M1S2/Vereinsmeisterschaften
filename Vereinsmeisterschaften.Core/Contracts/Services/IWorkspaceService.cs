using System.Collections.ObjectModel;
using System.ComponentModel;
using Vereinsmeisterschaften.Core.Services;
using Vereinsmeisterschaften.Core.Settings;

namespace Vereinsmeisterschaften.Core.Contracts.Services
{
    /// <summary>
    /// Interface for a service used to manage a workspace
    /// </summary>
    public interface IWorkspaceService : INotifyPropertyChanged, ISaveable
    {
        /// <summary>
        /// List with previous workspace paths
        /// </summary>
        ObservableCollection<string> LastWorkspacePaths { get; set; }

        /// <summary>
        /// Add a path to the <see cref="LastWorkspacePaths"/>
        /// </summary>
        /// <param name="path">Path to add</param>
        void AddLastWorkspacePath(string path);

        /// <summary>
        /// Clear all <see cref="LastWorkspacePaths"/>
        /// </summary>
        void ClearAllLastWorkspacePaths();

        /// <summary>
        /// If true, a workspace is loaded; if false, not workspace is loaded
        /// </summary>
        bool IsWorkspaceOpen { get; set; }

        /// <summary>
        /// If true, the workspace wasn't loaded from previously saved files. If at least one part of the workspace is loaded from a file, this will be false.
        /// </summary>
        bool IsCompletelyNewWorkspace { get; set; }

        /// <summary>
        /// Settings for the current workspace
        /// </summary>
        WorkspaceSettings Settings { get; set; }

        /// <summary>
        /// Settings loaded from the file for the current workspace. This can be used to compare for changes.
        /// </summary>
        WorkspaceSettings SettingsPersistedInFile { get; }

        /// <summary>
        /// Unsaved changes exist in the <see cref="PersonService"/>
        /// </summary>
        bool HasUnsavedChanges_Persons { get; }

        /// <summary>
        /// Unsaved changes exist in the <see cref="CompetitionService" or <see cref="CompetitionDistanceRuleService"/>/>
        /// </summary>
        bool HasUnsavedChanges_Competitions { get; }

        /// <summary>
        /// Unsaved changes exist in the <see cref="RaceService"/>
        /// </summary>
        bool HasUnsavedChanges_Races { get; }

        /// <summary>
        /// Unsaved changes exist in the <see cref="Settings"/>. This is true if the <see cref="Settings"/> was changed since loading it from the file.
        /// </summary>
        bool HasUnsavedChanges_Settings { get; }

        /// <summary>
        /// Call the <see cref="ICompetitionService.ResetToLoadedState" and <see cref="ICompetitionDistanceRuleService.ResetToLoadedState"/>/>
        /// </summary>
        void ResetCompetitionsToLoadedState();

        /// <summary>
        /// Call the <see cref="IPersonService.ResetToLoadedState"/>
        /// </summary>
        void ResetPersonsToLoadedState();

        /// <summary>
        /// Call the <see cref="IRaceService.ResetToLoadedState"/>
        /// </summary>
        void ResetRacesToLoadedState();

        /// <summary>
        /// Reset the <see cref="Settings"> to the state when the <see cref="Load(string, CancellationToken)"/> method was called.
        /// </summary>
        void ResetSettingsToLoadedState();

        /// <summary>
        /// Close the current workspace (set the current path to <see cref="string.Empty"/> and the <see cref="Settings"/> to <see langword="null"/>)
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="save">If true, <see cref="Save(CancellationToken, string)"/> is called before</param>
        /// <returns>true if saving during close succeeded; false if saving failed (e.g. canceled)</returns>
        Task<bool> CloseWorkspace(CancellationToken cancellationToken, bool save = true);
    }
}
