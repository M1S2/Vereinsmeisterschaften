using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Resources;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using Vereinsmeisterschaften.Contracts.Services;
using Vereinsmeisterschaften.Contracts.ViewModels;
using Vereinsmeisterschaften.Controls;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Helpers;
using Vereinsmeisterschaften.Core.Services;
using Vereinsmeisterschaften.Core.Settings;
using Vereinsmeisterschaften.Properties;

namespace Vereinsmeisterschaften.ViewModels;

/// <summary>
/// ViewModel for the workspace manager.
/// </summary>
public class WorkspaceManagerViewModel : ObservableObject, IWorkspaceManagerViewModel
{
    /// <summary>
    /// This is the file name for the default templates ZIP (this name should match the name used in the PostBuildEvent of the Vereinsmeisterschaften.csproj)
    /// </summary>
    public const string DEFAULT_TEMPLATE_ZIP_FILE_NAME = "DefaultTemplates.zip";

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    #region Properties

    /// <inheritdoc/>
    public string CurrentWorkspaceFolder => _workspaceService.PersistentPath;

    /// <inheritdoc/>
    public bool HasUnsavedChanges => _workspaceService.HasUnsavedChanges;

    /// <inheritdoc/>
    public ObservableCollection<string> LastWorkspacePaths => _workspaceService.LastWorkspacePaths;

    #endregion

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    #region Commands: Close / Save / Load / New

    private ICommand _closeWorkspaceCommand;
    /// <inheritdoc/>
    public ICommand CloseWorkspaceCommand => _closeWorkspaceCommand ?? (_closeWorkspaceCommand = new RelayCommand(async() =>
    {
        try
        {
            bool save = true, cancel = false;
            (save, cancel) = await _shellVM.CheckForUnsavedChangesAndQueryUserAction();

            if (!cancel)
            {
                await _workspaceService?.CloseWorkspace(CancellationToken.None, save);

                // Only navigate to the main page, when the current page isn't main page or workspace page
                if (!(_navigationService?.CurrentFrameViewModel is MainViewModel) && !(_navigationService?.CurrentFrameViewModel is WorkspaceViewModel))
                {
                    // Open the main page
                    _navigationService.NavigateTo<MainViewModel>();
                }
            }
        }
        catch (Exception ex)
        {
            await _dialogCoordinator.ShowMessageAsync(_shellVM, Resources.ErrorString, ex.Message);
        }
    }, () => _workspaceService.IsWorkspaceOpen));

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    private ICommand _saveWorkspaceCommand;
    /// <inheritdoc/>
    public ICommand SaveWorkspaceCommand => _saveWorkspaceCommand ?? (_saveWorkspaceCommand = new RelayCommand(async() =>
    {
        try
        {
            await _workspaceService?.Save(CancellationToken.None);
        }
        catch (Exception ex)
        {
            await _dialogCoordinator.ShowMessageAsync(_shellVM, Resources.ErrorString, ex.Message);
        }
    }, () => _workspaceService.IsWorkspaceOpen));

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    private ICommand _loadWorkspaceCommand;
    /// <inheritdoc/>
    public ICommand LoadWorkspaceCommand => _loadWorkspaceCommand ?? (_loadWorkspaceCommand = new RelayCommand(async() =>
    {
        bool save = true, cancel = false;
        (save, cancel) = await _shellVM.CheckForUnsavedChangesAndQueryUserAction();

        if (!cancel)
        {
            try
            {
                OpenFolderDialog folderDialog = new OpenFolderDialog();
                folderDialog.Multiselect = false;
                folderDialog.InitialDirectory = CurrentWorkspaceFolder;
                if (folderDialog.ShowDialog() == true)
                {
                    if (_workspaceService?.IsWorkspaceOpen ?? false)
                    {
                        await _workspaceService?.CloseWorkspace(CancellationToken.None, save);
                    }
                    bool result = await _workspaceService?.Load(folderDialog.FolderName, CancellationToken.None);
                    OnWorkspaceLoaded?.Invoke(this, folderDialog.FolderName);
                    OnPropertyChanged(nameof(LastWorkspacePaths));

                    _navigationService.ReloadCurrent();     // Navigate to the current page to trigger the reload of the page

                    if (!result)
                    {
                        await _dialogCoordinator.ShowMessageAsync(_shellVM, Resources.ErrorString, Resources.WorkspaceNotLoadedString);
                    }
                }
            }
            catch (Exception ex)
            {
                await _dialogCoordinator.ShowMessageAsync(_shellVM, Resources.ErrorString, ex.Message);
            }
        }
    }));

