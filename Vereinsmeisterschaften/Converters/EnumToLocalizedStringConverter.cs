using System;
using System.Globalization;
using System.Resources;
using System.Windows.Data;
using Vereinsmeisterschaften.Core;

namespace Vereinsmeisterschaften.Converters
{
    /// <summary>
    /// Get a localized string based on the enum value to convert.
    /// The entries in the separate Enums or EnumsCore .resx file must have the format "{EnumType}_{EnumValue}"
    /// </summary>
    [ValueConversion(typeof(Enum), typeof(string))]
    public class EnumToLocalizedStringConverter : IValueConverter
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
            if (value == null || !value.GetType().IsEnum)
            {
                return null;
            }
            else
            {
                Enum enumValue = (Enum)value;

                // https://stackoverflow.com/questions/17380900/enum-localization
                ResourceManager rm = new ResourceManager(typeof(Properties.Enums));
                string resourceDisplayName = rm.GetString(enumValue.GetType().Name + "_" + enumValue);
                if(string.IsNullOrWhiteSpace(resourceDisplayName))
                {
                    ResourceManager rmCore = new ResourceManager(typeof(Core.Properties.EnumsCore));
                    resourceDisplayName = rmCore.GetString(enumValue.GetType().Name + "_" + enumValue);
                }
                return string.IsNullOrWhiteSpace(resourceDisplayName) ? string.Format("{0}", enumValue) : resourceDisplayName;
            }
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
