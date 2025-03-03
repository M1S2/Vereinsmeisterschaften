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

    public ICommand WorkspaceCommand => _workspaceCommand ?? (_workspaceCommand = new RelayCommand(() => _navigationService.NavigateTo(typeof(WorkspaceViewModel).FullName)));
    public ICommand PeopleCommand => _peopleCommand ?? (_peopleCommand = new RelayCommand(() => _navigationService.NavigateTo(typeof(PeopleViewModel).FullName), () => _workspaceService.IsWorkspaceOpen));
    public ICommand PrepareDocumentsCommand => _prepareDocumentsCommand ?? (_prepareDocumentsCommand = new RelayCommand(() => _navigationService.NavigateTo(typeof(PrepareDocumentsViewModel).FullName), () => _workspaceService.IsWorkspaceOpen));
    public ICommand TimeInputCommand => _timeInputCommand ?? (_timeInputCommand = new RelayCommand(() => _navigationService.NavigateTo(typeof(TimeInputViewModel).FullName), () => _workspaceService.IsWorkspaceOpen));
    public ICommand ResultsCommand => _resultsCommand ?? (_resultsCommand = new RelayCommand(() => _navigationService.NavigateTo(typeof(ResultsViewModel).FullName), () => _workspaceService.IsWorkspaceOpen));
    
    private readonly INavigationService _navigationService;
    private ICommand _workspaceCommand;
    private ICommand _peopleCommand;
    private ICommand _prepareDocumentsCommand;
    private ICommand _timeInputCommand;
    private ICommand _resultsCommand;
    
    IWorkspaceService _workspaceService;

    public MainViewModel(INavigationService navigationService, IWorkspaceService workspaceService)
    {
        _navigationService = navigationService;
        _workspaceService = workspaceService;

        workspaceService.OnWorkspaceFinished += (sender, e) => CompetitionYear = workspaceService?.Settings?.CompetitionYear ?? 0;
        workspaceService.OnIsWorkspaceOpenChanged += (sender, e) =>
        {
            ((RelayCommand)PeopleCommand).NotifyCanExecuteChanged();
            ((RelayCommand)PrepareDocumentsCommand).NotifyCanExecuteChanged();
            ((RelayCommand)TimeInputCommand).NotifyCanExecuteChanged();
            ((RelayCommand)ResultsCommand).NotifyCanExecuteChanged();
        };
    }

    public void OnNavigatedTo(object parameter)
    {
        CompetitionYear = _workspaceService?.Settings?.CompetitionYear ?? 0;
    }

    public void OnNavigatedFrom()
    {
    }
}
