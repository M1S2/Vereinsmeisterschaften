using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Vereinsmeisterschaften.Behaviors
{
    /// <summary>
    /// Behavior to defer scrolling to the parent scroll viewer when the current one can't scroll anymore.
    /// </summary>
    public static class NestedScrollViewersBehavior
    {
        public static readonly DependencyProperty DeferScrollingToParentWhenNeededProperty = DependencyProperty.RegisterAttached("DeferScrollingToParentWhenNeeded", typeof(bool), typeof(NestedScrollViewersBehavior), new PropertyMetadata(false, OnDeferScrollingToParentWhenNeededChanged));

        public static void SetIsEnabled(DependencyObject element, bool value)
            => element.SetValue(DeferScrollingToParentWhenNeededProperty, value);

        public static bool GetIsEnabled(DependencyObject element)
            => (bool)element.GetValue(DeferScrollingToParentWhenNeededProperty);

        private static void OnDeferScrollingToParentWhenNeededChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScrollViewer scrollViewer)
            {
                if ((bool)e.NewValue)
                {
                    scrollViewer.AddHandler(UIElement.PreviewMouseWheelEvent, new MouseWheelEventHandler(OnPreviewMouseWheel), true);
                }
                else
                {
                    scrollViewer.RemoveHandler(UIElement.PreviewMouseWheelEvent, new MouseWheelEventHandler(OnPreviewMouseWheel));
                }
            }
        }

        /// <summary>
        /// Handle the scroll event of the inner scroll viewer and defer the scrolling event to the parent element when the inner scroll viewer is at its bounds
        /// </summary>
        /// <see href="https://serialseb.com/blog/2007/09/03/wpf-tips-6-preventing-scrollviewer-from/"/>
        private static void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is ScrollViewer senderScrollViewer && !e.Handled &&
                ((e.Delta < 0 && senderScrollViewer.VerticalOffset == senderScrollViewer.ScrollableHeight)  // inner scroll viewer scrolled to the end
                || (e.Delta > 0 && senderScrollViewer.VerticalOffset == 0)))                                // inner scroll viewer scrolled to the start
            {
                e.Handled = true;
                MouseWheelEventArgs eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                eventArg.Source = sender;
                UIElement parent = ((Control)sender)?.Parent as UIElement;
                parent?.RaiseEvent(eventArg);
            }
        }
    }
}
