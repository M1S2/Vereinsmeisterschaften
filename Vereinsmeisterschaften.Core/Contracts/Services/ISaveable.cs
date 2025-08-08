namespace Vereinsmeisterschaften.Core.Contracts.Services;

/// <summary>
/// Interface for saveable objects
/// </summary>
public interface ISaveable
{
    /// <summary>
    /// Path to the used file
    /// </summary>
    string PersistentPath { get; }

    /// <summary>
    /// Load from the given file
    /// This is using a separate Task because the file possibly can be large.
    /// </summary>
    /// <param name="path">Path from where to load</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>true if loading succeeded; false if importing failed (e.g. canceled)</returns>
    Task<bool> Load(string path, CancellationToken cancellationToken);

    /// <summary>
    /// Save to a file
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <param name="path">Path to which to save. If this is empty, the <see cref="PersistentPath"/> is used </param>
    /// <returns>true if saving succeeded; false if saving failed (e.g. canceled)</returns>
    Task<bool> Save(CancellationToken cancellationToken, string path = "");

    /// <summary>
    /// Event that is raised when the file operation progress changes
    /// </summary>
    event ProgressDelegate OnFileProgress;

    /// <summary>
    /// Event that is raised when the file operation is finished.
    /// </summary>
    event EventHandler OnFileFinished;

    /// <summary>
    /// Check if there are unsave changes.
    /// True, if unsaved changes exist; otherwise false.
    /// </summary>
    bool HasUnsavedChanges { get; }
}