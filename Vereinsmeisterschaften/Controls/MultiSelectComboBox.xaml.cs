using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Input;

namespace Vereinsmeisterschaften.Controls
{
    /// <summary>
    /// Control for a ComboBox that allows multiple selection via CheckBoxes.
    /// </summary>
    public partial class MultiSelectComboBox : UserControl
    {
        #region Constructor

        public MultiSelectComboBox()
        {
            InitializeComponent();
        }
        
        #endregion

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Dependency Properties

        /// <summary>
        /// List with all available items
        /// </summary>
        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(MultiSelectComboBox), new PropertyMetadata(null));

        /// <summary>
        /// List with all selected items
        /// </summary>
        public IList SelectedItems
        {
            get => (IList)GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }
        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(nameof(SelectedItems), typeof(IList), typeof(MultiSelectComboBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Template used to display the Checkbox contents and the selected items
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }
        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(MultiSelectComboBox), new PropertyMetadata(null));

        /// <summary>
        /// True, when the popup with the selection options is open
        /// </summary>
        public bool IsPopupOpen
        {
            get { return (bool)GetValue(IsPopupOpenProperty); }
            set { SetValue(IsPopupOpenProperty, value); }
        }
        public static readonly DependencyProperty IsPopupOpenProperty = DependencyProperty.Register(nameof(IsPopupOpen), typeof(bool), typeof(MultiSelectComboBox), new PropertyMetadata(false, OnIsPopupOpenChanged));

        private static void OnIsPopupOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiSelectComboBox control = d as MultiSelectComboBox;
            if(control != null && control.IsPopupOpen)
            {
                control.refreshPopupCheckBoxes();
            }
        }

        #endregion

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Popup Toggle Handling

        /// <summary>
        /// Toggle the popup open/closed when clicking on the control, but not when clicking the remove button of an item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTogglePopup(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (SelectedItems == null)
            {
                SelectedItems = new ObservableCollection<object>();
            }

            // Look for a Button in the visual tree of the original source (remove button of an item)
            // When the remove button was clicked → make sure the popup is closed
            Button removeButton = findParentOfType<Button>(e.OriginalSource as DependencyObject);
            if (removeButton != null)
            {
                IsPopupOpen = false;
                return;
            }

            IsPopupOpen = !IsPopupOpen;
        }

        // https://stopbyte.com/t/how-can-i-show-popup-on-button-click-and-hide-it-on-second-click-or-user-clicks-outside-staysopen-false/80/5
        private void PART_ComboBoxContent_MouseEnter(object sender, MouseEventArgs e)
        {
            PART_Popup.StaysOpen = true;
        }

        // https://stopbyte.com/t/how-can-i-show-popup-on-button-click-and-hide-it-on-second-click-or-user-clicks-outside-staysopen-false/80/5
        private void PART_ComboBoxContent_MouseLeave(object sender, MouseEventArgs e)
        {
            PART_Popup.StaysOpen = false;
        }

        /// <summary>
        /// Refresh the state of all CheckBoxes in the popup.
        /// </summary>
        private void refreshPopupCheckBoxes()
        {
            if (PART_Popup?.Child is DependencyObject popupChild)
            {
                foreach (var cb in findVisualChildren<CheckBox>(popupChild))
                {
                    MultiBindingExpression multi = BindingOperations.GetMultiBindingExpression(cb, CheckBox.IsCheckedProperty);
                    multi?.UpdateTarget();
                }
            }
        }

        #endregion

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region CheckBox Handling

        /// <summary>
        /// Checkbox checked event handler to add the item to the SelectedItems collection.
        /// </summary>
        /// <param name="sender">Sending <see cref="CheckBox"/></param>
        /// <param name="e"><see cref="RoutedEventArgs"/></param>
        private void PART_PopupCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox cb && cb.DataContext != null && SelectedItems != null)
            {
                if (!SelectedItems.Contains(cb.DataContext))
                {
                    SelectedItems.Add(cb.DataContext);
                }
            }
        }

        /// <summary>
        /// Checkbox unchecked event handler to remove the item from the SelectedItems collection.
        /// </summary>
        /// <param name="sender">Sending <see cref="CheckBox"/></param>
        /// <param name="e"><see cref="RoutedEventArgs"/></param>
        private void PART_PopupCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox cb && cb.DataContext != null && SelectedItems != null)
            {
                if (SelectedItems.Contains(cb.DataContext))
                {
                    SelectedItems.Remove(cb.DataContext);
                }
            }
        }

        #endregion

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Remove Item Command

        /// <summary>
        /// Command to remove an item from the SelectedItems collection.
        /// </summary>
        [ICommand]
        private void RemoveItem(object item)
        {
            if (item != null && SelectedItems != null && SelectedItems.Contains(item))
            {
                SelectedItems.Remove(item);
            }
        }

        #endregion
        
        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Helper Methods

        /// <summary>
        /// Find the next parent with the requested type from the child
        /// </summary>
        /// <typeparam name="T">Requested parent type</typeparam>
        /// <param name="child">Child to start the search from</param>
        /// <returns>Next parent with requested type of <see langword="null"/> if not found</returns>
        private T findParentOfType<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject current = child;
            while (current != null)
            {
                if (current is T t)
                {
                    return t;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            return null;
        }

        /// <summary>
        /// Find all visual children of a given type.
        /// </summary>
        /// <typeparam name="T">Children type</typeparam>
        /// <param name="depObj">Parent object to start the search from</param>
        /// <returns>List of children with the requested type</returns>
        private static IEnumerable<T> findVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T t)
                    {
                        yield return t;
                    }

                    foreach (T childOfChild in findVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        #endregion
    }
}
