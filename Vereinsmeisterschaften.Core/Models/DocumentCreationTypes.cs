using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vereinsmeisterschaften.Core.Models
{
    /// <summary>
    /// Different types of documents that can be created
    /// </summary>
    public enum DocumentCreationTypes
    {
        /// <summary>
        /// Certificates for all person starts
        /// </summary>
        Certificates,

        /// <summary>
        /// Overview list of all person starts
        /// </summary>
        OverviewList,

        /// <summary>
        /// List with all races and the planned order
        /// </summary>
        RaceStartList,

        /// <summary>
        /// Time input forms for all races
        /// </summary>
        TimeForms,

        /// <summary>
        /// List with the overall result
        /// </summary>
        ResultList
    }
}
