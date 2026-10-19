using System.Windows;
using System.Windows.Controls;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Views
{
    /// <summary>
    /// Interaktionslogik für ResultsUserControlPodiumPersonStart.xaml
    /// </summary>
    public partial class ResultsUserControlPodiumPersonStart : UserControl
    {
        public enum PodiumPersonStartDisplayStyles
        {
            Normal,
            Small
        }


        public PodiumPersonStartDisplayStyles DisplayStyle
        {
            get { return (PodiumPersonStartDisplayStyles)GetValue(DisplayStyleProperty); }
            set { SetValue(DisplayStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayStyleProperty = DependencyProperty.Register(nameof(DisplayStyle), typeof(PodiumPersonStartDisplayStyles), typeof(ResultsUserControlPodiumPersonStart), new PropertyMetadata(PodiumPersonStartDisplayStyles.Normal));


        public PersonStart Start
        {
            get { return (PersonStart)GetValue(StartProperty); }
            set { SetValue(StartProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Start.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartProperty = DependencyProperty.Register(nameof(Start), typeof(PersonStart), typeof(ResultsUserControlPodiumPersonStart), new PropertyMetadata(null));


        public ResultPodiumsPlaces PodiumsPlace
        {
            get { return (ResultPodiumsPlaces)GetValue(PodiumsPlaceProperty); }
            set { SetValue(PodiumsPlaceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PodiumsPlaces.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PodiumsPlaceProperty = DependencyProperty.Register(nameof(PodiumsPlace), typeof(ResultPodiumsPlaces), typeof(ResultsUserControlPodiumPersonStart), new PropertyMetadata(ResultPodiumsPlaces.Gold));


        public ResultsUserControlPodiumPersonStart()
        {
            InitializeComponent();
        }
    }
}
