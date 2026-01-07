using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Documents;
using Vereinsmeisterschaften.Core.Helpers;

namespace Vereinsmeisterschaften.Core.Analytics
{
    /// <summary>
    /// Analytics module to calculate the age distribution of all persons
    /// </summary>
    public class AnalyticsModuleAgeDistribution : IAnalyticsModule
    {
        private IPersonService _personService;

        /// <summary>
        /// Constructor for the <see cref="AnalyticsModuleAgeDistribution"/>
        /// </summary>
        /// <param name="personService"><see cref="IPersonService"/> object</param>
        public AnalyticsModuleAgeDistribution(IPersonService personService)
        {
            _personService = personService;
        }

        /// <inheritdoc/>
        public bool AnalyticsAvailable => _personService.PersonCount > 0;

        /// <summary>
        /// Dictionary that counts the number of persons (value) per birth year (key)
        /// </summary>
        public Dictionary<UInt16, int> NumberPersonsPerBirthYear => _personService.GetPersons()
                                                                                  .Where(p => p.IsActive)
                                                                                  .GroupBy(p => p.BirthYear)
                                                                                  .OrderBy(g => g.Key)
                                                                                  .ToDictionary(g => g.Key, g => g.Count());

        /// <inheritdoc/>
        public DocXPlaceholderHelper.TextPlaceholders CollectDocumentPlaceholderContents()
        {
            DocXPlaceholderHelper.TextPlaceholders textPlaceholder = new DocXPlaceholderHelper.TextPlaceholders();
            // Create a string for each dictionary entry including a ASCII diagramm (Format e.g.: Birth Year 2000: 3x | ###)
            string moduleAgeDistributionString = string.Join(Environment.NewLine,
                                                             NumberPersonsPerBirthYear
                                                                .Select(kv => $"{Properties.Resources.BirthYearString} {kv.Key}: {kv.Value.ToString().PadLeft(2)}x | {new string('#', kv.Value)}"));
            foreach (string placeholder in Placeholders.Placeholders_AnalyticsAgeDistribution) { textPlaceholder.Add(placeholder, moduleAgeDistributionString); }
            return textPlaceholder;
        }

        /// <inheritdoc/>
        public List<string> SupportedDocumentPlaceholderKeys => new List<string>()
        {
            Placeholders.PLACEHOLDER_KEY_ANALYTICS_AGE_DISTRIBUTION
        };
    }
}
