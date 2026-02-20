using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Helpers;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Analytics
{
    /// <summary>
    /// Analytics module to collect the competition times for all ages and all styles
    /// </summary>
    public class AnalyticsModuleCompetitionTimes : IAnalyticsModule
    {
        #region Model
        public class ModelCompetitionTimes
        {
            public byte Age { get; set; }
            public TimeSpan Time { get; set; }
            public bool IsTimeFromRudolphTable { get; set; }
            public bool IsTimeInterpolatedFromRudolphTable { get; set; }
            public bool IsOpenAgeTimeFromRudolphTable { get; set; }
        }
        #endregion

        private ICompetitionService _competitionService;

        /// <summary>
        /// Constructor for the <see cref="AnalyticsModuleCompetitionTimes"/>
        /// </summary>
        /// <param name="competitionService"><see cref="ICompetitionService"/> object</param>
        public AnalyticsModuleCompetitionTimes(ICompetitionService competitionService)
        {
            _competitionService = competitionService;
        }

        /// <inheritdoc/>
        public bool AnalyticsAvailable => _competitionService.CompetitionCount > 0;

        /// <summary>
        /// List with <see cref="ModelCompetitionTimes"/> for the requested gender and style
        /// </summary>
        /// <param name="gender"><see cref="Genders"/></param>
        /// <param name="swimmingStyle"><see cref="SwimmingStyles"/></param>
        /// <returns>List with <see cref="ModelCompetitionTimes"/> objects</returns>
        public List<ModelCompetitionTimes> GetCompetitionTimesPerAge(Genders gender, SwimmingStyles swimmingStyle)
            => _competitionService.GetCompetitions()
                                  .Where(c => c.Gender == gender && c.SwimmingStyle == swimmingStyle)
                                  .OrderBy(c => c.Age)
                                  .Select(c => new ModelCompetitionTimes()
                                               {
                                                   Age = c.Age,
                                                   Time = c.BestTime,
                                                   IsTimeFromRudolphTable = c.IsTimeFromRudolphTable,
                                                   IsTimeInterpolatedFromRudolphTable = c.IsTimeInterpolatedFromRudolphTable,
                                                   IsOpenAgeTimeFromRudolphTable = c.IsOpenAgeTimeFromRudolphTable
                                               }
                                  ).ToList();

        /// <inheritdoc/>
        public DocXPlaceholderHelper.TextPlaceholders CollectDocumentPlaceholderContents() => null;

        /// <inheritdoc/>
        public List<string> SupportedDocumentPlaceholderKeys => null;
    }
}
