using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Vereinsmeisterschaften.Contracts.Services;
using Vereinsmeisterschaften.Contracts.Views;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Documents;
using Vereinsmeisterschaften.Core.Models;
using Vereinsmeisterschaften.Core.Services;
using Vereinsmeisterschaften.Models;
using Vereinsmeisterschaften.Services;
using Vereinsmeisterschaften.ViewModels;
using Vereinsmeisterschaften.Views;

namespace Vereinsmeisterschaften;

// For more information about application lifecycle events see https://docs.microsoft.com/dotnet/framework/wpf/app-development/application-management-overview

// WPF UI elements use language en-US by default.
// If you need to support other cultures make sure you add converters and review dates and numbers in your UI to ensure everything adapts correctly.
// Tracking issue for improving this is https://github.com/dotnet/wpf/issues/1946
public partial class App : Application
{
    private IHost _host;

    public T GetService<T>()
        where T : class
        => _host.Services.GetService(typeof(T)) as T;

    public App()
    {
    }

    private async void OnStartup(object sender, StartupEventArgs e)
    {
        var appLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        // For more information about .NET generic host see  https://docs.microsoft.com/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.0
        _host = Host.CreateDefaultBuilder(e.Args)
                .ConfigureAppConfiguration(c =>
                {
                    c.SetBasePath(appLocation);
                })
                .ConfigureServices(ConfigureServices)
                .Build();

        await _host.StartAsync();
    }

    private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        // TODO: Register your services, viewmodels and pages here

        // App Host
        services.AddHostedService<ApplicationHostService>();

        // Activation Handlers

        // Core Services
        services.AddSingleton<IFileService, FileService>();
        services.AddSingleton<IDocumentService, DocumentService>();
        services.AddSingleton<IPersonService, PersonService>();
        services.AddSingleton<ICompetitionService, CompetitionService>();
        services.AddSingleton<IWorkspaceService, WorkspaceService>();
        services.AddSingleton<IScoreService, ScoreService>();
        services.AddSingleton<IRaceService, RaceService>();
        services.AddSingleton<IDocumentPlaceholderResolver<PersonStart>, DocumentPlaceholderResolverPersonStart>();
        services.AddSingleton<IDocumentPlaceholderResolver<Person>, DocumentPlaceholderResolverPerson>();
        services.AddSingleton<IDocumentPlaceholderResolver<Race>, DocumentPlaceholderResolverRace>();
        services.AddSingleton<IDocumentStrategy, DocumentStrategyCertificates>();
        services.AddSingleton<IDocumentStrategy, DocumentStrategyOverviewList>();
        services.AddSingleton<IDocumentStrategy, DocumentStrategyRaceStartList>();
        services.AddSingleton<IDocumentStrategy, DocumentStrategyTimeForms>();
        services.AddSingleton<IDocumentStrategy, DocumentStrategyResultList>();
        services.AddSingleton<IDocumentStrategy, DocumentStrategyResultListDetail>();

        // Services
        services.AddSingleton<IApplicationInfoService, ApplicationInfoService>();
        services.AddSingleton<ISystemService, SystemService>();
        services.AddSingleton<IPersistAndRestoreService, PersistAndRestoreService>();
        services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
        services.AddSingleton<IPageService, PageService>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IDialogCoordinator>(DialogCoordinator.Instance);

        // Views and ViewModels
        services.AddSingleton<IShellWindow, ShellWindow>();
        services.AddSingleton<ShellViewModel>();

        services.AddSingleton<MainViewModel>();
        services.AddSingleton<MainPage>();

        services.AddSingleton<WorkspaceViewModel>();
        services.AddSingleton<WorkspacePage>();

        services.AddSingleton<PeopleViewModel>();
        services.AddSingleton<PeoplePage>();

        services.AddSingleton<PrepareRacesViewModel>();
        services.AddSingleton<PrepareRacesPage>();

        services.AddSingleton<CreateDocumentsViewModel>();
        services.AddSingleton<CreateDocumentsPage>();

        services.AddSingleton<TimeInputViewModel>();
        services.AddSingleton<TimeInputPage>();

        services.AddSingleton<ResultsViewModel>();
        services.AddSingleton<ResultsPage>();

        services.AddSingleton<SettingsViewModel>();
        services.AddSingleton<SettingsPage>();

        // Configuration
        services.Configure<AppConfig>(context.Configuration.GetSection(nameof(AppConfig)));
    }

    private async void OnExit(object sender, ExitEventArgs e)
    {
        await _host.StopAsync();
        _host.Dispose();
        _host = null;
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        // TODO: Please log and handle the exception as appropriate to your scenario
        // For more info see https://docs.microsoft.com/dotnet/api/system.windows.application.dispatcherunhandledexception?view=netcore-3.0
    }
}