    // ----------------------------------------------------------------------------------------------------------------------------------------------
    
    private ICommand _loadLastWorkspaceCommand;
    /// <inheritdoc/>
    public ICommand LoadLastWorkspaceCommand => _loadLastWorkspaceCommand ?? (_loadLastWorkspaceCommand = new RelayCommand<string>(async (param) =>
    {
        bool save = true, cancel = false;
        (save, cancel) = await _shellVM.CheckForUnsavedChangesAndQueryUserAction();

        if (!cancel)
        {
            try
            {
                string path = param as string;
                if (_workspaceService?.IsWorkspaceOpen ?? false)
                {
                    await _workspaceService?.CloseWorkspace(CancellationToken.None, save);
                }
                bool result = await _workspaceService?.Load(path, CancellationToken.None);
                OnWorkspaceLoaded?.Invoke(this, path);
                OnPropertyChanged(nameof(LastWorkspacePaths));

                _navigationService.ReloadCurrent();     // Navigate to the current page to trigger the reload of the page

                if (!result)
                {
                    await _dialogCoordinator.ShowMessageAsync(_shellVM, Resources.ErrorString, Resources.WorkspaceNotLoadedString);
                }
            }
            catch (Exception ex)
            {
                await _dialogCoordinator.ShowMessageAsync(_shellVM, Resources.ErrorString, ex.Message);
            }
        }
    }));

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    private ICommand _openWorkspaceFolderCommand;
    /// <inheritdoc/>
    public ICommand OpenWorkspaceFolderCommand => _openWorkspaceFolderCommand ?? (_openWorkspaceFolderCommand = new RelayCommand(() =>
    {
        try
        {
            System.Diagnostics.Process.Start("explorer.exe", CurrentWorkspaceFolder);
        }
        catch (Exception ex)
        {
            _dialogCoordinator.ShowMessageAsync(_shellVM, Resources.ErrorString, ex.Message);
        }
    }, () => _workspaceService.IsWorkspaceOpen));
    
    // ----------------------------------------------------------------------------------------------------------------------------------------------

