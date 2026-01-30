namespace Vereinsmeisterschaften.Core.Models
{
    /// <summary>
    /// Comparer that only uses the most basic properties of a <see cref="Competition"/> to determine equality:
    /// - <see cref="Competition.Gender"/>
    /// - <see cref="Competition.SwimmingStyle"/>
    /// - <see cref="Competition.Age"/>
    /// </summary>
    public class CompetitionBasicEqualityComparer : IEqualityComparer<Competition>
    {
        /// <summary>
        /// Check if two <see cref="Competition"/> objects are equal based on basic properties.
        /// </summary>
        /// <param name="x">First <see cref="Competition"/> object used for comparison</param>
        /// <param name="y">Second <see cref="Competition"/> object used for comparison</param>
        /// <returns>True, if both objects equal</returns>
        public bool Equals(Competition x, Competition y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;

            return (x.Gender, x.SwimmingStyle, x.Age).Equals((y.Gender, y.SwimmingStyle, y.Age));
        }

        /// <summary>
        /// Get the hash code for a <see cref="Competition"/> object based on basic properties.
        /// </summary>
        /// <param name="obj"><see cref="Competition"/> to get the hash code for</param>
        /// <returns>Hash code</returns>
        public int GetHashCode(Competition obj)
            => obj == null ? 0 : (obj.Gender, obj.SwimmingStyle, obj.Age).GetHashCode();
    }
}
