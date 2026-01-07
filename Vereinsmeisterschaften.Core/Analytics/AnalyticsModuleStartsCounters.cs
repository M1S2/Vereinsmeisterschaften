using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Documents;
using Vereinsmeisterschaften.Core.Helpers;

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

        /// <summary>
        /// Number of active starts with missing competitions
        /// </summary>
        public int NumberOfStartsWithMissingCompetition => _personService.GetAllPersonStarts().Count(s => s.IsActive && !s.IsCompetitionObjAssigned);

        /// <summary>
        /// Number of active starts with a competition assigned (valid starts)
        /// </summary>
        public int NumberOfValidStarts => _personService.GetAllPersonStarts().Count(s => s.IsActive && s.IsCompetitionObjAssigned);

        /// <inheritdoc/>
        public DocXPlaceholderHelper.TextPlaceholders CollectDocumentPlaceholderContents()
        {
            DocXPlaceholderHelper.TextPlaceholders textPlaceholder = new DocXPlaceholderHelper.TextPlaceholders();
            foreach (string placeholder in Placeholders.Placeholders_AnalyticsStartsCountersNumberStarts) { textPlaceholder.Add(placeholder, NumberOfStarts.ToString()); }
            foreach (string placeholder in Placeholders.Placeholders_AnalyticsStartsCountersNumberValidStarts) { textPlaceholder.Add(placeholder, NumberOfValidStarts.ToString()); }
            foreach (string placeholder in Placeholders.Placeholders_AnalyticsStartsCountersNumberInactiveStarts) { textPlaceholder.Add(placeholder, NumberOfInactiveStarts.ToString()); }
            foreach (string placeholder in Placeholders.Placeholders_AnalyticsStartsCountersNumberStartsWithMissingCompetition) { textPlaceholder.Add(placeholder, NumberOfStartsWithMissingCompetition.ToString()); }
            return textPlaceholder;
        }

        /// <inheritdoc/>
        public List<string> SupportedDocumentPlaceholderKeys => new List<string>()
        {
            Placeholders.PLACEHOLDER_KEY_ANALYTICS_STARTS_COUNTERS_NUMBER_STARTS,
            Placeholders.PLACEHOLDER_KEY_ANALYTICS_STARTS_COUNTERS_NUMBER_VALID_STARTS,
            Placeholders.PLACEHOLDER_KEY_ANALYTICS_STARTS_COUNTERS_NUMBER_INACTIVE_STARTS,
            Placeholders.PLACEHOLDER_KEY_ANALYTICS_STARTS_COUNTERS_NUMBER_STARTS_WITH_MISSING_COMPETITION
        };
    }
}
