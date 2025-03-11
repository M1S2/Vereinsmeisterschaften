using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Net.NetworkInformation;
using Vereinsmeisterschaften.Contracts.ViewModels;
using Vereinsmeisterschaften.Core.Contracts.Services;

namespace Vereinsmeisterschaften.ViewModels;

public class PrepareDocumentsViewModel : ObservableObject, INavigationAware
{
    private ICompetitionService _competitionService;
    private IWorkspaceService _workspaceService;

    public PrepareDocumentsViewModel(ICompetitionService competitionService, IWorkspaceService workspaceService)
    {
        _competitionService = competitionService;
        _workspaceService = workspaceService;
    }

    public void OnNavigatedTo(object parameter)
    {
        _competitionService.CalculateRunOrder(_workspaceService?.Settings?.CompetitionYear ?? 0);
    }

    public void OnNavigatedFrom()
    {
    }
}
