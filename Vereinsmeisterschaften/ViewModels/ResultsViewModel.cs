using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Data;
using Vereinsmeisterschaften.Contracts.ViewModels;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;
using Vereinsmeisterschaften.Core.Services;
using Vereinsmeisterschaften.Core.Settings;

namespace Vereinsmeisterschaften.ViewModels;

/// <summary>
/// ViewModel for the results page, showing the sorted persons based on their scores.
/// </summary>
public partial class ResultsViewModel : ObservableObject, INavigationAware
{
    /// <summary>
    /// List with all persons sorted by score
    /// </summary>
    [ObservableProperty]
    private List<Person> _sortedPersons;

    private ResultTypes _resultType = ResultTypes.Overall;
    /// <summary>
    /// Type of result to view
    /// </summary>
    public ResultTypes ResultType
    {
        get => _resultType;
        set
        {
            if (SetProperty(ref _resultType, value))
            {
                updatePersonsAndPlaces();
            }
        }
    }

    /// <summary>
    /// List with all starts on the gold podium place (highest scores)
    /// </summary>
    public List<PersonStart> PodiumGoldStarts => _scoreService.GetWinnersPodiumStarts(ResultType, ResultPodiumsPlaces.Gold);

    /// <summary>
    /// List with all starts on the silver podium place (second highest scores)
    /// </summary>
    public List<PersonStart> PodiumSilverStarts => _scoreService.GetWinnersPodiumStarts(ResultType, ResultPodiumsPlaces.Silver);

    /// <summary>
    /// List with all starts on the bronze podium place (third highest scores)
    /// </summary>
    public List<PersonStart> PodiumBronzeStarts => _scoreService.GetWinnersPodiumStarts(ResultType, ResultPodiumsPlaces.Bronze);

    private IScoreService _scoreService;

    /// <summary>
    /// Constructor of the results view model
    /// </summary>
    /// <param name="scoreService"><see cref="IScoreService"/></param>
    public ResultsViewModel(IScoreService scoreService)
    {
        _scoreService = scoreService;
        ResultType = ResultTypes.Overall;
        updatePersonsAndPlaces();
    }

    /// <inheritdoc/>
    public void OnNavigatedTo(object parameter)
    {
        updatePersonsAndPlaces();
    }

    /// <inheritdoc/>
    public void OnNavigatedFrom()
    {
    }

    private void updatePersonsAndPlaces()
    {
        SortedPersons = _scoreService.GetPersonsSortedByScore(ResultType, true);
        _scoreService.UpdateResultListPlacesForAllPersons();
        OnPropertyChanged(nameof(PodiumGoldStarts));
        OnPropertyChanged(nameof(PodiumSilverStarts));
        OnPropertyChanged(nameof(PodiumBronzeStarts));
    }
}
