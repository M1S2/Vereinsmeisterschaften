using System;
using System.Collections.Generic;
using System.Text;

namespace Vereinsmeisterschaften.Core.Analytics
{
    /// <summary>
    /// Interface for an analytics module
    /// </summary>
    public interface IAnalyticsModule
    {
        /// <summary>
        /// Flag indicating if the analytics module has data.
        /// </summary>
        bool AnalyticsAvailable { get; }
    }
}
