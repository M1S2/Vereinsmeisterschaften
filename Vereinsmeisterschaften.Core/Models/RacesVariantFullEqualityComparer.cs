namespace Vereinsmeisterschaften.Core.Models
{
    /// <summary>
    /// Comparer that only uses most properties of a <see cref="RacesVariant"/> to determine equality:
    /// - <see cref="RacesVariant.Score"/>
    /// - <see cref="RacesVariant.Races"/>
    /// </summary>
    public class RacesVariantFullEqualityComparer : IEqualityComparer<RacesVariant>
    {
        /// <summary>
        /// Check if two <see cref="RacesVariant"/> objects are equal based on basic properties.
        /// </summary>
        /// <param name="x">First <see cref="RacesVariant"/> object used for comparison</param>
        /// <param name="y">Second <see cref="RacesVariant"/> object used for comparison</param>
        /// <returns>True, if both objects equal</returns>
        public bool Equals(RacesVariant x, RacesVariant y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;

            return x.Score == y.Score && x.Races.SequenceEqual(y.Races);
        }

        /// <summary>
        /// Get the hash code for a <see cref="RacesVariant"/> object based on basic properties.
        /// </summary>
        /// <param name="obj"><see cref="RacesVariant"/> to get the hash code for</param>
        /// <returns>Hash code</returns>
        public int GetHashCode(RacesVariant obj)
            => obj == null ? 0 : (obj.Races, obj.Score).GetHashCode();
    }
}
