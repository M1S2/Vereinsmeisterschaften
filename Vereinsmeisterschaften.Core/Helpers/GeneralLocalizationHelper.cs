using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Vereinsmeisterschaften.Core.Helpers
{
    /// <summary>
    /// Class to help with general localization tasks, e.g. retrieving all translations for a resource key.
    /// </summary>
    public static class GeneralLocalizationHelper
    {
        /// <summary>
        /// List with all available cultures for the current assembly.
        /// </summary>
        private static IEnumerable<CultureInfo> _availableCultures = null;

        /// <summary>
        /// Cache for already retrieved translations. Used to speed up multiple requests for the same resource key.
        /// </summary>
        private static Dictionary<string, Dictionary<string, string>> _allTranslationsCache = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Get all available cultures for the current assembly on first access.
        /// </summary>
        static GeneralLocalizationHelper()
        {
            Assembly mainAssembly = typeof(GeneralLocalizationHelper).Assembly;
            _availableCultures = getAvailableCultures(mainAssembly);
        }

        /// <summary>
        /// Get all translations for a given resource key from the <see cref="ResourceManager"/>.
        /// </summary>
        /// <param name="rm"><see cref="ResourceManager"/> to use</param>
        /// <param name="resourceKey">Key to find</param>
        /// <returns>Dictionary with <see cref="KeyValuePair{string, string}"/> {TwoLetterISOLanguageName, Resource}</returns>
        public static Dictionary<string, string> GetAllTranslationsForKey(ResourceManager rm, string resourceKey)
        {
            if (_allTranslationsCache.ContainsKey(resourceKey))
            {
                return _allTranslationsCache[resourceKey];
            }

            Dictionary<string, string> result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string neutral = rm.GetString(resourceKey, CultureInfo.InvariantCulture);
            if (!string.IsNullOrEmpty(neutral))
            {
                result["neutral"] = neutral;
            }

            foreach (CultureInfo culture in _availableCultures)
            {
                try
                {
                    string value = rm.GetString(resourceKey, culture);
                    if (!string.IsNullOrEmpty(value))
                    {
                        result[culture.TwoLetterISOLanguageName] = value;
                    }
                }
                catch (MissingManifestResourceException)
                {
                    // Culture has no own resx - ignore
                }
            }

            _allTranslationsCache[resourceKey] = result;
            return result;
        }

        /// <summary>
        /// Get all available cultures for the given assembly by checking for existing satellite assemblies.
        /// </summary>
        /// <param name="assembly">Assembly for which to find available cultures</param>
        /// <returns>List with <see cref="CultureInfo/></returns>
        private static IEnumerable<CultureInfo> getAvailableCultures(Assembly assembly)
        {
            string baseDir = Path.GetDirectoryName(assembly.Location)!;

            foreach (string dir in Directory.GetDirectories(baseDir))
            {
                string name = Path.GetFileName(dir);
                CultureInfo? culture = null;

                try
                {
                    culture = CultureInfo.GetCultureInfo(name);
                }
                catch (CultureNotFoundException)
                {
                    // No valid culture folder
                }

                if (culture != null)
                {
                    string satellitePath = Path.Combine(dir, assembly.GetName().Name + ".resources.dll");
                    if (File.Exists(satellitePath))
                    {
                        yield return culture;
                    }
                }
            }
        }
    }
}
