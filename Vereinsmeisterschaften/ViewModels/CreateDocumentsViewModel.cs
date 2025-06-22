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
using Vereinsmeisterschaften.Core.Helpers;
using Vereinsmeisterschaften.Core.Models;
using Vereinsmeisterschaften.Core.Services;
using Vereinsmeisterschaften.Properties;

namespace Vereinsmeisterschaften.ViewModels;

public class CreateDocumentsViewModel : ObservableObject
{
    /// <summary>
    /// Dictionary to hold the state of whether a document creation process is currently running for each <see cref="DocumentCreationTypes"/> type.
    /// </summary>
    public Dictionary<DocumentCreationTypes, bool> IsDocumentCreationRunning { get; } = new Dictionary<DocumentCreationTypes, bool>();

    /// <summary>
    /// Dictionary to hold the state of whether a document creation process was successful for each <see cref="DocumentCreationTypes"/> type.
    /// </summary>
    public Dictionary<DocumentCreationTypes, bool> IsDocumentCreationSuccessful { get; } = new Dictionary<DocumentCreationTypes, bool>();

    private void changeDocumentCreationRunningState(DocumentCreationTypes documentType, bool isRunning)
    {
        if (IsDocumentCreationRunning.ContainsKey(documentType))
        {
            IsDocumentCreationRunning[documentType] = isRunning;
            OnPropertyChanged(nameof(IsDocumentCreationRunning));
            ((RelayCommand<DocumentCreationTypes>)CreateDocumentCommand).NotifyCanExecuteChanged();
        }
    }

    private void changeDocumentCreationSuccessfulState(DocumentCreationTypes documentType, bool isSuccessful)
    {
        if (IsDocumentCreationSuccessful.ContainsKey(documentType))
        {
            IsDocumentCreationSuccessful[documentType] = isSuccessful;
            OnPropertyChanged(nameof(IsDocumentCreationSuccessful));
            ((RelayCommand<DocumentCreationTypes>)CreateDocumentCommand).NotifyCanExecuteChanged();
        }
    }
    
    /// <summary>
    /// Indicates whether at leas one document creation process is currently running (either certificates or overview list or race start list).
    /// </summary>
    public bool IsAnyDocumentCreationRunning => IsDocumentCreationRunning.Any(r => r.Value == true);

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    #region Create Certificates Properties

    private int _numberCreatedCertificates = 0;
    /// <summary>
    /// Number of created certificates during the last creation process.
    /// </summary>
    public int NumberCreatedCertificates
    {
        get => _numberCreatedCertificates;
        set => SetProperty(ref _numberCreatedCertificates, value);
    }

    // ----------------------------------------------------------------------------------------------------------------------------------------------

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

    #endregion

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    #region Placeholder Strings

    public string PlaceholderString_CompetitionYear => string.Join(Environment.NewLine, DocumentService.Placeholders_CompetitionYear.Select(p => $"{DocXPlaceholderHelper.PlaceholderMarker}{p}{DocXPlaceholderHelper.PlaceholderMarker}"));
    public string PlaceholderString_Name => string.Join(Environment.NewLine, DocumentService.Placeholders_Name.Select(p => $"{DocXPlaceholderHelper.PlaceholderMarker}{p}{DocXPlaceholderHelper.PlaceholderMarker}"));
    public string PlaceholderString_BirthYear => string.Join(Environment.NewLine, DocumentService.Placeholders_BirthYear.Select(p => $"{DocXPlaceholderHelper.PlaceholderMarker}{p}{DocXPlaceholderHelper.PlaceholderMarker}"));
    public string PlaceholderString_Distance => string.Join(Environment.NewLine, DocumentService.Placeholders_Distance.Select(p => $"{DocXPlaceholderHelper.PlaceholderMarker}{p}{DocXPlaceholderHelper.PlaceholderMarker}"));
    public string PlaceholderString_SwimmingStyle => string.Join(Environment.NewLine, DocumentService.Placeholders_SwimmingStyle.Select(p => $"{DocXPlaceholderHelper.PlaceholderMarker}{p}{DocXPlaceholderHelper.PlaceholderMarker}"));
    public string PlaceholderString_CompetitionID => string.Join(Environment.NewLine, DocumentService.Placeholders_CompetitionID.Select(p => $"{DocXPlaceholderHelper.PlaceholderMarker}{p}{DocXPlaceholderHelper.PlaceholderMarker}"));
    public string PlaceholderString_Score => string.Join(Environment.NewLine, DocumentService.Placeholders_Score.Select(p => $"{DocXPlaceholderHelper.PlaceholderMarker}{p}{DocXPlaceholderHelper.PlaceholderMarker}"));
    public string PlaceholderString_ResultListPlace => string.Join(Environment.NewLine, DocumentService.Placeholders_ResultListPlace.Select(p => $"{DocXPlaceholderHelper.PlaceholderMarker}{p}{DocXPlaceholderHelper.PlaceholderMarker}"));

