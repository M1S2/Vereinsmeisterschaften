using System.Windows.Controls;

using Vereinsmeisterschaften.ViewModels;

namespace Vereinsmeisterschaften.Views;

public partial class ResultsPage : Page
{
    public ResultsPage(ResultsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
