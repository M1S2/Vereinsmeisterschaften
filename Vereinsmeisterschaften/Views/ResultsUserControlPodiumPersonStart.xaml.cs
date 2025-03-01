using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Views
{
    /// <summary>
    /// Interaktionslogik für ResultsUserControlPodiumPersonStart.xaml
    /// </summary>
    public partial class ResultsUserControlPodiumPersonStart : UserControl
    {
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
