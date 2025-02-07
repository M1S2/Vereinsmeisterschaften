using System.Windows.Controls;

using Vereinsmeisterschaften.ViewModels;

namespace Vereinsmeisterschaften.Views;

public partial class PeoplePage : Page
{
    public PeoplePage(PeopleViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
