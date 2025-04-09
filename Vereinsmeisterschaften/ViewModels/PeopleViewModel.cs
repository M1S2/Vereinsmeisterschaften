using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Input;
using System.Reactive;
using System.Reactive.Linq;
using Vereinsmeisterschaften.Contracts.ViewModels;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;
using Vereinsmeisterschaften.Properties;
using System.Reactive.Subjects;
using System.Xml.Linq;

namespace Vereinsmeisterschaften.ViewModels;

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
    private IDialogCoordinator _dialogCoordinator;
    public PeopleViewModel(IPersonService personService, IDialogCoordinator dialogCoordinator)
    {
        _personService = personService;
        _dialogCoordinator = dialogCoordinator;
        People = new ObservableCollection<Person>();

        // Delay the filtering to make the typing smoother
        filterInputSubject.Throttle(TimeSpan.FromMilliseconds(100)).ObserveOn(SynchronizationContext.Current).Subscribe((b) => PeopleCollectionView.Refresh());
    }

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    private ICommand _addPersonCommand;
    public ICommand AddPersonCommand => _addPersonCommand ?? (_addPersonCommand = new RelayCommand(() =>
    {
        Person person = new Person();
        _personService.AddPerson(person);
        person.PropertyChanged += (sender, e) => OnPropertyChanged(nameof(DuplicatePersonString));
    }));

    private ICommand _removePersonCommand;
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

    public void OnNavigatedFrom()
    {
    }

    public void OnNavigatedTo(object parameter)
    {
        People = _personService?.GetPersons();
        PeopleCollectionView = CollectionViewSource.GetDefaultView(People);
        PeopleCollectionView.Filter += PersonFilterPredicate;

        foreach(Person person in People)
        {
            person.PropertyChanged += (sender, e) => OnPropertyChanged(nameof(DuplicatePersonString));
        }
    }
}
