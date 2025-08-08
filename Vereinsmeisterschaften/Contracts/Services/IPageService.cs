using System.Windows.Controls;

namespace Vereinsmeisterschaften.Contracts.Services;

/// <summary>
/// Interface for a service to manage pages in the application.
/// </summary>
public interface IPageService
{
    /// <summary>
    /// Get the page type for a given key.
    /// </summary>
    /// <param name="key">Key for the page</param>
    /// <returns><see cref="Type"/> for the page</returns>
    /// <exception cref="ArgumentException">Thrown when the page with the key wasn't found.</exception>
    Type GetPageType(string key);

    /// <summary>
    /// Get the page instance for a given key.
    /// </summary>
    /// <param name="key">Page key</param>
    /// <returns><see cref="Page"/> object</returns>
    Page GetPage(string key);
}
