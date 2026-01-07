using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Documents;
using Vereinsmeisterschaften.Core.Helpers;
using Vereinsmeisterschaften.Core.Models;
using Vereinsmeisterschaften.Core.Services;
using static Vereinsmeisterschaften.Core.Analytics.AnalyticsModulePlacesAgeDistribution;

namespace Vereinsmeisterschaften.Core.Analytics
{
    /// <summary>
    /// Analytics module to calculate the ages for each race
    /// </summary>
    public class AnalyticsModuleRacesAges : IAnalyticsModule
    {
        #region Model
        public class ModelRaceAges
        {
            public int RaceID { get; set; }
            public List<ushort> BirthYears { get; set; }
        }

        #endregion

        private IRaceService _raceService;

        /// <summary>
        /// Constructor for the <see cref="AnalyticsModuleRacesAges"/>
        /// </summary>
        /// <param name="raceService"><see cref="IRaceService"/> object</param>
        public AnalyticsModuleRacesAges(IRaceService raceService)
        {
            _raceService = raceService;
        }

        /// <summary>
        /// This analytics is only available, when a persisted race is available
        /// </summary>
        public bool AnalyticsAvailable => _raceService?.PersistedRacesVariant != null;

        /// <summary>
        /// List with <see cref="ModelRaceAgeSpan"/> ordered by the race id and the birth year inside the corresponding list.
        /// </summary>
        public List<ModelRaceAges> AgeListsPerRace => _raceService.PersistedRacesVariant?.Races.Select((race, index) => new ModelRaceAges()
                                                                                                            {
                                                                                                                RaceID = race.RaceID,
                                                                                                                BirthYears = race.Starts.Where(s => s.IsActive).Select(s => s.PersonObj.BirthYear).OrderBy(b => b).ToList()
                                                                                                            }).ToList();

        /// <inheritdoc/>
        public DocXPlaceholderHelper.TextPlaceholders CollectDocumentPlaceholderContents()
        {
            DocXPlaceholderHelper.TextPlaceholders textPlaceholder = new DocXPlaceholderHelper.TextPlaceholders();
            // Create a string for each list entry (Format e.g.: Race 1: 2000, 2010, 2015)
            string placeholderString = string.Join(Environment.NewLine,
                                                   AgeListsPerRace
                                                        .Select(e => $"{Properties.Resources.RaceString} {e.RaceID.ToString().PadLeft(2)}: {string.Join(", ", e.BirthYears)}"));
            foreach (string placeholder in Placeholders.Placeholders_AnalyticsRacesAges) { textPlaceholder.Add(placeholder, placeholderString); }
            return textPlaceholder;
        }

        /// <inheritdoc/>
        public List<string> SupportedDocumentPlaceholderKeys => new List<string>()
        {
            Placeholders.PLACEHOLDER_KEY_ANALYTICS_RACES_AGES
        };

    }
}
