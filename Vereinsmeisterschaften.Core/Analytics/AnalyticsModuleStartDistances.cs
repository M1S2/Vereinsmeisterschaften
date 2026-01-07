using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Documents;
using Vereinsmeisterschaften.Core.Helpers;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Analytics
{
    /// <summary>
    /// Analytics module to calculate the number of starts per distance
    /// </summary>
    public class AnalyticsModuleStartDistances : IAnalyticsModule
    {
        private IPersonService _personService;

        /// <summary>
        /// Constructor for the <see cref="AnalyticsModuleStartDistances"/>
        /// </summary>
        /// <param name="personService"><see cref="IPersonService"/> object</param>
        public AnalyticsModuleStartDistances(IPersonService personService)
        {
            _personService = personService;
        }

        /// <inheritdoc/>
        public bool AnalyticsAvailable => _personService.PersonCount > 0;

        /// <summary>
        /// Number of valid starts per distance. The list is ordered descending by the number.
        /// </summary>
        public Dictionary<ushort, int> NumberStartsPerDistance => _personService.GetAllPersonStarts()
                                                                                .Where(s => s.IsActive && s.CompetitionObj != null)
                                                                                .GroupBy(s => s.CompetitionObj.Distance)
                                                                                .ToDictionary(g => g.Key, g => g.Count())
                                                                                .OrderByDescending(d => d.Value)
                                                                                .ToDictionary();

        /// <summary>
        /// Percentage of valid starts per distance. The list is ordered descending by the percentage.
        /// </summary>
        public Dictionary<ushort, double> PercentageStartsPerDistance => NumberStartsPerDistance.ToDictionary(d => d.Key, d => (d.Value / (double)_personService.GetAllPersonStarts().Count(s => s.IsActive && s.CompetitionObj != null)) * 100)
                                                                                                .OrderByDescending(d => d.Value)
                                                                                                .ToDictionary();

        /// <inheritdoc/>
        public DocXPlaceholderHelper.TextPlaceholders CollectDocumentPlaceholderContents()
        {
            DocXPlaceholderHelper.TextPlaceholders textPlaceholder = new DocXPlaceholderHelper.TextPlaceholders();
            int maxNumStarts = NumberStartsPerDistance.Max(kv => kv.Value);
            // Create a string for each dictionary entry including a ASCII diagramm (Format e.g.: 100m: 3x | ###  10%)
            string moduleStartDistancesString = string.Join(Environment.NewLine,
                                                            NumberStartsPerDistance
                                                                .Select(kv =>
                                                                {
                                                                    ushort distance = kv.Key;
                                                                    string distanceString = $"{distance} m";
                                                                    int count = kv.Value;
                                                                    double percentage = PercentageStartsPerDistance.TryGetValue(distance, out var p) ? p : 0;
                                                                    return $"{distanceString,5}: {count,2}x | {new string('#', count).PadRight(maxNumStarts)}   {percentage.ToString("N1")}%";
                                                                }));

            foreach (string placeholder in Placeholders.Placeholders_AnalyticsStartDistances) { textPlaceholder.Add(placeholder, moduleStartDistancesString); }
            return textPlaceholder;
        }

        /// <inheritdoc/>
        public List<string> SupportedDocumentPlaceholderKeys => new List<string>()
        {
            Placeholders.PLACEHOLDER_KEY_ANALYTICS_START_DISTANCES
        };
    }
}
