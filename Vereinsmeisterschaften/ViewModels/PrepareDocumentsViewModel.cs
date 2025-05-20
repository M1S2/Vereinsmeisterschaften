using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Vereinsmeisterschaften.Contracts.Services;
using Vereinsmeisterschaften.Contracts.ViewModels;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;
using Vereinsmeisterschaften.Core.Services;

namespace Vereinsmeisterschaften.ViewModels;

public class PrepareDocumentsViewModel : ObservableObject, INavigationAware
{
    private IDocumentService _documentService;

    public PrepareDocumentsViewModel(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    public async void OnNavigatedTo(object parameter)
    {
        await _documentService.CreateCertificates();
        await _documentService.CreateOverviewList();
        MessageBox.Show("Fertig");
    }

    public void OnNavigatedFrom()
    {
    }
}
