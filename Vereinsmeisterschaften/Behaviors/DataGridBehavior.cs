using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.ComponentModel;

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
        /// Attached property to enable displaying row numbers in the DataGrid row headers.
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
                    ea.Row.Header = ea.Row.GetIndex();
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
                        ForEach(d => d.Header = d.GetIndex());
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
