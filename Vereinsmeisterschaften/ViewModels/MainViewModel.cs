using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Vereinsmeisterschaften.Contracts.Services;
using Vereinsmeisterschaften.Contracts.ViewModels;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Settings;

namespace Vereinsmeisterschaften.ViewModels;

/// <summary>
/// View model for the main view of the application.
/// </summary>
public class MainViewModel : ObservableObject
{
    /// <summary>
    /// Competition year setting value get from the workspace settings.
    /// </summary>
    public ushort CompetitionYear => _workspaceService?.Settings?.GetSettingValue<ushort>(WorkspaceSettings.GROUP_GENERAL, WorkspaceSettings.SETTING_GENERAL_COMPETITIONYEAR) ?? 0;

    /// <summary>
    /// Command to navigate to the workspace view.
    /// </summary>
    public ICommand WorkspaceCommand => _workspaceCommand ?? (_workspaceCommand = new RelayCommand(() => _navigationService.NavigateTo(typeof(WorkspaceViewModel).FullName)));

    /// <summary>
    /// Command to navigate to the competition view.
    /// </summary>
    public ICommand CompetitionCommand => _competitionCommand ?? (_competitionCommand = new RelayCommand(() => _navigationService.NavigateTo(typeof(CompetitionViewModel).FullName), () => _workspaceService.IsWorkspaceOpen));

    /// <summary>
    /// Command to navigate to the people view.
    /// </summary>
    public ICommand PeopleCommand => _peopleCommand ?? (_peopleCommand = new RelayCommand(() => _navigationService.NavigateTo(typeof(PeopleViewModel).FullName), () => _workspaceService.IsWorkspaceOpen));

    /// <summary>
    /// Command to navigate to the prepare races view.
    /// </summary>
    public ICommand PrepareRacesCommand => _prepareRacesCommand ?? (_prepareRacesCommand = new RelayCommand(() => _navigationService.NavigateTo(typeof(PrepareRacesViewModel).FullName), () => _workspaceService.IsWorkspaceOpen));

    /// <summary>
    /// Command to navigate to the prepare documents view.
    /// </summary>
    public ICommand PrepareDocumentsCommand => _prepareDocumentsCommand ?? (_prepareDocumentsCommand = new RelayCommand(() => _navigationService.NavigateTo(typeof(CreateDocumentsViewModel).FullName), () => _workspaceService.IsWorkspaceOpen));

    /// <summary>
    /// Command to navigate to the time input view.
    /// </summary>
    public ICommand TimeInputCommand => _timeInputCommand ?? (_timeInputCommand = new RelayCommand(() => _navigationService.NavigateTo(typeof(TimeInputViewModel).FullName), () => _workspaceService.IsWorkspaceOpen));

    /// <summary>
    /// Command to navigate to the results view.
    /// </summary>
    public ICommand ResultsCommand => _resultsCommand ?? (_resultsCommand = new RelayCommand(() => _navigationService.NavigateTo(typeof(ResultsViewModel).FullName), () => _workspaceService.IsWorkspaceOpen));
    
    private readonly INavigationService _navigationService;
    private ICommand _workspaceCommand;
    private ICommand _competitionCommand;
    private ICommand _peopleCommand;
    private ICommand _prepareRacesCommand;
    private ICommand _prepareDocumentsCommand;
    private ICommand _timeInputCommand;
    private ICommand _resultsCommand;
    
    private IWorkspaceService _workspaceService;

    /// <summary>
    /// Constructor for the MainViewModel.
    /// </summary>
    /// <param name="navigationService"><see cref="INavigationService"/> object</param>
    /// <param name="workspaceService"><see cref="IWorkspaceService"/> object</param>
    public MainViewModel(INavigationService navigationService, IWorkspaceService workspaceService)
    {
        _navigationService = navigationService;
        _workspaceService = workspaceService;
        _workspaceService.PropertyChanged += _workspaceService_PropertyChanged;
    }

    private void _workspaceService_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(IWorkspaceService.Settings): OnPropertyChanged(nameof(CompetitionYear)); break;
            case nameof(IWorkspaceService.IsWorkspaceOpen):
                {
                    ((RelayCommand)CompetitionCommand).NotifyCanExecuteChanged();
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
}
