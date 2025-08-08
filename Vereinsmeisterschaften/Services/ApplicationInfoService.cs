using System.Diagnostics;
using System.Reflection;

using Vereinsmeisterschaften.Contracts.Services;

namespace Vereinsmeisterschaften.Services;

/// <summary>
/// Service to provide application information such as version.
/// </summary>
public class ApplicationInfoService : IApplicationInfoService
{
    /// <summary>
    /// Constructor of the application info service.
    /// </summary>
    public ApplicationInfoService()
    {
    }

    /// <inheritdoc/>
    public Version GetVersion()
    {
        // Set the app version in Vereinsmeisterschaften > Properties > Package > PackageVersion
        string assemblyLocation = Assembly.GetExecutingAssembly().Location;
        var version = FileVersionInfo.GetVersionInfo(assemblyLocation).FileVersion;
        return new Version(version);
    }
}
