using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Analytics
{
    /// <summary>
    /// Analytics module to calculate the distances between the starts of each person in the persisted race variant
    /// </summary>
    public class AnalyticsModuleStartDistances : IAnalyticsModule
    {
        private IRaceService _raceService;

        /// <summary>
        /// Constructor for the <see cref="AnalyticsModuleStartDistances"/>
        /// </summary>
        /// <param name="raceService"><see cref="IRaceService"/> object</param>
        public AnalyticsModuleStartDistances(IRaceService raceService)
        {
            _raceService = raceService;
        }

        /// <summary>
        /// This analytics is only available, when a persisted race is available
        /// </summary>
        public bool AnalyticsAvailable => _raceService?.PersistedRacesVariant != null;

        /// <summary>
        /// Distances between the starts (value) per person (key).
        /// Only persons with more than one start are part of this list (otherwise a distance can't be calculated).
        /// A distance of 1 means the starts are consecutive. A lower value isn't possible.
        /// </summary>
        public Dictionary<Person, List<int>> DistancesBetweenStartsPerPerson
        {
            get
            {
                Dictionary<Person, List<int>> distancesBetweenStartsPerPerson = new Dictionary<Person, List<int>>();
                if (!AnalyticsAvailable) { return distancesBetweenStartsPerPerson; }

                RacesVariant racesVariant = _raceService.PersistedRacesVariant;

                Dictionary<Person, int> lastRaceIndex = new Dictionary<Person, int>();
                for (int i = 0; i < racesVariant.Races.Count; i++)
                {
                    foreach (PersonStart personStart in racesVariant.Races[i].Starts)
                    {
                        // Only consider active starts
                        if (!personStart.IsActive) { continue; }

                        Person person = personStart.PersonObj;

                        if (lastRaceIndex.TryGetValue(person, out int lastIndex))
                        {
                            int distance = i - lastIndex;
                            if (!distancesBetweenStartsPerPerson.ContainsKey(person) || distancesBetweenStartsPerPerson[person] == null)
                            {
                                distancesBetweenStartsPerPerson[person] = new List<int>();
                            }
                            distancesBetweenStartsPerPerson[person].Add(distance);
                        }

                        lastRaceIndex[person] = i;
                    }
                }
                return distancesBetweenStartsPerPerson;
            }
        }
    }
}
