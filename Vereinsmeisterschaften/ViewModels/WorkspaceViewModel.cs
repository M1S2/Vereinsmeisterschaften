using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Vereinsmeisterschaften.Contracts.ViewModels;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;
using Vereinsmeisterschaften.Core.Services;
using Vereinsmeisterschaften.Core.Settings;
using Vereinsmeisterschaften.Properties;

namespace Vereinsmeisterschaften.ViewModels;

public class WorkspaceViewModel : ObservableObject, INavigationAware
{
    #region Properties

    public string CurrentWorkspaceFolder => _workspaceService.PersistentPath;

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    public bool HasUnsavedChanges => _workspaceService.HasUnsavedChanges;
    public bool HasUnsavedChanges_Persons => _workspaceService?.HasUnsavedChanges_Persons ?? false;
    public bool HasUnsavedChanges_Competitions => _workspaceService?.HasUnsavedChanges_Competitions ?? false;
    public bool HasUnsavedChanges_Races => _workspaceService?.HasUnsavedChanges_Races ?? false;
    public bool HasUnsavedChanges_Settings => _workspaceService?.HasUnsavedChanges_Settings ?? false;

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    public WorkspaceSettings Settings => _workspaceService?.Settings;
    public WorkspaceSettings SettingsPersistedInFile => _workspaceService?.SettingsPersistedInFile;

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    public int NumberPersons => _personService?.PersonCount ?? 0;
    public int NumberStarts => _personService?.PersonStarts ?? 0;

    #endregion

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    #region Commands: Close / Save / Load

