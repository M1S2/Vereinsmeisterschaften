using System.Globalization;
using System.Windows.Data;

namespace Vereinsmeisterschaften.Converters;

public class BooleanToStringConverter : IValueConverter
{
    public string TrueString { get; set; } = "True";
    public string FalseString { get; set; } = "False";

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if(value != null && value is bool boolVal)
        {
            return boolVal ? TrueString : FalseString;
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
