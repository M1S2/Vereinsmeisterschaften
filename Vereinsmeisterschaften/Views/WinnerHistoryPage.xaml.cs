using System.Windows.Controls;

using Vereinsmeisterschaften.ViewModels;

namespace Vereinsmeisterschaften.Views;

public partial class WinnerHistoryPage : Page
{
    public WinnerHistoryPage(WinnerHistoryViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
