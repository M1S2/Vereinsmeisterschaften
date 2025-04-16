using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Contracts.Services
{
    /// <summary>
    /// Interface for a service used to manage <see cref="Race"/> and <see cref="RacesVariant"/> objects
    /// </summary>
    public interface IRaceService : INotifyPropertyChanged, ISaveable
    {
        /// <summary>
        /// List with all <see cref="RacesVariant"/> (including the loaded and calculated ones)
        /// </summary>
        ObservableCollection<RacesVariant> AllRacesVariants { get; }

        /// <summary>
        /// <see cref="RacesVariant"/> object that is persisted (saved/loaded to/from a file).
        /// </summary>
        RacesVariant PersistedRacesVariant { get; }

        /// <summary>
        /// Calculate some <see cref="RacesVariant"/> objects for all person starts
        /// </summary>
        /// <param name="competitionYear">Year in which the competition takes place</param>
        /// <param name="cancellationToken">Cancellation token that can be used to cancel this calculation</param>
        /// <param name="numberAvailableSwimLanes">Number of available swimming lanes. This determines the maximum number of parallel starts</param>
        /// <param name="onProgress">Callback used to report progress of the calculation</param>
        /// <returns>All results if calculation was finished successfully; otherwise <see langword="null"/></returns>
        Task<ObservableCollection<RacesVariant>> CalculateRacesVariants(ushort competitionYear, CancellationToken cancellationToken, int numberAvailableSwimLanes = 3, ProgressDelegate onProgress = null);

        /// <summary>
        /// Add a new <see cref="RacesVariant"/> to the list <see cref="AllRacesVariants"/>
        /// </summary>
        /// <param name="racesVariant"><see cref="RacesVariant"/> to add</param>
        void AddRacesVariant(RacesVariant racesVariant);

        /// <summary>
        /// Remove the given <see cref="RacesVariant"/> object from the list <see cref="AllRacesVariants"/>
        /// </summary>
        /// <param name="racesVariant"><see cref="RacesVariant"/> object to remove</param>
        void RemoveRacesVariant(RacesVariant racesVariant);

        /// <summary>
        /// Remove all <see cref="RacesVariant"/> from the list <see cref="AllRacesVariants"/>
        /// </summary>
        void ClearAllRacesVariants();

        /// <summary>
        /// Sort the complete list <see cref="AllRacesVariants"/> descending by the <see cref="RacesVariant.Score"/>
        /// </summary>
        void SortVariantsByScore();

        /// <summary>
        /// Reassign all <see cref="RacesVariant.VariantID"/> so that the IDs start from 1 and have no gaps.
        /// </summary>
        /// <param name="oldVariantID">If not -1, the method returns the new variant ID after reordering that matches this old variant ID</param>
        /// <returns>New variant ID after reordering matching the oldVariantID; if oldVariantID == -1 this returns -1</returns>
        int RecalculateVariantIDs(int oldVariantID = -1);

        /// <summary>
        /// Remove non-existing <see cref="PersonStart"/> objects from all races in <see cref="AllRacesVariants"/>.
        /// Also delete empty races.
        /// </summary>
        void CleanupRacesVariants();
    }
}
