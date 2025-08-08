using System.Windows.Input;
using System.Resources;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using Vereinsmeisterschaften.Contracts.ViewModels;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;
using Vereinsmeisterschaften.Core.Documents;
using Vereinsmeisterschaften.Properties;

namespace Vereinsmeisterschaften.ViewModels;

/// <summary>
/// ViewModel for creating various types of documents such as certificates, overview
/// </summary>
public class CreateDocumentsViewModel : ObservableObject, INavigationAware
{
    /// <summary>
    /// Dictionary to hold the state of whether a document creation process is currently running for each <see cref="DocumentCreationTypes"/> type.
    /// </summary>
    public Dictionary<DocumentCreationTypes, bool> IsDocumentCreationRunning { get; } = new Dictionary<DocumentCreationTypes, bool>();

    /// <summary>
    /// Dictionary to hold the state of whether a document creation process was successful for each <see cref="DocumentCreationTypes"/> type.
    /// </summary>
    public Dictionary<DocumentCreationTypes, bool> IsDocumentCreationSuccessful { get; } = new Dictionary<DocumentCreationTypes, bool>();

    /// <summary>
    /// Dictionary to hold the state of whether data is available for document creation for each <see cref="DocumentCreationTypes"/> type.
    /// </summary>
    public Dictionary<DocumentCreationTypes, bool> IsDocumentDataAvailable { get; } = new Dictionary<DocumentCreationTypes, bool>();

    /// <summary>
    /// Dictionary to hold the state of whether the template file is available for document creation for each <see cref="DocumentCreationTypes"/> type.
    /// </summary>
    public Dictionary<DocumentCreationTypes, bool> IsDocumentTemplateAvailable { get; } = new Dictionary<DocumentCreationTypes, bool>();

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

    private void changeDocumentDataAvailableState(DocumentCreationTypes documentType, bool isDataAvailable)
    {
        if (IsDocumentDataAvailable.ContainsKey(documentType))
        {
            IsDocumentDataAvailable[documentType] = isDataAvailable;
            OnPropertyChanged(nameof(IsDocumentDataAvailable));
            ((RelayCommand<DocumentCreationTypes>)CreateDocumentCommand).NotifyCanExecuteChanged();
        }
    }

