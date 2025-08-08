using System.Resources;

namespace Vereinsmeisterschaften.Core.Helpers
{
    /// <summary>
    /// Get a localized string based on the enum value to convert.
    /// The entries in the separate EnumsCore .resx file must have the format "{EnumType}_{EnumValue}"
    /// </summary>
    public static class EnumCoreToLocalizedString
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
                string resourceDisplayName = rmCore.GetString(value.GetType().Name + "_" + value);
                return string.IsNullOrWhiteSpace(resourceDisplayName) ? string.Format("{0}", value) : resourceDisplayName;
            }
        }
    }
}
