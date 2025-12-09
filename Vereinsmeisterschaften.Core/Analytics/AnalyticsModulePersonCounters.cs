using System;
using System.Collections.Generic;
using System.Text;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Analytics
{
    /// <summary>
    /// Analytics module to get counters for the persons
    /// </summary>
    public class AnalyticsModulePersonCounters
    {
        private IPersonService _personService;

        /// <summary>
        /// Constructor for the <see cref="AnalyticsModulePersonCounters"/>
        /// </summary>
        /// <param name="personService"><see cref="IPersonService"/> object</param>
        public AnalyticsModulePersonCounters(IPersonService personService)
        {
            _personService = personService;
        }

        /// <summary>
        /// Total number of people
        /// </summary>
        public int NumberOfPeople => _personService.PersonCount;

        /// <summary>
        /// Number of inactive people
        /// </summary>
        public int NumberOfInactivePeople => _personService.GetPersons().Count(p => !p.IsActive);
    }
}
