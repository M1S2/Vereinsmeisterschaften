using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents.Serialization;
using System.Windows.Input;
using Vereinsmeisterschaften.Contracts.Services;
using Vereinsmeisterschaften.Contracts.ViewModels;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;
using Vereinsmeisterschaften.Core.Services;
using Vereinsmeisterschaften.Properties;

namespace Vereinsmeisterschaften.ViewModels;

public class PrepareDocumentsViewModel : ObservableObject
{
    private int _numberCreatedCertificates = 0;
    /// <summary>
    /// Number of created certificates during the last creation process.
    /// </summary>
    public int NumberCreatedCertificates
    {
        get => _numberCreatedCertificates;
        set => SetProperty(ref _numberCreatedCertificates, value);
    }

    private bool _isCertificateCreationRunning = false;
    /// <summary>
    /// Indicates whether the certificate creation process is currently running.
    /// </summary>
    public bool IsCertificateCreationRunning
    {
        get => _isCertificateCreationRunning;
        set
        {
            SetProperty(ref _isCertificateCreationRunning, value);
            OnPropertyChanged(nameof(IsDocumentCreationRunning));
            ((RelayCommand)CreateCertificatesCommand).NotifyCanExecuteChanged();
            ((RelayCommand)CreateOverviewListCommand).NotifyCanExecuteChanged();
            ((RelayCommand)CreateRaceStartListCommand).NotifyCanExecuteChanged();
        }
    }

    private bool _certificateCreationSuccessful = false;
    /// <summary>
    /// Indicates whether the last certificate creation process was successful.
    /// </summary>
    public bool CertificateCreationSuccessful
    {
        get => _certificateCreationSuccessful;
        set => SetProperty(ref _certificateCreationSuccessful, value);
    }

    private bool _isOverviewListCreationRunning = false;
    /// <summary>
    /// Indicates whether the overview list creation process is currently running.
    /// </summary>
    public bool IsOverviewListCreationRunning
    {
        get => _isOverviewListCreationRunning;
        set
        {
            SetProperty(ref _isOverviewListCreationRunning, value);
            OnPropertyChanged(nameof(IsDocumentCreationRunning));
            ((RelayCommand)CreateCertificatesCommand).NotifyCanExecuteChanged();
            ((RelayCommand)CreateOverviewListCommand).NotifyCanExecuteChanged();
            ((RelayCommand)CreateRaceStartListCommand).NotifyCanExecuteChanged();
        }
    }

    private bool _overviewListCreationSuccessful = false;
    /// <summary>
    /// Indicates whether the last overview list creation process was successful.
    /// </summary>
    public bool OverviewListCreationSuccessful
    {
        get => _overviewListCreationSuccessful;
        set => SetProperty(ref _overviewListCreationSuccessful, value);
    }

    private bool _isRaceStartListCreationRunning = false;
    /// <summary>
    /// Indicates whether the race start list creation process is currently running.
    /// </summary>
    public bool IsRaceStartListCreationRunning
    {
        get => _isRaceStartListCreationRunning;
        set
        {
            SetProperty(ref _isRaceStartListCreationRunning, value);
            OnPropertyChanged(nameof(IsDocumentCreationRunning));
            ((RelayCommand)CreateCertificatesCommand).NotifyCanExecuteChanged();
            ((RelayCommand)CreateOverviewListCommand).NotifyCanExecuteChanged();
            ((RelayCommand)CreateRaceStartListCommand).NotifyCanExecuteChanged();
        }
    }

    private bool _raceStartListCreationSuccessful = false;
    /// <summary>
    /// Indicates whether the last race start list creation process was successful.
    /// </summary>
    public bool RaceStartListCreationSuccessful
    {
        get => _raceStartListCreationSuccessful;
        set => SetProperty(ref _raceStartListCreationSuccessful, value);
    }

    /// <summary>
    /// Indicates whether at leas one document creation process is currently running (either certificates or overview list or race start list).
    /// </summary>
    public bool IsDocumentCreationRunning => IsCertificateCreationRunning || IsOverviewListCreationRunning || IsRaceStartListCreationRunning;
    
    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    private PersonStartFilters _personStartFilter = PersonStartFilters.None;
    /// <summary>
    /// Filter used to select which <see cref="PersonStart"/> objects should be printed.
    /// </summary>
    public PersonStartFilters PersonStartFilter
    {
        get => _personStartFilter;
        set => SetProperty(ref _personStartFilter, value);
    }

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// List with all available <see cref="Person"/> objects.
    /// </summary>
    public List<Person> AvailablePersons => _personService?.GetPersons().OrderBy(p => p.Name).ToList();

