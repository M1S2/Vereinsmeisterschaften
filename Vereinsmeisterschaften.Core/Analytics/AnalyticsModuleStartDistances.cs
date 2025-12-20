using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Analytics
{
    /// <summary>
    /// Analytics module to calculate the number of starts per distance
    /// </summary>
    public class AnalyticsModuleStartDistances : IAnalyticsModule
    {
        private IPersonService _personService;

        /// <summary>
        /// Constructor for the <see cref="AnalyticsModuleStartDistances"/>
        /// </summary>
        /// <param name="personService"><see cref="IPersonService"/> object</param>
        public AnalyticsModuleStartDistances(IPersonService personService)
        {
            _personService = personService;
        }

        /// <inheritdoc/>
        public bool AnalyticsAvailable => true;

        /// <summary>
        /// Number of valid starts per distance. The list is ordered descending by the number.
        /// </summary>
        public Dictionary<ushort, int> NumberStartsPerDistance => _personService.GetAllPersonStarts()
                                                                                .Where(s => s.IsActive && s.CompetitionObj != null)
                                                                                .GroupBy(s => s.CompetitionObj.Distance)
                                                                                .ToDictionary(g => g.Key, g => g.Count())
                                                                                .OrderByDescending(d => d.Value)
                                                                                .ToDictionary();

        /// <summary>
        /// Percentage of valid starts per distance. The list is ordered descending by the percentage.
        /// </summary>
        public Dictionary<ushort, double> PercentageStartsPerDistance => NumberStartsPerDistance.ToDictionary(d => d.Key, d => (d.Value / (double)_personService.GetAllPersonStarts().Count(s => s.IsActive && s.CompetitionObj != null)) * 100)
                                                                                                .OrderByDescending(d => d.Value)
                                                                                                .ToDictionary();
    }
}
