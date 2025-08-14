using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Windows.Data;
using System.Windows.Input;
using Vereinsmeisterschaften.Contracts.ViewModels;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;
using Vereinsmeisterschaften.Core.Services;
using Vereinsmeisterschaften.Properties;

namespace Vereinsmeisterschaften.ViewModels;

/// <summary>
/// ViewModel for the competitions page
/// </summary>
public class CompetitionViewModel : ObservableObject, INavigationAware
{
    private ObservableCollection<Competition> _competitions;
    /// <summary>
    /// List of competitions shown on the competition page
    /// </summary>
    public ObservableCollection<Competition> Competitions
    {
        get => _competitions;
        set => SetProperty(ref _competitions, value);
    }

    private ICollectionView _competitionsCollectionView;
    /// <summary>
    /// CollectionView used to display the list of competitions
    /// </summary>
    public ICollectionView CompetitionsCollectionView
    {
        get => _competitionsCollectionView;
        private set => SetProperty(ref _competitionsCollectionView, value);
    }

    private Competition _selectedCompetition;
    /// <summary>
    /// Currently selected <see cref="Competition"/>
    /// </summary>
    public Competition SelectedCompetition
    {
        get => _selectedCompetition;
        set => SetProperty(ref _selectedCompetition, value);
    }

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    private List<SwimmingStyles> _availableSwimmingStyles = Enum.GetValues(typeof(SwimmingStyles)).Cast<SwimmingStyles>().Where(s => s != SwimmingStyles.Unknown).ToList();
    /// <summary>
    /// List with all available <see cref="SwimmingStyles"/>
    /// </summary>
    public List<SwimmingStyles> AvailableSwimmingStyles => _availableSwimmingStyles;

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    private ICompetitionService _competitionService;
    private IDialogCoordinator _dialogCoordinator;

    /// <summary>
    /// Constructor of the competitions view model
    /// </summary>
    /// <param name="competitionService"><see cref="ICompetitionService"/> object</param>
    /// <param name="dialogCoordinator"><see cref="IDialogCoordinator"/> object</param>
    public CompetitionViewModel(ICompetitionService competitionService, IDialogCoordinator dialogCoordinator)
    {
        _competitionService = competitionService;
        _dialogCoordinator = dialogCoordinator;
    }

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    private ICommand _addCompetitionCommand;
    /// <summary>
    /// Command to add a new competition
    /// </summary>
    public ICommand AddCompetitionCommand => _addCompetitionCommand ?? (_addCompetitionCommand = new RelayCommand(() =>
    {
        Competition competition = new Competition();
        _competitionService.AddCompetition(competition);
        SelectedCompetition = competition;
    }));

    private ICommand _removeCompetitionCommand;
    /// <summary>
    /// Command to remove a competition from the list
    /// </summary>
    public ICommand RemoveCompetitionCommand => _removeCompetitionCommand ?? (_removeCompetitionCommand = new RelayCommand<Competition>(async (competition) =>
    {
        MessageDialogResult result = await _dialogCoordinator.ShowMessageAsync(this, Resources.RemoveCompetitionString, Resources.RemoveCompetitionConfirmationString, MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings() { NegativeButtonText = Resources.CancelString });
        if (result == MessageDialogResult.Affirmative)
        {
            _competitionService.RemoveCompetition(competition);
        }
    }));

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    /// <inheritdoc/>
    public void OnNavigatedTo(object parameter)
    {
        Competitions = _competitionService?.GetCompetitions();
        CompetitionsCollectionView = CollectionViewSource.GetDefaultView(Competitions);
        SelectedCompetition = Competitions?.FirstOrDefault();
    }

    /// <inheritdoc/>
    public void OnNavigatedFrom()
    {
    }
}
