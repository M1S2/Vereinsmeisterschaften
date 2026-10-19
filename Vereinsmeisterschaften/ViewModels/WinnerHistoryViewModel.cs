using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Vereinsmeisterschaften.Contracts.ViewModels;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;
using Vereinsmeisterschaften.Core.Helpers;

namespace Vereinsmeisterschaften.ViewModels;

/// <summary>
/// ViewModel for the winner history page.
/// </summary>
public partial class WinnerHistoryViewModel : ObservableObject, INavigationAware
{
    public WinnerHistoryElement CurrentWinners => _winnerHistoryService.CurrentWinners;

    public bool CurrentWinnersHasAtLeastOneWinner
    {
        get
        {
            WinnerHistoryElement currentWinners = CurrentWinners;
            return currentWinners?.WinnersGold?.Count > 0 || currentWinners?.WinnersSilver?.Count > 0 || currentWinners?.WinnersBronze?.Count > 0;
        }
    }

    public string CurrentWinnersInfoTextGold => getCurrentWinnerInfoText(ResultPodiumsPlaces.Gold);
    public string CurrentWinnersInfoTextSilver => getCurrentWinnerInfoText(ResultPodiumsPlaces.Silver);
    public string CurrentWinnersInfoTextBronze => getCurrentWinnerInfoText(ResultPodiumsPlaces.Bronze);

    private string getCurrentWinnerInfoText(ResultPodiumsPlaces podiumsPlace)
    {
        string infoString = "";
        Dictionary<WinnerName, int> previousWinnersCount = _winnerHistoryService.GetPreviousWinnersCountPerPerson(podiumsPlace, true);
        IEnumerable<WinnerName> currentWinnersOfPodiumPlace = podiumsPlace switch
        {
            ResultPodiumsPlaces.Gold => CurrentWinners?.WinnersGold ?? Enumerable.Empty<WinnerName>(),
            ResultPodiumsPlaces.Silver => CurrentWinners?.WinnersSilver ?? Enumerable.Empty<WinnerName>(),
            ResultPodiumsPlaces.Bronze => CurrentWinners?.WinnersBronze ?? Enumerable.Empty<WinnerName>(),
            _ => (CurrentWinners?.WinnersGold ?? Enumerable.Empty<WinnerName>()).Concat(CurrentWinners?.WinnersSilver ?? Enumerable.Empty<WinnerName>()).Concat(CurrentWinners?.WinnersBronze ?? Enumerable.Empty<WinnerName>()).ToList()
        };
        foreach (WinnerName name in currentWinnersOfPodiumPlace)
        {
            int numberOfOccurencesInPreviousWinners = (previousWinnersCount.ContainsKey(name) ? previousWinnersCount[name] : 0);
            infoString += string.Format(Properties.Resources.CurrentWinnersInfoFormatString, name.ToString(), numberOfOccurencesInPreviousWinners + 1, EnumCoreLocalizedStringHelper.Convert(podiumsPlace));
            infoString += Environment.NewLine;
        }
        return infoString.Trim(Environment.NewLine.ToCharArray());
    }

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public ObservableCollection<WinnerHistoryElement> PreviousWinners => _winnerHistoryService.GetPreviousWinnersList();

    public bool IsCurrentWinnersPartOfPreviousWinners => _winnerHistoryService.IsCurrentWinnersPartOfPreviousWinners;

    public int IndexOfCurrentWinnersInsidePreviousWinners => _winnerHistoryService.IndexOfCurrentWinnersInsidePreviousWinners;

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    private ICommand _addCurrentWinnersToPreviousCommand;
    /// <summary>
    /// Command to add the current winners to the list of previous ones.
    /// </summary>
    public ICommand AddCurrentWinnersToPreviousCommand => _addCurrentWinnersToPreviousCommand ?? (_addCurrentWinnersToPreviousCommand = new RelayCommand(() =>
    {
        _winnerHistoryService.CopyCurrentWinnersToPrevious();
        _winnerHistoryService.UpdateCurrentWinners();
        OnPropertyChanged(nameof(CurrentWinners));
        OnPropertyChanged(nameof(PreviousWinners));
        OnPropertyChanged(nameof(IsCurrentWinnersPartOfPreviousWinners));
        OnPropertyChanged(nameof(IndexOfCurrentWinnersInsidePreviousWinners));
        OnPropertyChanged(nameof(CurrentWinnersHasAtLeastOneWinner));
        OnPropertyChanged(nameof(CurrentWinnersInfoTextGold));
        OnPropertyChanged(nameof(CurrentWinnersInfoTextSilver));
        OnPropertyChanged(nameof(CurrentWinnersInfoTextBronze));
    }));

    private ICommand _removeCurrentWinnersFromPreviousCommand;
    /// <summary>
    /// Command to remove the current winners from the list of previous ones.
    /// </summary>
    public ICommand RemoveCurrentWinnersFromPreviousCommand => _removeCurrentWinnersFromPreviousCommand ?? (_removeCurrentWinnersFromPreviousCommand = new RelayCommand(() =>
    {
        _winnerHistoryService.RemoveCurrentWinnersFromPrevious();
        _winnerHistoryService.UpdateCurrentWinners();
        OnPropertyChanged(nameof(CurrentWinners));
        OnPropertyChanged(nameof(PreviousWinners));
        OnPropertyChanged(nameof(IsCurrentWinnersPartOfPreviousWinners));
        OnPropertyChanged(nameof(IndexOfCurrentWinnersInsidePreviousWinners));
        OnPropertyChanged(nameof(CurrentWinnersHasAtLeastOneWinner));
        OnPropertyChanged(nameof(CurrentWinnersInfoTextGold));
        OnPropertyChanged(nameof(CurrentWinnersInfoTextSilver));
        OnPropertyChanged(nameof(CurrentWinnersInfoTextBronze));
    }));

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    private IWinnerHistoryService _winnerHistoryService;

    /// <summary>
    /// Constructor of the <see cref="WinnerHistoryViewModel"/>
    /// </summary>
    /// <param name="winnerHistoryService"><see cref="IWinnerHistoryService"/></param>
    public WinnerHistoryViewModel(IWinnerHistoryService winnerHistoryService)
    {
        _winnerHistoryService = winnerHistoryService;
    }

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    /// <inheritdoc/>
    public void OnNavigatedTo(object parameter)
    {
        _winnerHistoryService.UpdateCurrentWinners();
        _winnerHistoryService.UpdatePreviousWinnersHistoryFlags();
        OnPropertyChanged(nameof(CurrentWinners));
        OnPropertyChanged(nameof(PreviousWinners));
        OnPropertyChanged(nameof(IsCurrentWinnersPartOfPreviousWinners));
        OnPropertyChanged(nameof(IndexOfCurrentWinnersInsidePreviousWinners));
        OnPropertyChanged(nameof(CurrentWinnersHasAtLeastOneWinner));
        OnPropertyChanged(nameof(CurrentWinnersInfoTextGold));
        OnPropertyChanged(nameof(CurrentWinnersInfoTextSilver));
        OnPropertyChanged(nameof(CurrentWinnersInfoTextBronze));
    }

    /// <inheritdoc/>
    public void OnNavigatedFrom()
    {
    }
}
