using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Vereinsmeisterschaften.Contracts.ViewModels;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.ViewModels;

public class ResultsViewModel : ObservableObject, INavigationAware
{
    private IScoreService _scoreService;
    private IWorkspaceService _workspaceService;

    public ResultsViewModel(IScoreService scoreService, IWorkspaceService workspaceService)
    {
        _scoreService = scoreService;
        _workspaceService = workspaceService;
    }

    public void OnNavigatedFrom()
    {
    }

    public void OnNavigatedTo(object parameter)
    {
        _scoreService.UpdateScoresForAllPersons(_workspaceService.Settings.CompetitionYear);
    }
}
