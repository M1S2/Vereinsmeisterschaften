using System.Windows.Controls;

using Vereinsmeisterschaften.ViewModels;

namespace Vereinsmeisterschaften.Views;

public partial class PrepareDocumentsPage : Page
{
    public PrepareDocumentsPage(PrepareDocumentsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
