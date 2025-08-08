using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Vereinsmeisterschaften.Contracts.Services;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Properties;

namespace Vereinsmeisterschaften.ViewModels;

/// <summary>
/// ViewModel for the main shell of the application.
/// </summary>
public class ShellViewModel : ObservableObject
{
    /// <summary>
    /// Path to the current workspace folder.
    /// </summary>
    public string CurrentWorkspaceFolder => _workspaceService.PersistentPath;

    /// <summary>
    /// True if the workspace has unsaved changes; otherwise false.
    /// </summary>
    public bool HasUnsavedChanges => _workspaceService.HasUnsavedChanges;

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    private readonly INavigationService _navigationService;
    private IDialogCoordinator _dialogCoordinator;
    private IWorkspaceService _workspaceService;

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    #region Hamburger menu items

    private HamburgerMenuItem _selectedMenuItem;
    /// <summary>
    /// Menu item that is currently selected in the hamburger menu.
    /// </summary>
    public HamburgerMenuItem SelectedMenuItem
    {
        get { return _selectedMenuItem; }
        set { SetProperty(ref _selectedMenuItem, value); }
    }

    private HamburgerMenuItem _selectedOptionsMenuItem;
    /// <summary>
    /// Menu item that is currently selected in the hamburger menu options section.
    /// </summary>
    public HamburgerMenuItem SelectedOptionsMenuItem
    {
        get { return _selectedOptionsMenuItem; }
        set { SetProperty(ref _selectedOptionsMenuItem, value); }
    }

    /// <summary>
    /// Available menu items in the hamburger menu.
    /// </summary>
    public ObservableCollection<HamburgerMenuItem> MenuItems { get; } = new ObservableCollection<HamburgerMenuItem>()
    {
        new HamburgerMenuGlyphItem() { Label = Resources.ShellMainPage, Glyph = "\uE80F", TargetPageType = typeof(MainViewModel) },
        new HamburgerMenuGlyphItem() { Label = Resources.ShellWorkspacePage, Glyph = "\uE821", TargetPageType = typeof(WorkspaceViewModel) },
        new HamburgerMenuGlyphItem() { Label = Resources.ShellPeoplePage, Glyph = "\uE77B", TargetPageType = typeof(PeopleViewModel) },
        new HamburgerMenuGlyphItem() { Label = Resources.ShellPrepareRacesPage, Glyph = "\uE7C1", TargetPageType = typeof(PrepareRacesViewModel) },
        new HamburgerMenuGlyphItem() { Label = Resources.ShellTimeInputPage, Glyph = "\uE916", TargetPageType = typeof(TimeInputViewModel) },
        new HamburgerMenuGlyphItem() { Label = Resources.ShellResultsPage, Glyph = "\uE9F9", TargetPageType = typeof(ResultsViewModel) },
        new HamburgerMenuGlyphItem() { Label = Resources.ShellCreateDocumentsPage, Glyph = "\uE8A5", TargetPageType = typeof(CreateDocumentsViewModel) },
    };

    /// <summary>
    /// Available menu items in the hamburger menu options section.
    /// </summary>
    public ObservableCollection<HamburgerMenuItem> OptionMenuItems { get; } = new ObservableCollection<HamburgerMenuItem>()
    {
        new HamburgerMenuGlyphItem() { Label = Resources.ShellSettingsPage, Glyph = "\uE713", TargetPageType = typeof(SettingsViewModel) }
    };

    #endregion

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    #region Commands

    private RelayCommand _goBackCommand;
    private ICommand _menuItemInvokedCommand;
    private ICommand _optionsMenuItemInvokedCommand;
    private ICommand _loadedCommand;
    private ICommand _unloadedCommand;
    private ICommand _closingCommand;
    private ICommand _saveWorkspaceCommand;

    /// <summary>
    /// Command to navigate back in the navigation stack.
    /// </summary>
    public RelayCommand GoBackCommand => _goBackCommand ?? (_goBackCommand = new RelayCommand(() => _navigationService.GoBack(), () => _navigationService.CanGoBack));

    /// <summary>
    /// Command that is invoked when a menu item in the hamburger menu is clicked.
    /// </summary>
    public ICommand MenuItemInvokedCommand => _menuItemInvokedCommand ?? (_menuItemInvokedCommand = new RelayCommand(() => NavigateTo(SelectedMenuItem.TargetPageType)));

    /// <summary>
    /// Command that is invoked when an options menu item in the hamburger menu is clicked.
    /// </summary>
    public ICommand OptionsMenuItemInvokedCommand => _optionsMenuItemInvokedCommand ?? (_optionsMenuItemInvokedCommand = new RelayCommand(() => NavigateTo(SelectedOptionsMenuItem.TargetPageType)));

    /// <summary>
    /// Command that is invoked when the shell is loaded.
    /// </summary>
    public ICommand LoadedCommand => _loadedCommand ?? (_loadedCommand = new RelayCommand(OnLoaded));

    /// <summary>
    /// Command that is invoked when the shell is unloaded.
    /// </summary>
    public ICommand UnloadedCommand => _unloadedCommand ?? (_unloadedCommand = new RelayCommand(OnUnloaded));

    /// <summary>
    /// Command that is invoked when the shell is closing.
    /// </summary>
    public ICommand ClosingCommand => _closingCommand ?? (_closingCommand = new RelayCommand<CancelEventArgs>(OnClosing));

