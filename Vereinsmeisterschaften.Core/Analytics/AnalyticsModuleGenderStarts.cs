using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Documents;
using Vereinsmeisterschaften.Core.Helpers;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Analytics
{
    /// <summary>
    /// Analytics module to calculate the gender deviation for the starts
    /// </summary>
    public class AnalyticsModuleGenderStarts : IAnalyticsModule
    {
        private IPersonService _personService;

        /// <summary>
        /// Constructor for the <see cref="AnalyticsModuleGenderStarts"/>
        /// </summary>
        /// <param name="personService"><see cref="IPersonService"/> object</param>
        public AnalyticsModuleGenderStarts(IPersonService personService)
        {
            _personService = personService;
        }

        /// <inheritdoc/>
        public bool AnalyticsAvailable => (MaleStartsCount + FemaleStartsCount) > 0;

        /// <summary>
        /// Number of starts that are male
        /// </summary>
        public int MaleStartsCount => _personService.GetAllPersonStarts(onlyValidStarts:true).Count(s => s.PersonObj?.Gender == Genders.Male);

        /// <summary>
        /// Percentage of starts that are male
        /// </summary>
        public double MaleStartsPercentage => (MaleStartsCount / (double)_personService.GetAllPersonStarts(onlyValidStarts:true).Count()) * 100;

        /// <summary>
        /// Number of starts that are female
        /// </summary>
        public int FemaleStartsCount => _personService.GetAllPersonStarts(onlyValidStarts:true).Count(s => s.PersonObj?.Gender == Genders.Female);

        /// <summary>
        /// Percentage of starts that are female
        /// </summary>
        public double FemaleStartsPercentage => (FemaleStartsCount / (double)_personService.GetAllPersonStarts(onlyValidStarts:true).Count()) * 100;

        /// <inheritdoc/>
        public DocXPlaceholderHelper.TextPlaceholders CollectDocumentPlaceholderContents()
        {
            DocXPlaceholderHelper.TextPlaceholders textPlaceholder = new DocXPlaceholderHelper.TextPlaceholders();
            foreach (string placeholder in Placeholders.Placeholders_AnalyticsGenderStartsMaleCount) { textPlaceholder.Add(placeholder, MaleStartsCount.ToString()); }
            foreach (string placeholder in Placeholders.Placeholders_AnalyticsGenderStartsMalePercentage) { textPlaceholder.Add(placeholder, MaleStartsPercentage.ToString("N1")); }
            foreach (string placeholder in Placeholders.Placeholders_AnalyticsGenderStartsFemaleCount) { textPlaceholder.Add(placeholder, FemaleStartsCount.ToString()); }
            foreach (string placeholder in Placeholders.Placeholders_AnalyticsGenderStartsFemalePercentage) { textPlaceholder.Add(placeholder, FemaleStartsPercentage.ToString("N1")); }
            return textPlaceholder;
        }

        /// <inheritdoc/>
        public List<string> SupportedDocumentPlaceholderKeys => new List<string>()
        {
            Placeholders.PLACEHOLDER_KEY_ANALYTICS_GENDER_STARTS_MALE_COUNT,
            Placeholders.PLACEHOLDER_KEY_ANALYTICS_GENDER_STARTS_MALE_PERCENTAGE,
            Placeholders.PLACEHOLDER_KEY_ANALYTICS_GENDER_STARTS_FEMALE_COUNT,
            Placeholders.PLACEHOLDER_KEY_ANALYTICS_GENDER_STARTS_FEMALE_PERCENTAGE
        };
    }
}
