using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Extensions.Logging;
using Vereinsmeisterschaften.Contracts.Services;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Properties;
using Windows.UI.WindowManagement;

namespace Vereinsmeisterschaften.ViewModels;

public class ShellViewModel : ObservableObject
{
#warning Change to non-fixed path!!!
    public const string DefaultWorkspaceFolder = @"C:\Users\Markus\Desktop\VM_TestData\Data1";

    public string CurrentWorkspaceFolder => _workspaceService.PersistentPath;

    public bool HasUnsavedChanges => _workspaceService.HasUnsavedChanges;

    private readonly INavigationService _navigationService;
    private HamburgerMenuItem _selectedMenuItem;
    private HamburgerMenuItem _selectedOptionsMenuItem;
    private RelayCommand _goBackCommand;
    private ICommand _menuItemInvokedCommand;
    private ICommand _optionsMenuItemInvokedCommand;
    private ICommand _loadedCommand;
    private ICommand _unloadedCommand;
    private ICommand _closingCommand;
    private ICommand _saveWorkspaceCommand;
    private IDialogCoordinator _dialogCoordinator;

    private IWorkspaceService _workspaceService;

    public HamburgerMenuItem SelectedMenuItem
    {
        get { return _selectedMenuItem; }
        set { SetProperty(ref _selectedMenuItem, value); }
    }

    public HamburgerMenuItem SelectedOptionsMenuItem
    {
        get { return _selectedOptionsMenuItem; }
        set { SetProperty(ref _selectedOptionsMenuItem, value); }
    }

    public ObservableCollection<HamburgerMenuItem> MenuItems { get; } = new ObservableCollection<HamburgerMenuItem>()
    {
        new HamburgerMenuGlyphItem() { Label = Resources.ShellMainPage, Glyph = "\uE80F", TargetPageType = typeof(MainViewModel) },
        new HamburgerMenuGlyphItem() { Label = Resources.ShellWorkspacePage, Glyph = "\uE821", TargetPageType = typeof(WorkspaceViewModel) },
        new HamburgerMenuGlyphItem() { Label = Resources.ShellPeoplePage, Glyph = "\uE77B", TargetPageType = typeof(PeopleViewModel) },
        new HamburgerMenuGlyphItem() { Label = Resources.ShellPrepareDocumentsPage, Glyph = "\uE8A5", TargetPageType = typeof(PrepareDocumentsViewModel) },
        new HamburgerMenuGlyphItem() { Label = Resources.ShellTimeInputPage, Glyph = "\uE916", TargetPageType = typeof(TimeInputViewModel) },
        new HamburgerMenuGlyphItem() { Label = Resources.ShellResultsPage, Glyph = "\uE9F9", TargetPageType = typeof(ResultsViewModel) },
    };

    public ObservableCollection<HamburgerMenuItem> OptionMenuItems { get; } = new ObservableCollection<HamburgerMenuItem>()
    {
        new HamburgerMenuGlyphItem() { Label = Resources.ShellSettingsPage, Glyph = "\uE713", TargetPageType = typeof(SettingsViewModel) }
    };

    public RelayCommand GoBackCommand => _goBackCommand ?? (_goBackCommand = new RelayCommand(OnGoBack, CanGoBack));

    public ICommand MenuItemInvokedCommand => _menuItemInvokedCommand ?? (_menuItemInvokedCommand = new RelayCommand(OnMenuItemInvoked));

    public ICommand OptionsMenuItemInvokedCommand => _optionsMenuItemInvokedCommand ?? (_optionsMenuItemInvokedCommand = new RelayCommand(OnOptionsMenuItemInvoked));

    public ICommand LoadedCommand => _loadedCommand ?? (_loadedCommand = new RelayCommand(OnLoaded));

    public ICommand UnloadedCommand => _unloadedCommand ?? (_unloadedCommand = new RelayCommand(OnUnloaded));

    public ICommand ClosingCommand => _closingCommand ?? (_closingCommand = new RelayCommand<CancelEventArgs>(OnClosing));

    public ICommand SaveWorkspaceCommand => _saveWorkspaceCommand ?? (_saveWorkspaceCommand = new RelayCommand(async () => await _workspaceService?.Save(CancellationToken.None)));

    public ShellViewModel(INavigationService navigationService, IDialogCoordinator dialogCoordinator, IWorkspaceService workspaceService)
    {
        _navigationService = navigationService;
        _dialogCoordinator = dialogCoordinator;
        _workspaceService = workspaceService;
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

    private async void OnLoaded()
    {
        _navigationService.Navigated += OnNavigated;
        _workspaceService.PropertyChanged += _workspaceService_PropertyChanged;

        await LoadWorkspace();
    }

    private void OnUnloaded()
    {
        _navigationService.Navigated -= OnNavigated;
        _workspaceService.PropertyChanged -= _workspaceService_PropertyChanged;
    }

    // Close behaviour: https://github.com/MahApps/MahApps.Metro/issues/3535
    bool ForceClose = false;
    public event EventHandler WindowCloseRequested;
    protected void OnClosing(CancelEventArgs e)
    {
        // force method to abort - we'll force a close explicitly
        e.Cancel = true;

        if (ForceClose)
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
                await SaveWorkspace();
            }
            ForceClose = true;
            WindowCloseRequested?.Invoke(this, null);   // Notify the ShellWindow to close
        }, DispatcherPriority.Normal);
    }

    private bool CanGoBack()
        => _navigationService.CanGoBack;

    private void OnGoBack()
        => _navigationService.GoBack();

    private void OnMenuItemInvoked()
        => NavigateTo(SelectedMenuItem.TargetPageType);

    private void OnOptionsMenuItemInvoked()
        => NavigateTo(SelectedOptionsMenuItem.TargetPageType);

    private void NavigateTo(Type targetViewModel)
    {
        if (targetViewModel != null)
        {
            _navigationService.NavigateTo(targetViewModel.FullName);
        }
    }

    private void OnNavigated(object sender, string viewModelName)
    {
        var item = MenuItems
                    .OfType<HamburgerMenuItem>()
                    .FirstOrDefault(i => viewModelName == i.TargetPageType?.FullName);
        if (item != null)
        {
            SelectedMenuItem = item;
        }
        else
        {
            SelectedOptionsMenuItem = OptionMenuItems
                    .OfType<HamburgerMenuItem>()
                    .FirstOrDefault(i => viewModelName == i.TargetPageType?.FullName);
        }

        GoBackCommand.NotifyCanExecuteChanged();
    }

    private async Task LoadWorkspace()
    {
        try
        {
            await _workspaceService.Load(DefaultWorkspaceFolder, CancellationToken.None);
        }
        catch (Exception ex)
        {
            await _dialogCoordinator.ShowMessageAsync(this, Resources.ErrorString, ex.Message);
        }
    }

    private async Task SaveWorkspace()
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
}
