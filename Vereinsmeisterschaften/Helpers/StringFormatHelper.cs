using System.Windows;

namespace Vereinsmeisterschaften.Helpers
{
    /// <summary>
    /// Helper for string format with WPF. This class enables Bindings for StringFormat.
    /// </summary>
    /// <see href="https://stackoverflow.com/questions/3099048/wpf-binding-and-dynamically-assigning-stringformat-property"/>
    public static class StringFormatHelper
    {
        #region Value

        public static DependencyProperty ValueProperty = DependencyProperty.RegisterAttached("Value", typeof(object), typeof(StringFormatHelper), new PropertyMetadata(null, OnValueChanged));

        private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            RefreshFormattedValue(obj);
        }

        public static object GetValue(DependencyObject obj)
        {
            return obj.GetValue(ValueProperty);
        }

        public static void SetValue(DependencyObject obj, object newValue)
        {
            obj.SetValue(ValueProperty, newValue);
        }

        #endregion

        #region Format

        public static DependencyProperty FormatProperty = DependencyProperty.RegisterAttached("Format", typeof(string), typeof(StringFormatHelper), new PropertyMetadata(null, OnFormatChanged));

        private static void OnFormatChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            RefreshFormattedValue(obj);
        }

        public static string GetFormat(DependencyObject obj)
        {
            return (string)obj.GetValue(FormatProperty);
        }

        public static void SetFormat(DependencyObject obj, string newFormat)
        {
            obj.SetValue(FormatProperty, newFormat);
        }

        #endregion

        #region FormattedValue

        public static DependencyProperty FormattedValueProperty = DependencyProperty.RegisterAttached("FormattedValue", typeof(string), typeof(StringFormatHelper), new PropertyMetadata(null));

        public static string GetFormattedValue(DependencyObject obj)
        {
            return (string)obj.GetValue(FormattedValueProperty);
        }

        public static void SetFormattedValue(DependencyObject obj, string newFormattedValue)
        {
            obj.SetValue(FormattedValueProperty, newFormattedValue);
        }

        #endregion

        private static void RefreshFormattedValue(DependencyObject obj)
        {
            object value = GetValue(obj);
            string format = GetFormat(obj);

            if (format != null)
            {
                if (!format.StartsWith("{0:"))
                {
                    format = String.Format("{{0:{0}}}", format);
                }

                SetFormattedValue(obj, String.Format(format, value));
            }
            else
            {
                SetFormattedValue(obj, value == null ? String.Empty : value.ToString());
            }
        }
    }

}
