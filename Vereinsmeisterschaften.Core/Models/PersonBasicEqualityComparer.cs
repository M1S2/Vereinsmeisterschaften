using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

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
        public bool Equals(Person x, Person y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;

            return (x.Name.ToUpper(), x.FirstName.ToUpper(), x.Gender, x.BirthYear).Equals((y.Name.ToUpper(), y.FirstName.ToUpper(), y.Gender, y.BirthYear));
        }

        public int GetHashCode(Person obj)
            => obj == null ? 0 : (obj.Name.ToUpper(), obj.FirstName.ToUpper(), obj.Gender, obj.BirthYear).GetHashCode();
    }
}
