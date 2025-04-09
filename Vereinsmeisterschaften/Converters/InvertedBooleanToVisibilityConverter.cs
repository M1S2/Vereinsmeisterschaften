using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Vereinsmeisterschaften.Converters;

public class InvertedBooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if(value != null && value is bool boolVal)
        {
            return boolVal ? Visibility.Collapsed : Visibility.Visible;
        }
        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
