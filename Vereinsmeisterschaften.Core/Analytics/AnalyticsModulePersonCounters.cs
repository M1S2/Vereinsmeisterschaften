using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Documents;
using Vereinsmeisterschaften.Core.Helpers;

namespace Vereinsmeisterschaften.Core.Analytics
{
    /// <summary>
    /// Analytics module to get counters for the persons
    /// </summary>
    public class AnalyticsModulePersonCounters : IAnalyticsModule
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

        /// <inheritdoc/>
        public bool AnalyticsAvailable => true;

        /// <summary>
        /// Total number of people
        /// </summary>
        public int NumberOfPeople => _personService.PersonCount;

        /// <summary>
        /// Number of active people
        /// </summary>
        public int NumberOfActivePeople => _personService.GetPersons().Count(p => p.IsActive);

        /// <summary>
        /// Number of inactive people
        /// </summary>
        public int NumberOfInactivePeople => _personService.GetPersons().Count(p => !p.IsActive);

        /// <inheritdoc/>
        public DocXPlaceholderHelper.TextPlaceholders CollectDocumentPlaceholderContents()
        {
            DocXPlaceholderHelper.TextPlaceholders textPlaceholder = new DocXPlaceholderHelper.TextPlaceholders();
            foreach (string placeholder in Placeholders.Placeholders_AnalyticsPersonCountersNumberPeople) { textPlaceholder.Add(placeholder, NumberOfPeople.ToString()); }
            foreach (string placeholder in Placeholders.Placeholders_AnalyticsPersonCountersNumberActivePeople) { textPlaceholder.Add(placeholder, NumberOfActivePeople.ToString()); }
            foreach (string placeholder in Placeholders.Placeholders_AnalyticsPersonCountersNumberInactivePeople) { textPlaceholder.Add(placeholder, NumberOfInactivePeople.ToString()); }
            return textPlaceholder;
        }

        /// <inheritdoc/>
        public List<string> SupportedDocumentPlaceholderKeys => new List<string>()
        {
            Placeholders.PLACEHOLDER_KEY_ANALYTICS_PERSON_COUNTERS_NUMBER_PEOPLE,
            Placeholders.PLACEHOLDER_KEY_ANALYTICS_PERSON_COUNTERS_NUMBER_ACTIVE_PEOPLE,
            Placeholders.PLACEHOLDER_KEY_ANALYTICS_PERSON_COUNTERS_NUMBER_INACTIVE_PEOPLE
        };
    }
}
