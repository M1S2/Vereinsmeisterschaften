using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Documents;
using Vereinsmeisterschaften.Core.Helpers;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Analytics
{
    /// <summary>
    /// Analytics module to find the person with the most podium places
    /// </summary>
    public class AnalyticsModulePodiumWinners : IAnalyticsModule
    {
        private IWinnerHistoryService _winnerHistoryService;

        /// <summary>
        /// Constructor for the <see cref="AnalyticsModulePodiumWinners"/>
        /// </summary>
        /// <param name="winnerHistoryService"><see cref="IWinnerHistoryService"/> object</param>
        public AnalyticsModulePodiumWinners(IWinnerHistoryService winnerHistoryService)
        {
            _winnerHistoryService = winnerHistoryService;
        }

        /// <inheritdoc/>
        public bool AnalyticsAvailable => _winnerHistoryService.GetPreviousWinnersList().Count() > 0;

        /// <summary>
        /// Return a dictionary how often each person was on the specific podium.
        /// Only persons with at least one time on the podium are returned.
        /// </summary>
        /// <param name="podiumPlace">Podium place used to count.</param>
        /// <returns>Dictionary with names and the number of specific podium times (ordered by the count).</returns>
        public Dictionary<WinnerName, int> GetPreviousWinnersCountPerPerson(ResultPodiumsPlaces podiumPlace = ResultPodiumsPlaces.All)
            => _winnerHistoryService.GetPreviousWinnersCountPerPerson(podiumPlace, false);

        /// <inheritdoc/>
        public DocXPlaceholderHelper.TextPlaceholders CollectDocumentPlaceholderContents()
        {
            DocXPlaceholderHelper.TextPlaceholders textPlaceholder = new DocXPlaceholderHelper.TextPlaceholders();

            List<ResultPodiumsPlaces> availablePodiumPlaces = Enum.GetValues(typeof(ResultPodiumsPlaces)).Cast<ResultPodiumsPlaces>().ToList();

            foreach (ResultPodiumsPlaces podiumsPlace in availablePodiumPlaces)
            {
                Dictionary<WinnerName, int> previousWinnerCountPerPerson = GetPreviousWinnersCountPerPerson(podiumsPlace);

                // Determine the maximum length of these names (will be used for padding)
                int maxNameStrLen = previousWinnerCountPerPerson.Max(kv => kv.Key?.ToString().Length) ?? 30;

                // Create a string for each dictionary entry including a ASCII diagramm (Format e.g.: Firstname, Name: 3x | ###)
                string moduleString = string.Join(Environment.NewLine,
                                                  previousWinnerCountPerPerson
                                                    .Select(kv => $"{kv.Key}: ".PadRight(maxNameStrLen + 2) +
                                                                  $"{kv.Value.ToString()}x | {new string('#', kv.Value)}"));

                List<string> podiumPlacePlaceholders = podiumsPlace switch
                {
                    ResultPodiumsPlaces.Gold => Placeholders.Placeholders_AnalyticsPodiumWinners_Gold,
                    ResultPodiumsPlaces.Silver => Placeholders.Placeholders_AnalyticsPodiumWinners_Silver,
                    ResultPodiumsPlaces.Bronze => Placeholders.Placeholders_AnalyticsPodiumWinners_Bronze,
                    ResultPodiumsPlaces.All => Placeholders.Placeholders_AnalyticsPodiumWinners,
                    _ => new List<string>()
                };

                foreach (string placeholder in podiumPlacePlaceholders) { textPlaceholder.Add(placeholder, moduleString); }
            }
            return textPlaceholder;
        }

        /// <inheritdoc/>
        public List<string> SupportedDocumentPlaceholderKeys => new List<string>()
        {
            Placeholders.PLACEHOLDER_KEY_ANALYTICS_PODIUM_WINNERS,
            Placeholders.PLACEHOLDER_KEY_ANALYTICS_PODIUM_WINNERS_GOLD,
            Placeholders.PLACEHOLDER_KEY_ANALYTICS_PODIUM_WINNERS_SILVER,
            Placeholders.PLACEHOLDER_KEY_ANALYTICS_PODIUM_WINNERS_BRONZE
        };
    }
}