    #endregion

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    private IDocumentService _documentService;
    private IPersonService _personService;
    private IDialogCoordinator _dialogCoordinator;

    public CreateDocumentsViewModel(IDocumentService documentService, IPersonService personService, IDialogCoordinator dialogCoordinator)
    {
        _documentService = documentService;
        _personService = personService;
        _dialogCoordinator = dialogCoordinator;

        List<DocumentCreationTypes> availableDocumentCreationTypes = Enum.GetValues(typeof(DocumentCreationTypes)).Cast<DocumentCreationTypes>().ToList();
        IsDocumentCreationRunning.Clear();
        IsDocumentCreationSuccessful.Clear();
        foreach (DocumentCreationTypes type in availableDocumentCreationTypes)
        {
            IsDocumentCreationRunning.Add(type, false);
            IsDocumentCreationSuccessful.Add(type, false);
        }
    }

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    private ICommand _createDocumentCommand;
    /// <summary>
    /// Command to create documents. The type of document to create is determined by the command parameter (must be of type <see cref="DocumentCreationTypes"/>).
    /// </summary>
    public ICommand CreateDocumentCommand => _createDocumentCommand ?? (_createDocumentCommand = new RelayCommand<DocumentCreationTypes>(async (documentType) =>
    {
        changeDocumentCreationRunningState(documentType, true);
        try
        {
            changeDocumentCreationSuccessfulState(documentType, false);
            switch (documentType)
            {
                case DocumentCreationTypes.Certificates:
                    {
                        object filterParam = null;
                        switch (PersonStartFilter)
                        {
                            case PersonStartFilters.None: filterParam = null; break;
                            case PersonStartFilters.Person: filterParam = FilteredPerson; break;
                            case PersonStartFilters.SwimmingStyle: filterParam = FilteredSwimmingStyle; break;
                            case PersonStartFilters.CompetitionID: filterParam = FilteredCompetitionID; break;
                            default: break;
                        }

                        NumberCreatedCertificates = await _documentService.CreateCertificates(true, PersonStartFilter, filterParam);
                        break;
                    }
                case DocumentCreationTypes.OverviewList:
                    {
                        await _documentService.CreateOverviewList();
                        break;
                    }
                case DocumentCreationTypes.RaceStartList:
                    {
                        await _documentService.CreateRaceStartList();
                        break;
                    }
                case DocumentCreationTypes.ResultList:
                    {
                        await _documentService.CreateResultList();
                        break;
                    }
                default: throw new ArgumentOutOfRangeException(nameof(documentType), documentType, "Unknown document type for creation command.");
            }
            changeDocumentCreationSuccessfulState(documentType, true);
        }
        catch (Exception ex)
        {
            await _dialogCoordinator.ShowMessageAsync(this, Resources.ErrorString, ex.Message);
        }
        changeDocumentCreationRunningState(documentType, false);
    }, (documentType) => !IsAnyDocumentCreationRunning));

}
