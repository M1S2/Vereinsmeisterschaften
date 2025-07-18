﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using Vereinsmeisterschaften.Contracts.Services;
using Vereinsmeisterschaften.Contracts.ViewModels;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Services;
using Vereinsmeisterschaften.Core.Settings;

namespace Vereinsmeisterschaften.ViewModels;

public class MainViewModel : ObservableObject, INavigationAware
{
    public ushort CompetitionYear => _workspaceService?.Settings?.GetSettingValue<ushort>(WorkspaceSettings.GROUP_GENERAL, WorkspaceSettings.SETTING_GENERAL_COMPETITIONYEAR) ?? 0;

    public ICommand WorkspaceCommand => _workspaceCommand ?? (_workspaceCommand = new RelayCommand(() => _navigationService.NavigateTo(typeof(WorkspaceViewModel).FullName)));
    public ICommand PeopleCommand => _peopleCommand ?? (_peopleCommand = new RelayCommand(() => _navigationService.NavigateTo(typeof(PeopleViewModel).FullName), () => _workspaceService.IsWorkspaceOpen));
    public ICommand PrepareRacesCommand => _prepareRacesCommand ?? (_prepareRacesCommand = new RelayCommand(() => _navigationService.NavigateTo(typeof(PrepareRacesViewModel).FullName), () => _workspaceService.IsWorkspaceOpen));
    public ICommand PrepareDocumentsCommand => _prepareDocumentsCommand ?? (_prepareDocumentsCommand = new RelayCommand(() => _navigationService.NavigateTo(typeof(CreateDocumentsViewModel).FullName), () => _workspaceService.IsWorkspaceOpen));
    public ICommand TimeInputCommand => _timeInputCommand ?? (_timeInputCommand = new RelayCommand(() => _navigationService.NavigateTo(typeof(TimeInputViewModel).FullName), () => _workspaceService.IsWorkspaceOpen));
    public ICommand ResultsCommand => _resultsCommand ?? (_resultsCommand = new RelayCommand(() => _navigationService.NavigateTo(typeof(ResultsViewModel).FullName), () => _workspaceService.IsWorkspaceOpen));
    
    private readonly INavigationService _navigationService;
    private ICommand _workspaceCommand;
    private ICommand _peopleCommand;
    private ICommand _prepareRacesCommand;
    private ICommand _prepareDocumentsCommand;
    private ICommand _timeInputCommand;
    private ICommand _resultsCommand;
    
    IWorkspaceService _workspaceService;

    public MainViewModel(INavigationService navigationService, IWorkspaceService workspaceService)
    {
        _navigationService = navigationService;
        _workspaceService = workspaceService;
    }

    private void _workspaceService_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(IWorkspaceService.Settings): OnPropertyChanged(nameof(CompetitionYear)); break;
            case nameof(IWorkspaceService.IsWorkspaceOpen):
                {
                    ((RelayCommand)PeopleCommand).NotifyCanExecuteChanged();
                    ((RelayCommand)PrepareRacesCommand).NotifyCanExecuteChanged();
                    ((RelayCommand)PrepareDocumentsCommand).NotifyCanExecuteChanged();
                    ((RelayCommand)TimeInputCommand).NotifyCanExecuteChanged();
                    ((RelayCommand)ResultsCommand).NotifyCanExecuteChanged();
                    break;
                }
            default: break;
        }
    }

    public void OnNavigatedTo(object parameter)
    {
        _workspaceService.PropertyChanged += _workspaceService_PropertyChanged;
    }

    public void OnNavigatedFrom()
    {
        _workspaceService.PropertyChanged -= _workspaceService_PropertyChanged;
    }
}
