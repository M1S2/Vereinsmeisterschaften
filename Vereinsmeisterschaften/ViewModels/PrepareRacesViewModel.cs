using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using Vereinsmeisterschaften.Controls;
using Vereinsmeisterschaften.Contracts.ViewModels;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;
using Vereinsmeisterschaften.Core.Services;
using Vereinsmeisterschaften.Core.Settings;

namespace Vereinsmeisterschaften.ViewModels;

/// <summary>
/// View model for the race preparation page.
/// </summary>
public class PrepareRacesViewModel : ObservableObject, INavigationAware
{
    #region Calculated Races

    /// <summary>
    /// List with all <see cref="RacesVariant"/>
    /// </summary>
    public ObservableCollection<RacesVariant> AllRacesVariants => _raceService?.AllRacesVariants;

    /// <summary>
    /// True, if there is at least one element in <see cref="AllRacesVariants"/>
    /// </summary>
    public bool AreRacesVariantsAvailable => AllRacesVariants?.Count > 0;

    /// <summary>
    /// True, if there are friend groups available in the <see cref="IPersonService"/>
    /// </summary>
    public bool AreFriendGroupsAvailable => _personService.NumberFriendGroups > 0;

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// <see cref="RacesVariant.VariantID"/> of the <see cref="CurrentRacesVariant"/>
    /// Use 0 to select the <see cref="RaceService.PersistedRacesVariant"/> or the first element in <see cref="AllRacesVariants"/> if <see cref="CurrentRacesVariant"/> is null.
    /// Use -1 to clear the current selection in the combobox
    /// </summary>
    public int CurrentVariantID
    {
        get => CurrentRacesVariant?.VariantID ?? -1;
        set
        {
            if (value > 0)
            {
                CurrentRacesVariant = AllRacesVariants?.Where(r => r.VariantID == value).FirstOrDefault();
            }
            else if(value == 0)
            {
                CurrentRacesVariant = (_raceService?.PersistedRacesVariant != null) ? _raceService?.PersistedRacesVariant : AllRacesVariants?.FirstOrDefault();
            }
            else
            {
                CurrentRacesVariant = null;
            }
            OnPropertyChanged();
        }
    }

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    private RacesVariant _currentRacesVariant;
    /// <summary>
    /// <see cref="RacesVariant"/> that is currently displayed on the view
    /// </summary>
    public RacesVariant CurrentRacesVariant
    {
        get => _currentRacesVariant;
        set
        {
            if (SetProperty(ref _currentRacesVariant, value, new RacesVariantFullEqualityComparer()))
            {
                _currentRacesVariant?.UpdateNotAssignedStarts(_personService.GetAllPersonStarts());
                OnPropertyChanged(nameof(CurrentRacesVariantIsPersistent));
            }
        }
    }

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Recalculate the variant IDs for all elements in <see cref="AllRacesVariants"/>
    /// </summary>
    private void recalculateVariantIDs()
    {
        int currentID = CurrentVariantID;
        CurrentVariantID = -1;      // Set to -1 to clear the current selection in the combobox
        int newVariantID = _raceService?.RecalculateVariantIDs(currentID) ?? -1;
        CurrentVariantID = newVariantID == -1 ? 0 : newVariantID;       // If newVariantID is -1, select the persisted race or first element instead of nothing
        OnPropertyChanged(nameof(AllRacesVariants));
        OnPropertyChanged(nameof(AreRacesVariantsAvailable));
    }

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Property wrapper for the <see cref="RacesVariant.IsPersistent"/> property of the <see cref="CurrentRacesVariant"/>.
    /// This wrapper first disables the <see cref="RacesVariant.IsPersistent"/> property of all elements in <see cref="AllRacesVariants"/> and then sets the property of the <see cref="CurrentRacesVariant"/>
    /// </summary>
    public bool CurrentRacesVariantIsPersistent
    {
        get => CurrentRacesVariant?.IsPersistent ?? false;
        set
        {
            if (CurrentRacesVariant != null)
            {
                foreach (RacesVariant item in AllRacesVariants)
                {
                    item.IsPersistent = false;
                }
                CurrentRacesVariant.IsPersistent = value;
            }
            OnPropertyChanged();
        }
    }

    #endregion

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    #region Highlight Feature

    private HighlightPersonStartModes _highlightPersonStartMode;
    /// <summary>
    /// Currently active highlight mode.
    /// </summary>
    public HighlightPersonStartModes HighlightPersonStartMode
    {
        get => _highlightPersonStartMode;
        set
        {
            if (SetProperty(ref _highlightPersonStartMode, value))
            {
                recalculateHighlightedPersonStarts();
            }
        }
    }

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// List with all available <see cref="Person"/> objects.
    /// </summary>
    public List<Person> AvailablePersons => _personService?.GetPersons().OrderBy(p => p.Name).ToList();

