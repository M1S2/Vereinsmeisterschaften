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
    public class EnumValueToAvailableEnumValuesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Type enumType && enumType.IsEnum)
            {
                return Enum.GetValues(enumType);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
