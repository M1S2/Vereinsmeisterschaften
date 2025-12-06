using System.Windows.Controls;
using Vereinsmeisterschaften.ViewModels;

namespace Vereinsmeisterschaften.Views;

public partial class AnalyticsPage : Page
{
    public AnalyticsPage(AnalyticsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
