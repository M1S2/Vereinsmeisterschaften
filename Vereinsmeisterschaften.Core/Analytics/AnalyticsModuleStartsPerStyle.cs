using System;
using System.Collections.Generic;
using System.Text;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Analytics
{
    /// <summary>
    /// Analytics module to calculate the number of starts per style
    /// </summary>
    public class AnalyticsModuleStartsPerStyle
    {
        private IPersonService _personService;

        /// <summary>
        /// Constructor for the <see cref="AnalyticsModuleStartsPerStyle"/>
        /// </summary>
        /// <param name="personService"><see cref="IPersonService"/> object</param>
        public AnalyticsModuleStartsPerStyle(IPersonService personService)
        {
            _personService = personService;
        }

        /// <summary>
        /// Percentage of starts that are male
        /// </summary>
        public Dictionary<SwimmingStyles, int> NumberStartsPerStyle => _personService.GetAllPersonStarts()
                                                                                     .Where(s => s.IsActive)
                                                                                     .GroupBy(s => s.Style)
                                                                                     .ToDictionary(g => g.Key, g => g.Count())
                                                                                     .OrderByDescending(g => g.Value)
                                                                                     .ToDictionary();
    }
}
