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
    private readonly IThemeSelectorService _themeSelectorService;
    private readonly IApplicationInfoService _applicationInfoService;
    private AppTheme _theme;
    private string _versionDescription;
    private ICommand _setThemeCommand;

    /// <summary>
    /// App Theme (dark, light)
    /// </summary>
    public AppTheme Theme
    {
        get { return _theme; }
        set { SetProperty(ref _theme, value); }
    }

    /// <summary>
    /// Version string for the application
    /// </summary>
    public string VersionDescription
    {
        get { return _versionDescription; }
        set { SetProperty(ref _versionDescription, value); }
    }

    /// <summary>
    /// Command to change the app theme
    /// </summary>
    public ICommand SetThemeCommand => _setThemeCommand ?? (_setThemeCommand = new RelayCommand<string>(OnSetTheme));

    /// <summary>
    /// Constructor of the settings view model
    /// </summary>
    /// <param name="themeSelectorService"><see cref="IThemeSelectorService"/> object</param>
    /// <param name="applicationInfoService"><see cref="IApplicationInfoService"/> object</param>
    public SettingsViewModel(IThemeSelectorService themeSelectorService, IApplicationInfoService applicationInfoService)
    {
        _themeSelectorService = themeSelectorService;
        _applicationInfoService = applicationInfoService;
    }

    /// <inheritdoc/>
    public void OnNavigatedTo(object parameter)
    {
        VersionDescription = $"{Properties.Resources.AppDisplayName} - {_applicationInfoService.GetVersion()}";
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
