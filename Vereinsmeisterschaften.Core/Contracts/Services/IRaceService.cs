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
        /// List with all <see cref="CompetitionRaces"/> (including the loaded and calculated ones)
        /// </summary>
        ObservableCollection<CompetitionRaces> AllCompetitionRaces { get; }

        /// <summary>
        /// <see cref="CompetitionRaces"/> object that is persisted (saved/loaded to/from a file).
        /// </summary>
        CompetitionRaces PersistedCompetitionRaces { get; }

        /// <summary>
        /// Calculate some combination variants for all person starts
        /// </summary>
        /// <param name="competitionYear">Year in which the competition takes place</param>
        /// <param name="cancellationToken">Cancellation token that can be used to cancel this calculation</param>
        /// <param name="numberAvailableSwimLanes">Number of available swimming lanes. This determines the maximum number of parallel starts</param>
        /// <param name="onProgress">Callback used to report progress of the calculation</param>
        /// <returns>All results if calculation was finished successfully; otherwise <see langword="null"/></returns>
        Task<ObservableCollection<CompetitionRaces>> CalculateCompetitionRaces(ushort competitionYear, CancellationToken cancellationToken, int numberAvailableSwimLanes = 3, ProgressDelegate onProgress = null);

        /// <summary>
        /// Add a new <see cref="CompetitionRaces"/> to the list <see cref="AllCompetitionRaces"/>
        /// </summary>
        /// <param name="competitionRaces"><see cref="CompetitionRaces"/> to add</param>
        void AddCompetitionRaces(CompetitionRaces competitionRaces);

        /// <summary>
        /// Remove the given <see cref="CompetitionRaces"/> object from the list <see cref="AllCompetitionRaces"/>
        /// </summary>
        /// <param name="competitionRaces"><see cref="CompetitionRaces"/> object to remove</param>
        void RemoveCompetitionRaces(CompetitionRaces competitionRaces);

        /// <summary>
        /// Remove all <see cref="CompetitionRaces"/> from the list <see cref="AllCompetitionRaces"/>
        /// </summary>
        void ClearAllCompetitionRaces();

        /// <summary>
        /// Sort the complete list <see cref="AllCompetitionRaces"/> descending by the <see cref="CompetitionRaces.Score"/>
        /// </summary>
        void SortVariantsByScore();

        /// <summary>
        /// Reassign all <see cref="CompetitionRaces.VariantID"/> so that the IDs start from 1 and have no gaps.
        /// </summary>
        /// <param name="oldVariantID">If not -1, the method returns the new variant ID after reordering that matches this old variant ID</param>
        /// <returns>New variant ID after reordering matching the oldVariantID; if oldVariantID == -1 this returns -1</returns>
        int RecalculateVariantIDs(int oldVariantID = -1);

        /// <summary>
        /// Remove non-existing <see cref="PersonStart"/> objects from all races in <see cref="BestCompetitionRaces"/> and <see cref="LastCalculatedCompetitionRaces"/>.
        /// Also delete empty races.
        /// </summary>
        void CleanupCompetitionRaces();
    }
}
