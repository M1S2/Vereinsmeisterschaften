using System;
using System.Globalization;
using System.Resources;
using System.Windows.Data;
using Vereinsmeisterschaften.Core;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Converters
{
    /// <summary>
    /// Get all available enum values for a given enum value (used to determine the enum type).
    /// </summary>
    [ValueConversion(typeof(Type), typeof(Array))]
    public class EnumValueToAvailableEnumValuesConverter : IValueConverter
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
            if (value is Type enumType && enumType.IsEnum)
            {
                return Enum.GetValues(enumType);
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
}
