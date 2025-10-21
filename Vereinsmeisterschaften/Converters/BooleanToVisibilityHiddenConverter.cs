using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Vereinsmeisterschaften.Converters;

/// <summary>
/// Converts a boolean value to a <see cref="Visibility"/> value.
/// True -> Visible
/// False -> Hidden
/// </summary>
[ValueConversion(typeof(bool), typeof(Visibility))]
public class BooleanToVisibilityHiddenConverter : IValueConverter
{
    /// <summary>
    /// Conversion method.
    /// </summary>
    /// <param name="value">Value used for conversion</param>
    /// <param name="targetType">Target <see cref="Type"/></param>
    /// <param name="parameter">ConverterParameter</param>
    /// <param name="culture"><see cref="CultureInfo"/></param>
    /// <returns>Converted object</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if(value != null && value is bool boolVal)
        {
            return boolVal ? Visibility.Visible : Visibility.Hidden;
        }
        return Visibility.Visible;
    }

    /// <summary>
    /// Back conversion method. Not implemented for this converter.
    /// </summary>
    /// <param name="value">Value used for conversion</param>
    /// <param name="targetType">Target <see cref="Type"/></param>
    /// <param name="parameter">ConverterParameter</param>
    /// <param name="culture"><see cref="CultureInfo"/></param>
    /// <returns>Back conversion result</returns>
    /// <exception cref="NotImplementedException"></exception>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
