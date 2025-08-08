using System.ComponentModel;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using Vereinsmeisterschaften.Contracts.ViewModels;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.ViewModels;

/// <summary>
/// ViewModel for the Time Input page.
/// </summary>
public class TimeInputViewModel : ObservableObject, INavigationAware
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

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    #region Filter Feature

    private TimeInputPersonStartFilterModes _filterPersonStartMode = TimeInputPersonStartFilterModes.None;
    /// <summary>
    /// Currently active filter mode.
    /// </summary>
    public TimeInputPersonStartFilterModes FilterPersonStartMode
    {
        get => _filterPersonStartMode;
        set { SetProperty(ref _filterPersonStartMode, value); AvailablePersonStartsCollectionView.Refresh(); }
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
        set { SetProperty(ref _filteredPerson, value); AvailablePersonStartsCollectionView.Refresh(); }
    }

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    private int _filteredRaceID = 1;
    /// <summary>
    /// All <see cref="PersonStart"/> elements that are part of the race with the <see cref="Race.RaceID"/> in the <see cref="PersistedRacesVariant"/> will be filtered if the <see cref="FilterPersonStartMode"/> is <see cref="TimeInputPersonStartFilterModes.RaceNumber"/>
    /// </summary>
    public int FilteredRaceID
    {
        get => _filteredRaceID;
        set { SetProperty(ref _filteredRaceID, value); AvailablePersonStartsCollectionView.Refresh(); }
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

    /// <summary>
    /// Constructor of the time input view model
    /// </summary>
    /// <param name="personService"><see cref="IPersonService"/> object</param>
    /// <param name="raceService"><see cref="IRaceService"/> object</param>
    public TimeInputViewModel(IPersonService personService, IRaceService raceService)
    {
        _personService = personService;
        _raceService = raceService;
    }

    /// <inheritdoc/>
    public void OnNavigatedTo(object parameter)
    {
        AvailablePersonStarts = _personService.GetAllPersonStarts();
        AvailablePersonStartsCollectionView = CollectionViewSource.GetDefaultView(AvailablePersonStarts);
        AvailablePersonStartsCollectionView.Filter += AvailablePersonStartsFilterPredicate;

        OnPropertyChanged(nameof(PersistedRacesVariant));
        OnPropertyChanged(nameof(AvailablePersons));
    }

    /// <inheritdoc/>
    public void OnNavigatedFrom()
    {
    }
}
