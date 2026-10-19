using System.Windows;
using System.Windows.Controls;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Views
{
    /// <summary>
    /// Interaktionslogik für WinnerHistoryUserControlPodium.xaml
    /// </summary>
    public partial class WinnerHistoryUserControlPodium : UserControl
    {
        public string PersonFirstName
        {
            get { return (string)GetValue(PersonFirstNameProperty); }
            set { SetValue(PersonFirstNameProperty, value); }
        }
        // Using a DependencyProperty as the backing store for PersonFirstName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PersonFirstNameProperty = DependencyProperty.Register(nameof(PersonFirstName), typeof(string), typeof(WinnerHistoryUserControlPodium), new PropertyMetadata(null));


        public string PersonName
        {
            get { return (string)GetValue(PersonNameProperty); }
            set { SetValue(PersonNameProperty, value); }
        }
        // Using a DependencyProperty as the backing store for PersonName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PersonNameProperty = DependencyProperty.Register(nameof(PersonName), typeof(string), typeof(WinnerHistoryUserControlPodium), new PropertyMetadata(null));


        public ResultPodiumsPlaces PodiumsPlace
        {
            get { return (ResultPodiumsPlaces)GetValue(PodiumsPlaceProperty); }
            set { SetValue(PodiumsPlaceProperty, value); }
        }
        // Using a DependencyProperty as the backing store for PodiumsPlaces.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PodiumsPlaceProperty = DependencyProperty.Register(nameof(PodiumsPlace), typeof(ResultPodiumsPlaces), typeof(WinnerHistoryUserControlPodium), new PropertyMetadata(ResultPodiumsPlaces.Gold));


        public bool Highlighted
        {
            get { return (bool)GetValue(HighlightedProperty); }
            set { SetValue(HighlightedProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Highlighted.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HighlightedProperty = DependencyProperty.Register(nameof(Highlighted), typeof(bool), typeof(WinnerHistoryUserControlPodium), new PropertyMetadata(false));


        public WinnerHistoryUserControlPodium()
        {
            InitializeComponent();
        }
    }
}