    private Person _highlightedPerson;
    /// <summary>
    /// All <see cref="PersonStart"/> elements that match this <see cref="Person"/> will be highlighted if the <see cref="HighlightPersonStartMode"/> is <see cref="HighlightPersonStartModes.Person"/>
    /// </summary>
    public Person HighlightedPerson
    {
        get => _highlightedPerson;
        set
        {
            if (SetProperty(ref _highlightedPerson, value))
            {
                recalculateHighlightedPersonStarts();
            }
        }
    }

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    private List<SwimmingStyles> _availableSwimmingStyles = Enum.GetValues(typeof(SwimmingStyles)).Cast<SwimmingStyles>().Where(s => s != SwimmingStyles.Unknown).ToList();
    /// <summary>
    /// List with all available <see cref="SwimmingStyles"/>
    /// </summary>
    public List<SwimmingStyles> AvailableSwimmingStyles => _availableSwimmingStyles;

    private SwimmingStyles _highlightedSwimmingStyle;
    /// <summary>
    /// All <see cref="PersonStart"/> elements that match this <see cref="SwimmingStyles"/> will be highlighted if the <see cref="HighlightPersonStartMode"/> is <see cref="HighlightPersonStartModes.SwimmingStyle"/>
    /// </summary>
    public SwimmingStyles HighlightedSwimmingStyle
    {
        get => _highlightedSwimmingStyle;
        set
        {
            if (SetProperty(ref _highlightedSwimmingStyle, value))
            {
                recalculateHighlightedPersonStarts();
            }
        }
    }

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    private Genders _highlightedGender;
    /// <summary>
    /// All <see cref="PersonStart"/> elements that match this <see cref="Genders"/> will be highlighted if the <see cref="HighlightPersonStartMode"/> is <see cref="HighlightPersonStartModes.Gender"/>
    /// </summary>
    public Genders HighlightedGender
    {
        get => _highlightedGender;
        set
        {
            if (SetProperty(ref _highlightedGender, value))
            {
                recalculateHighlightedPersonStarts();
            }
        }
    }

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    private ushort _highlightedDistance;
    /// <summary>
    /// All <see cref="PersonStart"/> elements that match this distance will be highlighted if the <see cref="HighlightPersonStartMode"/> is <see cref="HighlightPersonStartModes.Distance"/>
    /// </summary>
    public ushort HighlightedDistance
    {
        get => _highlightedDistance;
        set
        {
            if (SetProperty(ref _highlightedDistance, value))
            {
                recalculateHighlightedPersonStarts();
            }
        }
    }

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Recalculate the <see cref="PersonStart.IsHighlighted"/> property for all <see cref="PersonStart"/> objects depending on the <see cref="HighlightPersonStartMode"/>
    /// </summary>
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

    #region Drop Handlers

    /// <summary>
    /// Drop Handler for the <see cref="CurrentRacesVariant"/>
    /// </summary>
    public DropAllowedHandler DropAllowedHandlerObj { get; } = new DropAllowedHandler();

    /// <summary>
    /// Drop Handler for the parking lot region containing the <see cref="RacesVariant.NotAssignedStarts"/>
    /// </summary>
    public DropAllowedHandlerParkingLot DropAllowedHandlerParkingLotObj { get; } = new DropAllowedHandlerParkingLot();

    #endregion

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    private IRaceService _raceService;
    private IWorkspaceService _workspaceService;
    private IPersonService _personService;
    private IDialogCoordinator _dialogCoordinator;

    /// <summary>
    /// Constructor of the prepare races view model
    /// </summary>
    /// <param name="raceService"><see cref="IRaceService"/> object</param>
    /// <param name="workspaceService"><see cref="IWorkspaceService"/> object</param>
    /// <param name="personService"><see cref="IPersonService"/> object</param>
    /// <param name="dialogCoordinator"><see cref="IDialogCoordinator"/> object</param>
    public PrepareRacesViewModel(IRaceService raceService, IWorkspaceService workspaceService, IPersonService personService, IDialogCoordinator dialogCoordinator)
    {
        _raceService = raceService;
        _workspaceService = workspaceService;
        _personService = personService;
        _dialogCoordinator = dialogCoordinator;

        _raceService.PropertyChanged += (sender, e) =>
        {
            switch (e.PropertyName)
            {
                case nameof(RaceService.AllRacesVariants):
                    OnPropertyChanged(nameof(CurrentRacesVariant));
                    OnPropertyChanged(nameof(AllRacesVariants));
                    OnPropertyChanged(nameof(AreRacesVariantsAvailable));
                    break;
                case nameof(RaceService.PersistedRacesVariant):
                    OnPropertyChanged(nameof(CurrentRacesVariant));
                    break;
                default:
                    break;
            }
        };
        _raceService.AllRacesVariants.CollectionChanged += (sender, e) =>
        {
            ((RelayCommand)AddNewRaceCommand).NotifyCanExecuteChanged();
            ((RelayCommand)CleanupRacesCommand).NotifyCanExecuteChanged();
            ((RelayCommand)RemoveRaceVariantCommand).NotifyCanExecuteChanged();
            ((RelayCommand)ReorderRaceVariantsCommand).NotifyCanExecuteChanged();
        };
    }

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    #region Calculate Races Variants Command

