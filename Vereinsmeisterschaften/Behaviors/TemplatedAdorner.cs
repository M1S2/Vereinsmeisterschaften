using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Vereinsmeisterschaften.Behaviors
{
    /// <summary>
    /// Adorner that uses a template
    /// </summary>
    /// <see href="https://github.com/msiddiqi/EmptyItemsControlOverlay/blob/master/EmtptyItemsControlOverlay/Adorners/TemplatedAdorner.cs"/>
    public class TemplatedAdorner : Adorner
    {
        private ContentPresenter _contentPresenter;

        #region Constructor

        /// <summary>
        /// Constructor for the <see cref="TemplatedAdorner"/>
        /// </summary>
        /// <param name="adornedElement"><see cref="UIElement"/> that is adorned</param>
        /// <param name="contentDataTemplate"><see cref="DataTemplate"/> used for the <see cref="ContentPresenter"/> inside this <see cref="Adorner"/></param>
        /// <param name="content">Object used for the <see cref="ContentPresenter"/> inside this <see cref="Adorner"/></param>
        public TemplatedAdorner(UIElement adornedElement, DataTemplate contentDataTemplate, object content) : base(adornedElement)
        {
            _contentPresenter = new ContentPresenter
            {
                ContentTemplate = contentDataTemplate,
                Content = content,
                DataContext = content
            };

            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(AdornedElement);
            adornerLayer?.Add(this);
        }

        #endregion

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Overridden

        protected override int VisualChildrenCount => 1;

        protected override Visual GetVisualChild(int index) => this._contentPresenter;

        protected override Size MeasureOverride(Size constraint) => this.AdornedElement.RenderSize;

        protected override Size ArrangeOverride(Size finalSize)
        {
            _contentPresenter.Arrange(new Rect(new Point(0, 0), finalSize));
            return finalSize;
        }

        #endregion
    }
}
