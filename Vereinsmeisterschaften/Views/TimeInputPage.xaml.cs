using System.Windows.Controls;

using Vereinsmeisterschaften.ViewModels;

namespace Vereinsmeisterschaften.Views;

public partial class TimeInputPage : Page
{
    public TimeInputPage(TimeInputViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
