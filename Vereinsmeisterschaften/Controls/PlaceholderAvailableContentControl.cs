using System.Windows.Controls;
using System.Windows;

namespace Vereinsmeisterschaften.Controls
{
    /// <summary>
    /// Content control that selects its template based on the availability of a placeholder.
    /// </summary>
    public class PlaceholderAvailableContentControl : ContentControl
    {
        /// <summary>
        /// Dependency property for the boolean value that determines if the placeholder is available.
        /// </summary>
        public bool IsPlaceholderAvailable
        {
            get => (bool)GetValue(IsPlaceholderAvailableValueProperty);
            set => SetValue(IsPlaceholderAvailableValueProperty, value);
        }
        public static readonly DependencyProperty IsPlaceholderAvailableValueProperty = DependencyProperty.Register(nameof(IsPlaceholderAvailable), typeof(bool), typeof(PlaceholderAvailableContentControl), new PropertyMetadata(false, OnAnyPropertyChanged));

        /// <summary>
        /// Dependency property for the boolean value that determines if the placeholder is supported inside normal text.
        /// </summary>
        public bool IsSupportedForText
        {
            get => (bool)GetValue(IsSupportedForTextProperty);
            set => SetValue(IsSupportedForTextProperty, value);
        }
        public static readonly DependencyProperty IsSupportedForTextProperty = DependencyProperty.Register(nameof(IsSupportedForText), typeof(bool), typeof(PlaceholderAvailableContentControl), new PropertyMetadata(false, OnAnyPropertyChanged));

        /// <summary>
        /// Dependency property for the boolean value that determines if the placeholder is supported inside tables.
        /// </summary>
        public bool IsSupportedForTable
        {
            get => (bool)GetValue(IsSupportedForTableProperty);
            set => SetValue(IsSupportedForTableProperty, value);
        }
        public static readonly DependencyProperty IsSupportedForTableProperty = DependencyProperty.Register(nameof(IsSupportedForTable), typeof(bool), typeof(PlaceholderAvailableContentControl), new PropertyMetadata(false, OnAnyPropertyChanged));

        /// <summary>
        /// Dependency property for a template that is used, when the placeholder is not available
        /// </summary>
        public ControlTemplate NotAvailableTemplate
        {
            get => (ControlTemplate)GetValue(NotAvailableTemplateProperty);
            set => SetValue(NotAvailableTemplateProperty, value);
        }
        public static readonly DependencyProperty NotAvailableTemplateProperty = DependencyProperty.Register(nameof(NotAvailableTemplate), typeof(ControlTemplate), typeof(PlaceholderAvailableContentControl), new PropertyMetadata(null, OnAnyPropertyChanged));

        /// <summary>
        /// Dependency property for a template that is used, when the placeholder is only available inside normal text
        /// </summary>
        public ControlTemplate AvailableOnlyTextTemplate
        {
            get => (ControlTemplate)GetValue(AvailableOnlyTextTemplateProperty);
            set => SetValue(AvailableOnlyTextTemplateProperty, value);
        }
        public static readonly DependencyProperty AvailableOnlyTextTemplateProperty = DependencyProperty.Register(nameof(AvailableOnlyTextTemplate), typeof(ControlTemplate), typeof(PlaceholderAvailableContentControl), new PropertyMetadata(null, OnAnyPropertyChanged));

        /// <summary>
        /// Dependency property for a template that is used, when the placeholder is only available inside tables
        /// </summary>
        public ControlTemplate AvailableOnlyTableTemplate
        {
            get => (ControlTemplate)GetValue(AvailableOnlyTableTemplateProperty);
            set => SetValue(AvailableOnlyTableTemplateProperty, value);
        }
        public static readonly DependencyProperty AvailableOnlyTableTemplateProperty = DependencyProperty.Register(nameof(AvailableOnlyTableTemplate), typeof(ControlTemplate), typeof(PlaceholderAvailableContentControl), new PropertyMetadata(null, OnAnyPropertyChanged));

        /// <summary>
        /// Dependency property for a template that is used, when the placeholder is available inside normal text and inside tables
        /// </summary>
        public ControlTemplate AvailableTextAndTableTemplate
        {
            get => (ControlTemplate)GetValue(AvailableTextAndTableTemplateProperty);
            set => SetValue(AvailableTextAndTableTemplateProperty, value);
        }
        public static readonly DependencyProperty AvailableTextAndTableTemplateProperty = DependencyProperty.Register(nameof(AvailableTextAndTableTemplate), typeof(ControlTemplate), typeof(PlaceholderAvailableContentControl), new PropertyMetadata(null, OnAnyPropertyChanged));


        /// <summary>
        /// Callback for when any of the properties change.
        /// </summary>
        private static void OnAnyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PlaceholderAvailableContentControl)d;
            control.UpdateTemplate();
        }

        /// <summary>
        /// Updates the template based on the current values
        /// </summary>
        private void UpdateTemplate()
        {
            if (IsPlaceholderAvailable && IsSupportedForText && IsSupportedForTable)
            {
                Template = AvailableTextAndTableTemplate;
            }
            else if (IsPlaceholderAvailable && IsSupportedForText && !IsSupportedForTable)
            {
                Template = AvailableOnlyTextTemplate;
            }
            else if (IsPlaceholderAvailable && !IsSupportedForText && IsSupportedForTable)
            {
                Template = AvailableOnlyTableTemplate;
            }
            else
            {
                Template = NotAvailableTemplate;
            }
            ApplyTemplate();
        }

        /// <summary>
        /// Called when the control's template is applied. This is where we set the initial template.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            UpdateTemplate();
        }
    }
}
