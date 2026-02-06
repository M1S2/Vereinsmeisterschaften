using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.ComponentModel;
using System.Windows.Input;

namespace Vereinsmeisterschaften.Behaviors
{
    /// <summary>
    /// Behavior to display row numbers in the DataGrid row headers and scroll to the selected item in the DataGrid.
    /// </summary>
    /// <see href="https://stackoverflow.com/questions/4663771/wpf-4-datagrid-getting-the-row-number-into-the-rowheader/4663799#4663799"/>
    public class DataGridBehavior
    {
        #region DisplayRowNumber

        /// <summary>
        /// Attached property to enable displaying row numbers (1-based) in the DataGrid row headers.
        /// </summary>
        public static DependencyProperty DisplayRowNumberProperty =
            DependencyProperty.RegisterAttached("DisplayRowNumber",
                                                typeof(bool),
                                                typeof(DataGridBehavior),
                                                new FrameworkPropertyMetadata(false, OnDisplayRowNumberChanged));

        /// <summary>
        /// Get the value of the DisplayRowNumber attached property from a DependencyObject.
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject"/></param>
        /// <returns>Value of the DisplayRowNumber attached property</returns>
        public static bool GetDisplayRowNumber(DependencyObject obj)
        {
            return (bool)obj.GetValue(DisplayRowNumberProperty);
        }

        /// <summary>
        /// Sets the value of the DisplayRowNumber attached property on a DependencyObject.
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject"/></param>
        /// <param name="value">Value for the ObserveSelectedItem attached property</param>
        public static void SetDisplayRowNumber(DependencyObject obj, bool value)
        {
            obj.SetValue(DisplayRowNumberProperty, value);
        }

        private static void OnDisplayRowNumberChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = obj as DataGrid;
            if ((bool)e.NewValue == true)
            {
                EventHandler<DataGridRowEventArgs> loadedRowHandler = null;
                loadedRowHandler = (sender, ea) =>
                {
                    if (GetDisplayRowNumber(dataGrid) == false)
                    {
                        dataGrid.LoadingRow -= loadedRowHandler;
                        return;
                    }
                    ea.Row.Header = ea.Row.GetIndex() + 1;
                };
                dataGrid.LoadingRow += loadedRowHandler;

                ItemsChangedEventHandler itemsChangedHandler = null;
                itemsChangedHandler = (sender, ea) =>
                {
                    if (GetDisplayRowNumber(dataGrid) == false)
                    {
                        dataGrid.ItemContainerGenerator.ItemsChanged -= itemsChangedHandler;
                        return;
                    }
                    GetVisualChildCollection<DataGridRow>(dataGrid).
                        ForEach(d => d.Header = d.GetIndex() + 1);
                };
                dataGrid.ItemContainerGenerator.ItemsChanged += itemsChangedHandler;
            }
        }

        #endregion // DisplayRowNumber

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region ObserveSelectedItem

        /// <summary>
        /// Attached property to observe the selected item in a DataGrid and scroll to it when it changes.
        /// </summary>
        public static readonly DependencyProperty ObserveSelectedItemProperty =
            DependencyProperty.RegisterAttached("ObserveSelectedItem",
                                                typeof(object),
                                                typeof(DataGridBehavior),
                                                new PropertyMetadata(null, OnObserveSelectedItemChanged));

        /// <summary>
        /// Gets the value of the ObserveSelectedItem attached property from a DependencyObject.
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject"/></param>
        /// <returns>Value of the ObserveSelectedItem attached property</returns>
        public static object GetObserveSelectedItem(DependencyObject obj)
            => obj.GetValue(ObserveSelectedItemProperty);

        /// <summary>
        /// Sets the value of the ObserveSelectedItem attached property on a DependencyObject.
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject"/></param>
        /// <param name="value">Value for the ObserveSelectedItem attached property</param>
        public static void SetObserveSelectedItem(DependencyObject obj, object value)
            => obj.SetValue(ObserveSelectedItemProperty, value);

