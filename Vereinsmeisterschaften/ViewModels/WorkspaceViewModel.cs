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

    public int NumberPersons => _personService?.PersonCount ?? 0;

    public int NumberStarts => _personService?.PersonStarts ?? 0;

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    private ICommand _closeWorkspaceCommand;
    public ICommand CloseWorkspaceCommand => _closeWorkspaceCommand ?? (_closeWorkspaceCommand = new RelayCommand(async() =>
    {
        await _workspaceService?.CloseWorkspace(true, CancellationToken.None);
        OnPropertyChanged(nameof(NumberPersons));
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
            OnPropertyChanged(nameof(NumberPersons));
        }
    }));

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    private IWorkspaceService _workspaceService;
    private IPersonService _personService;

    public WorkspaceViewModel(IWorkspaceService workspaceService, IPersonService personService)
    {
        _workspaceService = workspaceService;
        _personService = personService;
        _workspaceService.OnIsWorkspaceOpenChanged += (sender, e) =>
        {
            OnPropertyChanged(nameof(CurrentWorkspaceFolder));
            OnPropertyChanged(nameof(CompetitionYear));
            ((RelayCommand)LoadWorkspaceCommand).NotifyCanExecuteChanged();
            ((RelayCommand)SaveWorkspaceCommand).NotifyCanExecuteChanged();
            ((RelayCommand)CloseWorkspaceCommand).NotifyCanExecuteChanged();
        };
    }

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    public void OnNavigatedTo(object parameter)
    {
        OnPropertyChanged(nameof(NumberPersons));
    }

    public void OnNavigatedFrom()
    {
    }
}
