using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Documents;
using Vereinsmeisterschaften.Core.Helpers;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Analytics
{
    /// <summary>
    /// Analytics module to calculate the number of starts per persons
    /// </summary>
    public class AnalyticsModuleStartsPerPerson : IAnalyticsModule
    {
        private IPersonService _personService;

        /// <summary>
        /// Constructor for the <see cref="AnalyticsModuleStartsPerPerson"/>
        /// </summary>
        /// <param name="personService"><see cref="IPersonService"/> object</param>
        public AnalyticsModuleStartsPerPerson(IPersonService personService)
        {
            _personService = personService;
        }

        /// <inheritdoc/>
        public bool AnalyticsAvailable => _personService.PersonCount > 0;

        /// <summary>
        /// Number of starts (value) per person (key). The list is ordered descending by number of starts.
        /// </summary>
        public Dictionary<Person, int> NumberStartsPerPerson => _personService.GetPersons()
                                                                              .Where(p => p.IsActive)
                                                                              .ToDictionary(p => p, p => p.Starts.Count(s => s.Value != null && s.Value.IsCompetitionObjAssigned))
                                                                              .OrderByDescending(p => p.Value)
                                                                              .ToDictionary();

        /// <inheritdoc/>
        public DocXPlaceholderHelper.TextPlaceholders CollectDocumentPlaceholderContents()
        {
            DocXPlaceholderHelper.TextPlaceholders textPlaceholder = new DocXPlaceholderHelper.TextPlaceholders();
            // Create a string for each dictionary entry including a ASCII diagramm (Format e.g.: Firstname, Name: 3x | ###)
            string moduleStartsPerPersonString = string.Join(Environment.NewLine,
                                                             NumberStartsPerPerson
                                                                .Select(kv => $"{kv.Key.FirstName}, {kv.Key.Name}: ".PadRight(30) + $"{kv.Value.ToString()}x | {new string('#', kv.Value)}"));
            foreach (string placeholder in Placeholders.Placeholders_AnalyticsStartsPerPerson) { textPlaceholder.Add(placeholder, moduleStartsPerPersonString); }
            return textPlaceholder;
        }

        /// <inheritdoc/>
        public List<string> SupportedDocumentPlaceholderKeys => new List<string>()
        {           
            Placeholders.PLACEHOLDER_KEY_ANALYTICS_STARTS_PER_PERSON
        };
    }
}
