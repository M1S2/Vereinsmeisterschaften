namespace Vereinsmeisterschaften.Core.Models
{
    /// <summary>
    /// Comparer that only uses the most basic properties of a <see cref="Person"/> to determine equality:
    /// - <see cref="Person.Name"/>
    /// - <see cref="Person.FirstName"/>
    /// - <see cref="Person.Gender"/>
    /// - <see cref="Person.BirthYear"/>
    /// </summary>
    public class PersonBasicEqualityComparer : IEqualityComparer<Person>
    {
        /// <summary>
        /// Check if two <see cref="Person"/> objects are equal based on basic properties.
        /// </summary>
        /// <param name="x">First <see cref="Person"/> object used for comparison</param>
        /// <param name="y">Second <see cref="Person"/> object used for comparison</param>
        /// <returns>True, if both objects equal</returns>
        public bool Equals(Person x, Person y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;

            return (x.Name.ToUpper(), x.FirstName.ToUpper(), x.Gender, x.BirthYear).Equals((y.Name.ToUpper(), y.FirstName.ToUpper(), y.Gender, y.BirthYear));
        }

        /// <summary>
        /// Get the hash code for a <see cref="Person"/> object based on basic properties.
        /// </summary>
        /// <param name="obj"><see cref="Person"/> to get the hash code for</param>
        /// <returns>Hash code</returns>
        public int GetHashCode(Person obj)
            => obj == null ? 0 : (obj.Name.ToUpper(), obj.FirstName.ToUpper(), obj.Gender, obj.BirthYear).GetHashCode();
    }
}
