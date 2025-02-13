using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using Vereinsmeisterschaften.Contracts.Services;
using Vereinsmeisterschaften.Contracts.ViewModels;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Services;

namespace Vereinsmeisterschaften.ViewModels;

public class MainViewModel : ObservableObject, INavigationAware
{
    private ushort _competitionYear;
    public ushort CompetitionYear
    {
        get => _competitionYear;
        set => SetProperty(ref _competitionYear, value);
    }

    public ICommand PeopleCommand => _peopleCommand ?? (_peopleCommand = new RelayCommand(() => _navigationService.NavigateTo(typeof(PeopleViewModel).FullName)));
    public ICommand PrepareDocumentsCommand => _prepareDocumentsCommand ?? (_prepareDocumentsCommand = new RelayCommand(() => _navigationService.NavigateTo(typeof(PrepareDocumentsViewModel).FullName)));
    public ICommand TimeInputCommand => _timeInputCommand ?? (_timeInputCommand = new RelayCommand(() => _navigationService.NavigateTo(typeof(TimeInputViewModel).FullName)));
    public ICommand ResultsCommand => _resultsCommand ?? (_resultsCommand = new RelayCommand(() => _navigationService.NavigateTo(typeof(ResultsViewModel).FullName)));
    public ICommand SettingsCommand => _settingsCommand ?? (_settingsCommand = new RelayCommand(() => _navigationService.NavigateTo(typeof(SettingsViewModel).FullName)));

    private readonly INavigationService _navigationService;
    private ICommand _peopleCommand;
    private ICommand _prepareDocumentsCommand;
    private ICommand _timeInputCommand;
    private ICommand _resultsCommand;
    private ICommand _settingsCommand;

    IWorkspaceService _workspaceService;

    public MainViewModel(INavigationService navigationService, IWorkspaceService workspaceService)
    {
        _navigationService = navigationService;
        _workspaceService = workspaceService;

        workspaceService.OnWorkspaceFinished += (sender, e) => CompetitionYear = workspaceService?.Settings?.CompetitionYear ?? 0;
    }

    public void OnNavigatedTo(object parameter)
    {
        CompetitionYear = _workspaceService?.Settings?.CompetitionYear ?? 0;
    }

    public void OnNavigatedFrom()
    {
    }
}
