using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.ViewModels
{
    /// <summary>
    /// Enum to define the different modes to highlight a <see cref="PersonStart"/>
    /// </summary>
    public enum HighlightPersonStartModes
    {
        /// <summary>
        /// No highlighting
        /// </summary>
        None,

        /// <summary>
        /// Highlight all <see cref="PersonStart"/> objects that have the same <see cref="SwimmingStyles"/>
        /// </summary>
        SwimmingStyle,

        /// <summary>
        /// Highlight all <see cref="PersonStart"/> objects that have the same <see cref="Person"/> object
        /// </summary>
        Person,

        /// <summary>
        /// Highlight all <see cref="PersonStart"/> objects that have the same <see cref="Genders"/>
        /// </summary>
        Gender,

        /// <summary>
        /// Highlight all <see cref="PersonStart"/> objects that have the same distance for the competition
        /// </summary>
        Distance
    }
}
