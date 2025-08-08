using System.Windows.Controls;
using System.Windows;

namespace Vereinsmeisterschaften.Controls
{
    /// <summary>
    /// Content control that switches its template based on a boolean value.
    /// </summary>
    public class BoolTemplateContentControl : ContentControl
    {
        /// <summary>
        /// Dependency property for the boolean value that determines which template to use.
        /// True -> <see cref="TrueTemplate"/>
        /// False -> see <see cref="FalseTemplate"/>
        /// </summary>
        public bool BoolValue
        {
            get => (bool)GetValue(BoolValueProperty);
            set => SetValue(BoolValueProperty, value);
        }
        public static readonly DependencyProperty BoolValueProperty = DependencyProperty.Register(nameof(BoolValue), typeof(bool), typeof(BoolTemplateContentControl), new PropertyMetadata(false, OnAnyPropertyChanged));

        /// <summary>
        /// Dependency properties for the templates to use when <see cref="BoolValue"/> is <see langword="true"/>
        /// </summary>
        public ControlTemplate TrueTemplate
        {
            get => (ControlTemplate)GetValue(TrueTemplateProperty);
            set => SetValue(TrueTemplateProperty, value);
        }
        public static readonly DependencyProperty TrueTemplateProperty = DependencyProperty.Register(nameof(TrueTemplate), typeof(ControlTemplate), typeof(BoolTemplateContentControl), new PropertyMetadata(null, OnAnyPropertyChanged));

        /// <summary>
        /// Dependency properties for the templates to use when <see cref="BoolValue"/> is <see langword="false"/>
        /// </summary>
        public ControlTemplate FalseTemplate
        {
            get => (ControlTemplate)GetValue(FalseTemplateProperty);
            set => SetValue(FalseTemplateProperty, value);
        }
        public static readonly DependencyProperty FalseTemplateProperty = DependencyProperty.Register(nameof(FalseTemplate), typeof(ControlTemplate), typeof(BoolTemplateContentControl), new PropertyMetadata(null, OnAnyPropertyChanged));

        /// <summary>
        /// Callback for when any of the properties change.
        /// </summary>
        private static void OnAnyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (BoolTemplateContentControl)d;
            control.UpdateTemplate();
        }

        /// <summary>
        /// Updates the template based on the current value of <see cref="BoolValue"/>
        /// </summary>
        private void UpdateTemplate()
        {
            Template = BoolValue ? TrueTemplate : FalseTemplate;
            ApplyTemplate();
        }

        /// <summary>
        /// Called when the control's template is applied. This is where we set the initial template based on <see cref="BoolValue"/>
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            UpdateTemplate();
        }
    }
}
