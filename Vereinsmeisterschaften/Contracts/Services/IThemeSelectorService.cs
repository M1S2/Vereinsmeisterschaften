using Vereinsmeisterschaften.Models;

namespace Vereinsmeisterschaften.Contracts.Services;

/// <summary>
/// Interface for a service to manage the application theme selection.
/// </summary>
public interface IThemeSelectorService
{
    /// <summary>
    /// Initializes the theme manager with high contrast themes and set the current theme.
    /// </summary>
    void InitializeTheme();

    /// <summary>
    /// Sets the application theme based on the provided AppTheme enum value.
    /// </summary>
    /// <param name="theme"><see cref="AppTheme"/> enum value to determine the theme to set</param>
    void SetTheme(AppTheme theme);

    /// <summary>
    /// Gets the current application theme from the application properties.
    /// </summary>
    /// <returns><see cref="AppTheme"/></returns>
    AppTheme GetCurrentTheme();
}
