using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Vereinsmeisterschaften.Contracts.Services;
using Vereinsmeisterschaften.Contracts.ViewModels;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;
using Vereinsmeisterschaften.Core.Services;

namespace Vereinsmeisterschaften.ViewModels;

public class PrepareRacesViewModel : ObservableObject, INavigationAware
{
    #region Calculated Races

    public List<CompetitionRaces> CalculatedCompetitionRaces => _raceService?.LastCalculatedCompetitionRaces;

    public CompetitionRaces BestCompetitionRaces => _raceService?.BestCompetitionRaces;

    public bool AreRacesAvailable => CalculatedCompetitionRaces?.Count > 0 || BestCompetitionRaces != null;

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    private int _indexCurrentCompetitionRace;
    public int IndexCurrentCompetitionRace
    {
        get => _indexCurrentCompetitionRace;
        set
        {
            if (value >= 0 && value < CalculatedCompetitionRaces?.Count)
            {
                SetProperty(ref _indexCurrentCompetitionRace, value);
                CurrentCompetitionRace = CalculatedCompetitionRaces[_indexCurrentCompetitionRace];
            }
            else if(value == -1)
            {
                SetProperty(ref _indexCurrentCompetitionRace, value);
                CurrentCompetitionRace = BestCompetitionRaces;
            }
        }
    }

