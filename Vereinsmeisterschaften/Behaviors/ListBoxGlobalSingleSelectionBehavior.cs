using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Vereinsmeisterschaften.Behaviors
{
    /// <summary>
    /// Behavior to enforce single selection across multiple <see cref="ListBox"/> controls.
    /// All <see cref="ListBox"> controls with this behavior enabled will ensure that only one item is selected at any time.
    /// </summary>
    public static class ListBoxGlobalSingleSelectionBehavior
    {
        private static readonly HashSet<ListBox> _globalSingleSelectionListBoxes = new();
        private static bool _isUpdatingSelection;

        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(ListBoxGlobalSingleSelectionBehavior), new PropertyMetadata(false, OnIsEnabledChanged));

        public static void SetIsEnabled(DependencyObject element, bool value)
            => element.SetValue(IsEnabledProperty, value);

        public static bool GetIsEnabled(DependencyObject element)
            => (bool)element.GetValue(IsEnabledProperty);

        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListBox listBox)
            {
                if ((bool)e.NewValue)
                {
                    if (_globalSingleSelectionListBoxes.Add(listBox))
                    {
                        listBox.AddHandler(UIElement.PreviewMouseDownEvent, new MouseButtonEventHandler(OnPreviewMouseDown), true);
                    }
                }
                else
                {
                    if (_globalSingleSelectionListBoxes.Remove(listBox))
                    {
                        listBox.RemoveHandler(UIElement.PreviewMouseDownEvent, new MouseButtonEventHandler(OnPreviewMouseDown));
                    }
                }
            }
        }

        private static void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_isUpdatingSelection)
                return;

            // Check if the click was on a ListBoxItem
            ListBoxItem sourceItem = FindParent<ListBoxItem>(e.OriginalSource as DependencyObject);
            if (sourceItem == null)
                return;

            // Find the ListBox in which the click occurred
            ListBox sourceList = FindParent<ListBox>(sourceItem);
            if (sourceList == null || !_globalSingleSelectionListBoxes.Contains(sourceList))
                return;

            try
            {
                _isUpdatingSelection = true;

                // Deselect all other lists before processing the click
                foreach (ListBox listBox in _globalSingleSelectionListBoxes.Where(l => l != sourceList))
                {
                    if (listBox.SelectedItem != null)
                    {
                        listBox.SelectedItem = null;
                    }
                }
            }
            finally
            {
                _isUpdatingSelection = false;
            }
        }

        private static T FindParent<T>(DependencyObject element) where T : DependencyObject
        {
            while (element != null && element is not T)
            {
                element = VisualTreeHelper.GetParent(element);
            }
            return element as T;
        }
    }
}
