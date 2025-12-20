using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Analytics
{
    /// <summary>
    /// Analytics module to calculate the age distribution over the result places
    /// </summary>
    public class AnalyticsModulePlacesAgeDistribution : IAnalyticsModule
    {
        #region Model
        public class ModelPlacesAgeDistribution
        {
            public int ResultPlace { get; set; }
            public ushort BirthYear { get; set; }
            public Person PersonObj { get; set; }
        }
        #endregion
        
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
        /// List with <see cref="ModelPlacesAgeDistribution"/> ordered by the result place.
        /// </summary>
        public List<ModelPlacesAgeDistribution> BirthYearsPerResultPlace => _scoreService.GetPersonsSortedByScore(ResultTypes.Overall, true)
                                                                                         .Select((person, index) => new ModelPlacesAgeDistribution() { ResultPlace = index + 1, BirthYear = person.BirthYear, PersonObj = person }).ToList();

    }
}
