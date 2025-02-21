using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Vereinsmeisterschaften.Contracts.ViewModels;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;
using Vereinsmeisterschaften.Properties;

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

#warning Genders are not localized in the UI at the moment !!!
    private List<Genders> _availablePersonGenders = Enum.GetValues(typeof(Genders)).Cast<Genders>().ToList();
    public List<Genders> AvailablePersonGenders => _availablePersonGenders;

    private IPersonService _personService;
    private IDialogCoordinator _dialogCoordinator;
    public PeopleViewModel(IPersonService personService, IDialogCoordinator dialogCoordinator)
    {
        _personService = personService;
        _dialogCoordinator = dialogCoordinator;
        People = new ObservableCollection<Person>();
    }

    private ICommand _addPersonCommand;
    public ICommand AddPersonCommand => _addPersonCommand ?? (_addPersonCommand = new RelayCommand(() =>
    {
        Person person = new Person();
        _personService.AddPerson(person);
    }));

    private ICommand _removePersonCommand;
    public ICommand RemovePersonCommand => _removePersonCommand ?? (_removePersonCommand = new RelayCommand<Person>(async (person) =>
    {
        MessageDialogResult result = await _dialogCoordinator.ShowMessageAsync(this, Resources.RemovePersonString, Resources.RemovePersonConfirmationString, MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings() { NegativeButtonText = Resources.CancelString });
        if (result == MessageDialogResult.Affirmative)
        {
            People.Remove(person);
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
