namespace Vereinsmeisterschaften.Core.Models
{
    /// <summary>
    /// Comparer that compares a <see cref="PersonStart"/> without regarding the <see cref="PersonStart.IsActive"/>
    /// </summary>
    public class PersonStartWithoutIsActiveEqualityComparer : IEqualityComparer<PersonStart>
    {
        /// <summary>
        /// Check if two <see cref="PersonStart"/> objects are equal ignoring the <see cref="PersonStart.IsActive"/> property.
        /// </summary>
        /// <param name="x">First <see cref="PersonStart"/> object used for comparison</param>
        /// <param name="y">Second <see cref="PersonStart"/> object used for comparison</param>
        /// <returns>True, if both objects equal</returns>
        public bool Equals(PersonStart x, PersonStart y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;

            return (x.PersonObj, x.Style, x.Time).Equals((y.PersonObj, y.Style, y.Time));
        }

        /// <summary>
        /// Get the hash code for a <see cref="PersonStart"/> object ignoring the <see cref="PersonStart.IsActive"/> property.
        /// </summary>
        /// <param name="obj"><see cref="PersonStart"/> to get the hash code for</param>
        /// <returns>Hash code</returns>
        public int GetHashCode(PersonStart obj)
            => obj == null ? 0 : (obj.PersonObj, obj.Style, obj.Time).GetHashCode();
    }
}
