using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Documents;
using Vereinsmeisterschaften.Core.Helpers;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Analytics
{
    /// <summary>
    /// Analytics module to calculate the gender deviation for the persons
    /// </summary>
    public class AnalyticsModuleGenderPersons : IAnalyticsModule
    {
        private IPersonService _personService;

        /// <summary>
        /// Constructor for the <see cref="AnalyticsModuleGenderPersons"/>
        /// </summary>
        /// <param name="personService"><see cref="IPersonService"/> object</param>
        public AnalyticsModuleGenderPersons(IPersonService personService)
        {
            _personService = personService;
        }

        /// <inheritdoc/>
        public bool AnalyticsAvailable => _personService.PersonCount > 0;

        /// <summary>
        /// Number of people that are male
        /// </summary>
        public int MalePersonCount => _personService.GetPersons().Count(p => p.IsActive && p.Gender == Genders.Male);

        /// <summary>
        /// Percentage of people that are male
        /// </summary>
        public double MalePersonPercentage => (MalePersonCount / (double)_personService.GetPersons().Count(p => p.IsActive)) * 100;

        /// <summary>
        /// Number of people that are female
        /// </summary>
        public int FemalePersonCount => _personService.GetPersons().Count(p => p.IsActive && p.Gender == Genders.Female);

        /// <summary>
        /// Percentage of people that are female
        /// </summary>
        public double FemalePersonPercentage => (FemalePersonCount / (double)_personService.GetPersons().Count(p => p.IsActive)) * 100;

        /// <inheritdoc/>
        public DocXPlaceholderHelper.TextPlaceholders CollectDocumentPlaceholderContents()
        {
            DocXPlaceholderHelper.TextPlaceholders textPlaceholder = new DocXPlaceholderHelper.TextPlaceholders();
            foreach (string placeholder in Placeholders.Placeholders_AnalyticsGenderPersonsMaleCount) { textPlaceholder.Add(placeholder, MalePersonCount.ToString()); }
            foreach (string placeholder in Placeholders.Placeholders_AnalyticsGenderPersonsMalePercentage) { textPlaceholder.Add(placeholder, MalePersonPercentage.ToString("N1")); }
            foreach (string placeholder in Placeholders.Placeholders_AnalyticsGenderPersonsFemaleCount) { textPlaceholder.Add(placeholder, FemalePersonCount.ToString()); }
            foreach (string placeholder in Placeholders.Placeholders_AnalyticsGenderPersonsFemalePercentage) { textPlaceholder.Add(placeholder, FemalePersonPercentage.ToString("N1")); }
            return textPlaceholder;
        }

        /// <inheritdoc/>
        public List<string> SupportedDocumentPlaceholderKeys => new List<string>()
        {
            Placeholders.PLACEHOLDER_KEY_ANALYTICS_GENDER_PERSONS_MALE_COUNT,
            Placeholders.PLACEHOLDER_KEY_ANALYTICS_GENDER_PERSONS_MALE_PERCENTAGE,
            Placeholders.PLACEHOLDER_KEY_ANALYTICS_GENDER_PERSONS_FEMALE_COUNT,
            Placeholders.PLACEHOLDER_KEY_ANALYTICS_GENDER_PERSONS_FEMALE_PERCENTAGE
        };
    }
}
