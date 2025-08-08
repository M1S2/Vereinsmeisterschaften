using System.Globalization;
using System.Windows.Data;

namespace Vereinsmeisterschaften.Converters;

/// <summary>
/// Converts a boolean value to a string representation.
/// True -> TrueString
/// False -> FalseString
/// </summary>
[ValueConversion(typeof(bool), typeof(string))]
public class BooleanToStringConverter : IValueConverter
{
    /// <summary>
    /// String to return when the boolean value is true.
    /// </summary>
    public string TrueString { get; set; } = "True";

    /// <summary>
    /// String to return when the boolean value is false.
    /// </summary>
    public string FalseString { get; set; } = "False";

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
            return boolVal ? TrueString : FalseString;
        }
        return null;
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