    private ICommand _createNewWorkspaceCommand;
    /// <inheritdoc/>
    public ICommand CreateNewWorkspaceCommand => _createNewWorkspaceCommand ?? (_createNewWorkspaceCommand = new RelayCommand(async () =>
    {
        bool save = true, cancel = false;
        (save, cancel) = await _shellVM.CheckForUnsavedChangesAndQueryUserAction();

        if (!cancel)
        {
            try
            {
                NewWorkspaceSettingsDialog dialog = new NewWorkspaceSettingsDialog();
                dialog.PreviousWorkspaceFolder = CurrentWorkspaceFolder;
                await _dialogCoordinator.ShowMetroDialogAsync(_shellVM, dialog);
                MessageDialogResult dialogResult = await dialog.WaitForDialogButtonPressAsync();
                await _dialogCoordinator.HideMetroDialogAsync(_shellVM, dialog);

                if(dialogResult == MessageDialogResult.Affirmative)
                {
                    if (_workspaceService?.IsWorkspaceOpen ?? false)
                    {
                        await _workspaceService?.CloseWorkspace(CancellationToken.None, save);
                    }

                    // Copy files from the old workspace
                    if (!string.IsNullOrEmpty(dialog.PreviousWorkspaceFolder) && dialog.CopyCompetitions)
                    {
                        copyFilesFromOldWorkspace(dialog.PreviousWorkspaceFolder, dialog.NewWorkspaceFolder, WorkspaceService.KEY_FILENAME_COMPETITIONS);
                    }
                    if (!string.IsNullOrEmpty(dialog.PreviousWorkspaceFolder) && dialog.CopyCompetitionDistanceRules)
                    {
                        copyFilesFromOldWorkspace(dialog.PreviousWorkspaceFolder, dialog.NewWorkspaceFolder, WorkspaceService.KEY_FILENAME_COMPETITIONDISTANCERULES);
                    }
                    if (!string.IsNullOrEmpty(dialog.PreviousWorkspaceFolder) && dialog.CopyTemplates)
                    {
                        copyTemplatesFolderFromOldWorkspace(dialog.PreviousWorkspaceFolder, dialog.NewWorkspaceFolder);
                    }

                    bool result = await _workspaceService?.Load(dialog.NewWorkspaceFolder, CancellationToken.None);
                    OnWorkspaceLoaded?.Invoke(this, dialog.NewWorkspaceFolder);
                    OnPropertyChanged(nameof(LastWorkspacePaths));

                    if (!result)
                    {
                        await _dialogCoordinator.ShowMessageAsync(_shellVM, Resources.ErrorString, Resources.WorkspaceNotLoadedString);
                        return;
                    }

                    // Save the workspace to create the workspace files
                    await _workspaceService.Save(CancellationToken.None);
                                        
                    if (dialog.CopyDefaultTemplates)
                    {
                        // Extract the default templates from the ZIP file to the workspace folder (subfolder for the templates)
                        string templateZipPath = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath), DEFAULT_TEMPLATE_ZIP_FILE_NAME);
                        string zipExtractPath = Path.Combine(_workspaceService.PersistentPath, WorkspaceSettings.DEFAULT_TEMPLATE_FOLDER_NAME);
                        if (File.Exists(templateZipPath))
                        {
                            if (Directory.Exists(zipExtractPath))
                            {
                                Directory.Delete(zipExtractPath, true);
                            }
                            ZipFile.ExtractToDirectory(templateZipPath, Path.Combine(_workspaceService.PersistentPath, WorkspaceSettings.DEFAULT_TEMPLATE_FOLDER_NAME));
                        }
                    }

                    // Open the workspace page
                    _navigationService.NavigateTo<WorkspaceViewModel>();
                }
            }
            catch (Exception ex)
            {
                await _dialogCoordinator.ShowMessageAsync(_shellVM, Resources.ErrorString, ex.Message);
            }
        }
    }));

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Copy the file identified by the <paramref name="fileNameResourceKey"/> from the <paramref name="oldWorkspacePath"/> to the <paramref name="newWorkspacePath"/>
    /// Only use this before the workspace is loaded because this method only copies files.
    /// </summary>
    /// <param name="oldWorkspacePath">Path to the old workspace with the source file</param>
    /// <param name="newWorkspacePath">Path to the new workspace where the file should be copied to</param>
    /// <param name="fileNameResourceKey">Key used to identify the file (e.g. <see cref="WorkspaceService.KEY_FILENAME_COMPETITIONS"/>)</param>
    private void copyFilesFromOldWorkspace(string oldWorkspacePath, string newWorkspacePath, string fileNameResourceKey)
    {
        if (!Directory.Exists(oldWorkspacePath) || string.IsNullOrEmpty(newWorkspacePath) || oldWorkspacePath == newWorkspacePath) { return; }

        ResourceManager fileNameResources = new ResourceManager(typeof(Core.Properties.Resources));

        // Get all supported file names that are searched in the old workspace path and combine them to full pathes
        List<string> localizedFileNames = GeneralLocalizationHelper.GetAllTranslationsForKey(fileNameResources, fileNameResourceKey).Values.ToList();
        List<string> localizedFilePaths = localizedFileNames.Select(f => Path.Combine(oldWorkspacePath, f)).ToList();
        foreach (string localizedFilePath in localizedFilePaths)
        {
            if (File.Exists(localizedFilePath))
            {
                string newFileName = fileNameResources.GetString(fileNameResourceKey);
                // Copy the file to the new workspace
                File.Copy(localizedFilePath, Path.Combine(newWorkspacePath, newFileName), true);
                continue;
            }
        }
    }

    // ----------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Copy the templates folder from the <paramref name="oldWorkspacePath"/> to the <paramref name="newWorkspacePath"/>
    /// </summary>
    /// <param name="oldWorkspacePath">Path to the old workspace with the source file</param>
    /// <param name="newWorkspacePath">Path to the new workspace where the file should be copied to</param>
    private void copyTemplatesFolderFromOldWorkspace(string oldWorkspacePath, string newWorkspacePath)
    {
        if (!Directory.Exists(oldWorkspacePath) || string.IsNullOrEmpty(newWorkspacePath) || oldWorkspacePath == newWorkspacePath) { return; }

        string oldTemplateFolder = Path.Combine(oldWorkspacePath, WorkspaceSettings.DEFAULT_TEMPLATE_FOLDER_NAME);
        string newTemplateFolder = Path.Combine(newWorkspacePath, WorkspaceSettings.DEFAULT_TEMPLATE_FOLDER_NAME);

        if(!Directory.Exists(oldTemplateFolder)) { return; }
        if(Directory.Exists(newTemplateFolder))
        {
            Directory.Delete(newTemplateFolder, true);
        }
        Directory.CreateDirectory(newTemplateFolder);

        // https://stackoverflow.com/questions/58744/copy-the-entire-contents-of-a-directory-in-c-sharp
        // create all of the directories (if nested ones exist)
        foreach (string dirPath in Directory.GetDirectories(oldTemplateFolder, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(dirPath.Replace(oldTemplateFolder, newTemplateFolder));
        }
        //Copy all the files & Replaces any files with the same name
        foreach (string newPath in Directory.GetFiles(oldTemplateFolder, "*.*", SearchOption.AllDirectories))
        {
            File.Copy(newPath, newPath.Replace(oldTemplateFolder, newTemplateFolder), true);
        }
    }

    #endregion

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    private ICommand _clearAllLastWorkspacePathsCommand;
    /// <inheritdoc/>
    public ICommand ClearAllLastWorkspacePathsCommand => _clearAllLastWorkspacePathsCommand ?? (_clearAllLastWorkspacePathsCommand = new RelayCommand(() =>
    {
        _workspaceService?.ClearAllLastWorkspacePaths();
    }));

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    #region Events

    /// <inheritdoc/>   
    public event IWorkspaceManagerViewModel.WorkspaceLoadedDelegate OnWorkspaceLoaded;

    #endregion

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    private IWorkspaceService _workspaceService;
    private IDialogCoordinator _dialogCoordinator;
    private ShellViewModel _shellVM;
    private INavigationService _navigationService;

    /// <summary>
    /// Constructor of the workspace view model
    /// </summary>
    /// <param name="workspaceService"><see cref="IWorkspaceService"/> object</param>
    /// <param name="dialogCoordinator"><see cref="IDialogCoordinator"/> object</param>
    /// <param name="shellVM"><see cref="ShellViewModel"/> object used for dialog display</param>
    /// <param name="navigationService"><see cref="INavigationService"/> object</param>
    public WorkspaceManagerViewModel(IWorkspaceService workspaceService, IDialogCoordinator dialogCoordinator, ShellViewModel shellVM, INavigationService navigationService)
    {
        _workspaceService = workspaceService;
        _dialogCoordinator = dialogCoordinator;
        _shellVM = shellVM;
        _navigationService = navigationService;

        _workspaceService.PropertyChanged -= _workspaceService_PropertyChanged;
        _workspaceService.PropertyChanged += _workspaceService_PropertyChanged;
    }

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    private void _workspaceService_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(IWorkspaceService.HasUnsavedChanges): OnPropertyChanged(nameof(HasUnsavedChanges)); break;
            case nameof(IWorkspaceService.PersistentPath): OnPropertyChanged(nameof(CurrentWorkspaceFolder)); break;
            case nameof(IWorkspaceService.LastWorkspacePaths): OnPropertyChanged(nameof(LastWorkspacePaths)); break;
            case nameof(IWorkspaceService.IsWorkspaceOpen):
                {
                    OnPropertyChanged(nameof(CurrentWorkspaceFolder));
                    ((RelayCommand)LoadWorkspaceCommand).NotifyCanExecuteChanged();
                    ((RelayCommand)SaveWorkspaceCommand).NotifyCanExecuteChanged();
                    ((RelayCommand)CloseWorkspaceCommand).NotifyCanExecuteChanged();
                    ((RelayCommand)OpenWorkspaceFolderCommand).NotifyCanExecuteChanged();
                    break;
                }
            default: break;
        }
    }
}
