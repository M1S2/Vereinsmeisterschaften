﻿using System.Windows.Controls;
using System.Windows.Navigation;

using Vereinsmeisterschaften.Contracts.Services;
using Vereinsmeisterschaften.Contracts.ViewModels;

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