    private ICommand _calculateRacesVariantsCommand;
    /// <summary>
    /// Command to calculate new <see cref="RacesVariant"/> variants
    /// </summary>
    public ICommand CalculateRacesVariantsCommand => _calculateRacesVariantsCommand ?? (_calculateRacesVariantsCommand = new RelayCommand(async() => 
    {
        DoubleProgressDialog _progressDialog = new DoubleProgressDialog();
        _progressDialog.Title = Properties.Resources.CalculateRacesVariantsString;
        _progressDialog.ProgressDescription1 = Properties.Resources.IterationProgressString;
        _progressDialog.ProgressDescription2 = Properties.Resources.SolutionProgressString;
        _progressDialog.ProgressNumberDecimals = 1;

        double nextProgressLevelIteration = 0.0;
        ProgressDelegate onProgressIteration = (sender, progress, currentStep) =>
        {
            if (progress >= nextProgressLevelIteration)
            {
                nextProgressLevelIteration += 0.1;   // Only report all 0.1%. This is enough.
                _progressDialog.Progress1 = progress;
            }
        };
        double nextProgressLevelSolution = 0.0;
        ProgressDelegate onProgressSolution = (sender, progress, currentStep) =>
        {
            if (progress >= nextProgressLevelSolution)
            {
                nextProgressLevelSolution += 0.5;   // Only report all 0.5%. This is enough.
                _progressDialog.Progress2 = progress;
            }
        };

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        _progressDialog.OnCanceled += (sender, e) => cancellationTokenSource.Cancel();
        await _dialogCoordinator.ShowMetroDialogAsync(this, _progressDialog);

        try
        {
            await _raceService.CalculateRacesVariants(cancellationTokenSource.Token, onProgressIteration, onProgressSolution);
            recalculateVariantIDs();

            await _dialogCoordinator.HideMetroDialogAsync(this, _progressDialog);

            int numberVariants = AllRacesVariants.Count;
            ushort numberRequestedVariants = _workspaceService?.Settings?.GetSettingValue<ushort>(WorkspaceSettings.GROUP_RACE_CALCULATION, WorkspaceSettings.SETTING_RACE_CALCULATION_NUM_RACE_VARIANTS_AFTER_CALCULATION) ?? 0;
            if (numberVariants < numberRequestedVariants)
            {
                // Less variants than required are calculated    
                await _dialogCoordinator.ShowMessageAsync(this, Properties.Resources.CalculateRacesVariantsString, string.Format(Properties.Resources.CalculationWarningTooLessVariantsString, numberVariants, numberRequestedVariants));
            }
        }
        catch (OperationCanceledException)
        {
            await _dialogCoordinator.HideMetroDialogAsync(this, _progressDialog);
        }
        catch (Exception ex)
        {
            await _dialogCoordinator.HideMetroDialogAsync(this, _progressDialog);
            await _dialogCoordinator.ShowMessageAsync(this, Properties.Resources.ErrorString, ex.Message);
        }
    }));
#endregion

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    #region Other Commands

