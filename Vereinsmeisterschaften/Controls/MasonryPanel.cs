using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Vereinsmeisterschaften.Controls
{
    /// <summary>
    /// Panel to arrange elements in a masonry style
    /// Created by ChatGPT
    /// </summary>
    public class MasonryPanel : Panel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement child in InternalChildren)
                child.Measure(availableSize);

            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double x = 0, y = 0, rowHeight = 0;

            foreach (UIElement child in InternalChildren)
            {
                if (x + child.DesiredSize.Width > finalSize.Width)
                {
                    x = 0;
                    y += rowHeight;
                    rowHeight = 0;
                }

                child.Arrange(new Rect(new Point(x, y), child.DesiredSize));

                x += child.DesiredSize.Width;
                rowHeight = Math.Max(rowHeight, child.DesiredSize.Height);
            }

            return finalSize;
        }
    }
}
