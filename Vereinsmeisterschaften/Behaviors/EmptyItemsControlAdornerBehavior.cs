using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Xaml.Behaviors;

namespace Vereinsmeisterschaften.Behaviors
{
    /// <summary>
    /// Behavior used to display an <see cref="TemplatedAdorner"/> when the <see cref="ItemsControl"/> is empty
    /// </summary>
    /// <see href="https://github.com/msiddiqi/EmptyItemsControlOverlay/blob/master/EmtptyItemsControlOverlay/Behaviors/EmptyItemsControlAdornerBehavior.cs"/>
    public class EmptyItemsControlAdornerBehavior : Behavior<ItemsControl>
    {
        #region Overridden Members

        protected override void OnAttached()
        {
            base.OnAttached();

            _adornedElement = this.AssociatedObject;
            _adornedElement.Loaded += AdornedElement_Loaded;            

            ICollectionView collectionViewSource = CollectionViewSource.GetDefaultView(_adornedElement.Items);
            if (collectionViewSource != null)
            {
                collectionViewSource.CollectionChanged += ItemsChanged;
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            _adornedElement.Loaded -= AdornedElement_Loaded;

            ICollectionView collectionViewSource = CollectionViewSource.GetDefaultView(_adornedElement.ItemsSource);
            if (collectionViewSource != null)
            {
                collectionViewSource.CollectionChanged -= ItemsChanged;
            }
        }

        #endregion

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Dependency properties
        
        /// <summary>
        /// <see cref="DataTemplate"/> used for the <see cref="ContentPresenter"/> of the <see cref="TemplatedAdorner"/>
        /// </summary>
        public DataTemplate DataTemplate
        {
            get { return (DataTemplate)GetValue(DataTemplateProperty); }
            set { SetValue(DataTemplateProperty, value); }
        }
        public static DependencyProperty DataTemplateProperty = DependencyProperty.Register(nameof(DataTemplate), typeof(DataTemplate), typeof(EmptyItemsControlAdornerBehavior));

        /// <summary>
        /// Data used for the <see cref="ContentPresenter.Content"/> of the <see cref="TemplatedAdorner"/>
        /// </summary>
        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }
        public static DependencyProperty DataProperty = DependencyProperty.Register(nameof(Data), typeof(object), typeof(EmptyItemsControlAdornerBehavior));

        #endregion

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Private Members and Events

        private ItemsControl _adornedElement;
        private TemplatedAdorner _itemsControlAdorner;

        private void AdornedElement_Loaded(object sender, RoutedEventArgs e)
        {
            _itemsControlAdorner = new TemplatedAdorner(_adornedElement, this.DataTemplate, this.Data);
            UpdateAdornerVisibility();
        }

        private void ItemsChanged(object sender, NotifyCollectionChangedEventArgs args)
            => UpdateAdornerVisibility();

        private void UpdateAdornerVisibility()
        {
            if (_itemsControlAdorner == null) { return; }
            _itemsControlAdorner.Visibility = _adornedElement.Items.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion
    }
}
