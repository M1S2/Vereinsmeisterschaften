using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using Vereinsmeisterschaften.Contracts.ViewModels;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;
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
    private IWorkspaceService _workspaceService;
    private IDialogCoordinator _dialogCoordinator;
    private ShellViewModel _shellVM;

    /// <summary>
    /// Constructor of the competitions view model
    /// </summary>
    /// <param name="competitionService"><see cref="ICompetitionService"/> object</param>
    /// <param name="workspaceService"><see cref="IWorkspaceService"/> object</param>
    /// <param name="dialogCoordinator"><see cref="IDialogCoordinator"/> object</param>
    /// <param name="shellVM"><see cref="ShellViewModel"/> object used for dialog display</param>
    public CompetitionViewModel(ICompetitionService competitionService, IWorkspaceService workspaceService, IDialogCoordinator dialogCoordinator, ShellViewModel shellVM)
    {
        _competitionService = competitionService;
        _workspaceService = workspaceService;
        _dialogCoordinator = dialogCoordinator;
        _shellVM = shellVM;
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
        MessageDialogResult result = await _dialogCoordinator.ShowMessageAsync(_shellVM, Resources.RemoveCompetitionString, Resources.RemoveCompetitionConfirmationString, MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings() { NegativeButtonText = Resources.CancelString });
        if (result == MessageDialogResult.Affirmative)
        {
            _competitionService.RemoveCompetition(competition);
        }
    }));

    private ICommand _updateCompetitionTimesFromRudolphTableCommand;
    /// <summary>
    /// Command to update the competition times from a rudolph table.
    /// </summary>
    public ICommand UpdateCompetitionTimesFromRudolphTableCommand => _updateCompetitionTimesFromRudolphTableCommand ?? (_updateCompetitionTimesFromRudolphTableCommand = new RelayCommand(async () =>
    {
        OpenFileDialog fileDialog = new OpenFileDialog();
        fileDialog.InitialDirectory = _workspaceService.PersistentPath;
        fileDialog.Filter = Resources.FileDialogCsvFilterString;
        fileDialog.Title = Resources.SelectRudolphTableString;
        if (fileDialog.ShowDialog() == DialogResult.OK)
        {
            string inputStr = await _dialogCoordinator.ShowInputAsync(_shellVM, Resources.RudolphTableString, Resources.EnterRudolphScoreString, new MetroDialogSettings() { NegativeButtonText = Resources.CancelString });
            if (inputStr != null)
            {
                byte rudolphScore = 0;
                if (byte.TryParse(inputStr, out rudolphScore) && rudolphScore >= 1 && rudolphScore <= 20)
                {
                    _competitionService.UpdateAllCompetitionTimesFromRudolphTable(fileDialog.FileName, rudolphScore);
                    await _dialogCoordinator.ShowMessageAsync(_shellVM, Resources.RudolphTableString, Resources.FinishedUpdateFromRudolphTableString);
                }
                else
                {
                    await _dialogCoordinator.ShowMessageAsync(_shellVM, Resources.ErrorString, Resources.ErrorUpdatingFromRudolphTableString);
                }
            }
            else
            {
                // User canceled
            }
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
