using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using Vereinsmeisterschaften.Contracts.ViewModels;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Settings;
using Vereinsmeisterschaften.Properties;

namespace Vereinsmeisterschaften.ViewModels;

/// <summary>
/// ViewModel for the workspace, managing the current workspace folder, settings, and commands to load, save, and close the workspace.
/// </summary>
public class WorkspaceViewModel : ObservableObject, INavigationAware
{
    #region Properties

    /// <summary>
    /// Current workspace folder path.
    /// </summary>
    public string CurrentWorkspaceFolder => _workspaceService.PersistentPath;

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// True if the workspace has unsaved changes, false otherwise.
    /// </summary>
    public bool HasUnsavedChanges => _workspaceService.HasUnsavedChanges;

    /// <summary>
    /// True if the workspace has unsaved changes in persons.
    /// </summary>
    public bool HasUnsavedChanges_Persons => _workspaceService?.HasUnsavedChanges_Persons ?? false;

    /// <summary>
    /// True if the workspace has unsaved changes in competitions.
    /// </summary>
    public bool HasUnsavedChanges_Competitions => _workspaceService?.HasUnsavedChanges_Competitions ?? false;

    /// <summary>
    /// True if the workspace has unsaved changes in races.
    /// </summary>
    public bool HasUnsavedChanges_Races => _workspaceService?.HasUnsavedChanges_Races ?? false;

    /// <summary>
    /// True if the workspace has unsaved changes in settings.
    /// </summary>
    public bool HasUnsavedChanges_Settings => _workspaceService?.HasUnsavedChanges_Settings ?? false;

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Settings for the current workspace.
    /// </summary>
    public WorkspaceSettings Settings => _workspaceService?.Settings;

    /// <summary>
    /// Settings for the current workspace that are persisted in the file.
    /// </summary>
    public WorkspaceSettings SettingsPersistedInFile => _workspaceService?.SettingsPersistedInFile;

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Number of persons in the current workspace.
    /// </summary>
    public int NumberPersons => _personService?.PersonCount ?? 0;

    /// <summary>
    /// Number of starts in the current workspace.
    /// </summary>
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
                    initSettingsGroups(_workspaceService.Settings);
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

    #region Settings Groups Handling

    private ObservableCollection<WorkspaceSettingsGroupViewModel> _settingsGroups;
    /// <summary>
    /// List of <see cref="WorkspaceSettingsGroupViewModel"/> instances representing the settings groups in the workspace.
    /// </summary>
    public ObservableCollection<WorkspaceSettingsGroupViewModel> SettingsGroups
    {
        get => _settingsGroups;
        private set => SetProperty(ref _settingsGroups, value);
    }

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
                WorkspaceSettingViewConfigs.GroupKeyLabelsDict[group.GroupKey],
                group.Settings.Select(setting =>
                {
                    WorkspaceSettingEditorTypes editorType = WorkspaceSettingViewConfigs.SettingKeyConfigDict[setting.Key].Editor;
                    DataTemplate editorTemplate = settingEditorTemplatesResourceDictionary[editorType.ToString() + "EditorTemplate"] as DataTemplate;

                    var valueType = setting.ValueType;
                    var genericType = typeof(WorkspaceSettingViewModel<>).MakeGenericType(valueType);
                    return (IWorkspaceSettingViewModel)Activator.CreateInstance(genericType, 
                                                                                setting,
                                                                                WorkspaceSettingViewConfigs.SettingKeyConfigDict[setting.Key].Label,
                                                                                WorkspaceSettingViewConfigs.SettingKeyConfigDict[setting.Key].Tooltip,
                                                                                WorkspaceSettingViewConfigs.SettingKeyConfigDict[setting.Key].Icon,
                                                                                editorTemplate,
                                                                                WorkspaceSettingViewConfigs.SettingKeyConfigDict[setting.Key].SupportResetToDefault)!;
                }));

            SettingsGroups.Add(groupVm);
        }
    }

    #endregion

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    private IWorkspaceService _workspaceService;
    private IPersonService _personService;
    private IDialogCoordinator _dialogCoordinator;

    /// <summary>
    /// Constructor of the workspace view model
    /// </summary>
    /// <param name="workspaceService"><see cref="IWorkspaceService"/> object</param>
    /// <param name="personService"><see cref="IPersonService"/> object</param>
    /// <param name="dialogCoordinator"><see cref="IDialogCoordinator"/> object</param>
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

    /// <inheritdoc/>
    public void OnNavigatedTo(object parameter)
    {
        _workspaceService.PropertyChanged += _workspaceService_PropertyChanged;

        OnPropertyChanged(nameof(CurrentWorkspaceFolder));
        OnPropertyChanged(nameof(Settings));
        OnPropertyChanged(nameof(NumberPersons));
        OnPropertyChanged(nameof(NumberStarts));
        OnPropertyChanged(nameof(HasUnsavedChanges));
        OnPropertyChanged(nameof(HasUnsavedChanges_Competitions));
        OnPropertyChanged(nameof(HasUnsavedChanges_Persons));
        OnPropertyChanged(nameof(HasUnsavedChanges_Races));
        OnPropertyChanged(nameof(HasUnsavedChanges_Settings));

        initSettingsGroups(_workspaceService.Settings);
    }

    /// <inheritdoc/>
    public void OnNavigatedFrom()
    {
        _workspaceService.PropertyChanged -= _workspaceService_PropertyChanged;
    }
}
