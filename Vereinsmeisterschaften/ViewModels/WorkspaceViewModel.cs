using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;
using Vereinsmeisterschaften.Contracts.ViewModels;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;
using Vereinsmeisterschaften.Core.Services;
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

    #region General Settings Changed Properties / Reset Commands

    /// <summary>
    /// True, if the <see cref="WorkspaceSettings.CompetitionYear"/> property in the <see cref="Settings"/> and <see cref="SettingsPersistedInFile"/> differ.
    /// </summary>
    public bool SettingsChanged_CompetitionYear => Settings?.CompetitionYear != SettingsPersistedInFile?.CompetitionYear;

    private ICommand _settingResetCompetitionYearCommand;
    /// <summary>
    /// Command to reset the <see cref="WorkspaceSettings.CompetitionYear"/> property in <see cref="Settings"/> to the value loaded from the config file.
    /// </summary>
    public ICommand SettingResetCompetitionYearCommand => _settingResetCompetitionYearCommand ?? (_settingResetCompetitionYearCommand = new RelayCommand(() => Settings.CompetitionYear = SettingsPersistedInFile.CompetitionYear));

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// True, if one of the general settings changed against the persisted value.
    /// </summary>
    public bool SettingsChanged_GroupGeneral => SettingsChanged_CompetitionYear;

    private ICommand _settingResetGroupGeneralCommand;
    /// <summary>
    /// Command to reset all general settings to the value loaded from the config file.
    /// </summary>
    public ICommand SettingResetGroupGeneralCommand => _settingResetGroupGeneralCommand ?? (_settingResetGroupGeneralCommand = new RelayCommand(() =>
    {
        SettingResetCompetitionYearCommand.Execute(null);
    }));

    #endregion

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    #region Race Calculation Settings Changed Properties / Reset Commands

    /// <summary>
    /// True, if the <see cref="WorkspaceSettings.NumberOfSwimLanes"/> property in the <see cref="Settings"/> and <see cref="SettingsPersistedInFile"/> differ.
    /// </summary>
    public bool SettingsChanged_NumberOfSwimLanes => Settings?.NumberOfSwimLanes != SettingsPersistedInFile?.NumberOfSwimLanes;

    private ICommand _settingResetNumberOfSwimLanesCommand;
    /// <summary>
    /// Command to reset the <see cref="WorkspaceSettings.NumberOfSwimLanes"/> property in <see cref="Settings"/> to the value loaded from the config file.
    /// </summary>
    public ICommand SettingResetNumberOfSwimLanesCommand => _settingResetNumberOfSwimLanesCommand ?? (_settingResetNumberOfSwimLanesCommand = new RelayCommand(() => Settings.NumberOfSwimLanes = SettingsPersistedInFile.NumberOfSwimLanes));

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// True, if the <see cref="WorkspaceSettings.NumberRacesVariantsAfterCalculation"/> property in the <see cref="Settings"/> and <see cref="SettingsPersistedInFile"/> differ.
    /// </summary>
    public bool SettingsChanged_NumberRacesVariantsAfterCalculation => Settings?.NumberRacesVariantsAfterCalculation != SettingsPersistedInFile?.NumberRacesVariantsAfterCalculation;

    private ICommand _settingResetNumberRacesVariantsAfterCalculationCommand;
    /// <summary>
    /// Command to reset the <see cref="WorkspaceSettings.NumberRacesVariantsAfterCalculation"/> property in <see cref="Settings"/> to the value loaded from the config file.
    /// </summary>
    public ICommand SettingResetNumberRacesVariantsAfterCalculationCommand => _settingResetNumberRacesVariantsAfterCalculationCommand ?? (_settingResetNumberRacesVariantsAfterCalculationCommand = new RelayCommand(() => Settings.NumberRacesVariantsAfterCalculation = SettingsPersistedInFile.NumberRacesVariantsAfterCalculation));

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// True, if the <see cref="WorkspaceSettings.MaxRacesVariantCalculationLoops"/> property in the <see cref="Settings"/> and <see cref="SettingsPersistedInFile"/> differ.
    /// </summary>
    public bool SettingsChanged_MaxRacesVariantCalculationLoops => Settings?.MaxRacesVariantCalculationLoops != SettingsPersistedInFile?.MaxRacesVariantCalculationLoops;

    private ICommand _settingResetMaxRacesVariantCalculationLoopsCommand;
    /// <summary>
    /// Command to reset the <see cref="WorkspaceSettings.MaxRacesVariantCalculationLoops"/> property in <see cref="Settings"/> to the value loaded from the config file.
    /// </summary>
    public ICommand SettingResetMaxRacesVariantCalculationLoopsCommand => _settingResetMaxRacesVariantCalculationLoopsCommand ?? (_settingResetMaxRacesVariantCalculationLoopsCommand = new RelayCommand(() => Settings.MaxRacesVariantCalculationLoops = SettingsPersistedInFile.MaxRacesVariantCalculationLoops));

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// True, if the <see cref="WorkspaceSettings.MinRacesVariantsScore"/> property in the <see cref="Settings"/> and <see cref="SettingsPersistedInFile"/> differ.
    /// </summary>
    public bool SettingsChanged_MinRacesVariantsScore => Settings?.MinRacesVariantsScore != SettingsPersistedInFile?.MinRacesVariantsScore;

    private ICommand _settingResetMinRacesVariantsScoreCommand;
    /// <summary>
    /// Command to reset the <see cref="WorkspaceSettings.MinRacesVariantsScore"/> property in <see cref="Settings"/> to the value loaded from the config file.
    /// </summary>
    public ICommand SettingResetMinRacesVariantsScoreCommand => _settingResetMinRacesVariantsScoreCommand ?? (_settingResetMinRacesVariantsScoreCommand = new RelayCommand(() => Settings.MinRacesVariantsScore = SettingsPersistedInFile.MinRacesVariantsScore));

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// True, if one of the race calculation settings changed against the persisted value.
    /// </summary>
    public bool SettingsChanged_GroupRaceCalculation => SettingsChanged_NumberOfSwimLanes || SettingsChanged_NumberRacesVariantsAfterCalculation || SettingsChanged_MaxRacesVariantCalculationLoops || SettingsChanged_MinRacesVariantsScore;

    private ICommand _settingResetGroupRaceCalculationCommand;
    /// <summary>
    /// Command to reset all race calculation settings to the value loaded from the config file.
    /// </summary>
    public ICommand SettingResetGroupRaceCalculationCommand => _settingResetGroupRaceCalculationCommand ?? (_settingResetGroupRaceCalculationCommand = new RelayCommand(() =>
    {
        SettingResetNumberOfSwimLanesCommand.Execute(null);
        SettingResetNumberRacesVariantsAfterCalculationCommand.Execute(null);
        SettingResetMaxRacesVariantCalculationLoopsCommand.Execute(null);
        SettingResetMinRacesVariantsScoreCommand.Execute(null);
    }));

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// True, if one of the race calculation settings changed against the default value.
    /// </summary>
    public bool SettingsNonDefault_GroupRaceCalculation => (Settings?.NumberOfSwimLanes != WorkspaceSettings.DEFAULT_NUMBER_OF_SWIM_LANES) || 
                                                           (Settings?.NumberRacesVariantsAfterCalculation != WorkspaceSettings.DEFAULT_NUMBER_RACESVARIANTS_AFTER_CALCULATION) || 
                                                           (Settings?.MaxRacesVariantCalculationLoops != WorkspaceSettings.DEFAULT_MAX_RACESVARIANTS_CALCULATION_LOOPS) ||
                                                           (Settings?.MinRacesVariantsScore != WorkspaceSettings.DEFAULT_MIN_RACESVARIANTS_SCORE);

    private ICommand _settingDefaultGroupRaceCalculationCommand;
    /// <summary>
    /// Command to set all race calculation settings to the default values.
    /// </summary>
    public ICommand SettingDefaultGroupRaceCalculationCommand => _settingDefaultGroupRaceCalculationCommand ?? (_settingDefaultGroupRaceCalculationCommand = new RelayCommand(() =>
    {
        Settings.NumberOfSwimLanes = WorkspaceSettings.DEFAULT_NUMBER_OF_SWIM_LANES;
        Settings.NumberRacesVariantsAfterCalculation = WorkspaceSettings.DEFAULT_NUMBER_RACESVARIANTS_AFTER_CALCULATION;
        Settings.MaxRacesVariantCalculationLoops = WorkspaceSettings.DEFAULT_MAX_RACESVARIANTS_CALCULATION_LOOPS;
        Settings.MinRacesVariantsScore = WorkspaceSettings.DEFAULT_MIN_RACESVARIANTS_SCORE;
    }));

    #endregion

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    #region Document Creation Settings Changed Properties / Reset Commands

    /// <summary>
    /// Root path of the workspace, which is used to resolve relative paths in the <see cref="WorkspaceSettings"/>.
    /// </summary>
    public string WorkspaceRootPath => _workspaceService?.PersistentPath ?? string.Empty;

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// True, if the <see cref="WorkspaceSettings.DocumentOutputFolder"/> property in the <see cref="Settings"/> and <see cref="SettingsPersistedInFile"/> differ.
    /// </summary>
    public bool SettingsChanged_DocumentOutputFolder => Settings?.DocumentOutputFolder != SettingsPersistedInFile?.DocumentOutputFolder;

    private ICommand _settingResetDocumentOutputFolderCommand;
    /// <summary>
    /// Command to reset the <see cref="WorkspaceSettings.DocumentOutputFolder"/> property in <see cref="Settings"/> to the value loaded from the config file.
    /// </summary>
    public ICommand SettingResetDocumentOutputFolderCommand => _settingResetDocumentOutputFolderCommand ?? (_settingResetDocumentOutputFolderCommand = new RelayCommand(() => Settings.DocumentOutputFolder = SettingsPersistedInFile.DocumentOutputFolder));

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// True, if the <see cref="WorkspaceSettings.CertificateTemplatePath"/> property in the <see cref="Settings"/> and <see cref="SettingsPersistedInFile"/> differ.
    /// </summary>
    public bool SettingsChanged_CertificateTemplatePath => Settings?.CertificateTemplatePath != SettingsPersistedInFile?.CertificateTemplatePath;

    private ICommand _settingResetCertificateTemplatePathCommand;
    /// <summary>
    /// Command to reset the <see cref="WorkspaceSettings.CertificateTemplatePath"/> property in <see cref="Settings"/> to the value loaded from the config file.
    /// </summary>
    public ICommand SettingResetCertificateTemplatePathCommand => _settingResetCertificateTemplatePathCommand ?? (_settingResetCertificateTemplatePathCommand = new RelayCommand(() => Settings.CertificateTemplatePath = SettingsPersistedInFile.CertificateTemplatePath));

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// True, if the <see cref="WorkspaceSettings.OverviewlistTemplatePath"/> property in the <see cref="Settings"/> and <see cref="SettingsPersistedInFile"/> differ.
    /// </summary>
    public bool SettingsChanged_OverviewlistTemplatePath => Settings?.OverviewlistTemplatePath != SettingsPersistedInFile?.OverviewlistTemplatePath;

    private ICommand _settingResetOverviewlistTemplatePathCommand;
    /// <summary>
    /// Command to reset the <see cref="WorkspaceSettings.OverviewlistTemplatePath"/> property in <see cref="Settings"/> to the value loaded from the config file.
    /// </summary>
    public ICommand SettingResetOverviewlistTemplatePathCommand => _settingResetOverviewlistTemplatePathCommand ?? (_settingResetOverviewlistTemplatePathCommand = new RelayCommand(() => Settings.OverviewlistTemplatePath = SettingsPersistedInFile.OverviewlistTemplatePath));

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// True, if the <see cref="WorkspaceSettings.LibreOfficePath"/> property in the <see cref="Settings"/> and <see cref="SettingsPersistedInFile"/> differ.
    /// </summary>
    public bool SettingsChanged_LibreOfficePath => Settings?.LibreOfficePath != SettingsPersistedInFile?.LibreOfficePath;

    private ICommand _settingResetLibreOfficePathCommand;
    /// <summary>
    /// Command to reset the <see cref="WorkspaceSettings.LibreOfficePath"/> property in <see cref="Settings"/> to the value loaded from the config file.
    /// </summary>
    public ICommand SettingResetLibreOfficePathCommand => _settingResetLibreOfficePathCommand ?? (_settingResetLibreOfficePathCommand = new RelayCommand(() => Settings.LibreOfficePath = SettingsPersistedInFile.LibreOfficePath));

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// True, if one of the document creation settings changed against the persisted value.
    /// </summary>
    public bool SettingsChanged_GroupDocumentCreation => SettingsChanged_DocumentOutputFolder ||
                                                         SettingsChanged_CertificateTemplatePath ||
                                                         SettingsChanged_OverviewlistTemplatePath ||
                                                         SettingsChanged_LibreOfficePath;

    private ICommand _settingResetGroupDocumentCreationCommand;
    /// <summary>
    /// Command to reset all document creation settings to the value loaded from the config file.
    /// </summary>
    public ICommand SettingResetGroupDocumentCreationCommand => _settingResetGroupDocumentCreationCommand ?? (_settingResetGroupDocumentCreationCommand = new RelayCommand(() =>
    {
        SettingResetDocumentOutputFolderCommand.Execute(null);
        SettingResetCertificateTemplatePathCommand.Execute(null);
        SettingResetOverviewlistTemplatePathCommand.Execute(null);
        SettingResetLibreOfficePathCommand.Execute(null);
    }));

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// True, if one of the document creation settings changed against the default value.
    /// </summary>
    public bool SettingsNonDefault_GroupDocumentCreation => (Settings?.DocumentOutputFolder != WorkspaceSettings.DEFAULT_DOCUMENT_OUTPUT_FOLDER) ||
                                                            (Settings?.CertificateTemplatePath != WorkspaceSettings.DEFAULT_CERTIFICATE_TEMPLATE_PATH) ||
                                                            (Settings?.OverviewlistTemplatePath != WorkspaceSettings.DEFAULT_OVERVIEW_LIST_TEMPLATE_PATH) ||
                                                            (Settings?.LibreOfficePath != WorkspaceSettings.DEFAULT_LIBRE_OFFICE_PATH);

    private ICommand _settingDefaultGroupDocumentCreationCommand;
    /// <summary>
    /// Command to set all document creation settings to the default values.
    /// </summary>
    public ICommand SettingDefaultGroupDocumentCreationCommand => _settingDefaultGroupDocumentCreationCommand ?? (_settingDefaultGroupDocumentCreationCommand = new RelayCommand(() =>
    {
        Settings.CertificateTemplatePath = WorkspaceSettings.DEFAULT_CERTIFICATE_TEMPLATE_PATH;
        Settings.DocumentOutputFolder = WorkspaceSettings.DEFAULT_DOCUMENT_OUTPUT_FOLDER;
        Settings.OverviewlistTemplatePath = WorkspaceSettings.DEFAULT_OVERVIEW_LIST_TEMPLATE_PATH;
        Settings.LibreOfficePath = WorkspaceSettings.DEFAULT_LIBRE_OFFICE_PATH;
    }));

    #endregion

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
                    OnPropertyChanged(nameof(SettingsChanged_CompetitionYear));
                    OnPropertyChanged(nameof(SettingsChanged_GroupGeneral));
                    OnPropertyChanged(nameof(SettingsChanged_NumberOfSwimLanes));
                    OnPropertyChanged(nameof(SettingsChanged_NumberRacesVariantsAfterCalculation));
                    OnPropertyChanged(nameof(SettingsChanged_MaxRacesVariantCalculationLoops));
                    OnPropertyChanged(nameof(SettingsChanged_MinRacesVariantsScore));
                    OnPropertyChanged(nameof(SettingsChanged_GroupRaceCalculation));
                    OnPropertyChanged(nameof(SettingsNonDefault_GroupRaceCalculation));
                    OnPropertyChanged(nameof(SettingsChanged_DocumentOutputFolder));
                    OnPropertyChanged(nameof(SettingsChanged_CertificateTemplatePath));
                    OnPropertyChanged(nameof(SettingsChanged_OverviewlistTemplatePath));
                    OnPropertyChanged(nameof(SettingsChanged_LibreOfficePath));
                    OnPropertyChanged(nameof(SettingsChanged_GroupDocumentCreation));
                    OnPropertyChanged(nameof(SettingsNonDefault_GroupDocumentCreation));
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
    }

    public void OnNavigatedFrom()
    {
        _workspaceService.PropertyChanged -= _workspaceService_PropertyChanged;
    }
}