    private void changeDocumentTemplateAvailableState(DocumentCreationTypes documentType, bool isTemplateAvailable)
    {
        if (IsDocumentTemplateAvailable.ContainsKey(documentType))
        {
            IsDocumentTemplateAvailable[documentType] = isTemplateAvailable;
            OnPropertyChanged(nameof(IsDocumentTemplateAvailable));
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

    #region Create Time Forms Properties

    private int _numberCreatedTimeForms = 0;
    /// <summary>
    /// Number of created time forms during the last creation process.
    /// </summary>
    public int NumberCreatedTimeForms
    {
        get => _numberCreatedTimeForms;
        set => SetProperty(ref _numberCreatedTimeForms, value);
    }

    #endregion

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    #region Placeholders

    private ObservableCollection<DocumentPlaceholderViewConfig> _placeholderViewConfigs = new ObservableCollection<DocumentPlaceholderViewConfig>();
    /// <summary>
    /// Collection of <see cref="DocumentPlaceholderViewConfig"/> objects that define the available placeholders for documents.
    /// </summary>
    public ObservableCollection<DocumentPlaceholderViewConfig> PlaceholderViewConfigs
    {
        get => _placeholderViewConfigs;
        private set => SetProperty(ref _placeholderViewConfigs, value);
    }

    /// <summary>
    /// Initializes the <see cref="PlaceholderViewConfigs"/> collection with available placeholders.
    /// </summary>
    private void initPlaceholderViewConfigs()
    {
        PlaceholderViewConfigs.Clear();
        foreach(KeyValuePair<string, List<string>> placeholders in Placeholders.PlaceholderDict)
        {
            string placeholderKey = placeholders.Key;
            ResourceManager rm = new ResourceManager(typeof(Properties.PlaceholderResources));
            string resourceStringName = rm.GetString($"PlaceholderName{placeholderKey}") ?? "?";
            string resourceStringInfo = rm.GetString($"PlaceholderInfo{placeholderKey}") ?? "?";

            Dictionary<DocumentCreationTypes, bool> isSupportedForDocumentType = new Dictionary<DocumentCreationTypes, bool>();
            Dictionary<DocumentCreationTypes, string> postfixNumbersSupportedForDocumentType = new Dictionary<DocumentCreationTypes, string>();
            foreach (IDocumentStrategy strategy in _documentStrategies)
            {
                DocumentCreationTypes documentCreationType = strategy.DocumentType;
                bool isCurrentPlaceholderSupportedForStrategy = strategy.SupportedPlaceholderKeys.Contains(placeholderKey);

                int indexOfKey = strategy.SupportedPlaceholderKeys.IndexOf(placeholderKey);
                int postfixNumbersSupported = (indexOfKey >= 0 && indexOfKey < strategy.PostfixNumbersSupported.Count) ? strategy.PostfixNumbersSupported[indexOfKey] : 0;
                string postfixNumbersSupportedStr = postfixNumbersSupported > 0 ? postfixNumbersSupported.ToString() : string.Empty;

                if (!isSupportedForDocumentType.ContainsKey(documentCreationType))
                {
                    isSupportedForDocumentType.Add(documentCreationType, isCurrentPlaceholderSupportedForStrategy);
                    postfixNumbersSupportedForDocumentType.Add(documentCreationType, postfixNumbersSupportedStr);
                }
                else
                {
                    isSupportedForDocumentType[documentCreationType] = isCurrentPlaceholderSupportedForStrategy;
                    postfixNumbersSupportedForDocumentType[documentCreationType] = postfixNumbersSupportedStr;
                }
            }

            DocumentPlaceholderViewConfig placeholderViewConfig = new DocumentPlaceholderViewConfig()
            {
                Key = placeholderKey,
                Name = resourceStringName,
                Info = resourceStringInfo,
                Placeholders = string.Join(Environment.NewLine, placeholders.Value.Select(p => $"{_documentService.PlaceholderMarker}{p}{_documentService.PlaceholderMarker}")),
                IsSupportedForDocumentType = isSupportedForDocumentType,
                PostfixNumbersSupportedForDocumentType = postfixNumbersSupportedForDocumentType
            };
            PlaceholderViewConfigs.Add(placeholderViewConfig);
        }
        OnPropertyChanged(nameof(PlaceholderViewConfigs));
    }

    #endregion

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    private IDocumentService _documentService;
    private IPersonService _personService;
    private IDialogCoordinator _dialogCoordinator;
    private IEnumerable<IDocumentStrategy> _documentStrategies;

    /// <summary>
    /// Constructor for the <see cref="CreateDocumentsViewModel"/> class.
    /// </summary>
    /// <param name="documentService"><see cref="IDocumentService"/> object</param>
    /// <param name="personService"><see cref="IPersonService"/> object</param>
    /// <param name="dialogCoordinator"><see cref="IDialogCoordinator"/> object</param>
    /// <param name="documentStrategies">List with <see cref="IDocumentStrategy"/> objects</param>
    public CreateDocumentsViewModel(IDocumentService documentService, IPersonService personService, IDialogCoordinator dialogCoordinator, IEnumerable<IDocumentStrategy> documentStrategies)
    {
        _documentService = documentService;
        _personService = personService;
        _dialogCoordinator = dialogCoordinator;
        _documentStrategies = documentStrategies;

        List<DocumentCreationTypes> availableDocumentCreationTypes = Enum.GetValues(typeof(DocumentCreationTypes)).Cast<DocumentCreationTypes>().ToList();
        IsDocumentCreationRunning.Clear();
        IsDocumentCreationSuccessful.Clear();
        IsDocumentDataAvailable.Clear();
        IsDocumentTemplateAvailable.Clear();
        foreach (DocumentCreationTypes type in availableDocumentCreationTypes)
        {
            IsDocumentCreationRunning.Add(type, false);
            IsDocumentCreationSuccessful.Add(type, false);
            IsDocumentDataAvailable.Add(type, false);
            IsDocumentTemplateAvailable.Add(type, false);
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

                        _documentService.SetCertificateCreationFilters(PersonStartFilter, filterParam);
                        NumberCreatedCertificates = await _documentService.CreateDocument(DocumentCreationTypes.Certificates);
                        break;
                    }
                case DocumentCreationTypes.OverviewList:
                    {
                        await _documentService.CreateDocument(DocumentCreationTypes.OverviewList);
                        break;
                    }
                case DocumentCreationTypes.RaceStartList:
                    {
                        await _documentService.CreateDocument(DocumentCreationTypes.RaceStartList);
                        break;
                    }
                case DocumentCreationTypes.TimeForms:
                    {
                        NumberCreatedTimeForms = await _documentService.CreateDocument(DocumentCreationTypes.TimeForms);
                        break;
                    }
                case DocumentCreationTypes.ResultList:
                    {
                        await _documentService.CreateDocument(DocumentCreationTypes.ResultList);
                        break;
                    }
                case DocumentCreationTypes.ResultListDetail:
                    {
                        await _documentService.CreateDocument(DocumentCreationTypes.ResultListDetail);
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


    /// <inheritdoc/>
    public void OnNavigatedTo(object parameter)
    {
        foreach(IDocumentStrategy strategy in _documentStrategies)
        {
            object[] items = strategy.GetItems();
            bool isDataAvailable = items != null;
            changeDocumentDataAvailableState(strategy.DocumentType, isDataAvailable);

            bool isTemplateAvailable = !string.IsNullOrEmpty(strategy.TemplatePath) && 
                                       System.IO.File.Exists(strategy.TemplatePath) &&
                                       System.IO.Path.GetExtension(strategy.TemplatePath) == ".docx";
            changeDocumentTemplateAvailableState(strategy.DocumentType, isTemplateAvailable);

            if (!isDataAvailable || !isTemplateAvailable)
            {
                changeDocumentCreationRunningState(strategy.DocumentType, false);
                changeDocumentCreationSuccessfulState(strategy.DocumentType, false);
            }
        }

        initPlaceholderViewConfigs();
    }

    /// <inheritdoc/>
    public void OnNavigatedFrom()
    {
    }
}