        private static void OnObserveSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataGrid dataGrid && e.NewValue != null)
            {
                DependencyPropertyDescriptor descriptor = DependencyPropertyDescriptor.FromProperty(ObserveSelectedItemProperty, typeof(DataGrid));
                descriptor?.AddValueChanged(d, (sender, args) =>
                {
                    DataGrid dg = (DataGrid)sender;
                    object selectedItem = GetObserveSelectedItem(dg);
                    if (selectedItem != null)
                    {
                        dg.Dispatcher.InvokeAsync(() =>
                        {
                            dg.ScrollIntoView(selectedItem);
                        });
                    }
                });
            }
        }

        #endregion

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region ThreeStateSort

        /// <summary>
        /// Attached property to enable three state sorting (Ascending, Descending, Disabled) and MultiColumn sorting with Shift key in a DataGrid.
        /// </summary>
        public static readonly DependencyProperty EnableThreeStateSortingProperty =
            DependencyProperty.RegisterAttached("EnableThreeStateSorting",
                                                typeof(bool),
                                                typeof(DataGridBehavior),
                                                new PropertyMetadata(false, OnEnableThreeStateSortingChanged));

        /// <summary>
        /// Gets the value of the EnableThreeStateSorting attached property from a DependencyObject.
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject"/></param>
        /// <returns>Value of the EnableThreeStateSorting attached property</returns>
        public static bool GetEnableThreeStateSorting(DependencyObject obj)
            => (bool)obj.GetValue(EnableThreeStateSortingProperty);

        /// <summary>
        /// Sets the value of the EnableThreeStateSorting attached property on a DependencyObject.
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject"/></param>
        /// <param name="value">Value for the EnableThreeStateSorting attached property</param>
        public static void SetEnableThreeStateSorting(DependencyObject obj, bool value)
            => obj.SetValue(EnableThreeStateSortingProperty, value);

        private static void OnEnableThreeStateSortingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not DataGrid dataGrid)
                return;

            if ((bool)e.NewValue)
                dataGrid.Sorting += OnDataGridSorting;
            else
                dataGrid.Sorting -= OnDataGridSorting;
        }

        // Code was generated with the help of ChatGPT
        private static void OnDataGridSorting(object sender, DataGridSortingEventArgs e)
        {
            DataGrid grid = (DataGrid)sender;
            string path = e.Column.SortMemberPath;
            if (string.IsNullOrEmpty(path)) { return; }

            bool shiftPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);
            e.Handled = true;

            // Get the new sort direction based on the current sort direction
            ListSortDirection? newDirection = e.Column.SortDirection switch
            {
                null => ListSortDirection.Ascending,
                ListSortDirection.Ascending => ListSortDirection.Descending,
                ListSortDirection.Descending => null,
                _ => null
            };

            // If Shift is NOT pressed → clear all previous sortings
            if (!shiftPressed)
            {
                foreach (DataGridColumn c in grid.Columns)
                {
                    if (!ReferenceEquals(c, e.Column))
                    {
                        c.SortDirection = null;
                    }
                }
                grid.Items.SortDescriptions.Clear();
            }

            // Remove the column from SortDescriptions if it exists
            SortDescription existingSortDescription = grid.Items.SortDescriptions.FirstOrDefault(sd => sd.PropertyName == path);
            if (!string.IsNullOrEmpty(existingSortDescription.PropertyName))
            {
                grid.Items.SortDescriptions.Remove(existingSortDescription);
            }

            if (newDirection != null)
            {
                // Add new SortDescription with the new direction
                grid.Items.SortDescriptions.Add(new SortDescription(path, newDirection.Value));
                e.Column.SortDirection = newDirection;
            }
            else
            {
                // No sorting → reset column
                e.Column.SortDirection = null;
            }

            // Apply sorting
            grid.Items.Refresh();
        }

        #endregion

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Get Visuals

        private static List<T> GetVisualChildCollection<T>(object parent) where T : Visual
        {
            List<T> visualCollection = new List<T>();
            GetVisualChildCollection(parent as DependencyObject, visualCollection);
            return visualCollection;
        }

        private static void GetVisualChildCollection<T>(DependencyObject parent, List<T> visualCollection) where T : Visual
        {
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is T)
                {
                    visualCollection.Add(child as T);
                }
                if (child != null)
                {
                    GetVisualChildCollection(child, visualCollection);
                }
            }
        }

        #endregion // Get Visuals

    }
}
