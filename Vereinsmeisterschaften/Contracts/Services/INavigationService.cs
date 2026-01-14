using System.Windows;
using System.Windows.Controls;

namespace Vereinsmeisterschaften.Contracts.Services;

/// <summary>
/// Interface for a service for handling navigation within the application.
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Event that is raised when navigation occured.
    /// </summary>
    event EventHandler<string> Navigated;
    
    /// <summary>
    /// True if the frame can navigate back, false otherwise.
    /// </summary>
    bool CanGoBack { get; }

    /// <summary>
    /// Current <see cref="FrameworkElement"/> that is displayed in the <see cref="Frame"/>. Most times this is a page.
    /// </summary>
    FrameworkElement CurrentFrameContent { get; }
    
    /// <summary>
    /// View model used by the <see cref="CurrentFrameContent"/>
    /// </summary>
    object CurrentFrameViewModel { get; }

    /// <summary>
    /// Initializes the navigation service with the provided frame.
    /// </summary>
    /// <param name="shellFrame">Use this <see cref="Frame"/> for navigation</param>
    void Initialize(Frame shellFrame);

    /// <summary>
    /// Navigates to the specified page using its key.
    /// </summary>
    /// <param name="pageKey">Page key</param>
    /// <param name="parameter">Optional parameter</param>
    /// <param name="clearNavigation">True to clear the navigation</param>
    /// <returns>True on navigation success</returns>
    bool NavigateTo(string pageKey, object parameter = null, bool clearNavigation = false);

    /// <summary>
    /// Navigates to the specified page using its view model type.
    /// </summary>
    /// <param name="parameter">Optional parameter</param>
    /// <param name="clearNavigation">True to clear the navigation</param>
    /// <returns>True on navigation success</returns>
    bool NavigateTo<T_VM>(object parameter = null, bool clearNavigation = false);

    /// <summary>
    /// Goes back to the previous page in the navigation stack if possible.
    /// </summary>
    void GoBack();

    /// <summary>
    /// Unsubscribes from navigation events and clears the frame reference.
    /// </summary>
    void UnsubscribeNavigation();

    /// <summary>
    /// Cleans the navigation stack of the frame.
    /// </summary>
    void CleanNavigation();
}