    public int IndexCurrentCompetitionRaceDisplay
    {
        get => IndexCurrentCompetitionRace + 1;
        set => IndexCurrentCompetitionRace = value - 1;
    }

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    private CompetitionRaces _currentCompetitionRace;
    public CompetitionRaces CurrentCompetitionRace
    {
        get => _currentCompetitionRace;
        set
        {
            SetProperty(ref _currentCompetitionRace, value);
            updateNotAssignedStarts();
            if (_currentCompetitionRace != null)
            {
                _currentCompetitionRace.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == nameof(CompetitionRaces.IsValid))
                    {
                        OnPropertyChanged(nameof(CurrentCompetitionRaceIsValid));
                    }
                };
            }
        }
    }

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    private List<PersonStart> _notAssignedStarts;
    public List<PersonStart> NotAssignedStarts
    {
        get => _notAssignedStarts;
        set => SetProperty(ref _notAssignedStarts, value);
    }

    private void updateNotAssignedStarts()
    {
        List<PersonStart> allStarts = _personService?.GetAllPersonStarts();
        List<PersonStart> raceStarts = CurrentCompetitionRace?.GetAllStarts();
        if (allStarts == null)
        {
            NotAssignedStarts = new List<PersonStart>();
        }
        else if(raceStarts == null)
        {
            NotAssignedStarts = allStarts;
        }
        else
        {
            NotAssignedStarts = allStarts?.Except(raceStarts)?.ToList();
        }
    }

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// The <see cref="CurrentCompetitionRace"/> is consideres valid when:
    /// - The <see cref="CompetitionRaces.IsValid"/> property is true
    /// - There are no empty unassigned races
    /// </summary>
    public bool CurrentCompetitionRaceIsValid => (CurrentCompetitionRace?.IsValid ?? true) && NotAssignedStarts.Count == 0;

    #endregion

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    #region Highlight Feature

    private HighlightPersonStartModes _highlightPersonStartMode;
    public HighlightPersonStartModes HighlightPersonStartMode
    {
        get => _highlightPersonStartMode;
        set { SetProperty(ref _highlightPersonStartMode, value); recalculateHighlightedPersonStarts(); }
    }

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    public ObservableCollection<Person> AvailablePersons => _personService?.GetPersons();

    private Person _highlightedPerson;
    public Person HighlightedPerson
    {
        get => _highlightedPerson;
        set { SetProperty(ref _highlightedPerson, value); recalculateHighlightedPersonStarts(); }
    }

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    private List<SwimmingStyles> _availableSwimmingStyles = Enum.GetValues(typeof(SwimmingStyles)).Cast<SwimmingStyles>().Where(s => s != SwimmingStyles.Unknown).ToList();
    public List<SwimmingStyles> AvailableSwimmingStyles => _availableSwimmingStyles;

    private SwimmingStyles _highlightedSwimmingStyle;
    public SwimmingStyles HighlightedSwimmingStyle
    {
        get => _highlightedSwimmingStyle;
        set { SetProperty(ref _highlightedSwimmingStyle, value); recalculateHighlightedPersonStarts(); }
    }

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    private Genders _highlightedGender;
    public Genders HighlightedGender
    {
        get => _highlightedGender;
        set { SetProperty(ref _highlightedGender, value); recalculateHighlightedPersonStarts(); }
    }

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    private void recalculateHighlightedPersonStarts()
    {
        List<PersonStart> personStarts = _personService.GetAllPersonStarts();
        foreach (PersonStart personStart in personStarts)
        {
            switch (HighlightPersonStartMode)
            {
                case HighlightPersonStartModes.Person: personStart.IsHighlighted = personStart.PersonObj.Equals(HighlightedPerson); break;
                case HighlightPersonStartModes.SwimmingStyle: personStart.IsHighlighted = personStart.Style.Equals(HighlightedSwimmingStyle); break;
                case HighlightPersonStartModes.Gender: personStart.IsHighlighted = personStart.PersonObj.Gender.Equals(HighlightedGender); break;
                case HighlightPersonStartModes.All50m: personStart.IsHighlighted = personStart.CompetitionObj?.Distance == 50; break;
                case HighlightPersonStartModes.All100m: personStart.IsHighlighted = personStart.CompetitionObj?.Distance == 100; break;
                case HighlightPersonStartModes.All200m: personStart.IsHighlighted = personStart.CompetitionObj?.Distance == 200; break;
                case HighlightPersonStartModes.None: personStart.IsHighlighted = false; break;
                default: personStart.IsHighlighted = false; break;
            }
        }
    }

    #endregion

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public DropAllowedHandler DropAllowedHandlerObj { get; } = new DropAllowedHandler();

    public DropAllowedHandlerParkingLot DropAllowedHandlerParkingLotObj { get; } = new DropAllowedHandlerParkingLot();

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    private IRaceService _raceService;
    private IWorkspaceService _workspaceService;
    private IPersonService _personService;
    private IDialogCoordinator _dialogCoordinator;
    private ProgressDialogController _progressController;

    public PrepareRacesViewModel(IRaceService raceService, IWorkspaceService workspaceService, IPersonService personService, IDialogCoordinator dialogCoordinator)
    {
        _raceService = raceService;
        _workspaceService = workspaceService;
        _personService = personService;
        _dialogCoordinator = dialogCoordinator;

        if (BestCompetitionRaces != null) { BestCompetitionRaces.PropertyChanged += (sender, e) => updateNotAssignedStarts(); }

        updateNotAssignedStarts();

        _raceService.PropertyChanged += (sender, e) =>
        {
            switch (e.PropertyName)
            {
                case nameof(RaceService.BestCompetitionRaces):
                    OnPropertyChanged(nameof(BestCompetitionRaces));
                    if (BestCompetitionRaces != null) { BestCompetitionRaces.PropertyChanged += (sender, e) => updateNotAssignedStarts(); }
                    break;
                case nameof(RaceService.CalculateCompetitionRaces):
                    OnPropertyChanged(nameof(CalculatedCompetitionRaces));
                    OnPropertyChanged(nameof(AreRacesAvailable));
                    break;
                default:
                    break;
            }
        };
    }

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    #region Calculate Competition Races Command

    private ICommand _calculateCompetitionRacesCommand;
    public ICommand CalculateCompetitionRacesCommand => _calculateCompetitionRacesCommand ?? (_calculateCompetitionRacesCommand = new RelayCommand(async() => 
    {
        ProgressDelegate onProgress = (sender, progress, currentStep) =>
        {
            if (progress % 0.5 == 0)
            {
                _progressController?.SetProgress(progress / 100);
                _progressController?.SetMessage(string.IsNullOrEmpty(currentStep) ? $"{progress:F1}%" : $"{currentStep}: {progress:F1}%");
            }
        };

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        _progressController = await _dialogCoordinator.ShowProgressAsync(this, Properties.Resources.CalculateCompetitionRacesString, "", true);
        _progressController.Canceled += (sender, e) => cancellationTokenSource.Cancel();

        try
        {
            await _raceService.CalculateCompetitionRaces(_workspaceService?.Settings?.CompetitionYear ?? 0, cancellationTokenSource.Token, 3, onProgress);
            IndexCurrentCompetitionRace = 0;
            OnPropertyChanged(nameof(CalculatedCompetitionRaces));
            OnPropertyChanged(nameof(BestCompetitionRaces));
            OnPropertyChanged(nameof(NotAssignedStarts));
            OnPropertyChanged(nameof(AreRacesAvailable));
        }
        catch (OperationCanceledException)
        {
            /* Nothing to do here if user canceled */
        }
        catch (Exception ex)
        {
            await _progressController?.CloseAsync();
            await _dialogCoordinator.ShowMessageAsync(this, Properties.Resources.ErrorString, ex.Message);
        }
        finally
        {
            await _progressController?.CloseAsync();
        }
    }));

    #endregion

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    #region Other Commands

    private ICommand _addNewRaceCommand;
    public ICommand AddNewRaceCommand => _addNewRaceCommand ?? (_addNewRaceCommand = new RelayCommand(() =>
    {
        if(CurrentCompetitionRace != null)
        {
            CurrentCompetitionRace.Races.Add(new Race());
        }
    }));

    #endregion

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public void OnNavigatedTo(object parameter)
    {
        OnPropertyChanged(nameof(CalculatedCompetitionRaces));
        OnPropertyChanged(nameof(BestCompetitionRaces));
        OnPropertyChanged(nameof(NotAssignedStarts));
        OnPropertyChanged(nameof(AreRacesAvailable));
        IndexCurrentCompetitionRaceDisplay = BestCompetitionRaces == null ? 1 : 0;
    }

    public void OnNavigatedFrom()
    {
    }
}