    private ICommand _addNewRaceCommand;
    /// <summary>
    /// Command to add a new <see cref="Race"/> to the <see cref="CurrentRacesVariant"/>
    /// </summary>
    public ICommand AddNewRaceCommand => _addNewRaceCommand ?? (_addNewRaceCommand = new RelayCommand(() =>
    {
        if(CurrentRacesVariant != null)
        {
            CurrentRacesVariant.Races.Add(new Race());
        }
    }, () => AreRacesVariantsAvailable));

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    private ICommand _cleanupRacesCommand;
    /// <summary>
    /// Command to cleanup all <see cref="RacesVariant"/> in <see cref="RaceService.AllRacesVariants"/>
    /// </summary>
    public ICommand CleanupRacesCommand => _cleanupRacesCommand ?? (_cleanupRacesCommand = new RelayCommand(() =>
    {
        _raceService?.CleanupRacesVariants();
    }, () => AreRacesVariantsAvailable));

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    private ICommand _addNewRaceVariantCommand;
    /// <summary>
    /// Add a new <see cref="RacesVariant"/> element to the <see cref="RaceService.AllRacesVariants"/>
    /// </summary>
    public ICommand AddNewRaceVariantCommand => _addNewRaceVariantCommand ?? (_addNewRaceVariantCommand = new RelayCommand(() =>
    {
        RacesVariant newVariant = new RacesVariant(_workspaceService);
        _raceService?.AddRacesVariant(newVariant);
        CurrentRacesVariant = newVariant;
        OnPropertyChanged(nameof(AreRacesVariantsAvailable));
        OnPropertyChanged(nameof(CurrentRacesVariant));
        OnPropertyChanged(nameof(AllRacesVariants));
        OnPropertyChanged(nameof(CurrentVariantID));
    }));

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    private ICommand _removeRaceVariantCommand;
    /// <summary>
    /// Remove the <see cref="CurrentRacesVariant"/> from the <see cref="RaceService.AllRacesVariants"/>
    /// </summary>
    public ICommand RemoveRaceVariantCommand => _removeRaceVariantCommand ?? (_removeRaceVariantCommand = new RelayCommand(async () =>
    {
        // Ask the user for deletion confirmation
        MessageDialogResult result = await _dialogCoordinator.ShowMessageAsync(this, Properties.Resources.DeleteConfirmationTitleString,
            Properties.Resources.DeleteRaceVariantConfirmationString,
            MessageDialogStyle.AffirmativeAndNegative,
            new MetroDialogSettings() { AffirmativeButtonText = Properties.Resources.RemoveRaceVariantString, NegativeButtonText = Properties.Resources.CancelString });

        if (result == MessageDialogResult.Affirmative)
        {
            int index = AllRacesVariants.IndexOf(CurrentRacesVariant);
            _raceService?.RemoveRacesVariant(CurrentRacesVariant);
            if (index == AllRacesVariants.Count && AllRacesVariants.Count >= 1)
            {
                index--;
            }
            CurrentRacesVariant = null;      // Set to null and then to the correct value to update the combobox on the view
            CurrentRacesVariant = (index >= 0 && index < AllRacesVariants.Count) ? AllRacesVariants[index] : AllRacesVariants?.FirstOrDefault();
            OnPropertyChanged(nameof(AreRacesVariantsAvailable));
            OnPropertyChanged(nameof(CurrentRacesVariant));
            OnPropertyChanged(nameof(AllRacesVariants));
            OnPropertyChanged(nameof(CurrentVariantID));
        }
    }, () => AreRacesVariantsAvailable));

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    private ICommand _copyRaceVariantCommand;
    /// <summary>
    /// Copy the <see cref="CurrentRacesVariant"/> and add it as new variant to <see cref="RaceService.AllRacesVariants"/>
    /// </summary>
    public ICommand CopyRaceVariantCommand => _copyRaceVariantCommand ?? (_copyRaceVariantCommand = new RelayCommand(() =>
    {
        RacesVariant copyVariant = new RacesVariant(CurrentRacesVariant, true, false, _workspaceService);
        _raceService?.AddRacesVariant(copyVariant);
        CurrentRacesVariant = copyVariant;
        OnPropertyChanged(nameof(AreRacesVariantsAvailable));
        OnPropertyChanged(nameof(CurrentRacesVariant));
        OnPropertyChanged(nameof(AllRacesVariants));
        OnPropertyChanged(nameof(CurrentVariantID));
    }, () => CurrentRacesVariant != null));

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    private ICommand _reorderRaceVariantsCommand;
    /// <summary>
    /// Reorder the <see cref="RacesVariant"/> variants by score and then recalculate the variant IDs.
    /// </summary>
    public ICommand ReorderRaceVariantsCommand => _reorderRaceVariantsCommand ?? (_reorderRaceVariantsCommand = new RelayCommand(() =>
    {
        _raceService?.SortVariantsByScore();
        recalculateVariantIDs();
    }, () => AreRacesVariantsAvailable));

    #endregion

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    /// <inheritdoc/>
    public void OnNavigatedTo(object parameter)
    {
        _raceService.CleanupRacesVariants(false);
        _raceService.RecalculateVariantIDs();

        ushort numSwimLanes = _workspaceService?.Settings?.GetSettingValue<ushort>(WorkspaceSettings.GROUP_RACE_CALCULATION, WorkspaceSettings.SETTING_RACE_CALCULATION_NUMBER_OF_SWIM_LANES) ?? 0;
        DropAllowedHandlerObj.MaxItemsInTargetCollection = numSwimLanes;

        OnPropertyChanged(nameof(AllRacesVariants));
        OnPropertyChanged(nameof(AreRacesVariantsAvailable));
        OnPropertyChanged(nameof(AvailablePersons));
        OnPropertyChanged(nameof(CurrentRacesVariant));
        OnPropertyChanged(nameof(AreFriendGroupsAvailable));
        CurrentVariantID = 0;
    }

    /// <inheritdoc/>
    public void OnNavigatedFrom()
    {
    }
}
