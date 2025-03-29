using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Contracts.Services
{
    /// <summary>
    /// Interface for a service used to manage Race objects
    /// </summary>
    public interface IRaceService : INotifyPropertyChanged, ISaveable
    {
        /// <summary>
        /// List with the the <see cref="CompetitionRaces"/> of the last time <see cref="CalculateRunOrder(ushort, CancellationToken, int, ProgressDelegate)"/> was called
        /// </summary>
        List<CompetitionRaces> LastCalculatedCompetitionRaces { get; set; }

        /// <summary>
        /// <see cref="CompetitionRaces"/> object that is marked as best result.
        /// </summary>
        CompetitionRaces BestCompetitionRaces { get; set; }

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
