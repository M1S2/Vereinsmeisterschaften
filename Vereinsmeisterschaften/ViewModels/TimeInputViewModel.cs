using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using Vereinsmeisterschaften.Contracts.ViewModels;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.ViewModels;

public class TimeInputViewModel : ObservableObject, INavigationAware
{
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

    public RacesVariant PersistedRacesVariant => _raceService?.PersistedRacesVariant;

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    #region Filter Feature

    private FilterPersonStartModes _filterPersonStartMode = FilterPersonStartModes.None;
    /// <summary>
    /// Currently active filter mode.
    /// </summary>
    public FilterPersonStartModes FilterPersonStartMode
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
    /// All <see cref="PersonStart"/> elements that match this <see cref="Person"/> will be filtered if the <see cref="FilterPersonStartMode"/> is <see cref="FilterPersonStartModes.Person"/>
    /// </summary>
    public Person FilteredPerson
    {
        get => _filteredPerson;
        set { SetProperty(ref _filteredPerson, value); AvailablePersonStartsCollectionView.Refresh(); }
    }

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    private int _filteredRaceID = 1;
    /// <summary>
    /// All <see cref="PersonStart"/> elements that are part of the race with the <see cref="Race.RaceID"/> in the <see cref="PersistedRacesVariant"/> will be filtered if the <see cref="FilterPersonStartMode"/> is <see cref="FilterPersonStartModes.RaceNumber"/>
    /// </summary>
    public int FilteredRaceID
    {
        get => _filteredRaceID;
        set { SetProperty(ref _filteredRaceID, value); AvailablePersonStartsCollectionView.Refresh(); }
    }

    // ----------------------------------------------------------------------------------------------------------------------------------------------
    
    /// <summary>
    /// Function used when filtering the <see cref="PersonStart"> list
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
                    case FilterPersonStartModes.Person:
                        return personStart?.PersonObj == FilteredPerson;
                    case FilterPersonStartModes.RaceID:
                        Race race = PersistedRacesVariant?.Races?.Where(r => r.Starts.Contains(personStart)).FirstOrDefault();
                        return race == null ? false : race.RaceID == FilteredRaceID;
                    case FilterPersonStartModes.None:
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

    public TimeInputViewModel(IPersonService personService, IRaceService raceService)
    {
        _personService = personService;
        _raceService = raceService;
    }

    public void OnNavigatedTo(object parameter)
    {
        AvailablePersonStarts = _personService.GetAllPersonStarts();
        AvailablePersonStartsCollectionView = CollectionViewSource.GetDefaultView(AvailablePersonStarts);
        AvailablePersonStartsCollectionView.Filter += AvailablePersonStartsFilterPredicate;

        OnPropertyChanged(nameof(PersistedRacesVariant));
        OnPropertyChanged(nameof(AvailablePersons));
    }

    public void OnNavigatedFrom()
    {
    }
}
