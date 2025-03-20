using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Vereinsmeisterschaften.Contracts.Services;
using Vereinsmeisterschaften.Contracts.ViewModels;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Services;

namespace Vereinsmeisterschaften.ViewModels;

public class PrepareDocumentsViewModel : ObservableObject, INavigationAware
{
    private ICompetitionService _competitionService;
    private IWorkspaceService _workspaceService;
    private IDialogCoordinator _dialogCoordinator;
    private ProgressDialogController _progressController;

    public PrepareDocumentsViewModel(ICompetitionService competitionService, IWorkspaceService workspaceService, IDialogCoordinator dialogCoordinator)
    {
        _competitionService = competitionService;
        _workspaceService = workspaceService;
        _dialogCoordinator = dialogCoordinator;
    }

    private ICommand _calculateRunOrderCommand;
    public ICommand CalculateRunOrderCommand => _calculateRunOrderCommand ?? (_calculateRunOrderCommand = new RelayCommand(async() => 
    {
        ProgressDelegate onProgress = (sender, progress, currentStep) =>
        {
            if (progress % 0.5 == 0)
            {
                _progressController?.SetProgress(progress / 100);
                _progressController?.SetMessage($"{currentStep}: {progress:F1}%");
            }
        };

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        _progressController = await _dialogCoordinator.ShowProgressAsync(this, Properties.Resources.PrepareDocumentsPageTitle, "", true);
        _progressController.Canceled += (sender, e) => cancellationTokenSource.Cancel();

        try
        {
            List<List<int>> result = await _competitionService.CalculateRunOrder(_workspaceService?.Settings?.CompetitionYear ?? 0, cancellationTokenSource.Token, 3, onProgress);
        }
        catch (OperationCanceledException)
        {
            /* Nothing to do here if user canceled */
        }
        catch (Exception ex)
        {
            await _progressController?.CloseAsync();
            await _dialogCoordinator.ShowMessageAsync(this, Properties.Resources.ErrorString, ex.Message);
        }
        finally
        {
            await _progressController?.CloseAsync();
        }
    }));

    public void OnNavigatedTo(object parameter)
    {
    }

    public void OnNavigatedFrom()
    {
    }
}
