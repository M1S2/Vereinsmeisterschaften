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
            OnPropertyChanged(nameof(BestStartsForResultType));
        }
    }

    public List<PersonStart> BestStartsForResultType => _scoreService.GetBestPersonStarts(ResultType);

#warning ResultTypes are not localized in the UI at the moment !!!
    private List<ResultTypes> _availableResultTypes = Enum.GetValues(typeof(ResultTypes)).Cast<ResultTypes>().ToList();
    public List<ResultTypes> AvailableResultTypes => _availableResultTypes;

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
        ResultType = ResultTypes.Overall;
    }
}
