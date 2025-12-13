using Vereinsmeisterschaften.Core.Contracts.Services;

namespace Vereinsmeisterschaften.Core.Analytics
{
    /// <summary>
    /// Analytics module to get counters for the starts
    /// </summary>
    public class AnalyticsModuleStartsCounters : IAnalyticsModule
    {
        private IPersonService _personService;

        /// <summary>
        /// Constructor for the <see cref="AnalyticsModuleStartsCounters"/>
        /// </summary>
        /// <param name="personService"><see cref="IPersonService"/> object</param>
        public AnalyticsModuleStartsCounters(IPersonService personService)
        {
            _personService = personService;
        }

        /// <inheritdoc/>
        public bool AnalyticsAvailable => true;

        /// <summary>
        /// Total number of starts
        /// </summary>
        public int NumberOfStarts => _personService.GetAllPersonStarts().Count();

        /// <summary>
        /// Number of inactive starts
        /// </summary>
        public int NumberOfInactiveStarts => _personService.GetAllPersonStarts().Count(s => !s.IsActive);
    }
}
