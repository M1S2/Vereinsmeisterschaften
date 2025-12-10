using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
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
public partial class PeopleViewModel : ObservableObject, INavigationAware
{
    /// <summary>
    /// List of people shown on the person overview page
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<Person> _people;

    /// <summary>
    /// True if there are persons with empty starts
    /// </summary>
    public bool HasEmptyPersons => People?.Any(p => !p.HasStarts) ?? false;

    /// <summary>
    /// True if there are duplicate persons
    /// </summary>
    public bool HasDuplicatePersons => People?.Any(p => p.HasDuplicates) ?? false;

    private ICollectionView _peopleCollectionView;
    /// <summary>
    /// CollectionView used to display the list of people and filter
    /// </summary>
    public ICollectionView PeopleCollectionView
    {
        get => _peopleCollectionView;
        private set => SetProperty(ref _peopleCollectionView, value);
    }

    /// <summary>
    /// Currently selected <see cref="Person"/>
    /// </summary>
    [ObservableProperty]
    private Person _selectedPerson;

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    private string _filterText = "";
    /// <summary>
    /// Text used to filter the person list
    /// </summary>
    public string FilterText
    {
        get => _filterText;
        set
        {
            if (SetProperty(ref _filterText, value))
            {
                PeopleCollectionView.Refresh();
            }
        }
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
    private ShellViewModel _shellVM;

    /// <summary>
    /// Constructor of the people view model
    /// </summary>
    /// <param name="personService"><see cref="IPersonService"/> object</param>
    /// <param name="competitionService"><see cref="ICompetitionService"/> object</param>
    /// <param name="dialogCoordinator"><see cref="IDialogCoordinator"/> object</param>
    /// <param name="shellVM"><see cref="ShellViewModel"/> object used for dialog display</param>
    public PeopleViewModel(IPersonService personService, ICompetitionService competitionService, IDialogCoordinator dialogCoordinator, ShellViewModel shellVM)
    {
        _personService = personService;
        _competitionService = competitionService;
        _dialogCoordinator = dialogCoordinator;
        _shellVM = shellVM;
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
        SelectedPerson = person;
        OnPropertyChanged(nameof(HasDuplicatePersons));
        OnPropertyChanged(nameof(HasEmptyPersons));
    }));

    private ICommand _removePersonCommand;
    /// <summary>
    /// Command to remove a person from the list
    /// </summary>
    public ICommand RemovePersonCommand => _removePersonCommand ?? (_removePersonCommand = new RelayCommand<Person>(async (person) =>
    {
        MessageDialogResult result = await _dialogCoordinator.ShowMessageAsync(_shellVM, Resources.RemovePersonString, Resources.RemovePersonConfirmationString, MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings() { NegativeButtonText = Resources.CancelString });
        if (result == MessageDialogResult.Affirmative)
        {
            person.PropertyChanged -= Person_PropertyChanged;
            _personService.RemovePerson(person);
            OnPropertyChanged(nameof(HasDuplicatePersons));
            OnPropertyChanged(nameof(HasEmptyPersons));
        }
    }));

    private ICommand _personActiveInactiveCommand;
    /// <summary>
    /// Command to set a person active or inactive
    /// </summary>
    public ICommand PersonActiveInactiveCommand => _personActiveInactiveCommand ?? (_personActiveInactiveCommand = new RelayCommand<Person>((person) =>
    {
        person.IsActive = !person.IsActive;
    }));

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    #region Friend Groups Handling

    /// <summary>
    /// List with all friend group view models
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<FriendGroupViewModel> _friendGroups = new ObservableCollection<FriendGroupViewModel>();

    [ICommand]
    /// <summary>
    /// Add a new friend group
    /// </summary>
    public void AddNewFriendGroup()
    {
        int newGroupId = 0;
        if (FriendGroups?.Count > 0) { newGroupId = FriendGroups?.Max(g => g.GroupId) + 1 ?? 0; }
        FriendGroupViewModel newGroup = new FriendGroupViewModel()
        {
            GroupId = newGroupId,
            AvailableFriends = People,
            Friends = new ObservableCollection<Person>()
        };
        newGroup.Friends.CollectionChanged += (s, e) => Friends_CollectionChanged(s, e, newGroup);
        FriendGroups.Add(newGroup);
    }

    /// <summary>
    /// Remove the given friend group and remove the group ID from all persons belonging to this group
    /// </summary>
    /// <param name="group"><see cref="FriendGroupViewModel"/> to remove</param>
    [ICommand]
    public void RemoveFriendGroup(FriendGroupViewModel group)
    {
        foreach (Person person in group.Friends)
        {
            if (person.FriendGroupIDs.Contains(group.GroupId))
            {
                person.FriendGroupIDs.Remove(group.GroupId);
            }
        }
        FriendGroups.Remove(group);
        _personService.UpdateAllFriendReferencesFromFriendGroupIDs();
    }

    /// <summary>
    /// Creates the friend group view model based on the persons' friend group IDs and friends
    /// </summary>
    public void CreateFriendGroupViewModel()
    {
        _pauseFriendsCollectionChangedEvent = true;
        FriendGroups.Clear();
        _pauseFriendsCollectionChangedEvent = false;

        // Collect all unique group IDs from all persons
        List<int> allGroupIds = People.Select(p => p.FriendGroupIDs ?? Enumerable.Empty<int>()).Aggregate(new List<int>(), (acc, innerList) =>
        {
            acc.AddRange(innerList);
            return acc;
        }).Distinct().OrderBy(id => id).ToList();

        foreach (int groupId in allGroupIds)
        {
            // Create view model entry for the group
            FriendGroupViewModel group = new FriendGroupViewModel()
            {
                GroupId = groupId,
                AvailableFriends = People,
                // Get all persons belonging to this group
                Friends = new ObservableCollection<Person>(People.Where(p => p.FriendGroupIDs.Contains(groupId)))
            };
            group.Friends.CollectionChanged += (s, e) => Friends_CollectionChanged(s, e, group);
            FriendGroups.Add(group);
        }
    }

    private bool _pauseFriendsCollectionChangedEvent = false;
    private void Friends_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e, FriendGroupViewModel groupViewModel)
    {
        if (_pauseFriendsCollectionChangedEvent) { return; }

        int groupId = groupViewModel.GroupId;
        switch (e.Action)
        {
            case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                {
                    foreach (Person newPerson in e.NewItems)
                    {
                        if (!newPerson.FriendGroupIDs.Contains(groupId))
                        {
                            newPerson.FriendGroupIDs.Add(groupId);
                        }
                    }
                    break;
                }
            case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                {
                    foreach (Person oldPerson in e.OldItems)
                    {
                        if (oldPerson.FriendGroupIDs.Contains(groupId))
                        {
                            oldPerson.FriendGroupIDs.Remove(groupId);
                        }
                    }
                    break;
                }
            default: break;
        }
        _personService.UpdateAllFriendReferencesFromFriendGroupIDs();
    }

    #endregion

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


    /// <inheritdoc/>
    public void OnNavigatedTo(object parameter)
    {
        People = _personService?.GetPersons();
        PeopleCollectionView = CollectionViewSource.GetDefaultView(People);
        PeopleCollectionView.Filter += PersonFilterPredicate;
        SelectedPerson = People?.FirstOrDefault();

        foreach(Person person in People)
        {
            // Unsubscribe from and resubscribe to the event to avoid multiple subscriptions
            person.PropertyChanged -= Person_PropertyChanged; 
            person.PropertyChanged += Person_PropertyChanged;
        }

        CreateFriendGroupViewModel();

        OnPropertyChanged(nameof(HasDuplicatePersons));
        OnPropertyChanged(nameof(HasEmptyPersons));
    }

    /// <inheritdoc/>
    public void OnNavigatedFrom()
    {
        // Unsubscribe from the event to avoid raising this event on another page (not necessary there)
        foreach (Person person in People)
        {
            person.PropertyChanged -= Person_PropertyChanged;
        }
    }

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    #region Property changed event handler

    private void Person_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(Person.HasDuplicates): OnPropertyChanged(nameof(HasDuplicatePersons)); break;
            case nameof(Person.HasStarts): OnPropertyChanged(nameof(HasEmptyPersons)); break;
            default: break;
        }
    }

    #endregion
}