    private Person _filteredPerson;
    /// <summary>
    /// Only the <see cref="PersonStart"/> elements that match this <see cref="Person"/> will be printed if the <see cref="PersonStartFilter"/> is <see cref="PersonStartFilters.Person"/>
    /// </summary>
    public Person FilteredPerson
    {
        get => _filteredPerson;
        set => SetProperty(ref _filteredPerson, value);
    }

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    private List<SwimmingStyles> _availableSwimmingStyles = Enum.GetValues(typeof(SwimmingStyles)).Cast<SwimmingStyles>().Where(s => s != SwimmingStyles.Unknown).ToList();
    /// <summary>
    /// List with all available <see cref="SwimmingStyles"/>
    /// </summary>
    public List<SwimmingStyles> AvailableSwimmingStyles => _availableSwimmingStyles;

    private SwimmingStyles _filteredSwimmingStyle;
    /// <summary>
    /// Only the <see cref="PersonStart"/> elements that match this <see cref="SwimmingStyles"/> will be printed if the <see cref="PersonStartFilter"/> is <see cref="PersonStartFilters.SwimmingStyle"/>
    /// </summary>
    public SwimmingStyles FilteredSwimmingStyle
    {
        get => _filteredSwimmingStyle;
        set => SetProperty(ref _filteredSwimmingStyle, value);
    }

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    private int _filteredCompetitionID;
    /// <summary>
    /// Only the <see cref="PersonStart"/> elements that match this competition ID will be filtered if the <see cref="PersonStartFilter"/> is <see cref="PersonStartFilters.CompetitionID"/>
    /// </summary>
    public int FilteredCompetitionID
    {
        get => _filteredCompetitionID;
        set => SetProperty(ref _filteredCompetitionID, value);
    }

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    private IDocumentService _documentService;
    private IPersonService _personService;
    private IDialogCoordinator _dialogCoordinator;

    public PrepareDocumentsViewModel(IDocumentService documentService, IPersonService personService, IDialogCoordinator dialogCoordinator)
    {
        _documentService = documentService;
        _personService = personService;
        _dialogCoordinator = dialogCoordinator;
    }

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    private ICommand _createCertificatesCommand;
    /// <summary>
    /// Command to create certificates for all <see cref="PersonStart"/> objects that match the selected filter.
    /// </summary>
    public ICommand CreateCertificatesCommand => _createCertificatesCommand ?? (_createCertificatesCommand = new RelayCommand(async() =>
    {
        IsCertificateCreationRunning = true;

        object filterParam = null;
        switch (PersonStartFilter)
        {
            case PersonStartFilters.None: filterParam = null; break;
            case PersonStartFilters.Person: filterParam = FilteredPerson; break;
            case PersonStartFilters.SwimmingStyle: filterParam = FilteredSwimmingStyle; break;
            case PersonStartFilters.CompetitionID: filterParam = FilteredCompetitionID; break;
            default: break;
        }

        try
        {
            CertificateCreationSuccessful = false;
            NumberCreatedCertificates = await _documentService.CreateCertificates(true, PersonStartFilter, filterParam);
            CertificateCreationSuccessful = true;
        }
        catch(Exception ex)
        {
            await _dialogCoordinator.ShowMessageAsync(this, Resources.ErrorString, ex.Message);
        }
        IsCertificateCreationRunning = false;
    }, () => !IsDocumentCreationRunning));

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    private ICommand _createOverviewListCommand;
    /// <summary>
    /// Command to create an overview list of all <see cref="PersonStart"/> objects.
    /// </summary>
    public ICommand CreateOverviewListCommand => _createOverviewListCommand ?? (_createOverviewListCommand = new RelayCommand(async () =>
    {
        IsOverviewListCreationRunning = true;
        try
        {
            OverviewListCreationSuccessful = false;
            await _documentService.CreateOverviewList();
            OverviewListCreationSuccessful = true;
        }
        catch (Exception ex)
        {
            await _dialogCoordinator.ShowMessageAsync(this, Resources.ErrorString, ex.Message);
        }
        IsOverviewListCreationRunning = false;
    }, () => !IsDocumentCreationRunning));

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    private ICommand _createRaceStartListCommand;
    /// <summary>
    /// Command to create an race start list.
    /// </summary>
    public ICommand CreateRaceStartListCommand => _createRaceStartListCommand ?? (_createRaceStartListCommand = new RelayCommand(async () =>
    {
        IsRaceStartListCreationRunning = true;
        try
        {
            RaceStartListCreationSuccessful = false;
            await _documentService.CreateRaceStartList();
            RaceStartListCreationSuccessful = true;
        }
        catch (Exception ex)
        {
            await _dialogCoordinator.ShowMessageAsync(this, Resources.ErrorString, ex.Message);
        }
        IsRaceStartListCreationRunning = false;
    }, () => !IsDocumentCreationRunning));

}
