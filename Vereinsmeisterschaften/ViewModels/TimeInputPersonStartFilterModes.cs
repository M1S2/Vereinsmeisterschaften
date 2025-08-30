using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.ViewModels
{
    /// <summary>
    /// Enum to define the different modes to filter a <see cref="PersonStart"/>
    /// </summary>
    public enum TimeInputPersonStartFilterModes
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
        /// Filter by the <see cref="Race.RaceID"/>
        /// </summary>
        RaceID,

        /// <summary>
        /// Filter by the <see cref="Competition.CompetitionID"/>
        /// </summary>
        CompetitionID
    }
}
