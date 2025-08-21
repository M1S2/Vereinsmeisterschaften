using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Vereinsmeisterschaften.Contracts.Services;
using Vereinsmeisterschaften.Contracts.ViewModels;
using Vereinsmeisterschaften.Models;

namespace Vereinsmeisterschaften.ViewModels;

/// <summary>
/// ViewModel for the settings page.
/// </summary>
public class SettingsViewModel : ObservableObject, INavigationAware
{
    private AppTheme _theme;
    /// <summary>
    /// App Theme (dark, light)
    /// </summary>
    public AppTheme Theme
    {
        get { return _theme; }
        set { SetProperty(ref _theme, value); }
    }

    private ICommand _setThemeCommand;
    /// <summary>
    /// Command to change the app theme
    /// </summary>
    public ICommand SetThemeCommand => _setThemeCommand ?? (_setThemeCommand = new RelayCommand<string>(OnSetTheme));

    private readonly IThemeSelectorService _themeSelectorService;

    /// <summary>
    /// Constructor of the settings view model
    /// </summary>
    /// <param name="themeSelectorService"><see cref="IThemeSelectorService"/> object</param>
    public SettingsViewModel(IThemeSelectorService themeSelectorService)
    {
        _themeSelectorService = themeSelectorService;
    }

    /// <inheritdoc/>
    public void OnNavigatedTo(object parameter)
    {
        Theme = _themeSelectorService.GetCurrentTheme();
    }

    /// <inheritdoc/>
    public void OnNavigatedFrom()
    {
    }

    private void OnSetTheme(string themeName)
    {
        var theme = (AppTheme)Enum.Parse(typeof(AppTheme), themeName);
        _themeSelectorService.SetTheme(theme);
    }
}
