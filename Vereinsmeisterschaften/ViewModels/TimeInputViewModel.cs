using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Vereinsmeisterschaften.Contracts.ViewModels;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.ViewModels;

public class TimeInputViewModel : ObservableObject, INavigationAware
{
    public List<PersonStart> AvailablePersonStarts { get; set; }

    private readonly IPersonService _personService;

    public TimeInputViewModel(IPersonService personService)
    {
        _personService = personService;
    }

    public void OnNavigatedTo(object parameter)
    {
        AvailablePersonStarts = _personService.GetAllPersonStarts();
    }

    public void OnNavigatedFrom()
    {
    }
}
