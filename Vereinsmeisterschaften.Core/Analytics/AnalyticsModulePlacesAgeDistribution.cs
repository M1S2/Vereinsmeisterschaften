using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Analytics
{
    /// <summary>
    /// Analytics module to calculate the age distribution over the result places
    /// </summary>
    public class AnalyticsModulePlacesAgeDistribution : IAnalyticsModule
    {
        private IScoreService _scoreService;

        /// <summary>
        /// Constructor for the <see cref="AnalyticsModulePlacesAgeDistribution"/>
        /// </summary>
        /// <param name="scoreService"><see cref="IScoreService"/> object</param>
        public AnalyticsModulePlacesAgeDistribution(IScoreService scoreService)
        {
            _scoreService = scoreService;
        }

        /// <inheritdoc/>
        public bool AnalyticsAvailable => true;

        /// <summary>
        /// Birth year (value) per result place (key)
        /// </summary>
        public Dictionary<int, ushort> BirthYearsPerResultPlace => _scoreService.GetPersonsSortedByScore(ResultTypes.Overall, true)
                                                                                .Select((value, index) => new { value, index })
                                                                                .ToDictionary(pair => pair.index + 1, pair => pair.value.BirthYear);

    }
}
