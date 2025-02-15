using System;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Vereinsmeisterschaften.Contracts.Services;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Properties;

namespace Vereinsmeisterschaften.ViewModels;

public class ShellViewModel : ObservableObject
{
#warning Change to non-fixed path!!!
    public const string DefaultWorkspaceFolder = @"C:\Users\Markus\Desktop\VM_TestData\Data2";
    public CancellationTokenSource WorkspaceCancellationTokenSource = new CancellationTokenSource();

    public string CurrentWorkspaceFolder => _workspaceService.WorkspaceFolderPath;

    private readonly INavigationService _navigationService;
    private HamburgerMenuItem _selectedMenuItem;
    private HamburgerMenuItem _selectedOptionsMenuItem;
    private RelayCommand _goBackCommand;
    private ICommand _menuItemInvokedCommand;
    private ICommand _optionsMenuItemInvokedCommand;
    private ICommand _loadedCommand;
    private ICommand _unloadedCommand;
    private ICommand _closingCommand;
    private IDialogCoordinator _dialogCoordinator;
    private ProgressDialogController _progressDialogController;

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

    public ICommand ClosingCommand => _closingCommand ?? (_closingCommand = new RelayCommand(OnClosing));

    public ShellViewModel(INavigationService navigationService, IDialogCoordinator dialogCoordinator, IWorkspaceService workspaceService)
    {
        _navigationService = navigationService;
        _dialogCoordinator = dialogCoordinator;
        _workspaceService = workspaceService;

        _workspaceService.OnWorkspaceProgress += (sender, p, currentStep) =>
        {
            //_progressDialogController?.SetProgress(p / 100);
            //_progressDialogController?.SetMessage((p / 100).ToString("P0") + Environment.NewLine + currentStep);   // Format to percentage with 0 decimal digits
        };

        _workspaceService.OnWorkspaceFinished += (sender, e) =>
        {
            try
            {
                _progressDialogController?.CloseAsync();
            }
            catch (Exception) { /* Nothing to do here. Seems already to be closed.*/ }
        };
    }

    private async void OnLoaded()
    {
        _navigationService.Navigated += OnNavigated;

        await LoadWorkspace();
    }

    private void OnUnloaded()
    {
        _navigationService.Navigated -= OnNavigated;
    }

    private void OnClosing()
    {
        SaveWorkspace(false);
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

    private async Task LoadWorkspace(bool showProgressDialog = true)
    {
        WorkspaceCancellationTokenSource = new CancellationTokenSource();
        if (showProgressDialog)
        {
            _progressDialogController = await _dialogCoordinator.ShowProgressAsync(this, Resources.LoadWorkspaceString, "", true);
            _progressDialogController.Canceled += (sender, e) => WorkspaceCancellationTokenSource.Cancel();
        }
        try
        {
            await _workspaceService.OpenWorkspace(DefaultWorkspaceFolder, WorkspaceCancellationTokenSource.Token);
            OnPropertyChanged(nameof(CurrentWorkspaceFolder));
        }
        catch (Exception ex)
        {
            if (showProgressDialog)
            {
                await _progressDialogController.CloseAsync();
                await _dialogCoordinator.ShowMessageAsync(this, Resources.ErrorString, ex.Message);
            }
        }
    }

    private async Task SaveWorkspace(bool showProgressDialog = true)
    {
        WorkspaceCancellationTokenSource = new CancellationTokenSource();
        if (showProgressDialog)
        {
            _progressDialogController = await _dialogCoordinator.ShowProgressAsync(this, Resources.SaveWorkspaceString, "", true);
            _progressDialogController.Canceled += (sender, e) => WorkspaceCancellationTokenSource.Cancel();
        }
        try
        {
            await _workspaceService.SaveWorkspace(WorkspaceCancellationTokenSource.Token);
        }
        catch (Exception ex)
        {
            if (showProgressDialog)
            {
                await _progressDialogController.CloseAsync();
                await _dialogCoordinator.ShowMessageAsync(this, Resources.ErrorString, ex.Message);
            }
        }
    }
}
