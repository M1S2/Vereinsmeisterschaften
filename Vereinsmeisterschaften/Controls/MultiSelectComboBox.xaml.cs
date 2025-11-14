using CommunityToolkit.Mvvm.Input;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Vereinsmeisterschaften.Controls
{
    public partial class MultiSelectComboBox : UserControl
    {
        public MultiSelectComboBox()
        {
            InitializeComponent();
            SelectedItems = new ObservableCollection<object>();
        }

        // ItemsSource
        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(MultiSelectComboBox), new PropertyMetadata(null));

        // SelectedItems
        public ObservableCollection<object> SelectedItems
        {
            get => (ObservableCollection<object>)GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(nameof(SelectedItems), typeof(ObservableCollection<object>), typeof(MultiSelectComboBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        // ItemTemplate
        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(MultiSelectComboBox), new PropertyMetadata(null));


        private void OnTogglePopup(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (PART_Popup != null)
                PART_Popup.SetCurrentValue(Popup.IsOpenProperty, !PART_Popup.IsOpen);
        }

        private void OnChecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox cb && cb.DataContext != null)
            {
                if (!SelectedItems.Contains(cb.DataContext))
                    SelectedItems.Add(cb.DataContext);
            }
        }

        private void OnUnchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox cb && cb.DataContext != null)
            {
                if (SelectedItems.Contains(cb.DataContext))
                    SelectedItems.Remove(cb.DataContext);
            }
        }

        // Command für Entfernen eines Chips
        public ICommand RemoveItemCommand => new RelayCommand<object>(RemoveItem);

        private void RemoveItem(object item)
        {
            if (item != null && SelectedItems.Contains(item))
            {
                SelectedItems.Remove(item);
            }
        }
    }
}
