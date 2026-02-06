using System.Reflection.Metadata;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

using Vereinsmeisterschaften.Contracts.Services;
using Vereinsmeisterschaften.Contracts.ViewModels;
using Vereinsmeisterschaften.ViewModels;

namespace Vereinsmeisterschaften.Services;

/// <summary>
/// Service for handling navigation within the application.
/// </summary>
public class NavigationService : INavigationService
{
    private readonly IPageService _pageService;
    private Frame _frame;
    private object _lastParameterUsed;

    /// <inheritdoc/>
    public event EventHandler<string> Navigated;

    /// <inheritdoc/>
    public bool CanGoBack => _frame.CanGoBack;

    /// <inheritdoc/>
    public FrameworkElement CurrentFrameContent => _frame.Content as FrameworkElement;
    
    /// <inheritdoc/>
    public object CurrentFrameViewModel => CurrentFrameContent?.DataContext;

    /// <summary>
    /// Constructor for the NavigationService.
    /// </summary>
    /// <param name="pageService"><see cref="IPageService"/> object</param>
    public NavigationService(IPageService pageService)
    {
        _pageService = pageService;
    }

    /// <inheritdoc/>
    public void Initialize(Frame shellFrame)
    {
        if (_frame == null)
        {
            _frame = shellFrame;
            _frame.Navigated += OnNavigated;
        }
    }

    /// <inheritdoc/>
    public void UnsubscribeNavigation()
    {
        _frame.Navigated -= OnNavigated;
        _frame = null;
    }

    /// <inheritdoc/>
    public void GoBack()
    {
        if (_frame.CanGoBack)
        {
            var vmBeforeNavigation = _frame.GetDataContext();
            _frame.GoBack();
            if (vmBeforeNavigation is INavigationAware navigationAware)
            {
                navigationAware.OnNavigatedFrom();
            }
        }
    }

    /// <inheritdoc/>
    public bool NavigateTo(string pageKey, object parameter = null, bool clearNavigation = false)
    {
        var pageType = _pageService.GetPageType(pageKey);

        if (_frame.Content?.GetType() != pageType || (parameter != null && !parameter.Equals(_lastParameterUsed)))
        {
            _frame.Tag = clearNavigation;
            var page = _pageService.GetPage(pageKey);
            var navigated = _frame.Navigate(page, parameter);
            if (navigated)
            {
                _lastParameterUsed = parameter;
                var dataContext = _frame.GetDataContext();
                if (dataContext is INavigationAware navigationAware)
                {
                    navigationAware.OnNavigatedFrom();
                }
            }

            return navigated;
        }

        return false;
    }

    /// <inheritdoc/>
    public bool NavigateTo<T_VM>(object parameter = null, bool clearNavigation = false)
    {
        return NavigateTo(typeof(T_VM).FullName);
    }

    /// <inheritdoc/>
    public bool ReloadCurrent()
    {
        string pageKey = CurrentFrameViewModel.GetType().FullName;
        var pageType = _pageService.GetPageType(pageKey);

        _frame.Tag = false;     // clearNavigation
        var page = _pageService.GetPage(pageKey);
        var navigated = _frame.Navigate(page, _lastParameterUsed);
        if (navigated)
        {
            var dataContext = _frame.GetDataContext();
            if (dataContext is INavigationAware navigationAware)
            {
                navigationAware.OnNavigatedFrom();
            }
        }
        return navigated;
    }

    /// <inheritdoc/>
    public void CleanNavigation()
        => _frame.CleanNavigation();

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        if (sender is Frame frame)
        {
            bool clearNavigation = (bool)frame.Tag;
            if (clearNavigation)
            {
                frame.CleanNavigation();
            }

            var dataContext = frame.GetDataContext();
            if (dataContext is INavigationAware navigationAware)
            {
                navigationAware.OnNavigatedTo(e.ExtraData);
            }

            Navigated?.Invoke(sender, dataContext.GetType().FullName);
        }
    }
}
