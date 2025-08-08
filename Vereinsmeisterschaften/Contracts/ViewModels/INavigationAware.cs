namespace Vereinsmeisterschaften.Contracts.ViewModels;

/// <summary>
/// Interface for objects that need to handle navigation events.
/// </summary>
public interface INavigationAware
{
    /// <summary>
    /// OnNavigatedTo method to handle navigation to this object.
    /// </summary>
    /// <param name="parameter">Parameter that can be passed by the caller</param>
    void OnNavigatedTo(object parameter);

    /// <summary>
    /// OnNavigatedFrom method to handle navigation away from this object.
    /// </summary>
    void OnNavigatedFrom();
}
