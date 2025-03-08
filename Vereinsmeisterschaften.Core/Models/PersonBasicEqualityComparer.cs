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

            return x.Name.ToUpper().Equals(y.Name.ToUpper()) &&
                   x.FirstName.ToUpper().Equals(y.FirstName.ToUpper()) &&
                   x.Gender.Equals(y.Gender) &&
                   x.BirthYear.Equals(y.BirthYear);
        }

        public int GetHashCode(Person obj)
        {
            if(ReferenceEquals(obj, null)) return 0;
            
            int hashPersonName = obj.Name == null ? 0 : obj.Name.GetHashCode();
            int hashPersonFirstName = obj.FirstName == null ? 0 : obj.FirstName.GetHashCode();
            int hashPersonGender = obj.Gender.GetHashCode();
            int hashPersonBirthYear = obj.BirthYear.GetHashCode();
            return hashPersonName ^ hashPersonFirstName ^ hashPersonGender ^ hashPersonBirthYear;
        }
    }
}
