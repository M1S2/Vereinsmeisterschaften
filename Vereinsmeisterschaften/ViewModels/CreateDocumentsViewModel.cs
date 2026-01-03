using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Resources;
using System.Windows.Data;
using System.Windows.Input;
using Vereinsmeisterschaften.Contracts.ViewModels;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Documents;
using Vereinsmeisterschaften.Core.Helpers;
using Vereinsmeisterschaften.Core.Models;
using Vereinsmeisterschaften.Core.Settings;
using Vereinsmeisterschaften.Properties;

namespace Vereinsmeisterschaften.ViewModels;

/// <summary>
/// ViewModel for creating various types of documents such as certificates, overview
/// </summary>
public partial class CreateDocumentsViewModel : ObservableObject, INavigationAware
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

    /// <summary>
    /// Dictionary to hold the file paths of the last created documents for each <see cref="DocumentCreationTypes"/> type.
    /// </summary>
    public Dictionary<DocumentCreationTypes, string> LastDocumentFilePaths { get; } = new Dictionary<DocumentCreationTypes, string>();

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    private void changeDocumentCreationRunningState(DocumentCreationTypes documentType, bool isRunning)
        => setDictionaryValue(IsDocumentCreationRunning, documentType, isRunning, nameof(IsDocumentCreationRunning));
    
    private void changeDocumentCreationSuccessfulState(DocumentCreationTypes documentType, bool isSuccessful)
        => setDictionaryValue(IsDocumentCreationSuccessful, documentType, isSuccessful, nameof(IsDocumentCreationSuccessful));
    
    private void changeDocumentDataAvailableState(DocumentCreationTypes documentType, bool isDataAvailable)
        => setDictionaryValue(IsDocumentDataAvailable, documentType, isDataAvailable, nameof(IsDocumentDataAvailable));
    
    private void changeDocumentTemplateAvailableState(DocumentCreationTypes documentType, bool isTemplateAvailable)
        => setDictionaryValue(IsDocumentTemplateAvailable, documentType, isTemplateAvailable, nameof(IsDocumentTemplateAvailable));
    
    private void changeLastDocumentFilePath(DocumentCreationTypes documentType, string lastDocumentFilePaths)
        => setDictionaryValue(LastDocumentFilePaths, documentType, lastDocumentFilePaths, nameof(LastDocumentFilePaths));

    private void setDictionaryValue<T>(Dictionary<DocumentCreationTypes, T> dictionary, DocumentCreationTypes documentType, T value, string propertyName)
    {
        if (dictionary.ContainsKey(documentType))
        {
            dictionary[documentType] = value;
            OnPropertyChanged(propertyName);
            ((RelayCommand<DocumentCreationTypes>)CreateDocumentCommand).NotifyCanExecuteChanged();
        }
    }

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    #region Document Ordering and Filters Dictionaries

    /// <summary>
    /// Indicates whether at leas one document creation process is currently running (either certificates or overview list or race start list).
    /// </summary>
    public bool IsAnyDocumentCreationRunning => IsDocumentCreationRunning.Any(r => r.Value == true);

    /// <summary>
    /// Dictionary to hold the available item orderings for each <see cref="DocumentCreationTypes"/> type.
    /// </summary>
    public Dictionary<DocumentCreationTypes, IEnumerable<Enum>> AvailableItemOrderings { get; } = new Dictionary<DocumentCreationTypes, IEnumerable<Enum>>();

    /// <summary>
    /// Dictionary to hold the current item ordering for each <see cref="DocumentCreationTypes"/> type.
    /// </summary>
    public Dictionary<DocumentCreationTypes, Enum> CurrentItemOrderings { get; } = new Dictionary<DocumentCreationTypes, Enum>();

    /// <summary>
    /// Dictionary to hold the available item filters for each <see cref="DocumentCreationTypes"/> type.
    /// </summary>
    public Dictionary<DocumentCreationTypes, IEnumerable<Enum>> AvailableItemFilters { get; } = new Dictionary<DocumentCreationTypes, IEnumerable<Enum>>();

    /// <summary>
    /// Dictionary to hold the current item filter for each <see cref="DocumentCreationTypes"/> type.
    /// </summary>
    public Dictionary<DocumentCreationTypes, Enum> CurrentItemFilters { get; } = new Dictionary<DocumentCreationTypes, Enum>();

    /// <summary>
    /// Dictionary to hold the current item filter parameter for each <see cref="DocumentCreationTypes"/>
    /// </summary>
    public Dictionary<DocumentCreationTypes, object> CurrentItemFilterParameters { get; } = new Dictionary<DocumentCreationTypes, object>();

    #endregion

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    #region Create Certificates Properties

    
    /// <summary>
    /// Number of created certificates during the last creation process.
    /// </summary>
    [ObservableProperty]
    private int _numberCreatedCertificates = -1;
    
    // ----------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// List with all available <see cref="Person"/> objects.
    /// </summary>
    public List<Person> AvailablePersons => _personService?.GetPersons().OrderBy(p => p.Name).ToList();

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    private List<SwimmingStyles> _availableSwimmingStyles = Enum.GetValues(typeof(SwimmingStyles)).Cast<SwimmingStyles>().Where(s => s != SwimmingStyles.Unknown).ToList();
    /// <summary>
    /// List with all available <see cref="SwimmingStyles"/>
    /// </summary>
    public List<SwimmingStyles> AvailableSwimmingStyles => _availableSwimmingStyles;

    #endregion

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    #region Create Time Forms Properties

    /// <summary>
    /// Number of created time forms during the last creation process.
    /// </summary>
    [ObservableProperty]
    private int _numberCreatedTimeForms = -1;

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
    /// Collection view used to display the <see cref="PlaceholderViewConfigs"/>
    /// </summary>
    public ICollectionView PlaceholderViewConfigsView { get; private set; }

    /// <summary>
    /// Initializes the <see cref="PlaceholderViewConfigs"/> collection with available placeholders.
    /// </summary>
    private void initPlaceholderViewConfigs()
    {
        PlaceholderViewConfigs.Clear();
        foreach(KeyValuePair<string, (string groupName, List<string> placeholders)> placeholderEntry in Placeholders.PlaceholderDict)
        {
            string placeholderKey = placeholderEntry.Key;
            ResourceManager rm = new ResourceManager(typeof(Properties.PlaceholderResources));
            string resourceStringName = rm.GetString($"PlaceholderName{placeholderKey}") ?? "?";
            string resourceStringInfo = rm.GetString($"PlaceholderInfo{placeholderKey}") ?? "?";
            string resourceStringGroup = rm.GetString($"PlaceholderGroup{placeholderEntry.Value.groupName}") ?? "?";

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
                Group = resourceStringGroup,
                Key = placeholderKey,
                Name = resourceStringName,
                Info = resourceStringInfo,
                Placeholders = string.Join(Environment.NewLine, placeholderEntry.Value.placeholders.Select(p => $"{_documentService.PlaceholderMarker}{p}{_documentService.PlaceholderMarker}")),
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
    private IWorkspaceService _workspaceService;
    private IDialogCoordinator _dialogCoordinator;
    private ShellViewModel _shellVM;
    private IEnumerable<IDocumentStrategy> _documentStrategies;

    /// <summary>
    /// Constructor for the <see cref="CreateDocumentsViewModel"/> class.
    /// </summary>
    /// <param name="documentService"><see cref="IDocumentService"/> object</param>
    /// <param name="personService"><see cref="IPersonService"/> object</param>
    /// <param name="workspaceService"><see cref="IWorkspaceService"/> object</param>
    /// <param name="dialogCoordinator"><see cref="IDialogCoordinator"/> object</param>
    /// <param name="shellVM"><see cref="ShellViewModel"/> object used for dialog display</param>
    /// <param name="documentStrategies">List with <see cref="IDocumentStrategy"/> objects</param>
    public CreateDocumentsViewModel(IDocumentService documentService, IPersonService personService, IWorkspaceService workspaceService, IDialogCoordinator dialogCoordinator, ShellViewModel shellVM, IEnumerable<IDocumentStrategy> documentStrategies)
    {
        _documentService = documentService;
        _personService = personService;
        _workspaceService = workspaceService;
        _dialogCoordinator = dialogCoordinator;
        _shellVM = shellVM;
        _documentStrategies = documentStrategies;

        List<DocumentCreationTypes> availableDocumentCreationTypes = Enum.GetValues(typeof(DocumentCreationTypes)).Cast<DocumentCreationTypes>().ToList();
        IsDocumentCreationRunning.Clear();
        IsDocumentCreationSuccessful.Clear();
        IsDocumentDataAvailable.Clear();
        IsDocumentTemplateAvailable.Clear();
        LastDocumentFilePaths.Clear();
        AvailableItemOrderings.Clear();
        CurrentItemOrderings.Clear();
        AvailableItemFilters.Clear();
        CurrentItemFilters.Clear();
        CurrentItemFilterParameters.Clear();
        foreach (DocumentCreationTypes type in availableDocumentCreationTypes)
        {
            IsDocumentCreationRunning.Add(type, false);
            IsDocumentCreationSuccessful.Add(type, false);
            IsDocumentDataAvailable.Add(type, false);
            IsDocumentTemplateAvailable.Add(type, false);
            LastDocumentFilePaths.Add(type, string.Empty);
            AvailableItemOrderings.Add(type, _documentStrategies.FirstOrDefault(s => s.DocumentType == type)?.AvailableItemOrderings);
            CurrentItemOrderings.Add(type, _documentStrategies.FirstOrDefault(s => s.DocumentType == type)?.ItemOrdering);
            AvailableItemFilters.Add(type, _documentStrategies.FirstOrDefault(s => s.DocumentType == type)?.AvailableItemFilters);
            CurrentItemFilters.Add(type, _documentStrategies.FirstOrDefault(s => s.DocumentType == type)?.ItemFilter);
            CurrentItemFilterParameters.Add(type, _documentStrategies.FirstOrDefault(s => s.DocumentType == type)?.ItemFilterParameter);
        }

        PlaceholderViewConfigsView = CollectionViewSource.GetDefaultView(PlaceholderViewConfigs);
        PlaceholderViewConfigsView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(DocumentPlaceholderViewConfig.Group)));
    }

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    private ICommand _createDocumentCommand;
    /// <summary>
    /// Command to create documents. The type of document to create is determined by the command parameter (must be of type <see cref="DocumentCreationTypes"/>).
    /// </summary>
    public ICommand CreateDocumentCommand => _createDocumentCommand ?? (_createDocumentCommand = new RelayCommand<DocumentCreationTypes>(async (documentType) =>
    {
        int numCreatedPages = 0;
        string returnFilePath = string.Empty;

        // Update the item ordering and filter for all document strategies before creating the document
        List<DocumentCreationTypes> availableDocumentCreationTypes = Enum.GetValues(typeof(DocumentCreationTypes)).Cast<DocumentCreationTypes>().ToList();
        foreach (DocumentCreationTypes type in availableDocumentCreationTypes)
        {
            IDocumentStrategy strategy = _documentStrategies.FirstOrDefault(s => s.DocumentType == type);
            if (strategy != null)
            {
                strategy.ItemOrdering = CurrentItemOrderings[type];
                strategy.ItemFilter = CurrentItemFilters[type];
                strategy.ItemFilterParameter = CurrentItemFilterParameters[type];
            }
        }

        changeDocumentCreationRunningState(documentType, true);
        try
        {
            changeDocumentCreationSuccessfulState(documentType, false);
            switch (documentType)
            {
                case DocumentCreationTypes.Certificates:
                    {
                        (numCreatedPages, returnFilePath) = await _documentService.CreateDocument(DocumentCreationTypes.Certificates);
                        NumberCreatedCertificates = numCreatedPages;
                        break;
                    }
                case DocumentCreationTypes.OverviewList:
                    {
                        (numCreatedPages, returnFilePath) = await _documentService.CreateDocument(DocumentCreationTypes.OverviewList);
                        break;
                    }
                case DocumentCreationTypes.RaceStartList:
                    {
                        (numCreatedPages, returnFilePath) = await _documentService.CreateDocument(DocumentCreationTypes.RaceStartList);
                        break;
                    }
                case DocumentCreationTypes.TimeForms:
                    {
                        (numCreatedPages, returnFilePath) = await _documentService.CreateDocument(DocumentCreationTypes.TimeForms);
                        NumberCreatedTimeForms = numCreatedPages;
                        break;
                    }
                case DocumentCreationTypes.ResultList:
                    {
                        (numCreatedPages, returnFilePath) = await _documentService.CreateDocument(DocumentCreationTypes.ResultList);
                        break;
                    }
                case DocumentCreationTypes.ResultListDetail:
                    {
                        (numCreatedPages, returnFilePath) = await _documentService.CreateDocument(DocumentCreationTypes.ResultListDetail);
                        break;
                    }
                case DocumentCreationTypes.Analytics:
                    {
                        (numCreatedPages, returnFilePath) = await _documentService.CreateDocument(DocumentCreationTypes.Analytics);
                        break;
                    }
                default: throw new ArgumentOutOfRangeException(nameof(documentType), documentType, "Unknown document type for creation command.");
            }
            changeLastDocumentFilePath(documentType, returnFilePath);
            changeDocumentCreationSuccessfulState(documentType, true);
        }
        catch (Exception ex)
        {
            await _dialogCoordinator.ShowMessageAsync(_shellVM, Resources.ErrorString, ex.Message);
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
                                       File.Exists(strategy.TemplatePath) &&
                                       Path.GetExtension(strategy.TemplatePath) == ".docx";
            changeDocumentTemplateAvailableState(strategy.DocumentType, isTemplateAvailable);

            if (!isDataAvailable || !isTemplateAvailable)
            {
                changeDocumentCreationRunningState(strategy.DocumentType, false);
                changeDocumentCreationSuccessfulState(strategy.DocumentType, false);
            }

            // Find existing documents
            string templateFileNamePostfix = _workspaceService?.Settings?.GetSettingValue<string>(WorkspaceSettings.GROUP_DOCUMENT_CREATION, WorkspaceSettings.SETTING_DOCUMENT_CREATION_TEMPLATE_FILENAME_POSTFIX) ?? string.Empty;
            string documentOutputFolder = _workspaceService?.Settings?.GetSettingValue<string>(WorkspaceSettings.GROUP_DOCUMENT_CREATION, WorkspaceSettings.SETTING_DOCUMENT_CREATION_OUTPUT_FOLDER) ?? string.Empty;
            documentOutputFolder = FilePathHelper.MakePathAbsolute(documentOutputFolder, _workspaceService?.PersistentPath);

            string outputFileNameDocx = Path.GetFileNameWithoutExtension(strategy.TemplatePath);
            outputFileNameDocx = outputFileNameDocx.Replace(templateFileNamePostfix, "") + ".docx";
            string outputFilePathDocx = Path.Combine(documentOutputFolder, outputFileNameDocx);
            string outputFilePathPdf = outputFilePathDocx.Replace(".docx", ".pdf", StringComparison.InvariantCultureIgnoreCase);
            if(File.Exists(outputFilePathPdf))
            {
                changeLastDocumentFilePath(strategy.DocumentType, outputFilePathPdf);
                changeDocumentCreationSuccessfulState(strategy.DocumentType, true);     // necessary to show the path
            }
            else if(File.Exists(outputFilePathDocx))
            {
                changeLastDocumentFilePath(strategy.DocumentType, outputFilePathDocx);
                changeDocumentCreationSuccessfulState(strategy.DocumentType, true);     // necessary to show the path
            }
        }

        initPlaceholderViewConfigs();
    }

    /// <inheritdoc/>
    public void OnNavigatedFrom()
    {
    }
}
