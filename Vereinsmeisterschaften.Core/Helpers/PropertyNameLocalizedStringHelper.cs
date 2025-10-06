using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Helpers
{
    /// <summary>
    /// Helper class to convert property names to localized strings and vice versa.
    /// The entries in the separate PropertyNameLocalizations .resx file must have the format "{ClassType}_{PropertyName}"
    /// </summary>
    public static class PropertyNameLocalizedStringHelper
    {
        /// <summary>
        /// Convert an property name to a localized string.
        /// </summary>
        /// <param name="objectType">Type of the object the property is part of</param>
        /// <param name="propertyName">Name of a property</param>
        /// <returns>Localized <see cref="string"/></returns>
        public static string Convert(Type objectType, string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return string.Empty;
            }
            else
            {
                // https://stackoverflow.com/questions/17380900/enum-localization
                ResourceManager rmPropNames = new ResourceManager(typeof(Properties.PropertyNameLocalizations));
                string resourceDisplayName = rmPropNames.GetString($"{objectType.Name}_{propertyName}");
                return string.IsNullOrWhiteSpace(resourceDisplayName) ? propertyName : resourceDisplayName;
            }
        }

        /// <summary>
        /// Cache for already found properties to speed up multiple requests for the same type and input string.
        /// </summary>
        private static Dictionary<(Type, string), string> _findPropertyCache = new Dictionary<(Type, string), string>();

        /// <summary>
        /// Try to find a property by a localized property name string.
        /// </summary>
        /// <param name="objectType">Type of the object the property is part of</param>
        /// <param name="input">Input string (localized property name)</param>
        /// <returns>True on success; otherwise false</returns>
        public static string FindProperty(Type objectType, string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }
            if(_findPropertyCache.ContainsKey((objectType, input)))
            {
                return _findPropertyCache[(objectType, input)];
            }
            
            ResourceManager rmPropNames = new ResourceManager(typeof(Properties.PropertyNameLocalizations));

            // Search in resources (case-insensitive)
            foreach (PropertyInfo prop in objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                string resourceKey = $"{objectType.Name}_{prop.Name}";
                List<string> localizedPropertyNames = GeneralLocalizationHelper.GetAllTranslationsForKey(rmPropNames, resourceKey).Values.ToList();

                foreach (string localizedPropName in localizedPropertyNames)
                {
                    if (string.Equals(localizedPropName, input, StringComparison.OrdinalIgnoreCase))
                    {
                        _findPropertyCache[(objectType, input)] = prop.Name;
                        return prop.Name;
                    }
                }
            }

            // Fallback: direct property name might already match
            if (objectType.GetProperty(input, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase) is PropertyInfo p)
            {
                _findPropertyCache[(objectType, input)] = p.Name;
                return p.Name;
            }

            return string.Empty;
        }
    }
}