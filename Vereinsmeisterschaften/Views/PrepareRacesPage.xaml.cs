using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Vereinsmeisterschaften.ViewModels;

namespace Vereinsmeisterschaften.Views;

public partial class PrepareRacesPage : Page
{
    public PrepareRacesPage(PrepareRacesViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    /// <summary>
    /// Handle the scroll event of the inner list box (used to display one race) and defer the scrolling event to the parent element
    /// </summary>
    /// <see href="https://serialseb.com/blog/2007/09/03/wpf-tips-6-preventing-scrollviewer-from/"/>
    private void InnerListBoxRace_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (sender is ListBox && !e.Handled)
        {
            e.Handled = true;
            MouseWheelEventArgs eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            eventArg.RoutedEvent = UIElement.MouseWheelEvent;
            eventArg.Source = sender;
            UIElement parent = ((Control)sender)?.Parent as UIElement;
            parent?.RaiseEvent(eventArg);
        }
    }
}
