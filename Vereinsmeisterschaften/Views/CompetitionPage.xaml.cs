using System.Windows.Controls;

using Vereinsmeisterschaften.ViewModels;

namespace Vereinsmeisterschaften.Views;

public partial class CompetitionPage : Page
{
    public CompetitionPage(CompetitionViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
