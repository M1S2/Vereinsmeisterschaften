using System;
using System.Collections.Generic;
using System.Text;

namespace Vereinsmeisterschaften.Core.Models
{
    /// <summary>
    /// Available filters for <see cref="PersonStart"/> objects
    /// </summary>
    public enum PersonStartFilters
    {
        /// <summary>
        /// No filtering
        /// </summary>
        None,

        /// <summary>
        /// Filter by <see cref="Person"/> object
        /// </summary>
        Person,

        /// <summary>
        /// Filter by the <see cref="SwimmingStyles">
        /// </summary>
        SwimmingStyle,

        /// <summary>
        /// Filter by the <see cref="Competition.ID">
        /// </summary>
        CompetitionID
    }
}
