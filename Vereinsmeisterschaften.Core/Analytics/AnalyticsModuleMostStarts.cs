using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Analytics
{
    /// <summary>
    /// Analytics module to calculate the persons with the most starts
    /// </summary>
    public class AnalyticsModuleMostStarts : IAnalyticsModule
    {
        private IPersonService _personService;

        /// <summary>
        /// Constructor for the <see cref="AnalyticsModuleMostStarts"/>
        /// </summary>
        /// <param name="personService"><see cref="IPersonService"/> object</param>
        public AnalyticsModuleMostStarts(IPersonService personService)
        {
            _personService = personService;
        }

        /// <inheritdoc/>
        public bool AnalyticsAvailable => true;

        /// <summary>
        /// Number of starts (value) per person (key)
        /// </summary>
        public Dictionary<Person, int> NumberStartsPerPerson => _personService.GetPersons()
                                                                              .Where(s => s.IsActive)
                                                                              .ToDictionary(p => p, p => p.Starts.Count(s => s.Value != null))
                                                                              .OrderByDescending(p => p.Value)
                                                                              .ToDictionary();
    }
}
