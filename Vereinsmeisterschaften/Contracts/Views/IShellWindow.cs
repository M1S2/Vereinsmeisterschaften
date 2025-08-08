using System.Windows.Controls;

namespace Vereinsmeisterschaften.Contracts.Views;

/// <summary>
/// Interface for the main shell window of the application.
/// </summary>
public interface IShellWindow
{
    /// <summary>
    /// Gets the navigation frame used for navigating between pages.
    /// </summary>
    /// <returns><see cref="Frame"/></returns>
    Frame GetNavigationFrame();

    /// <summary>
    /// Shows the main shell window of the application.
    /// </summary>
    void ShowWindow();

    /// <summary>
    /// Closes the main shell window of the application.
    /// </summary>
    void CloseWindow();
}
