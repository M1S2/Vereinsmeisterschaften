using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace Vereinsmeisterschaften.Behaviors
{
    /// <summary>
    /// Behaviour to collapse the GridViewColumn when the Header isn't visible
    /// Set this behavior to an object of type <see cref="GridViewColumnHeader"/> in XAML: &lt;GridViewColumnHeader behaviors:GridViewBehaviors.CollapseableColumn="True" &gt;...
    /// Then set the <see cref="GridViewColumnHeader.Visibility"/> to <see cref="Visibility.Collapsed"/> or <see cref="Visibility.Hidden"/> to make the column invisible.
    /// </summary>
    /// <see href="https://stackoverflow.com/questions/1392811/c-wpf-make-a-gridviewcolumn-visible-false"/>
    public class GridViewBehaviors
    {
        private static Dictionary<GridViewColumn, double> _lastGridViewColumnWidths = new Dictionary<GridViewColumn, double>();

        /// <summary>
        /// Attached DependencyProperty to indicate whether the GridViewColumnHeader is collapseable.
        /// </summary>
        public static readonly DependencyProperty CollapseableColumnProperty = DependencyProperty.RegisterAttached("CollapseableColumn", typeof(bool), typeof(GridViewBehaviors), new UIPropertyMetadata(false, OnCollapseableColumnChanged));

        /// <summary>
        /// Get the value of the CollapseableColumn attached property from a DependencyObject.
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject"/></param>
        /// <returns>Value of the CollapseableColumn attached property</returns>
        public static bool GetCollapseableColumn(DependencyObject obj)
        {
            return (bool)obj.GetValue(CollapseableColumnProperty);
        }

        /// <summary>
        /// Sets the value of the CollapseableColumn attached property on a DependencyObject.
        /// </summary>
        /// <param name="obj"><see cref="DependencyObject"/></param>
        /// <param name="value">Value for the CollapseableColumn attached property</param>
        public static void SetCollapseableColumn(DependencyObject obj, bool value)
        {
            obj.SetValue(CollapseableColumnProperty, value);
        }

        private static void OnCollapseableColumnChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            GridViewColumnHeader header = obj as GridViewColumnHeader;
            if (header == null)
                return;

            header.IsVisibleChanged += new DependencyPropertyChangedEventHandler(AdjustWidth);
        }

        private static void AdjustWidth(object sender, DependencyPropertyChangedEventArgs e)
        {
            GridViewColumnHeader header = sender as GridViewColumnHeader;
            if (header == null)
                return;

            double newWidth = double.NaN;   // NaN = "Auto"

            if (header.Visibility == Visibility.Visible)
            {
                if (_lastGridViewColumnWidths.ContainsKey(header.Column))
                {
                    newWidth = _lastGridViewColumnWidths[header.Column];
                }
                else
                {
                    newWidth = header.Column.ActualWidth;
                    _lastGridViewColumnWidths.Add(header.Column, newWidth);
                }
                header.Column.Width = newWidth;
            }
            else
            {
                // Save the current width of the column
                if (header.Column.ActualWidth != 0)
                {
                    if (_lastGridViewColumnWidths.ContainsKey(header.Column))
                    {
                        _lastGridViewColumnWidths[header.Column] = header.Column.ActualWidth;
                    }
                    else
                    {
                        _lastGridViewColumnWidths.Add(header.Column, header.Column.ActualWidth);
                    }
                }
                header.Column.Width = 0;
            }
        }
    }
}
