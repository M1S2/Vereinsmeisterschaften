using System;
using System.Globalization;
using System.Resources;

namespace Vereinsmeisterschaften.Core.Helpers
{
    /// <summary>
    /// Get a localized string based on the enum value to convert.
    /// The entries in the separate EnumsCore .resx file must have the format "{EnumType}_{EnumValue}"
    /// </summary>
    public static class EnumCoreToLocalizedString
    {
        public static string Convert(Enum value)
        {
            if (value == null)
            {
                return null;
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
