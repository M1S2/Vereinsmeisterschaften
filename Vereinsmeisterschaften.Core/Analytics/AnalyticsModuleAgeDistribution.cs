using System;
using System.Collections.Generic;
using System.Text;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Analytics
{
    /// <summary>
    /// Analytics module to calculate the age distribution of all persons
    /// </summary>
    public class AnalyticsModuleAgeDistribution
    {
        private IPersonService _personService;

        /// <summary>
        /// Constructor for the <see cref="AnalyticsModuleAgeDistribution"/>
        /// </summary>
        /// <param name="personService"><see cref="IPersonService"/> object</param>
        public AnalyticsModuleAgeDistribution(IPersonService personService)
        {
            _personService = personService;
        }

        /// <summary>
        /// Dictionary that counts the number of persons (value) per birth year (key)
        /// </summary>
        public Dictionary<UInt16, int> NumberPersonsPerBirthYear => _personService.GetPersons()
                                                                                  .Where(p => p.IsActive)
                                                                                  .GroupBy(p => p.BirthYear)
                                                                                  .OrderBy(g => g.Key)
                                                                                  .ToDictionary(g => g.Key, g => g.Count());
    }
}
