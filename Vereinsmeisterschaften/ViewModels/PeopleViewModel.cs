using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using Vereinsmeisterschaften.Contracts.ViewModels;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;
using Vereinsmeisterschaften.Properties;

namespace Vereinsmeisterschaften.ViewModels;

/// <summary>
/// ViewModel for the person overview page
/// </summary>
public class PeopleViewModel : ObservableObject, INavigationAware
{
    private ObservableCollection<Person> _people;
    /// <summary>
    /// List of people shown on the person overview page
    /// </summary>
    public ObservableCollection<Person> People
    {
        get => _people;
        set => SetProperty(ref _people, value);
    }

    /// <summary>
    /// String used to describe, which person is duplicated
    /// </summary>
    public string DuplicatePersonString
    {
        get
        {
            List<Person> duplicates = _personService.CheckForDuplicatePerson();
            string duplicateString = string.Empty;
            foreach(Person person in duplicates)
            {
                duplicateString += person.Name + ", " + person.FirstName + Environment.NewLine;
            }
            return duplicateString.Trim(Environment.NewLine.ToCharArray());
        }
    }

    private ICollectionView _peopleCollectionView;
    /// <summary>
    /// CollectionView used to display the list of people and filter
    /// </summary>
    public ICollectionView PeopleCollectionView
    {
        get => _peopleCollectionView;
        private set => SetProperty(ref _peopleCollectionView, value);
    }

    private Person _selectedPerson;
    /// <summary>
    /// Currently selected <see cref="Person"/>
    /// </summary>
    public Person SelectedPerson
    {
        get => _selectedPerson;
        set => SetProperty(ref _selectedPerson, value);
    }

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    private Subject<bool> filterInputSubject = new Subject<bool>();         // Used to delay the filter while typing

    private string _filterText = "";
    /// <summary>
    /// Text used to filter the person list
    /// </summary>
    public string FilterText
    {
        get => _filterText;
        set { SetProperty(ref _filterText, value); filterInputSubject?.OnNext(true); }
    }

    /// <summary>
    /// Function used when filtering the person list
    /// </summary>
    public Predicate<object> PersonFilterPredicate
    {
        get
        {
            return (item) =>
            {
                Person person = item as Person;

                bool filterResult = person.Name.ToLower().Contains(FilterText?.ToLower()) ||
                                    person.FirstName.ToLower().Contains(FilterText?.ToLower()) ||
                                    (person.FirstName.ToLower() + " " + person.Name.ToLower()).Contains(FilterText?.ToLower()) ||
                                    (person.Name.ToLower() + " " + person.FirstName.ToLower()).Contains(FilterText?.ToLower()) ||
                                    (person.FirstName.ToLower() + ", " + person.Name.ToLower()).Contains(FilterText?.ToLower()) ||
                                    (person.Name.ToLower() + ", " + person.FirstName.ToLower()).Contains(FilterText?.ToLower()) ||
                                    person.BirthYear.ToString().Contains(FilterText ?? "");
                return filterResult;
            };
        }
    }

    private ICommand _clearFilterCommand;
    /// <summary>
    /// Command to clear the filter
    /// </summary>
    public ICommand ClearFilterCommand => _clearFilterCommand ?? (_clearFilterCommand = new RelayCommand(() =>
    {
        FilterText = string.Empty;
    }));

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    private IPersonService _personService;
    private ICompetitionService _competitionService;
    private IDialogCoordinator _dialogCoordinator;

    /// <summary>
    /// Constructor of the people view model
    /// </summary>
    /// <param name="personService"><see cref="IPersonService"/> object</param>
    /// <param name="competitionService"><see cref="ICompetitionService"/> object</param>
    /// <param name="dialogCoordinator"><see cref="IDialogCoordinator"/> object</param>
    public PeopleViewModel(IPersonService personService, ICompetitionService competitionService, IDialogCoordinator dialogCoordinator)
    {
        _personService = personService;
        _competitionService = competitionService;
        _dialogCoordinator = dialogCoordinator;

        // Delay the filtering to make the typing smoother
        filterInputSubject.Throttle(TimeSpan.FromMilliseconds(100)).ObserveOn(SynchronizationContext.Current).Subscribe((b) => PeopleCollectionView.Refresh());
    }

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    private ICommand _addPersonCommand;
    /// <summary>
    /// Command to add a new person
    /// </summary>
    public ICommand AddPersonCommand => _addPersonCommand ?? (_addPersonCommand = new RelayCommand(() =>
    {
        Person person = new Person();
        person.PropertyChanged += Person_PropertyChanged;
        _personService.AddPerson(person);
        _competitionService.UpdateAllCompetitionsForPerson(person);
        person.PropertyChanged += (sender, e) => OnPropertyChanged(nameof(DuplicatePersonString));
        SelectedPerson = person;
    }));

    private ICommand _removePersonCommand;
    /// <summary>
    /// Command to remove a person from the list
    /// </summary>
    public ICommand RemovePersonCommand => _removePersonCommand ?? (_removePersonCommand = new RelayCommand<Person>(async (person) =>
    {
        MessageDialogResult result = await _dialogCoordinator.ShowMessageAsync(this, Resources.RemovePersonString, Resources.RemovePersonConfirmationString, MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings() { NegativeButtonText = Resources.CancelString });
        if (result == MessageDialogResult.Affirmative)
        {
            _personService.RemovePerson(person);
            OnPropertyChanged(nameof(DuplicatePersonString));
        }
    }));

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    /// <inheritdoc/>
    public void OnNavigatedTo(object parameter)
    {
        People = _personService?.GetPersons();
        PeopleCollectionView = CollectionViewSource.GetDefaultView(People);
        PeopleCollectionView.Filter += PersonFilterPredicate;
        SelectedPerson = People?.FirstOrDefault();
        _competitionService.UpdateAllCompetitionsForPerson();

        foreach(Person person in People)
        {
            person.PropertyChanged -= Person_PropertyChanged; // Unsubscribe from the event to avoid multiple subscriptions
            person.PropertyChanged += Person_PropertyChanged;
        }

        OnPropertyChanged(nameof(DuplicatePersonString));
    }

    /// <inheritdoc/>
    public void OnNavigatedFrom()
    {
    }

    private void Person_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(nameof(DuplicatePersonString));
        if (e.PropertyName != nameof(Person.Starts) && 
            e.PropertyName != nameof(Person.AvailableCompetitions) && 
            e.PropertyName != nameof(Person.AvailableCompetitionsFlags) && 
            e.PropertyName != nameof(Person.IsUsingMaxAgeCompetitionDict) &&
            e.PropertyName != nameof(Person.HighestScore) &&
            e.PropertyName != nameof(Person.HighestScoreStyle) &&
            e.PropertyName != nameof(Person.HighestScoreCompetition))
        {
            _competitionService.UpdateAllCompetitionsForPerson(sender as Person);
        }
    }
}
