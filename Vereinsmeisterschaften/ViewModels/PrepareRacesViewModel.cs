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

    public ObservableCollection<CompetitionRaces> AllCompetitionRaces => _raceService?.AllCompetitionRaces;

    public bool AreRacesAvailable => AllCompetitionRaces?.Count > 0;

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    public int CurrentVariantID
    {
        get => CurrentCompetitionRace?.VariantID ?? -1;
        set
        {
            if (value > 0)
            {
                CurrentCompetitionRace = AllCompetitionRaces?.Where(r => r.VariantID == value).FirstOrDefault();
            }
            else if(value == 0)
            {
                CurrentCompetitionRace = (_raceService?.PersistedCompetitionRaces != null) ? _raceService?.PersistedCompetitionRaces : _raceService?.AllCompetitionRaces.FirstOrDefault();
            }
            else
            {
                CurrentCompetitionRace = null;
            }
            OnPropertyChanged();
        }
    }

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
            OnPropertyChanged(nameof(CurrentCompetitionRaceIsPersistent));
        }
    }

    private void recalculateVariantIDs()
    {
        int currentID = CurrentCompetitionRace?.VariantID ?? -1;
        CurrentVariantID = -1; // Set to -1 to clear the current selection in the combobox
        int newVariantID = _raceService?.RecalculateVariantIDs(currentID) ?? -1;
        OnPropertyChanged(nameof(AllCompetitionRaces));
        OnPropertyChanged(nameof(AreRacesAvailable));
        CurrentVariantID = newVariantID == -1 ? 0 : newVariantID;
        OnPropertyChanged(nameof(CurrentCompetitionRace));
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
        OnPropertyChanged(nameof(IsValid_AllStartsAssigned));
        OnPropertyChanged(nameof(CurrentCompetitionRaceIsValid));
    }

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// True when there are no empty unassigned races
    /// </summary>
    public bool IsValid_AllStartsAssigned => NotAssignedStarts.Count == 0;

    /// <summary>
    /// The <see cref="CurrentCompetitionRace"/> is consideres valid when:
    /// - The <see cref="CompetitionRaces.IsValid"/> property is true
    /// - There are no empty unassigned races
    /// </summary>
    public bool CurrentCompetitionRaceIsValid => (CurrentCompetitionRace?.IsValid ?? true) && IsValid_AllStartsAssigned;

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Property wrapper for the <see cref="CompetitionRaces.IsPersistent"/> property of the <see cref="CurrentCompetitionRace"/>.
    /// This wrapper first disables the <see cref="CompetitionRaces.IsPersistent"/> property of all elements in <see cref="AllCompetitionRaces"/> and then sets the property of the <see cref="CurrentCompetitionRace"/>
    /// </summary>
    public bool CurrentCompetitionRaceIsPersistent
    {
        get => CurrentCompetitionRace?.IsPersistent ?? false;
        set
        {
            if (CurrentCompetitionRace != null)
            {
                foreach (CompetitionRaces item in AllCompetitionRaces)
                {
                    item.IsPersistent = false;
                }
                CurrentCompetitionRace.IsPersistent = value;
            }
            OnPropertyChanged();
        }
    }

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

    private ushort _highlightedDistance;
    public ushort HighlightedDistance
    {
        get => _highlightedDistance;
        set { SetProperty(ref _highlightedDistance, value); recalculateHighlightedPersonStarts(); }
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
                case HighlightPersonStartModes.Distance: personStart.IsHighlighted = personStart.CompetitionObj?.Distance == HighlightedDistance; break;
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

        updateNotAssignedStarts();

        _raceService.PropertyChanged += (sender, e) =>
        {
            switch (e.PropertyName)
            {
                case nameof(RaceService.AllCompetitionRaces):
                    OnPropertyChanged(nameof(CurrentCompetitionRace));
                    OnPropertyChanged(nameof(AllCompetitionRaces));
                    OnPropertyChanged(nameof(AreRacesAvailable));
                    break;
                default:
                    break;
            }
            updateNotAssignedStarts();
        };
        _raceService.AllCompetitionRaces.CollectionChanged += (sender, e) =>
        {
            ((RelayCommand)AddNewRaceCommand).NotifyCanExecuteChanged();
            ((RelayCommand)CleanupRacesCommand).NotifyCanExecuteChanged();
            ((RelayCommand)RemoveRaceVariantCommand).NotifyCanExecuteChanged();
            ((RelayCommand)ReorderRaceVariantsCommand).NotifyCanExecuteChanged();
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
            
            OnPropertyChanged(nameof(NotAssignedStarts));
            recalculateVariantIDs();
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
    }, () => AreRacesAvailable));

    private ICommand _cleanupRacesCommand;
    public ICommand CleanupRacesCommand => _cleanupRacesCommand ?? (_cleanupRacesCommand = new RelayCommand(() =>
    {
        _raceService?.CleanupCompetitionRaces();
    }, () => AreRacesAvailable));

    private ICommand _addNewRaceVariantCommand;
    public ICommand AddNewRaceVariantCommand => _addNewRaceVariantCommand ?? (_addNewRaceVariantCommand = new RelayCommand(() =>
    {
        CompetitionRaces newVariant = new CompetitionRaces();
        _raceService?.AddCompetitionRaces(newVariant);
        CurrentCompetitionRace = newVariant;
        OnPropertyChanged(nameof(AreRacesAvailable));
        OnPropertyChanged(nameof(CurrentCompetitionRace));
        OnPropertyChanged(nameof(AllCompetitionRaces));
        OnPropertyChanged(nameof(CurrentVariantID));
    }));

    private ICommand _removeRaceVariantCommand;
    public ICommand RemoveRaceVariantCommand => _removeRaceVariantCommand ?? (_removeRaceVariantCommand = new RelayCommand(async () =>
    {
        MessageDialogResult result = await _dialogCoordinator.ShowMessageAsync(this, Properties.Resources.DeleteConfirmationTitleString,
            Properties.Resources.DeleteRaceVariantConfirmationString,
            MessageDialogStyle.AffirmativeAndNegative,
            new MetroDialogSettings() { AffirmativeButtonText = Properties.Resources.RemoveRaceVariantString, NegativeButtonText = Properties.Resources.CancelString });
        if (result == MessageDialogResult.Affirmative)
        {
            int index = AllCompetitionRaces.IndexOf(CurrentCompetitionRace);
            _raceService?.RemoveCompetitionRaces(CurrentCompetitionRace);
            if (index == AllCompetitionRaces.Count && AllCompetitionRaces.Count >= 1)
            {
                index--;
            }
            CurrentCompetitionRace = null;      // Set to null and then to the correct value to update the combobox on the view
            CurrentCompetitionRace = (index >= 0 && index < AllCompetitionRaces.Count) ? AllCompetitionRaces[index] : AllCompetitionRaces?.FirstOrDefault();
            OnPropertyChanged(nameof(AreRacesAvailable));
            OnPropertyChanged(nameof(CurrentCompetitionRace));
            OnPropertyChanged(nameof(AllCompetitionRaces));
            OnPropertyChanged(nameof(CurrentVariantID));
        }
    }, () => AreRacesAvailable));

    private ICommand _reorderRaceVariantsCommand;
    public ICommand ReorderRaceVariantsCommand => _reorderRaceVariantsCommand ?? (_reorderRaceVariantsCommand = new RelayCommand(() =>
    {
        _raceService?.SortVariantsByScore();
        recalculateVariantIDs();
    }, () => AreRacesAvailable));

    #endregion

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public void OnNavigatedTo(object parameter)
    {
        _raceService.CleanupCompetitionRaces();

        OnPropertyChanged(nameof(AllCompetitionRaces));
        OnPropertyChanged(nameof(NotAssignedStarts));
        OnPropertyChanged(nameof(AreRacesAvailable));
        CurrentCompetitionRace = (_raceService?.PersistedCompetitionRaces != null) ? _raceService?.PersistedCompetitionRaces : _raceService?.AllCompetitionRaces.FirstOrDefault();
    }

    public void OnNavigatedFrom()
    {
    }
}
