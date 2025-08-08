namespace Vereinsmeisterschaften.Models;

/// <summary>
/// Class representing the application configuration settings.
/// </summary>
public class AppConfig
{
    /// <summary>
    /// Configuration folder path where the application settings are stored.
    /// </summary>
    public string ConfigurationsFolder { get; set; }

    /// <summary>
    /// File name for the application properties file.
    /// </summary>
    public string AppPropertiesFileName { get; set; }
}