    /// <summary>
    /// Command to save the current workspace.
    /// </summary>
    public ICommand SaveWorkspaceCommand => _saveWorkspaceCommand ?? (_saveWorkspaceCommand = new RelayCommand(async () => await _workspaceService?.Save(CancellationToken.None)));

    #endregion

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    /// <summary>
    /// Constructor of the view model of the main shell of the application
    /// </summary>
    /// <param name="navigationService"><see cref="INavigationService"/> object</param>
    /// <param name="dialogCoordinator"><see cref="IDialogCoordinator"/> object</param>
    /// <param name="workspaceService"><see cref="IWorkspaceService"/> object</param>
    public ShellViewModel(INavigationService navigationService, IDialogCoordinator dialogCoordinator, IWorkspaceService workspaceService)
    {
        _navigationService = navigationService;
        _dialogCoordinator = dialogCoordinator;
        _workspaceService = workspaceService;
    }

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    #region Load and unload

    private async void OnLoaded()
    {
        _navigationService.Navigated += OnNavigated;
        _workspaceService.PropertyChanged += _workspaceService_PropertyChanged;

        // Load the workspace
        try
        {
            await _workspaceService.Load(App.Current.Properties["LastWorkspaceFolder"]?.ToString(), CancellationToken.None);
        }
        catch (Exception ex)
        {
            await _dialogCoordinator.ShowMessageAsync(this, Resources.ErrorString, ex.Message);
        }
    }

    private void OnUnloaded()
    {
        _navigationService.Navigated -= OnNavigated;
        _workspaceService.PropertyChanged -= _workspaceService_PropertyChanged;
    }

    private void _workspaceService_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(IWorkspaceService.HasUnsavedChanges): OnPropertyChanged(nameof(HasUnsavedChanges)); break;
            case nameof(IWorkspaceService.PersistentPath): OnPropertyChanged(nameof(CurrentWorkspaceFolder)); break;
            case nameof(IWorkspaceService.IsWorkspaceOpen):
                {
                    OnPropertyChanged(nameof(CurrentWorkspaceFolder));
                    foreach (HamburgerMenuItem menuItem in MenuItems)
                    {
                        if (menuItem.TargetPageType == typeof(MainViewModel) || menuItem.TargetPageType == typeof(WorkspaceViewModel)) { continue; }

                        menuItem.IsEnabled = _workspaceService.IsWorkspaceOpen;
                    }
                    break;
                }
            default: break;
        }
    }

    #endregion

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    #region Closing

    // Close behaviour: https://github.com/MahApps/MahApps.Metro/issues/3535
    private bool _forceClose = false;

    /// <summary>
    /// Event that is raised when the window close is requested.
    /// </summary>
    public event EventHandler WindowCloseRequested;
    protected void OnClosing(CancelEventArgs e)
    {
        // force method to abort - we'll force a close explicitly
        e.Cancel = true;

        if (_forceClose)
        {
            // cleanup code already ran
            e.Cancel = false;
            return;
        }

        // execute shutdown logic - set ForceClose and request Close() to actually close the window
        Dispatcher.CurrentDispatcher.InvokeAsync(async () =>
        {
            bool save = await checkForUnsavedChangesAndQueryUserAction();
            if (save)
            {
                try
                {
                    await _workspaceService.Save(CancellationToken.None);
                }
                catch (Exception ex)
                {
                    await _dialogCoordinator.ShowMessageAsync(this, Resources.ErrorString, ex.Message);
                }
            }
            App.Current.Properties["LastWorkspaceFolder"] = CurrentWorkspaceFolder;
            _forceClose = true;
            WindowCloseRequested?.Invoke(this, null);   // Notify the ShellWindow to close
        }, DispatcherPriority.Normal);
    }

    /// <summary>
    /// Show a dialog to the user when there are unsave changes. The user can choose to save or not save.
    /// </summary>
    /// <returns>If True, changes should be saved; otherwise save nothing</returns>
    private async Task<bool> checkForUnsavedChangesAndQueryUserAction()
    {
        bool save = false;
        if (_workspaceService?.HasUnsavedChanges ?? false)
        {
            MetroDialogSettings dialogButtonSettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = Resources.SaveString,
                NegativeButtonText = Resources.DontSaveString,
                DefaultButtonFocus = MessageDialogResult.Affirmative
            };
            MessageDialogResult dialogResult = await _dialogCoordinator.ShowMessageAsync(this, Resources.UnsavedChangesString, Resources.UnsavedChangesSavePromptString,
                                                                                        MessageDialogStyle.AffirmativeAndNegative, dialogButtonSettings);
            switch (dialogResult)
            {
                case MessageDialogResult.Affirmative: save = true; break;
                case MessageDialogResult.Negative: save = false; break;
                default: break;
            }
        }
        return save;
    }

    #endregion

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    private void NavigateTo(Type targetViewModel)
    {
        if (targetViewModel != null)
        {
            _navigationService.NavigateTo(targetViewModel.FullName);
        }
    }

    private void OnNavigated(object sender, string viewModelName)
    {
        var item = MenuItems.OfType<HamburgerMenuItem>().FirstOrDefault(i => viewModelName == i.TargetPageType?.FullName);
        if (item != null)
        {
            SelectedMenuItem = item;
        }
        else
        {
            SelectedOptionsMenuItem = OptionMenuItems.OfType<HamburgerMenuItem>().FirstOrDefault(i => viewModelName == i.TargetPageType?.FullName);
        }
        GoBackCommand.NotifyCanExecuteChanged();
    }
}
