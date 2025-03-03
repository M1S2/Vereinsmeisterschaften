using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Forms;
using System.Windows.Input;
using Vereinsmeisterschaften.Contracts.ViewModels;
using Vereinsmeisterschaften.Core.Contracts.Services;

namespace Vereinsmeisterschaften.ViewModels;

public class WorkspaceViewModel : ObservableObject, INavigationAware
{
    public string CurrentWorkspaceFolder => _workspaceService.WorkspaceFolderPath;

    public ushort CompetitionYear
    {
        get => _workspaceService?.Settings?.CompetitionYear ?? 0;
        set
        {
            if(_workspaceService != null && _workspaceService.Settings != null)
            {
                _workspaceService.Settings.CompetitionYear = value;
                OnPropertyChanged();
            }
        }
    }

    private ICommand _closeWorkspaceCommand;
    public ICommand CloseWorkspaceCommand => _closeWorkspaceCommand ?? (_closeWorkspaceCommand = new RelayCommand(async() =>
    {
        await _workspaceService?.CloseWorkspace(true, CancellationToken.None);
    }, () => _workspaceService.IsWorkspaceOpen));

    private ICommand _saveWorkspaceCommand;
    public ICommand SaveWorkspaceCommand => _saveWorkspaceCommand ?? (_saveWorkspaceCommand = new RelayCommand(async() =>
    {
        await _workspaceService?.SaveWorkspace(CancellationToken.None);
    }, () => _workspaceService.IsWorkspaceOpen));

    private ICommand _loadWorkspaceCommand;
    public ICommand LoadWorkspaceCommand => _loadWorkspaceCommand ?? (_loadWorkspaceCommand = new RelayCommand(async() =>
    {
        FolderBrowserDialog folderDialog = new FolderBrowserDialog();
        folderDialog.InitialDirectory = CurrentWorkspaceFolder;
        if(folderDialog.ShowDialog() == DialogResult.OK)
        {
            await _workspaceService?.OpenWorkspace(folderDialog.SelectedPath, CancellationToken.None);
        }
    }));

    private IWorkspaceService _workspaceService;

    public WorkspaceViewModel(IWorkspaceService workspaceService)
    {
        _workspaceService = workspaceService;
        _workspaceService.OnIsWorkspaceOpenChanged += (sender, e) =>
        {
            OnPropertyChanged(nameof(CurrentWorkspaceFolder));
            OnPropertyChanged(nameof(CompetitionYear));
            ((RelayCommand)LoadWorkspaceCommand).NotifyCanExecuteChanged();
            ((RelayCommand)SaveWorkspaceCommand).NotifyCanExecuteChanged();
            ((RelayCommand)CloseWorkspaceCommand).NotifyCanExecuteChanged();
        };
    }

    public void OnNavigatedTo(object parameter)
    {        
    }

    public void OnNavigatedFrom()
    {
    }
}