    private ICommand _closeWorkspaceCommand;
    /// <summary>
    /// Command to close the current workspace.
    /// </summary>
    public ICommand CloseWorkspaceCommand => _closeWorkspaceCommand ?? (_closeWorkspaceCommand = new RelayCommand(async() =>
    {
        try
        {
            bool save = true, cancel = false;
            (save, cancel) = await checkForUnsavedChangesAndQueryUserAction();

            if (!cancel)
            {
                await _workspaceService?.CloseWorkspace(CancellationToken.None, save);
                OnPropertyChanged(nameof(NumberPersons));
                OnPropertyChanged(nameof(NumberStarts));
            }
        }
        catch (Exception ex)
        {
            await _dialogCoordinator.ShowMessageAsync(this, Resources.ErrorString, ex.Message);
        }
    }, () => _workspaceService.IsWorkspaceOpen));

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    private ICommand _saveWorkspaceCommand;
    /// <summary>
    /// Command to save the current workspace to a folder.
    /// </summary>
    public ICommand SaveWorkspaceCommand => _saveWorkspaceCommand ?? (_saveWorkspaceCommand = new RelayCommand(async() =>
    {
        try
        {
            await _workspaceService?.Save(CancellationToken.None);
        }
        catch (Exception ex)
        {
            await _dialogCoordinator.ShowMessageAsync(this, Resources.ErrorString, ex.Message);
        }
    }, () => _workspaceService.IsWorkspaceOpen));

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    private ICommand _loadWorkspaceCommand;
    /// <summary>
    /// Command to load a workspace from a folder.
    /// </summary>
    public ICommand LoadWorkspaceCommand => _loadWorkspaceCommand ?? (_loadWorkspaceCommand = new RelayCommand(async() =>
    {
        bool save = true, cancel = false;
        (save, cancel) = await checkForUnsavedChangesAndQueryUserAction();

        if (!cancel)
        {
            try
            {
                FolderBrowserDialog folderDialog = new FolderBrowserDialog();
                folderDialog.InitialDirectory = CurrentWorkspaceFolder;
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    if (_workspaceService?.IsWorkspaceOpen ?? false)
                    {
                        await _workspaceService?.CloseWorkspace(CancellationToken.None, save);
                    }
                    await _workspaceService?.Load(folderDialog.SelectedPath, CancellationToken.None);
                    OnPropertyChanged(nameof(NumberPersons));
                    OnPropertyChanged(nameof(NumberStarts));
                }
            }
            catch (Exception ex)
            {
                await _dialogCoordinator.ShowMessageAsync(this, Resources.ErrorString, ex.Message);
            }
        }
    }));

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    private ICommand _openWorkspaceFolderCommand;
    /// <summary>
    /// Command to open the current workspace folder in the file explorer.
    /// </summary>
    public ICommand OpenWorkspaceFolderCommand => _openWorkspaceFolderCommand ?? (_openWorkspaceFolderCommand = new RelayCommand(() =>
    {
        try
        {
            System.Diagnostics.Process.Start("explorer.exe", CurrentWorkspaceFolder);
        }
        catch (Exception ex)
        {
            _dialogCoordinator.ShowMessageAsync(this, Resources.ErrorString, ex.Message);
        }
    }, () => _workspaceService.IsWorkspaceOpen));    

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Show a dialog to the user when there are unsave changes. The user can choose to save, not save or cancel.
    /// </summary>
    /// <returns>Tuple of two bools (save, cancel)</returns>
    private async Task<(bool saveOut, bool cancelOut)> checkForUnsavedChangesAndQueryUserAction()
    {
        bool save = true, cancel = false;
        if (_workspaceService?.HasUnsavedChanges ?? false)
        {
            MetroDialogSettings dialogButtonSettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = Resources.SaveString,
                NegativeButtonText = Resources.DontSaveString,
                FirstAuxiliaryButtonText = Resources.CancelString,
                DefaultButtonFocus = MessageDialogResult.Affirmative
            };
            MessageDialogResult dialogResult = await _dialogCoordinator.ShowMessageAsync(this, Resources.UnsavedChangesString, Resources.UnsavedChangesSavePromptString,
                                                                                        MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, dialogButtonSettings);
            switch (dialogResult)
            {
                case MessageDialogResult.Affirmative: save = true; cancel = false; break;
                case MessageDialogResult.Negative: save = false; cancel = false; break;
                case MessageDialogResult.FirstAuxiliary: save = false; cancel = true; break;
                default: break;
            }
        }
        return (save, cancel);
    }

    #endregion

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public ObservableCollection<WorkspaceSettingsGroupViewModel> SettingsGroups { get; private set; }

    private Dictionary<string, string> groupKeyLabelsDict = new Dictionary<string, string>()
    {
        { WorkspaceSettings.GROUP_GENERAL, Resources.SettingsGeneralString },
        { WorkspaceSettings.GROUP_RACE_CALCULATION, Resources.SettingsRacesVariantsCalculationString },
        { WorkspaceSettings.GROUP_DOCUMENT_CREATION, Resources.SettingsDocumentCreationString }
    };

    private Dictionary<string, WorkspaceSettingViewConfig> settingKeyConfigDict = new Dictionary<string, WorkspaceSettingViewConfig>()
    {
        { WorkspaceSettings.SETTING_GENERAL_COMPETITIONYEAR, new WorkspaceSettingViewConfig() { Label=Resources.CompetitionYearString, Tooltip = Resources.TooltipCompetitionYear, Icon = "\uE787", Editor = WorkspaceSettingEditorTypes.Numeric } },
        { WorkspaceSettings.SETTING_RACE_CALCULATION_NUMBER_OF_SWIM_LANES, new WorkspaceSettingViewConfig() { Label=Resources.NumberOfSwimLanesString, Tooltip = Resources.TooltipNumberOfSwimLanes, Icon = "\uE9E9", Editor = WorkspaceSettingEditorTypes.Numeric } },
        { WorkspaceSettings.SETTING_RACE_CALCULATION_NUM_RACE_VARIANTS_AFTER_CALCULATION, new WorkspaceSettingViewConfig() { Label=Resources.NumberRacesVariantsAfterCalculationString, Tooltip = Resources.TooltipNumberRacesVariantsAfterCalculation, Icon = "\uE7C1", Editor = WorkspaceSettingEditorTypes.Numeric } },
        { WorkspaceSettings.SETTING_RACE_CALCULATION_MAX_CALCULATION_LOOPS, new WorkspaceSettingViewConfig() { Label=Resources.MaxRacesVariantCalculationLoopsString, Tooltip = Resources.TooltipMaxRacesVariantCalculationLoops, Icon = "\uE895", Editor = WorkspaceSettingEditorTypes.Numeric } },
        { WorkspaceSettings.SETTING_RACE_CALCULATION_MIN_RACES_VARIANTS_SCORE, new WorkspaceSettingViewConfig() { Label=Resources.MinimumRacesVariantsScoreString, Tooltip = Resources.TooltipMinRacesVariantsScore, Icon = "\uEDE1", Editor = WorkspaceSettingEditorTypes.Numeric } },
        { WorkspaceSettings.SETTING_DOCUMENT_CREATION_OUTPUT_FOLDER, new WorkspaceSettingViewConfig() { Label=Resources.DocumentOutputFolderString, Tooltip = Resources.TooltipDocumentOutputFolder, Icon = "\uED25", Editor = WorkspaceSettingEditorTypes.Path } },
        { WorkspaceSettings.SETTING_DOCUMENT_CREATION_CERTIFICATE_TEMPLATE_PATH, new WorkspaceSettingViewConfig() { Label=Resources.CertificateTemplatePathString, Tooltip = Resources.TooltipCertificateTemplatePath, Icon = "\uE8A5", Editor = WorkspaceSettingEditorTypes.Path } },
        { WorkspaceSettings.SETTING_DOCUMENT_CREATION_OVERVIEW_LIST_TEMPLATE_PATH, new WorkspaceSettingViewConfig() { Label=Resources.OverviewlistTemplatePathString, Tooltip = Resources.TooltipOverviewlistTemplatePath, Icon = "\uE9D5", Editor = WorkspaceSettingEditorTypes.Path } },
        { WorkspaceSettings.SETTING_DOCUMENT_CREATION_LIBRE_OFFICE_PATH, new WorkspaceSettingViewConfig() { Label=Resources.LibreOfficePathString, Tooltip = Resources.TooltipLibreOfficePath, Icon = "\uE756", Editor = WorkspaceSettingEditorTypes.Path } }
    };

    private void initSettingsGroups(WorkspaceSettings model)
    {
        if(model == null) { return; }
        SettingsGroups = new ObservableCollection<WorkspaceSettingsGroupViewModel>();

        ResourceDictionary settingEditorTemplatesResourceDictionary = new ResourceDictionary();
        settingEditorTemplatesResourceDictionary.Source = new Uri("pack://application:,,,/Views/WorkspaceSettingEditorTemplates.xaml", UriKind.RelativeOrAbsolute);

        foreach (WorkspaceSettingsGroup group in model.Groups)
        {
            WorkspaceSettingsGroupViewModel groupVm = new WorkspaceSettingsGroupViewModel(
                group,
                groupKeyLabelsDict[group.GroupKey],
                group.Settings.Select(setting =>
                {
                    WorkspaceSettingEditorTypes editorType = settingKeyConfigDict[setting.Key].Editor;
                    DataTemplate editorTemplate = settingEditorTemplatesResourceDictionary[editorType.ToString() + "EditorTemplate"] as DataTemplate;

                    var valueType = setting.ValueType;
                    var genericType = typeof(WorkspaceSettingViewModel<>).MakeGenericType(valueType);
                    return (IWorkspaceSettingViewModel)Activator.CreateInstance(genericType, 
                                                                                setting,
                                                                                settingKeyConfigDict[setting.Key].Label,
                                                                                settingKeyConfigDict[setting.Key].Tooltip,
                                                                                settingKeyConfigDict[setting.Key].Icon,
                                                                                editorTemplate)!;
                }));

            SettingsGroups.Add(groupVm);
        }
    }

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    private IWorkspaceService _workspaceService;
    private IPersonService _personService;
    private IDialogCoordinator _dialogCoordinator;

    public WorkspaceViewModel(IWorkspaceService workspaceService, IPersonService personService, IDialogCoordinator dialogCoordinator)
    {
        _workspaceService = workspaceService;
        _personService = personService;
        _dialogCoordinator = dialogCoordinator;
    }

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    private void _workspaceService_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(IWorkspaceService.HasUnsavedChanges): OnPropertyChanged(nameof(HasUnsavedChanges)); break;
            case nameof(IWorkspaceService.HasUnsavedChanges_Persons): OnPropertyChanged(nameof(HasUnsavedChanges_Persons)); break;
            case nameof(IWorkspaceService.HasUnsavedChanges_Competitions): OnPropertyChanged(nameof(HasUnsavedChanges_Competitions)); break;
            case nameof(IWorkspaceService.HasUnsavedChanges_Races): OnPropertyChanged(nameof(HasUnsavedChanges_Races)); break;
            case nameof(IWorkspaceService.HasUnsavedChanges_Settings):
                {
                    OnPropertyChanged(nameof(HasUnsavedChanges_Settings));
                    OnPropertyChanged(nameof(SettingsGroups));
                    break;
                }
            case nameof(IWorkspaceService.PersistentPath): OnPropertyChanged(nameof(CurrentWorkspaceFolder)); break;
            case nameof(IWorkspaceService.IsWorkspaceOpen):
                {
                    OnPropertyChanged(nameof(CurrentWorkspaceFolder));
                    OnPropertyChanged(nameof(Settings));
                    ((RelayCommand)LoadWorkspaceCommand).NotifyCanExecuteChanged();
                    ((RelayCommand)SaveWorkspaceCommand).NotifyCanExecuteChanged();
                    ((RelayCommand)CloseWorkspaceCommand).NotifyCanExecuteChanged();
                    ((RelayCommand)OpenWorkspaceFolderCommand).NotifyCanExecuteChanged();
                    break;
                }
            default: break;
        }
    }

    public void OnNavigatedTo(object parameter)
    {
        _workspaceService.PropertyChanged += _workspaceService_PropertyChanged;

        OnPropertyChanged(nameof(CurrentWorkspaceFolder));
        OnPropertyChanged(nameof(Settings));
        OnPropertyChanged(nameof(NumberPersons));
        OnPropertyChanged(nameof(NumberStarts));
        OnPropertyChanged(nameof(HasUnsavedChanges));

        initSettingsGroups(_workspaceService.Settings);
    }

    public void OnNavigatedFrom()
    {
        _workspaceService.PropertyChanged -= _workspaceService_PropertyChanged;
    }
}
