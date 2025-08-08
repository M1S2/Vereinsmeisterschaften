namespace System.Windows.Controls;

/// <summary>
/// Helper class for <see cref="Frame"/> to access the DataContext and clean navigation history.
/// </summary>
public static class FrameExtensions
{
    /// <summary>
    /// Gets the DataContext of the current content of the Frame.
    /// </summary>
    /// <param name="frame"><see cref="Frame"/></param>
    /// <returns>DataContext of the <see cref="Frame.Content"/></returns>
    public static object GetDataContext(this Frame frame)
    {
        if (frame.Content is FrameworkElement element)
        {
            return element.DataContext;
        }

        return null;
    }

    /// <summary>
    /// Cleans the navigation history of the Frame by removing all back entries.
    /// </summary>
    /// <param name="frame"><see cref="Frame"/></param>
    public static void CleanNavigation(this Frame frame)
    {
        while (frame.CanGoBack)
        {
            frame.RemoveBackEntry();
        }
    }
}
