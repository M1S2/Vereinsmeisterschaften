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

    private ResultTypes _resultType = ResultTypes.Overall;
    public ResultTypes ResultType
    {
        get => _resultType;
        set
        {
            SetProperty(ref _resultType, value);
            SortedPersons = _scoreService.GetPersonsSortedByScore(_resultType);
            OnPropertyChanged(nameof(PodiumGoldStarts));
            OnPropertyChanged(nameof(PodiumSilverStarts));
            OnPropertyChanged(nameof(PodiumBronzeStarts));
        }
    }

    public List<PersonStart> PodiumGoldStarts => _scoreService.GetWinnersPodiumStarts(ResultType, ResultPodiumsPlaces.Gold);
    public List<PersonStart> PodiumSilverStarts => _scoreService.GetWinnersPodiumStarts(ResultType, ResultPodiumsPlaces.Silver);
    public List<PersonStart> PodiumBronzeStarts => _scoreService.GetWinnersPodiumStarts(ResultType, ResultPodiumsPlaces.Bronze);

    private IScoreService _scoreService;

    public ResultsViewModel(IScoreService scoreService)
    {
        _scoreService = scoreService;
        ResultType = ResultTypes.Overall;
    }

    public void OnNavigatedFrom()
    {
    }

    public void OnNavigatedTo(object parameter)
    {
    }
}
