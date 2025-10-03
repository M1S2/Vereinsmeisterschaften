using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Windows.Data;
using Vereinsmeisterschaften.Contracts.ViewModels;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;
using Vereinsmeisterschaften.Core.Settings;

namespace Vereinsmeisterschaften.ViewModels;

/// <summary>
/// ViewModel for the Time Input page.
/// </summary>
public partial class TimeInputViewModel : ObservableObject, INavigationAware
{
    /// <summary>
    /// List of all available <see cref="PersonStart"/> objects.
    /// </summary>
    public List<PersonStart> AvailablePersonStarts { get; set; }

    private ICollectionView _availablePersonStartsCollectionView;
    /// <summary>
    /// CollectionView used to display the list of available <see cref="PersonStart"/>
    /// </summary>
    public ICollectionView AvailablePersonStartsCollectionView
    {
        get => _availablePersonStartsCollectionView;
        private set => SetProperty(ref _availablePersonStartsCollectionView, value);
    }

    /// <summary>
    /// Gets the persisted variant of races as managed by the race service.
    /// </summary>
    public RacesVariant PersistedRacesVariant => _raceService?.PersistedRacesVariant;

    /// <summary>
    /// Number of digits used to display milliseconds in the time input control.
    /// </summary>
    [ObservableProperty]
    private int _timeInputMillisecondDigits;

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    #region Filter Feature

    private TimeInputPersonStartFilterModes _filterPersonStartMode = TimeInputPersonStartFilterModes.None;
    /// <summary>
    /// Currently active filter mode.
    /// </summary>
    public TimeInputPersonStartFilterModes FilterPersonStartMode
    {
        get => _filterPersonStartMode;
        set
        {
            if (SetProperty(ref _filterPersonStartMode, value))
            {
                AvailablePersonStartsCollectionView.Refresh();
            }
        }
    }

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// List with all available <see cref="Person"/> objects.
    /// </summary>
    public List<Person> AvailablePersons => _personService?.GetPersons().OrderBy(p => p.Name).ToList();

    private Person _filteredPerson;
    /// <summary>
    /// All <see cref="PersonStart"/> elements that match this <see cref="Person"/> will be filtered if the <see cref="FilterPersonStartMode"/> is <see cref="TimeInputPersonStartFilterModes.Person"/>
    /// </summary>
    public Person FilteredPerson
    {
        get => _filteredPerson;
        set
        {
            if (SetProperty(ref _filteredPerson, value))
            {
                AvailablePersonStartsCollectionView.Refresh();
            }
        }
    }

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    private int _filteredRaceID = 1;
    /// <summary>
    /// All <see cref="PersonStart"/> elements that are part of the race with the <see cref="Race.RaceID"/> in the <see cref="PersistedRacesVariant"/> will be filtered if the <see cref="FilterPersonStartMode"/> is <see cref="TimeInputPersonStartFilterModes.RaceNumber"/>
    /// </summary>
    public int FilteredRaceID
    {
        get => _filteredRaceID;
        set
        {
            if (SetProperty(ref _filteredRaceID, value))
            {
                AvailablePersonStartsCollectionView.Refresh();
            }
        }
    }

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    private int _filteredCompetitionID = 1;
    /// <summary>
    /// All <see cref="PersonStart"/> elements that match this competition ID will be filtered if the <see cref="FilterPersonStartMode"/> is <see cref="TimeInputPersonStartFilterModes.CompetitionID"/>
    /// </summary>
    public int FilteredCompetitionID
    {
        get => _filteredCompetitionID;
        set
        {
            if (SetProperty(ref _filteredCompetitionID, value))
            {
                AvailablePersonStartsCollectionView.Refresh();
            }
        }
    }

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Function used when filtering the <see cref="PersonStart"/> list
    /// </summary>
    public Predicate<object> AvailablePersonStartsFilterPredicate
    {
        get
        {
            return (item) =>
            {
                PersonStart personStart = item as PersonStart;

                bool filterResult = true;
                switch(FilterPersonStartMode)
                {
                    case TimeInputPersonStartFilterModes.Person:
                        return personStart?.PersonObj == FilteredPerson;
                    case TimeInputPersonStartFilterModes.RaceID:
                        Race race = PersistedRacesVariant?.Races?.Where(r => r.Starts.Contains(personStart)).FirstOrDefault();
                        return race == null ? false : race.RaceID == FilteredRaceID;
                    case TimeInputPersonStartFilterModes.CompetitionID:
                        return (personStart?.CompetitionObj?.Id ?? -1) == FilteredCompetitionID;
                    case TimeInputPersonStartFilterModes.None:
                    default:
                        break;
                }
                return filterResult;
            };
        }
    }

    #endregion

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    private readonly IPersonService _personService;
    private readonly IRaceService _raceService;
    private readonly IWorkspaceService _workspaceService;

    /// <summary>
    /// Constructor of the time input view model
    /// </summary>
    /// <param name="personService"><see cref="IPersonService"/> object</param>
    /// <param name="raceService"><see cref="IRaceService"/> object</param>
    /// <param name="workspaceService"><see cref="IWorkspaceService"/> object</param>
    public TimeInputViewModel(IPersonService personService, IRaceService raceService, IWorkspaceService workspaceService)
    {
        _personService = personService;
        _raceService = raceService;
        _workspaceService = workspaceService;
    }

    /// <inheritdoc/>
    public void OnNavigatedTo(object parameter)
    {
        AvailablePersonStarts = _personService.GetAllPersonStarts();
        AvailablePersonStartsCollectionView = CollectionViewSource.GetDefaultView(AvailablePersonStarts);
        AvailablePersonStartsCollectionView.Filter += AvailablePersonStartsFilterPredicate;

        TimeInputMillisecondDigits = _workspaceService?.Settings?.GetSettingValue<ushort>(WorkspaceSettings.GROUP_GENERAL, WorkspaceSettings.SETTING_GENERAL_TIMEINPUT_NUMBER_MILLISECOND_DIGITS) ?? 2;

        OnPropertyChanged(nameof(PersistedRacesVariant));
        OnPropertyChanged(nameof(AvailablePersons));
    }

    /// <inheritdoc/>
    public void OnNavigatedFrom()
    {
    }
}
