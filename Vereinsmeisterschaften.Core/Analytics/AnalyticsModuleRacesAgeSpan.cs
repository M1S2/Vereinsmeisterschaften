using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;
using Vereinsmeisterschaften.Core.Services;
using static Vereinsmeisterschaften.Core.Analytics.AnalyticsModulePlacesAgeDistribution;

namespace Vereinsmeisterschaften.Core.Analytics
{
    /// <summary>
    /// Analytics module to calculate the age spans for each race
    /// </summary>
    public class AnalyticsModuleRacesAgeSpan : IAnalyticsModule
    {
        #region Model
        public class ModelRaceAgeSpan
        {
            public int RaceID { get; set; }
            public List<ushort> BirthYears { get; set; }
        }

        #endregion

        private IRaceService _raceService;

        /// <summary>
        /// Constructor for the <see cref="AnalyticsModuleRacesAgeSpan"/>
        /// </summary>
        /// <param name="raceService"><see cref="IRaceService"/> object</param>
        public AnalyticsModuleRacesAgeSpan(IRaceService raceService)
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
        public List<ModelRaceAgeSpan> AgeListsPerRace => _raceService.PersistedRacesVariant?.Races.Select((race, index) => new ModelRaceAgeSpan()
                                                                                                            {
                                                                                                                RaceID = race.RaceID,
                                                                                                                BirthYears = race.Starts.Where(s => s.IsActive).Select(s => s.PersonObj.BirthYear).OrderBy(b => b).ToList()
                                                                                                            }).ToList();

    }
}
