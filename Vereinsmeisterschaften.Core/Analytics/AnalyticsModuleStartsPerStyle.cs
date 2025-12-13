using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Analytics
{
    /// <summary>
    /// Analytics module to calculate the number of starts per style
    /// </summary>
    public class AnalyticsModuleStartsPerStyle : IAnalyticsModule
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

        /// <inheritdoc/>
        public bool AnalyticsAvailable => true;

        /// <summary>
        /// Number of starts per style. The list is ordered descending by the number.
        /// </summary>
        public Dictionary<SwimmingStyles, int> NumberStartsPerStyle => _personService.GetAllPersonStarts()
                                                                                     .Where(s => s.IsActive)
                                                                                     .GroupBy(s => s.Style)
                                                                                     .ToDictionary(g => g.Key, g => g.Count())
                                                                                     .OrderByDescending(d => d.Value)
                                                                                     .ToDictionary();

        /// <summary>
        /// Percentage of starts per style. The list is ordered descending by the percentage.
        /// </summary>
        public Dictionary<SwimmingStyles, double> PercentageStartsPerStyle => NumberStartsPerStyle.ToDictionary(d => d.Key, d => (d.Value / (double)_personService.GetAllPersonStarts().Count(s => s.IsActive)) * 100)
                                                                                                  .OrderByDescending(d => d.Value)
                                                                                                  .ToDictionary();
    }
}
