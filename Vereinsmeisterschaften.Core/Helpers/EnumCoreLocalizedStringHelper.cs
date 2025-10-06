using System.Globalization;
using System.Resources;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Helpers
{
    /// <summary>
    /// Helper class to convert enum values to localized strings and vice versa.
    /// The entries in the separate EnumsCore .resx file must have the format "{EnumType}_{EnumValue}"
    /// </summary>
    public static class EnumCoreLocalizedStringHelper
    {
        /// <summary>
        /// Convert an enum value to a localized string.
        /// </summary>
        /// <param name="value"><see cref="Enum"/> value</param>
        /// <returns>Localized <see cref="string"/></returns>
        public static string Convert(Enum value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            else
            {
                // https://stackoverflow.com/questions/17380900/enum-localization
                ResourceManager rmCore = new ResourceManager(typeof(Properties.EnumsCore));
                string resourceDisplayName = rmCore.GetString($"{value.GetType().Name}_{value}");
                return string.IsNullOrWhiteSpace(resourceDisplayName) ? string.Format("{0}", value) : resourceDisplayName;
            }
        }

        /// <summary>
        /// Dictionary of known shortcuts for enum parsing (<see cref="TryParse{TEnum}(string, ResourceManager, CultureInfo, out TEnum)"/>)
        /// </summary>
        private static readonly Dictionary<Type, Dictionary<string, object>> _shortcuts = new Dictionary<Type, Dictionary<string, object>>()
        {
            [typeof(Genders)] = new(StringComparer.OrdinalIgnoreCase)
            {
                { "m", Genders.Male },
                { "w", Genders.Female },
                { "f", Genders.Female }
            }
        };

        /// <summary>
        /// Try to parse a string into an enum value.
        /// </summary>
        /// <typeparam name="TEnum">Type of the target enum</typeparam>
        /// <param name="input">Input string</param>
        /// <param name="result">Parsed enum</param>
        /// <returns>True on success; otherwise false</returns>
        public static bool TryParse<TEnum>(string input, out TEnum result) where TEnum : struct, Enum
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                result = default;
                return false;
            }

            // Try to parse the input directly as an enum name (case-insensitive)
            if (Enum.TryParse(input, true, out result))
            {
                return true;
            }

            // Check if the _shortcuts dictionary contains a mapping for the input
            if (_shortcuts.TryGetValue(typeof(TEnum), out var dict) && dict.TryGetValue(input.Trim(), out object value))
            {
                result = (TEnum)value;
                return true;
            }

            // Check localized resources
            ResourceManager rmCore = new ResourceManager(typeof(Properties.EnumsCore));
            foreach (TEnum enumValue in Enum.GetValues(typeof(TEnum)).Cast<TEnum>())
            {
                string enumKey = $"{typeof(TEnum).Name}_{enumValue}";
                List<string> localizedEnums = GeneralLocalizationHelper.GetAllTranslationsForKey(rmCore, enumKey).Values.ToList();

                foreach (string localizedEnum in localizedEnums)
                {
                    if (string.Equals(localizedEnum, input, StringComparison.OrdinalIgnoreCase))
                    {
                        result = enumValue;
                        return true;
                    }
                }
            }

            result = default;
            return false;
        }
    }
}