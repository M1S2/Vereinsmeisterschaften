using System.Globalization;
using System.Windows.Data;

namespace Vereinsmeisterschaften.Converters;

/// <summary>
/// Converter that converts an enum value to a boolean based on a specified enum value.
/// E.g. value = "Light", parameter = "Light" -> true
/// E.g. value = "Dark", parameter = "Dark" -> true
/// E.g. value = "Light", parameter = "Dark" -> false
/// </summary>
[ValueConversion(typeof(Enum), typeof(bool))]
public class EnumToBooleanConverter : IValueConverter
{
    /// <summary>
    /// Type of the enum used here.
    /// </summary>
    public Type EnumType { get; set; }

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
        if (parameter is string enumString)
        {
            if (Enum.IsDefined(EnumType, value))
            {
                var enumValue = Enum.Parse(EnumType, enumString);

                return enumValue.Equals(value);
            }
        }

        return false;
    }

    /// <summary>
    /// Back conversion method.
    /// </summary>
    /// <param name="value">Value used for conversion</param>
    /// <param name="targetType">Target <see cref="Type"/></param>
    /// <param name="parameter">ConverterParameter</param>
    /// <param name="culture"><see cref="CultureInfo"/></param>
    /// <returns>Back conversion result</returns>
    /// <exception cref="NotImplementedException"></exception>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter is string enumString)
        {
            return Enum.Parse(EnumType, enumString);
        }

        return null;
    }
}
