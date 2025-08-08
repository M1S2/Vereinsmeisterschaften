namespace Vereinsmeisterschaften.Contracts.Services;

/// <summary>
/// Interface for a service to provide application information such as version.
/// </summary>
public interface IApplicationInfoService
{
    /// <summary>
    /// Gets the version of the application.
    /// </summary>
    /// <returns>Application version</returns>
    Version GetVersion();
}
