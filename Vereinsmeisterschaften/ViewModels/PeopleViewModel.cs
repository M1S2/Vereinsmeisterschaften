using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Vereinsmeisterschaften.Contracts.ViewModels;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.ViewModels;

public class PeopleViewModel : ObservableObject, INavigationAware
{
    private List<Person> _people;
    /// <summary>
    /// List of people shown on the person overview page
    /// </summary>
    public List<Person> People
    {
        get => _people;
        set => SetProperty(ref _people, value);
    }

    private IPersonService _personService;

    public PeopleViewModel(IPersonService personService)
    {
        _personService = personService;
        People = new List<Person>();
    }

    public void OnNavigatedFrom()
    {
    }

    public void OnNavigatedTo(object parameter)
    {
        People.Clear();
        List<Person> servicePeople = _personService?.GetPersons();
        servicePeople?.ForEach(p => People.Add(p));
        OnPropertyChanged(nameof(People));
    }
}
