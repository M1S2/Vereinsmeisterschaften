using System.Windows.Controls;

using Vereinsmeisterschaften.ViewModels;

namespace Vereinsmeisterschaften.Views;

public partial class PrepareRacesPage : Page
{
    public PrepareRacesPage(PrepareRacesViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
