using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Vereinsmeisterschaften.Core.Models
{
    /// <summary>
    /// Different types of modes to search for the matching competition
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CompetitionSearchModes
    {
        /// <summary>
        /// Return the competition for which the age matches exactly
        /// </summary>
        OnlyExactAge,

        /// <summary>
        /// Return the competition for which the age matches exactly or use the competition with the next lower age
        /// </summary>
        ExactOrNextLowerAge,

        /// <summary>
        /// Return the competition for which the age matches exactly or use the competition with the next higher age
        /// </summary>
        ExactOrNextHigherAge,

        /// <summary>
        /// Return the competition for which the age matches exactly or use the competition with the next lower age where the age is less than or equal to the max age of the competitions
        /// </summary>
        ExactOrNextLowerOnlyMaxAge,

        /// <summary>
        /// Return the competition for which the age matches exactly or use the competition with the nearest age. If there are two competitions with the same distance, prefer the lower age. If the age is below the min age of the competitions, no competition is returned.
        /// </summary>
        ExactOrNearestPreferLower,

        /// <summary>
        /// Return the competition for which the age matches exactly or use the competition with the nearest age. If there are two competitions with the same distance, prefer the higher age. If the age is below the min age of the competitions, no competition is returned.
        /// </summary>
        ExactOrNearestPreferHigher
    }
}
