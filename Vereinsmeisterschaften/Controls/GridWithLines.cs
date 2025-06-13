using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace Vereinsmeisterschaften.Controls
{
    /// <summary>
    /// A custom Grid control that allows for displaying grid lines.
    /// https://www.codeproject.com/Tips/1039691/WPF-Grid-Control-with-Solid-GridLines
    /// </summary>
    public class GridWithLines : Grid
    {
        #region GridLinesVisibilityEnum
        public enum GridLinesVisibilityEnum
        {
            Both,
            Vertical,
            Horizontal,
            None
        }
        #endregion


        #region Properties
        public GridLinesVisibilityEnum GridLinesVisibility
        {
            get { return (GridLinesVisibilityEnum)GetValue(GridLinesVisibilityProperty); }
            set { SetValue(GridLinesVisibilityProperty, value); }
        }

        public static readonly DependencyProperty GridLinesVisibilityProperty = DependencyProperty.Register(nameof(GridLinesVisibility), typeof(GridLinesVisibilityEnum), typeof(GridWithLines), new UIPropertyMetadata(GridLinesVisibilityEnum.Both));

        public Brush GridLineBrush
        {
            get { return (Brush)GetValue(GridLineBrushProperty); }
            set { SetValue(GridLineBrushProperty, value); }
        }

        public static readonly DependencyProperty GridLineBrushProperty = DependencyProperty.Register(nameof(GridLineBrush), typeof(Brush), typeof(GridWithLines), new UIPropertyMetadata(Brushes.Black));

        public double GridLineThickness
        {
            get { return (double)GetValue(GridLineThicknessProperty); }
            set { SetValue(GridLineThicknessProperty, value); }
        }

        public static readonly DependencyProperty GridLineThicknessProperty = DependencyProperty.Register(nameof(GridLineThickness), typeof(double), typeof(GridWithLines), new UIPropertyMetadata(1.0));
        #endregion

        protected override void OnRender(DrawingContext dc)
        {
            if (GridLinesVisibility == GridLinesVisibilityEnum.Both)
            {
                foreach (var rowDefinition in RowDefinitions)
                {
                    dc.DrawLine(new Pen(GridLineBrush, GridLineThickness), new Point(0, rowDefinition.Offset), new Point(ActualWidth, rowDefinition.Offset));
                }

                foreach (var columnDefinition in ColumnDefinitions)
                {
                    dc.DrawLine(new Pen(GridLineBrush, GridLineThickness), new Point(columnDefinition.Offset, 0), new Point(columnDefinition.Offset, ActualHeight));
                }
                dc.DrawRectangle(Brushes.Transparent, new Pen(GridLineBrush, GridLineThickness), new Rect(0, 0, ActualWidth, ActualHeight));
            }
            else if (GridLinesVisibility == GridLinesVisibilityEnum.Vertical)
            {
                foreach (var columnDefinition in ColumnDefinitions)
                {
                    dc.DrawLine(new Pen(GridLineBrush, GridLineThickness), new Point(columnDefinition.Offset, 0), new Point(columnDefinition.Offset, ActualHeight));
                }
                dc.DrawRectangle(Brushes.Transparent, new Pen(GridLineBrush, GridLineThickness), new Rect(0, 0, ActualWidth, ActualHeight));
            }
            else if (GridLinesVisibility == GridLinesVisibilityEnum.Horizontal)
            {
                foreach (var rowDefinition in RowDefinitions)
                {
                    dc.DrawLine(new Pen(GridLineBrush, GridLineThickness), new Point(0, rowDefinition.Offset), new Point(ActualWidth, rowDefinition.Offset));
                }
                dc.DrawRectangle(Brushes.Transparent, new Pen(GridLineBrush, GridLineThickness), new Rect(0, 0, ActualWidth, ActualHeight));
            }
            else if (GridLinesVisibility == GridLinesVisibilityEnum.None)
            {

            }
            base.OnRender(dc);
        }

        static GridWithLines()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridWithLines), new FrameworkPropertyMetadata(typeof(GridWithLines)));
        }
    }
}
