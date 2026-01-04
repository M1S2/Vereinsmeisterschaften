using System.Diagnostics.Metrics;
using System.Reflection.Emit;
using Vereinsmeisterschaften.Core.Analytics;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Helpers;
using Vereinsmeisterschaften.Core.Models;
using Vereinsmeisterschaften.Core.Settings;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            else if (moduleType == typeof(AnalyticsModuleAgeDistribution))
            {
                AnalyticsModuleAgeDistribution moduleAgeDistribution = item as AnalyticsModuleAgeDistribution;
                // Create a string for each dictionary entry including a ASCII diagramm (Format e.g.: Birth Year 2000: 3x | ###)
                string moduleAgeDistributionString = string.Join(Environment.NewLine,
                                                                 moduleAgeDistribution.NumberPersonsPerBirthYear
                                                                    .Select(kv => $"{Properties.Resources.BirthYearString} {kv.Key}: {kv.Value.ToString().PadLeft(2)}x | {new string('#', kv.Value)}"));
                foreach (string placeholder in Placeholders.Placeholders_AnalyticsAgeDistribution) { textPlaceholder.Add(placeholder, moduleAgeDistributionString); }
            }
            else if (moduleType == typeof(AnalyticsModuleStartsPerPerson))
            {
                AnalyticsModuleStartsPerPerson moduleStartsPerPerson = item as AnalyticsModuleStartsPerPerson;
                // Create a string for each dictionary entry including a ASCII diagramm (Format e.g.: Firstname, Name: 3x | ###)
                string moduleStartsPerPersonString = string.Join(Environment.NewLine,
                                                                 moduleStartsPerPerson.NumberStartsPerPerson
                                                                    .Select(kv => $"{kv.Key.FirstName}, {kv.Key.Name}: ".PadRight(30) + $"{kv.Value.ToString()}x | {new string('#', kv.Value)}"));
                foreach (string placeholder in Placeholders.Placeholders_AnalyticsStartsPerPerson) { textPlaceholder.Add(placeholder, moduleStartsPerPersonString); }
            }
            else if (moduleType == typeof(AnalyticsModuleStartsPerStyle))
            {
                AnalyticsModuleStartsPerStyle moduleStartsPerStyle = item as AnalyticsModuleStartsPerStyle;
                int maxNumStarts = moduleStartsPerStyle.NumberStartsPerStyle.Max(kv => kv.Value);
                // Create a string for each dictionary entry including a ASCII diagramm (Format e.g.: SwimmingStyle: 3x | ###  10%)
                string moduleStartsPerStyleString = string.Join(Environment.NewLine,
                                                                moduleStartsPerStyle.NumberStartsPerStyle
                                                                    .Select(kv =>
                                                                                {
                                                                                    SwimmingStyles style = kv.Key;
                                                                                    string styleString = EnumCoreLocalizedStringHelper.Convert(style);
                                                                                    int count = kv.Value;
                                                                                    double percentage = moduleStartsPerStyle.PercentageStartsPerStyle.TryGetValue(style, out var p) ? p : 0;
                                                                                    return $"{styleString,-12}: {count,2}x | {new string('#', count).PadRight(maxNumStarts)}   {percentage.ToString("N1")}%";
                                                                                }));

                foreach (string placeholder in Placeholders.Placeholders_AnalyticsStartsPerStyle) { textPlaceholder.Add(placeholder, moduleStartsPerStyleString); }
            }
            else if (moduleType == typeof(AnalyticsModuleStartDistances))
            {
                AnalyticsModuleStartDistances moduleStartDistances = item as AnalyticsModuleStartDistances;
                int maxNumStarts = moduleStartDistances.NumberStartsPerDistance.Max(kv => kv.Value);
                // Create a string for each dictionary entry including a ASCII diagramm (Format e.g.: 100m: 3x | ###  10%)
                string moduleStartDistancesString = string.Join(Environment.NewLine,
                                                                moduleStartDistances.NumberStartsPerDistance
                                                                    .Select(kv =>
                                                                    {
                                                                        ushort distance = kv.Key;
                                                                        string distanceString = $"{distance} m";
                                                                        int count = kv.Value;
                                                                        double percentage = moduleStartDistances.PercentageStartsPerDistance.TryGetValue(distance, out var p) ? p : 0;
                                                                        return $"{distanceString,5}: {count,2}x | {new string('#', count).PadRight(maxNumStarts)}   {percentage.ToString("N1")}%";
                                                                    }));

                foreach (string placeholder in Placeholders.Placeholders_AnalyticsStartDistances) { textPlaceholder.Add(placeholder, moduleStartDistancesString); }
            }
            else if (moduleType == typeof(AnalyticsModulePersonCounters))
            {
                AnalyticsModulePersonCounters modulePersonCounters = item as AnalyticsModulePersonCounters;
                foreach (string placeholder in Placeholders.Placeholders_AnalyticsPersonCountersNumberPeople) { textPlaceholder.Add(placeholder, modulePersonCounters.NumberOfPeople.ToString()); }
                foreach (string placeholder in Placeholders.Placeholders_AnalyticsPersonCountersNumberActivePeople) { textPlaceholder.Add(placeholder, modulePersonCounters.NumberOfActivePeople.ToString()); }
                foreach (string placeholder in Placeholders.Placeholders_AnalyticsPersonCountersNumberInactivePeople) { textPlaceholder.Add(placeholder, modulePersonCounters.NumberOfInactivePeople.ToString()); }
            }
            else if (moduleType == typeof(AnalyticsModuleStartsCounters))
            {
                AnalyticsModuleStartsCounters moduleStartsCounters = item as AnalyticsModuleStartsCounters;
                foreach (string placeholder in Placeholders.Placeholders_AnalyticsStartsCountersNumberStarts) { textPlaceholder.Add(placeholder, moduleStartsCounters.NumberOfStarts.ToString()); }
                foreach (string placeholder in Placeholders.Placeholders_AnalyticsStartsCountersNumberValidStarts) { textPlaceholder.Add(placeholder, moduleStartsCounters.NumberOfValidStarts.ToString()); }
                foreach (string placeholder in Placeholders.Placeholders_AnalyticsStartsCountersNumberInactiveStarts) { textPlaceholder.Add(placeholder, moduleStartsCounters.NumberOfInactiveStarts.ToString()); }
                foreach (string placeholder in Placeholders.Placeholders_AnalyticsStartsCountersNumberStartsWithMissingCompetition) { textPlaceholder.Add(placeholder, moduleStartsCounters.NumberOfStartsWithMissingCompetition.ToString()); }
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
            Placeholders.PLACEHOLDER_KEY_ANALYTICS_GENDER_STARTS_FEMALE_PERCENTAGE,
            Placeholders.PLACEHOLDER_KEY_ANALYTICS_AGE_DISTRIBUTION,
            Placeholders.PLACEHOLDER_KEY_ANALYTICS_STARTS_PER_PERSON,
            Placeholders.PLACEHOLDER_KEY_ANALYTICS_STARTS_PER_STYLE,
            Placeholders.PLACEHOLDER_KEY_ANALYTICS_START_DISTANCES,
            Placeholders.PLACEHOLDER_KEY_ANALYTICS_PERSON_COUNTERS_NUMBER_PEOPLE,
            Placeholders.PLACEHOLDER_KEY_ANALYTICS_PERSON_COUNTERS_NUMBER_ACTIVE_PEOPLE,
            Placeholders.PLACEHOLDER_KEY_ANALYTICS_PERSON_COUNTERS_NUMBER_INACTIVE_PEOPLE,
            Placeholders.PLACEHOLDER_KEY_ANALYTICS_STARTS_COUNTERS_NUMBER_STARTS,
            Placeholders.PLACEHOLDER_KEY_ANALYTICS_STARTS_COUNTERS_NUMBER_VALID_STARTS,
            Placeholders.PLACEHOLDER_KEY_ANALYTICS_STARTS_COUNTERS_NUMBER_INACTIVE_STARTS,
            Placeholders.PLACEHOLDER_KEY_ANALYTICS_STARTS_COUNTERS_NUMBER_STARTS_WITH_MISSING_COMPETITION
        };

    }
}
