using Microsoft.Extensions.Hosting;

using Vereinsmeisterschaften.Contracts.Activation;
using Vereinsmeisterschaften.Contracts.Services;
using Vereinsmeisterschaften.Contracts.Views;
using Vereinsmeisterschaften.ViewModels;

namespace Vereinsmeisterschaften.Services;

/// </inheritdoc>
public class ApplicationHostService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly INavigationService _navigationService;
    private readonly IPersistAndRestoreService _persistAndRestoreService;
    private readonly IThemeSelectorService _themeSelectorService;
    private readonly IEnumerable<IActivationHandler> _activationHandlers;
    private IShellWindow _shellWindow;
    private bool _isInitialized;

    /// <summary>
    /// Constructor for the <see cref="ApplicationHostService"/>.
    /// </summary>
    /// <param name="serviceProvider"><see cref="IServiceProvider"/> object</param>
    /// <param name="activationHandlers">List of <see cref="IActivationHandler"/> objects</param>
    /// <param name="navigationService"><see cref="INavigationService"/> object</param>
    /// <param name="themeSelectorService"><see cref="IThemeSelectorService"/> object</param>
    /// <param name="persistAndRestoreService"><see cref="IPersistAndRestoreService"/> object</param>
    public ApplicationHostService(IServiceProvider serviceProvider, IEnumerable<IActivationHandler> activationHandlers, INavigationService navigationService, IThemeSelectorService themeSelectorService, IPersistAndRestoreService persistAndRestoreService)
    {
        _serviceProvider = serviceProvider;
        _activationHandlers = activationHandlers;
        _navigationService = navigationService;
        _themeSelectorService = themeSelectorService;
        _persistAndRestoreService = persistAndRestoreService;
    }

    /// <summary>
    /// Triggered when the application host is ready to start the service.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
    /// <returns><see cref="Task"/></returns>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // Initialize services that you need before app activation
        await InitializeAsync();

        await HandleActivationAsync();

        // Tasks after activation
        await StartupAsync();
        _isInitialized = true;
    }

    /// <summary>
    /// Triggered when the application host is performing a graceful shutdown.
    /// </summary>
    /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
    /// <returns><see cref="Task"/></returns>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _persistAndRestoreService.PersistData();
        await Task.CompletedTask;
    }

    private async Task InitializeAsync()
    {
        if (!_isInitialized)
        {
            _persistAndRestoreService.RestoreData();
            _themeSelectorService.InitializeTheme();
            await Task.CompletedTask;
        }
    }

    private async Task StartupAsync()
    {
        if (!_isInitialized)
        {
            await Task.CompletedTask;
        }
    }

    private async Task HandleActivationAsync()
    {
        var activationHandler = _activationHandlers.FirstOrDefault(h => h.CanHandle());

        if (activationHandler != null)
        {
            await activationHandler.HandleAsync();
        }

        await Task.CompletedTask;

        if (App.Current.Windows.OfType<IShellWindow>().Count() == 0)
        {
            // Default activation that navigates to the apps default page
            _shellWindow = _serviceProvider.GetService(typeof(IShellWindow)) as IShellWindow;
            _navigationService.Initialize(_shellWindow.GetNavigationFrame());
            _shellWindow.ShowWindow();
            _navigationService.NavigateTo<MainViewModel>();
            await Task.CompletedTask;
        }
    }
}
