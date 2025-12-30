using System.Windows;
using System.Windows.Controls;
using Vereinsmeisterschaften.Contracts.ViewModels;

namespace Vereinsmeisterschaften.Views
{
    /// <summary>
    /// Interaktionslogik für WorkspaceManagerUserControl.xaml
    /// </summary>
    public partial class WorkspaceManagerUserControl : UserControl
    {
        public bool IsSidebar
        {
            get { return (bool)GetValue(IsSidebarProperty); }
            set { SetValue(IsSidebarProperty, value); }
        }
        public static readonly DependencyProperty IsSidebarProperty = DependencyProperty.Register(nameof(IsSidebar), typeof(bool), typeof(WorkspaceManagerUserControl), new PropertyMetadata(true));


        public IWorkspaceManagerViewModel ViewModel { get; }

        public WorkspaceManagerUserControl()
        {
            App app = App.Current as App;
            ViewModel = app.GetService<IWorkspaceManagerViewModel>();
            DataContext = ViewModel;
            InitializeComponent();
        }
    }
}
