namespace Vereinsmeisterschaften.Contracts.Services;

/// <summary>
/// Interface for a service to persist and restore application data.
/// </summary>
public interface IPersistAndRestoreService
{
    /// <summary>
    /// Restores the application data from a file in the local application data folder.
    /// </summary>
    void RestoreData();

    /// <summary>
    /// Persists the application data to a file in the local application data folder.
    /// </summary>
    void PersistData();
}
