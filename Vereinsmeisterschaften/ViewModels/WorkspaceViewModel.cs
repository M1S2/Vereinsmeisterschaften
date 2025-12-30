using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Vereinsmeisterschaften.Contracts.ViewModels;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Settings;

namespace Vereinsmeisterschaften.ViewModels;

/// <summary>
/// ViewModel for the workspace, managing the current workspace folder, settings, and commands to load, save, and close the workspace.
/// </summary>
public partial class WorkspaceViewModel : ObservableObject, INavigationAware
{
    #region Properties

    /// <summary>
    /// Current workspace folder path.
    /// </summary>
    public string CurrentWorkspaceFolder => _workspaceService.PersistentPath;

    /// <summary>
    /// Is a workspace opened at the moment?
    /// </summary>
    public bool IsCurrentWorkspaceOpen => _workspaceService.IsWorkspaceOpen;

    /// <summary>
    /// True, when the WorkspaceManagerUserControl should be visible.
    /// </summary>
    [ObservableProperty]
    private bool _isWorkspaceManagerExpanded;

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

    #endregion

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    #region Reset Commands

    private ICommand _resetCompetitionsCommand;
    /// <summary>
    /// Command to reset the competitions.
    /// </summary>
    public ICommand ResetCompetitionsCommand => _resetCompetitionsCommand ?? (_resetCompetitionsCommand = new RelayCommand(() =>
    {
        _workspaceService.ResetCompetitionsToLoadedState();
        OnPropertyChanged(nameof(HasUnsavedChanges_Competitions));
    }));

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    private ICommand _resetPersonsCommand;
    /// <summary>
    /// Command to reset the persons.
    /// </summary>
    public ICommand ResetPersonsCommand => _resetPersonsCommand ?? (_resetPersonsCommand = new RelayCommand(() =>
    {
        _workspaceService.ResetPersonsToLoadedState();
        OnPropertyChanged(nameof(HasUnsavedChanges_Persons));
    }));

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    private ICommand _resetRacesCommand;
    /// <summary>
    /// Command to reset the races.
    /// </summary>
    public ICommand ResetRacesCommand => _resetRacesCommand ?? (_resetRacesCommand = new RelayCommand(() =>
    {
        _workspaceService.ResetRacesToLoadedState();
        OnPropertyChanged(nameof(HasUnsavedChanges_Races));
    }));

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    private ICommand _resetSettingsCommand;
    /// <summary>
    /// Command to reset the settings.
    /// </summary>
    public ICommand ResetSettingsCommand => _resetSettingsCommand ?? (_resetSettingsCommand = new RelayCommand(() =>
    {
        _workspaceService.ResetSettingsToLoadedState();
        OnPropertyChanged(nameof(HasUnsavedChanges_Settings));
        initSettingsGroups(_workspaceService.Settings);
    }));

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

        foreach (KeyValuePair<string, string> groupViewConfig in WorkspaceSettingViewConfigs.GroupKeyLabelsDict)
        {
            // Get the group from the model. If no group with this key exists, skip it.
            WorkspaceSettingsGroup groupModel = model?.Groups?.FirstOrDefault(g => g.GroupKey == groupViewConfig.Key);
            if (groupModel == null) { continue; }

            List<IWorkspaceSettingViewModel> settingsVms = new List<IWorkspaceSettingViewModel>();

            // Iterate over the setting view configs
            foreach (KeyValuePair<(string, string), WorkspaceSettingViewConfig> settingViewConfig in WorkspaceSettingViewConfigs.SettingKeyConfigDict)
            {
                (string groupKey, string settingKey) = settingViewConfig.Key;

                // Only consider settings that belong to the current group
                if (groupKey != groupViewConfig.Key) { continue; }

                // Get the setting from the model. If no setting with this key exists, skip it.
                IWorkspaceSetting settingModel = groupModel?.Settings?.FirstOrDefault(s => s.Key == settingKey);
                if (settingModel == null) { continue; }

                WorkspaceSettingEditorTypes editorType = settingViewConfig.Value?.Editor ?? WorkspaceSettingEditorTypes.String;
                DataTemplate editorTemplate = settingEditorTemplatesResourceDictionary[editorType.ToString() + "EditorTemplate"] as DataTemplate;

                // Create the view model for this setting
                Type genericType = typeof(WorkspaceSettingViewModel<>).MakeGenericType(settingModel.ValueType);
                IWorkspaceSettingViewModel settingVm = (IWorkspaceSettingViewModel)Activator.CreateInstance(genericType,
                                                                                                            settingModel,
                                                                                                            settingViewConfig.Value.Label,
                                                                                                            settingViewConfig.Value.Tooltip,
                                                                                                            settingViewConfig.Value.Icon,
                                                                                                            settingViewConfig.Value.IconGeometry,
                                                                                                            editorTemplate,
                                                                                                            settingViewConfig.Value.SupportResetToDefault)!;
                settingsVms.Add(settingVm);
            }

            WorkspaceSettingsGroupViewModel groupVm = new WorkspaceSettingsGroupViewModel(groupModel, groupViewConfig.Value, settingsVms);
            SettingsGroups.Add(groupVm);
        }
    }

    #endregion

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    private IWorkspaceService _workspaceService;
    private IWorkspaceManagerViewModel _workspaceManagerViewModel;

    /// <summary>
    /// Constructor of the workspace view model
    /// </summary>
    /// <param name="workspaceService"><see cref="IWorkspaceService"/> object</param>
    /// <param name="workspaceManagerViewModel"><see cref="IWorkspaceManagerViewModel"/> object used for workspace management commands</param>
    public WorkspaceViewModel(IWorkspaceService workspaceService, IWorkspaceManagerViewModel workspaceManagerViewModel)
    {
        _workspaceService = workspaceService;
        _workspaceManagerViewModel = workspaceManagerViewModel;
        _workspaceManagerViewModel.OnWorkspaceLoaded += (sender, path) => initSettingsGroups(_workspaceService.Settings);
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
                    OnPropertyChanged(nameof(IsCurrentWorkspaceOpen));
                    OnPropertyChanged(nameof(Settings));
                    IsWorkspaceManagerExpanded = !IsCurrentWorkspaceOpen;
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
        OnPropertyChanged(nameof(IsCurrentWorkspaceOpen));
        OnPropertyChanged(nameof(Settings));
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
