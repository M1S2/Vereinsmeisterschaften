using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Documents;
using Vereinsmeisterschaften.Core.Helpers;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Analytics
{
    /// <summary>
    /// Analytics module to calculate the number of starts per style
    /// </summary>
    public class AnalyticsModuleStartsPerStyle : IAnalyticsModule
    {
        private IPersonService _personService;

        /// <summary>
        /// Constructor for the <see cref="AnalyticsModuleStartsPerStyle"/>
        /// </summary>
        /// <param name="personService"><see cref="IPersonService"/> object</param>
        public AnalyticsModuleStartsPerStyle(IPersonService personService)
        {
            _personService = personService;
        }

        /// <inheritdoc/>
        public bool AnalyticsAvailable => _personService.PersonCount > 0;

        /// <summary>
        /// Number of valid starts per style. The list is ordered descending by the number.
        /// </summary>
        public Dictionary<SwimmingStyles, int> NumberStartsPerStyle => _personService.GetAllPersonStarts()
                                                                                     .Where(s => s.IsActive && s.IsCompetitionObjAssigned)
                                                                                     .GroupBy(s => s.Style)
                                                                                     .ToDictionary(g => g.Key, g => g.Count())
                                                                                     .OrderByDescending(d => d.Value)
                                                                                     .ToDictionary();

        /// <summary>
        /// Percentage of valid starts per style. The list is ordered descending by the percentage.
        /// </summary>
        public Dictionary<SwimmingStyles, double> PercentageStartsPerStyle => NumberStartsPerStyle.ToDictionary(d => d.Key, d => (d.Value / (double)_personService.GetAllPersonStarts().Count(s => s.IsActive && s.IsCompetitionObjAssigned)) * 100)
                                                                                                  .OrderByDescending(d => d.Value)
                                                                                                  .ToDictionary();

        /// <inheritdoc/>
        public DocXPlaceholderHelper.TextPlaceholders CollectDocumentPlaceholderContents()
        {
            DocXPlaceholderHelper.TextPlaceholders textPlaceholder = new DocXPlaceholderHelper.TextPlaceholders();
            Dictionary<SwimmingStyles, int> numberStartsPerStyle = NumberStartsPerStyle;

            // Get the localized names for each style and determine the maximum length of these strings (will be used for padding)
            Dictionary<SwimmingStyles, string> styleNameStringsDict = numberStartsPerStyle.ToDictionary(kv => kv.Key, kv => EnumCoreLocalizedStringHelper.Convert(kv.Key));
            int maxStyleNameStrLen = styleNameStringsDict.Max(kv => kv.Value?.Length) ?? 12;

            int maxNumStarts = numberStartsPerStyle.Max(kv => kv.Value);

            // Create a string for each dictionary entry including a ASCII diagramm (Format e.g.: SwimmingStyle: 3x | ###  10%)
            string moduleString = string.Join(Environment.NewLine,
                                              numberStartsPerStyle
                                                .Select(kv =>
                                                {
                                                    SwimmingStyles style = kv.Key;
                                                    string styleString = EnumCoreLocalizedStringHelper.Convert(style);
                                                    int count = kv.Value;
                                                    double percentage = PercentageStartsPerStyle.TryGetValue(style, out var p) ? p : 0;
                                                    return $"{styleNameStringsDict[kv.Key]}: ".PadRight(maxStyleNameStrLen + 2) + 
                                                            $"{count,2}x | {new string('#', count).PadRight(maxNumStarts)}   {percentage.ToString("N1")}%";
                                                }));

            foreach (string placeholder in Placeholders.Placeholders_AnalyticsStartsPerStyle) { textPlaceholder.Add(placeholder, moduleString); }
            return textPlaceholder;
        }

        /// <inheritdoc/>
        public List<string> SupportedDocumentPlaceholderKeys => new List<string>()
        {
            Placeholders.PLACEHOLDER_KEY_ANALYTICS_STARTS_PER_STYLE
        };
    }
}
