using System.Windows.Controls;

using Vereinsmeisterschaften.ViewModels;

namespace Vereinsmeisterschaften.Views;

public partial class CreateDocumentsPage : Page
{
    public CreateDocumentsPage(CreateDocumentsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
