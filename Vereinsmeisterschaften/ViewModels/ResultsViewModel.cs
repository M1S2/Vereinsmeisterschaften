using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Vereinsmeisterschaften.Contracts.ViewModels;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.ViewModels;

public class ResultsViewModel : ObservableObject, INavigationAware
{
    private List<Person> _sortedPersons;
    public List<Person> SortedPersons
    {
        get => _sortedPersons;
        set => SetProperty(ref _sortedPersons, value);
    }

    private IScoreService _scoreService;

    public ResultsViewModel(IScoreService scoreService)
    {
        _scoreService = scoreService;
    }

    public void OnNavigatedFrom()
    {
    }

    public void OnNavigatedTo(object parameter)
    {
        SortedPersons = _scoreService.GetPersonsSortedByScore();
    }
}
