using System;
using System.Collections.Generic;
using System.Text;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Contracts.Services
{
    /// <summary>
    /// Interface for a service used to get and store a list of Competition objects
    /// </summary>
    public interface ICompetitionService : ISaveable
    {
        /// <summary>
        /// Return all available Competitions
        /// </summary>
        /// <returns>List of <see cref="Competition"/> objects</returns>
        List<Competition> GetCompetitions();

        /// <summary>
        /// Clear all Competitions
        /// </summary>
        void ClearAll();

        /// <summary>
        /// Add a new <see cref="Competition"/> to the list of Competitions.
        /// </summary>
        /// <param name="person"><see cref="Competition"/> to add</param>
        void AddCompetition(Competition competition);

        /// <summary>
        /// Return the number of <see cref="Competition"/>
        /// </summary>
        /// <returns>Number of <see cref="Competition"/></returns>
        int CompetitionCount { get; }

        /// <summary>
        /// Return the competition that matches the person and swimming style
        /// </summary>
        /// <param name="person"><see cref="Person"/> used to search the <see cref="Competition"/></param>
        /// <param name="swimmingStyle"><see cref="SwimmingStyles"/> that must match the <see cref="Competition"/></param>
        /// <param name="competitionYear">Year in which the competition takes place</param>
        /// <returns>Found <see cref="Competition"/> or <see langword="null"/></returns>
        Competition GetCompetitionForPerson(Person person, SwimmingStyles swimmingStyle, ushort competitionYear);

        /// <summary>
        /// Update all <see cref="PersonStart"/> objects for the given <see cref="Person"/> with the corresponding <see cref="Competition"/> objects
        /// </summary>
        /// <param name="person"><see cref="Person"/> to update</param>
        /// <param name="competitionYear">Year in which the competition takes place</param>
        void UpdateAllCompetitionsForPersonStarts(Person person, ushort competitionYear);

        /// <summary>
        /// Update all <see cref="PersonStart"/> objects with the corresponding <see cref="Competition"/> objects
        /// </summary>
        /// <param name="competitionYear">Year in which the competition takes place</param>
        void UpdateAllCompetitionsForPersonStarts(ushort competitionYear);

        /// <summary>
        /// List with the the <see cref="CompetitionRaces"/> of the last time <see cref="CalculateRunOrder(ushort, CancellationToken, int, ProgressDelegate)"/> was called
        /// </summary>
        List<CompetitionRaces> LastCalculatedCompetitionRaces { get; set; }

        /// <summary>
        /// Calculate some combination variants for all person starts
        /// </summary>
        /// <param name="competitionYear">Year in which the competition takes place</param>
        /// <param name="cancellationToken">Cancellation token that can be used to cancel this calculation</param>
        /// <param name="numberAvailableSwimLanes">Number of available swimming lanes. This determines the maximum number of parallel starts</param>
        /// <param name="onProgress">Callback used to report progress of the calculation</param>
        /// <returns>All results if calculation was finished successfully; otherwise <see langword="null"/></returns>
        Task<List<CompetitionRaces>> CalculateCompetitionRaces(ushort competitionYear, CancellationToken cancellationToken, int numberAvailableSwimLanes = 3, ProgressDelegate onProgress = null);
    }
}
