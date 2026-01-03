using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Helpers;
using Vereinsmeisterschaften.Core.Models;
using Vereinsmeisterschaften.Core.Settings;
using Vereinsmeisterschaften.Core.Analytics;

namespace Vereinsmeisterschaften.Core.Documents
{
    /// <summary>
    /// Document placeholder resolver for the items of type <see cref="IAnalyticsModule"/>.
    /// </summary>
    public class DocumentPlaceholderResolverIAnalyticsModule : DocumentPlaceholderResolverBase<IAnalyticsModule>
    {
        /// <summary>
        /// Constructor for a document placeholder resolver.
        /// </summary>
        /// <param name="workspaceService"><see cref="IWorkspaceService"/> that can be used to access e.g. <see cref="Settings.WorkspaceSettings"/></param>
        public DocumentPlaceholderResolverIAnalyticsModule(IWorkspaceService workspaceService) : base(workspaceService)
        {
        }

        /// <summary>
        /// Take the <see cref="IAnalyticsModule"/> item and create <see cref="DocXPlaceholderHelper.TextPlaceholders"/>.
        /// </summary>
        /// <param name="item">Item to create placeholders from</param>
        /// <returns><see cref="DocXPlaceholderHelper.TextPlaceholders"/></returns>
        public override DocXPlaceholderHelper.TextPlaceholders ResolveTextPlaceholders(IAnalyticsModule item)
        {
            DocXPlaceholderHelper.TextPlaceholders textPlaceholder = new DocXPlaceholderHelper.TextPlaceholders();
            Type moduleType = item.GetType();
            if(moduleType == typeof(AnalyticsModuleGenderPersons))
            {
                AnalyticsModuleGenderPersons moduleGenderPersons = item as AnalyticsModuleGenderPersons;
                foreach (string placeholder in Placeholders.Placeholders_AnalyticsGenderPersonsMaleCount) { textPlaceholder.Add(placeholder, moduleGenderPersons.MalePersonCount.ToString()); }
                foreach (string placeholder in Placeholders.Placeholders_AnalyticsGenderPersonsMalePercentage) { textPlaceholder.Add(placeholder, moduleGenderPersons.MalePersonPercentage.ToString("N1")); }
                foreach (string placeholder in Placeholders.Placeholders_AnalyticsGenderPersonsFemaleCount) { textPlaceholder.Add(placeholder, moduleGenderPersons.FemalePersonCount.ToString()); }
                foreach (string placeholder in Placeholders.Placeholders_AnalyticsGenderPersonsFemalePercentage) { textPlaceholder.Add(placeholder, moduleGenderPersons.FemalePersonPercentage.ToString("N1")); }
            }
            else if (moduleType == typeof(AnalyticsModuleGenderStarts))
            {
                AnalyticsModuleGenderStarts moduleGenderStarts = item as AnalyticsModuleGenderStarts;
                foreach (string placeholder in Placeholders.Placeholders_AnalyticsGenderStartsMaleCount) { textPlaceholder.Add(placeholder, moduleGenderStarts.MaleStartsCount.ToString()); }
                foreach (string placeholder in Placeholders.Placeholders_AnalyticsGenderStartsMalePercentage) { textPlaceholder.Add(placeholder, moduleGenderStarts.MaleStartsPercentage.ToString("N1")); }
                foreach (string placeholder in Placeholders.Placeholders_AnalyticsGenderStartsFemaleCount) { textPlaceholder.Add(placeholder, moduleGenderStarts.FemaleStartsCount.ToString()); }
                foreach (string placeholder in Placeholders.Placeholders_AnalyticsGenderStartsFemalePercentage) { textPlaceholder.Add(placeholder, moduleGenderStarts.FemaleStartsPercentage.ToString("N1")); }
            }

            return textPlaceholder;
        }

        /// <inheritdoc/>
        public override List<string> SupportedPlaceholderKeys => new List<string>()
        {
            Placeholders.PLACEHOLDER_KEY_ANALYTICS_GENDER_PERSONS_MALE_COUNT,
            Placeholders.PLACEHOLDER_KEY_ANALYTICS_GENDER_PERSONS_MALE_PERCENTAGE,
            Placeholders.PLACEHOLDER_KEY_ANALYTICS_GENDER_PERSONS_FEMALE_COUNT,
            Placeholders.PLACEHOLDER_KEY_ANALYTICS_GENDER_PERSONS_FEMALE_PERCENTAGE,
            Placeholders.PLACEHOLDER_KEY_ANALYTICS_GENDER_STARTS_MALE_COUNT,
            Placeholders.PLACEHOLDER_KEY_ANALYTICS_GENDER_STARTS_MALE_PERCENTAGE,
            Placeholders.PLACEHOLDER_KEY_ANALYTICS_GENDER_STARTS_FEMALE_COUNT,
            Placeholders.PLACEHOLDER_KEY_ANALYTICS_GENDER_STARTS_FEMALE_PERCENTAGE
        };

    }
}
