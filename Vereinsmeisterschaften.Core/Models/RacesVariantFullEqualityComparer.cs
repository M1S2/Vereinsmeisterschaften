using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace Vereinsmeisterschaften.Core.Models
{
    /// <summary>
    /// Comparer that only uses most properties of a <see cref="RacesVariant"/> to determine equality:
    /// - <see cref="RacesVariant.Score"/>
    /// - <see cref="RacesVariant.Races"/>
    /// </summary>
    public class RacesVariantFullEqualityComparer : IEqualityComparer<RacesVariant>
    {
        public bool Equals(RacesVariant x, RacesVariant y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;

            return x.Score == y.Score && x.Races.SequenceEqual(y.Races);
        }

        public int GetHashCode(RacesVariant obj)
            => obj == null ? 0 : (obj.Races, obj.Score).GetHashCode();
    }
}
