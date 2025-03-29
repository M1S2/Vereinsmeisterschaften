using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Services
{
    /// <summary>
    /// service used to manage Race objects
    /// </summary>
    public class RaceService : ObservableObject, IRaceService
    {
        private IFileService _fileService;
        private IPersonService _personService;
        private ICompetitionService _competitionService;

        /// <summary>
        /// Constructor
        /// </summary>
        public RaceService(IFileService fileService, IPersonService personService, ICompetitionService competitionService)
        {
            LastCalculatedCompetitionRaces = null;
            _fileService = fileService;
            _personService = personService;
            _competitionService = competitionService;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// List with the the <see cref="CompetitionRaces"/> of the last time <see cref="CalculateRunOrder(ushort, CancellationToken, int, ProgressDelegate)"/> was called
        /// </summary>
        public List<CompetitionRaces> LastCalculatedCompetitionRaces { get; set; }

        /// <summary>
        /// Calculate some combination variants for all person starts
        /// </summary>
        /// <param name="competitionYear">Year in which the competition takes place</param>
        /// <param name="cancellationToken">Cancellation token that can be used to cancel this calculation</param>
        /// <param name="numberAvailableSwimLanes">Number of available swimming lanes. This determines the maximum number of parallel starts</param>
        /// <param name="onProgress">Callback used to report progress of the calculation</param>
        /// <returns>All results if calculation was finished successfully; otherwise <see langword="null"/></returns>
        public async Task<List<CompetitionRaces>> CalculateCompetitionRaces(ushort competitionYear, CancellationToken cancellationToken, int numberAvailableSwimLanes = 3, ProgressDelegate onProgress = null)
        {
            // Collect all starts
            _competitionService.UpdateAllCompetitionsForPersonStarts(competitionYear);
            List<PersonStart> starts = _personService.GetAllPersonStarts();

            // Create groups of competitions with same style and distance
            Dictionary<(SwimmingStyles, ushort), List<PersonStart>> groupedValuesStarts = new Dictionary<(SwimmingStyles, ushort), List<PersonStart>>();
            for (int i = 0; i < starts.Count; i++)
            {
                if (starts[i].CompetitionObj == null) { continue; }

                (SwimmingStyles, ushort) key = (starts[i].CompetitionObj.SwimmingStyle, starts[i].CompetitionObj.Distance);
                if (!groupedValuesStarts.ContainsKey(key))
                {
                    groupedValuesStarts.Add(key, new List<PersonStart>());
                }
                else if (groupedValuesStarts[key] == null)
                {
                    groupedValuesStarts[key] = new List<PersonStart>();
                }
                groupedValuesStarts[key].Add(starts[i]);
            }

            int numberOfResultsToGenerate = 100;
            CompetitionRaceGenerator generator = new CompetitionRaceGenerator(new Progress<double>(progress => onProgress?.Invoke(this, (float)progress, "")), numberOfResultsToGenerate, 1000000, 90);
            LastCalculatedCompetitionRaces = await generator.GenerateBestRacesAsync(groupedValuesStarts.Values.ToList(), cancellationToken);
            return LastCalculatedCompetitionRaces?.Count == 0 ? null : LastCalculatedCompetitionRaces;
        }
    }
}
