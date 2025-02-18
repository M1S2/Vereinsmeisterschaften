using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Vereinsmeisterschaften.Contracts.ViewModels;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;

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

    private Person _personInEditMode;
    public Person PersonInEditMode
    {
        get => _personInEditMode;
        set
        {
            foreach(Person person in People)
            {
                person.IsInEditMode = false;
            }
            if (value != null)
            {
                value.IsInEditMode = true;
            }
            SetProperty(ref _personInEditMode, value);
        }
    }

    private IPersonService _personService;

    public PeopleViewModel(IPersonService personService)
    {
        _personService = personService;
        People = new ObservableCollection<Person>();
    }

    private ICommand _addPersonCommand;
    public ICommand AddPersonCommand => _addPersonCommand ?? (_addPersonCommand = new RelayCommand(() =>
    {
        Person person = new Person();
        _personService.AddPerson(person);
        PersonInEditMode = person;
    }));

    private ICommand _editPersonCommand;
    public ICommand EditPersonCommand => _editPersonCommand ?? (_editPersonCommand = new RelayCommand<Person>((person) =>
    {
        if (PersonInEditMode != null && person.PersonID == PersonInEditMode.PersonID)
        {
            PersonInEditMode = null;
        }
        else
        {
            PersonInEditMode = person;
        }
    }));

    public void OnNavigatedFrom()
    {
    }

    public void OnNavigatedTo(object parameter)
    {
        People = _personService?.GetPersons();
    }
}
