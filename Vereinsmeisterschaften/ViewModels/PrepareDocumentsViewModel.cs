using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Vereinsmeisterschaften.Contracts.Services;
using Vereinsmeisterschaften.Contracts.ViewModels;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;
using Vereinsmeisterschaften.Core.Services;

namespace Vereinsmeisterschaften.ViewModels;

public class PrepareDocumentsViewModel : ObservableObject, INavigationAware
{
    #region Calculated Races

    public List<CompetitionRaces> CalculatedCompetitionRaces => _raceService?.LastCalculatedCompetitionRaces;

    public CompetitionRaces BestCompetitionRaces => _raceService?.BestCompetitionRaces;

    public bool AreRacesAvailable => CalculatedCompetitionRaces?.Count > 0 || BestCompetitionRaces != null;

    private int _indexCurrentCompetitionRace;
    public int IndexCurrentCompetitionRace
    {
        get => _indexCurrentCompetitionRace;
        set
        {
            if (value >= 0 && value < CalculatedCompetitionRaces?.Count)
            {
                SetProperty(ref _indexCurrentCompetitionRace, value);
                CurrentCompetitionRace = CalculatedCompetitionRaces[_indexCurrentCompetitionRace];
            }
            else if(value == -1)
            {
                SetProperty(ref _indexCurrentCompetitionRace, value);
                CurrentCompetitionRace = BestCompetitionRaces;
            }
        }
    }

    public int IndexCurrentCompetitionRaceDisplay
    {
        get => IndexCurrentCompetitionRace + 1;
        set => IndexCurrentCompetitionRace = value - 1;
    }

    private CompetitionRaces _currentCompetitionRace;
    public CompetitionRaces CurrentCompetitionRace
    {
        get => _currentCompetitionRace;
        set => SetProperty(ref _currentCompetitionRace, value);
    }

    #endregion

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    #region Highlight Feature

    private HighlightPersonStartModes _highlightPersonStartMode;
    public HighlightPersonStartModes HighlightPersonStartMode
    {
        get => _highlightPersonStartMode;
        set { SetProperty(ref _highlightPersonStartMode, value); recalculateHighlightedPersonStarts(); }
    }

#warning HighlightPersonStartModes are not localized in the UI at the moment !!!
    private List<HighlightPersonStartModes> _availableHighlightPersonStartModes = Enum.GetValues(typeof(HighlightPersonStartModes)).Cast<HighlightPersonStartModes>().ToList();
    public List<HighlightPersonStartModes> AvailableHighlightPersonStartModes => _availableHighlightPersonStartModes;

    public ObservableCollection<Person> AvailablePersons => _personService?.GetPersons();

    private Person _highlightedPerson;
    public Person HighlightedPerson
    {
        get => _highlightedPerson;
        set { SetProperty(ref _highlightedPerson, value); recalculateHighlightedPersonStarts(); }
    }

#warning SwimmingStyles are not localized in the UI at the moment !!!
    private List<SwimmingStyles> _availableSwimmingStyles = Enum.GetValues(typeof(SwimmingStyles)).Cast<SwimmingStyles>().Where(s => s != SwimmingStyles.Unknown).ToList();
    public List<SwimmingStyles> AvailableSwimmingStyles => _availableSwimmingStyles;

    private SwimmingStyles _highlightedSwimmingStyle;
    public SwimmingStyles HighlightedSwimmingStyle
    {
        get => _highlightedSwimmingStyle;
        set { SetProperty(ref _highlightedSwimmingStyle, value); recalculateHighlightedPersonStarts(); }
    }

    #endregion

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    private IRaceService _raceService;
    private IWorkspaceService _workspaceService;
    private IPersonService _personService;
    private IDialogCoordinator _dialogCoordinator;
    private ProgressDialogController _progressController;

    public PrepareDocumentsViewModel(IRaceService raceService, IWorkspaceService workspaceService, IPersonService personService, IDialogCoordinator dialogCoordinator)
    {
        _raceService = raceService;
        _workspaceService = workspaceService;
        _personService = personService;
        _dialogCoordinator = dialogCoordinator;
    }

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    #region Calculate Competition Races Command

    private ICommand _calculateCompetitionRacesCommand;
    public ICommand CalculateCompetitionRacesCommand => _calculateCompetitionRacesCommand ?? (_calculateCompetitionRacesCommand = new RelayCommand(async() => 
    {
        ProgressDelegate onProgress = (sender, progress, currentStep) =>
        {
            if (progress % 0.5 == 0)
            {
                _progressController?.SetProgress(progress / 100);
                _progressController?.SetMessage(string.IsNullOrEmpty(currentStep) ? $"{progress:F1}%" : $"{currentStep}: {progress:F1}%");
            }
        };

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        _progressController = await _dialogCoordinator.ShowProgressAsync(this, Properties.Resources.CalculateCompetitionRacesString, "", true);
        _progressController.Canceled += (sender, e) => cancellationTokenSource.Cancel();

        try
        {
            await _raceService.CalculateCompetitionRaces(_workspaceService?.Settings?.CompetitionYear ?? 0, cancellationTokenSource.Token, 3, onProgress);
            IndexCurrentCompetitionRace = 0;
            OnPropertyChanged(nameof(CalculatedCompetitionRaces));
            OnPropertyChanged(nameof(BestCompetitionRaces));
            OnPropertyChanged(nameof(AreRacesAvailable));
        }
        catch (OperationCanceledException)
        {
            /* Nothing to do here if user canceled */
        }
        catch (Exception ex)
        {
            await _progressController?.CloseAsync();
            await _dialogCoordinator.ShowMessageAsync(this, Properties.Resources.ErrorString, ex.Message);
        }
        finally
        {
            await _progressController?.CloseAsync();
        }
    }));

    #endregion

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public void OnNavigatedTo(object parameter)
    {
        OnPropertyChanged(nameof(CalculatedCompetitionRaces));
        OnPropertyChanged(nameof(BestCompetitionRaces));
        OnPropertyChanged(nameof(AreRacesAvailable));
        IndexCurrentCompetitionRaceDisplay = BestCompetitionRaces == null ? 1 : 0;
    }

    public void OnNavigatedFrom()
    {
    }

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    private void recalculateHighlightedPersonStarts()
    {
        List<PersonStart> personStarts = _personService.GetAllPersonStarts();
        foreach(PersonStart personStart in personStarts)
        {
            switch (HighlightPersonStartMode)
            {
                case HighlightPersonStartModes.Person: personStart.IsHighlighted = personStart.PersonObj.Equals(HighlightedPerson); break;
                case HighlightPersonStartModes.SwimmingStyle: personStart.IsHighlighted = personStart.Style.Equals(HighlightedSwimmingStyle); break;
                case HighlightPersonStartModes.All50m: personStart.IsHighlighted = personStart.CompetitionObj?.Distance == 50; break;
                case HighlightPersonStartModes.All100m: personStart.IsHighlighted = personStart.CompetitionObj?.Distance == 100; break;
                case HighlightPersonStartModes.All200m: personStart.IsHighlighted = personStart.CompetitionObj?.Distance == 200; break;
                case HighlightPersonStartModes.None: personStart.IsHighlighted = false; break;
                default: personStart.IsHighlighted = false; break;
            }
        }
    }
}
