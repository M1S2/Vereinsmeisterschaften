﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Forms;
using System.Windows.Input;
using Vereinsmeisterschaften.Contracts.ViewModels;
using Vereinsmeisterschaften.Core.Contracts.Services;

namespace Vereinsmeisterschaften.ViewModels;

public class WorkspaceViewModel : ObservableObject, INavigationAware
{
    public string CurrentWorkspaceFolder => _workspaceService.PersistentPath;

    public bool HasUnsavedChanges => _workspaceService.HasUnsavedChanges;

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
        OnPropertyChanged(nameof(NumberStarts));
    }, () => _workspaceService.IsWorkspaceOpen));

    private ICommand _saveWorkspaceCommand;
    public ICommand SaveWorkspaceCommand => _saveWorkspaceCommand ?? (_saveWorkspaceCommand = new RelayCommand(async() =>
    {
        await _workspaceService?.Save(CancellationToken.None);
    }, () => _workspaceService.IsWorkspaceOpen));

    private ICommand _loadWorkspaceCommand;
    public ICommand LoadWorkspaceCommand => _loadWorkspaceCommand ?? (_loadWorkspaceCommand = new RelayCommand(async() =>
    {
        FolderBrowserDialog folderDialog = new FolderBrowserDialog();
        folderDialog.InitialDirectory = CurrentWorkspaceFolder;
        if(folderDialog.ShowDialog() == DialogResult.OK)
        {
            await _workspaceService?.Load(folderDialog.SelectedPath, CancellationToken.None);
            OnPropertyChanged(nameof(NumberPersons));
            OnPropertyChanged(nameof(NumberStarts));
        }
    }));

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    private IWorkspaceService _workspaceService;
    private IPersonService _personService;

    public WorkspaceViewModel(IWorkspaceService workspaceService, IPersonService personService)
    {
        _workspaceService = workspaceService;
        _personService = personService;
    }

    // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    private void _workspaceService_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(IWorkspaceService.HasUnsavedChanges): OnPropertyChanged(nameof(HasUnsavedChanges)); break;
            case nameof(IWorkspaceService.PersistentPath): OnPropertyChanged(nameof(CurrentWorkspaceFolder)); break;
            case nameof(IWorkspaceService.IsWorkspaceOpen):
                {
                    OnPropertyChanged(nameof(CurrentWorkspaceFolder));
                    OnPropertyChanged(nameof(CompetitionYear));
                    ((RelayCommand)LoadWorkspaceCommand).NotifyCanExecuteChanged();
                    ((RelayCommand)SaveWorkspaceCommand).NotifyCanExecuteChanged();
                    ((RelayCommand)CloseWorkspaceCommand).NotifyCanExecuteChanged();
                    break;
                }
            default: break;
        }
    }

    public void OnNavigatedTo(object parameter)
    {
        _workspaceService.PropertyChanged += _workspaceService_PropertyChanged;

        OnPropertyChanged(nameof(CurrentWorkspaceFolder));
        OnPropertyChanged(nameof(CompetitionYear));
        OnPropertyChanged(nameof(NumberPersons));
        OnPropertyChanged(nameof(NumberStarts));
        OnPropertyChanged(nameof(HasUnsavedChanges));
    }

    public void OnNavigatedFrom()
    {
        _workspaceService.PropertyChanged -= _workspaceService_PropertyChanged;
    }
}
