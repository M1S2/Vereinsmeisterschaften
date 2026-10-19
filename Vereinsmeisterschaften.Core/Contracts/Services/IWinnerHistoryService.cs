using System.Collections.ObjectModel;
using System.ComponentModel;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Contracts.Services
{
    /// <summary>
    /// Interface for a service used to keep the history of all winners
    /// </summary>
    public interface IWinnerHistoryService : INotifyPropertyChanged, ISaveable
    {
        /// <summary>
        /// Save the reference to the <see cref="IScoreService"/> object.
        /// Dependency Injection in the constructor can't be used here because there would be a circular dependency.
        /// </summary>
        /// <param name="scoreService">Reference to the <see cref="IScoreService"/> implementation</param>
        void SetScoreServiceObj(IScoreService scoreService);

        /// <summary>
        /// Save the reference to the <see cref="IWorkspaceService"/> object.
        /// Dependency Injection in the constructor can't be used here because there would be a circular dependency.
        /// </summary>
        /// <param name="workspaceService">Reference to the <see cref="IWorkspaceService"/> implementation</param>
        void SetWorkspaceServiceObj(IWorkspaceService workspaceService);

        /// <summary>
        /// Winners of the current competition year. It is possible to have more than one person per podium place.
        /// </summary>
        WinnerHistoryElement CurrentWinners { get; }

        /// <summary>
        /// Check if the <see cref="CurrentWinners"/> is part of the <see cref="PreviousWinners"/> list.
        /// The <see cref="WinnerHistoryElement.Timestamp"> and all winner lists must match.
        /// </summary>
        bool IsCurrentWinnersPartOfPreviousWinners { get; }

        /// <summary>
        /// Get the index of the <see cref="CurrentWinners"/> inside of the <see cref="PreviousWinners"/> list.
        /// The <see cref="WinnerHistoryElement.Timestamp"> and all winner lists must match.
        /// If no matching element is found, -1 is returned.
        /// </summary>
        public int IndexOfCurrentWinnersInsidePreviousWinners { get; }

        /// <summary>
        /// Return all available previous <see cref="WinnerHistoryElement"/> entries
        /// </summary>
        /// <returns>List of <see cref="WinnerHistoryElement"/> objects</returns>
        ObservableCollection<WinnerHistoryElement> GetPreviousWinnersList();

        /// <summary>
        /// Clear all previous winners
        /// </summary>
        void ClearAll();

        /// <summary>
        /// Reset the list of previous winners to the state when the <see cref="Load(string, CancellationToken)"/> method was called.
        /// This will clear all <see cref="WinnerHistoryElement"> objects and add the <see cref="WinnerHistoryElement"> objects that were loaded at that time.
        /// </summary>
        void ResetToLoadedState();

        /// <summary>
        /// Add a new <see cref="WinnerHistoryElement"/> to the list of previous winners
        /// </summary>
        /// <param name="previousWinner"><see cref="WinnerHistoryElement"/> to add</param>
        void AddPreviousWinner(WinnerHistoryElement previousWinner);

        /// <summary>
        /// Remove the given <see cref="WinnerHistoryElement"/> from the list of previous winners
        /// </summary>
        /// <param name="previousWinner"><see cref="WinnerHistoryElement"> to remove</param>
        void RemovePreviousWinner(WinnerHistoryElement previousWinner);

        /// <summary>
        /// Copy the <see cref="CurrentWinners"/> to the list of previous <see cref="WinnerHistoryElement"/> list.
        /// If there is already an entry with the same timestamp, it will be replaced.
        /// </summary>
        void CopyCurrentWinnersToPrevious();

        /// <summary>
        /// Remove the <see cref="CurrentWinners"/> from the previous <see cref="WinnerHistoryElement"/> list.
        /// </summary>
        /// <returns><see langword="true"/> when an element was removed; otherwise <see langword="false"/></returns>
        bool RemoveCurrentWinnersFromPrevious();

        /// <summary>
        /// Update the list of all current winners and the corresponding flags inside.
        /// </summary>
        void UpdateCurrentWinners();

        /// <summary>
        /// Update the flags of all previous <see cref="WinnerHistoryElement"/> objects.
        /// </summary>
        void UpdatePreviousWinnersHistoryFlags();

        /// <summary>
        /// Return a dictionary how often each person was on the specific podium.
        /// Only persons with at least one time on the podium are returned.
        /// </summary>
        /// <param name="podiumPlace">Podium place used to count.</param>
        /// <param name="ignoreCurrentWinners">if <see langword="true"/> ignore the element in the previous winners list that is the current winners element</param>
        /// <returns>Dictionary with names and the number of specific podium times (ordered descending by the count).</returns>
        Dictionary<WinnerName, int> GetPreviousWinnersCountPerPerson(ResultPodiumsPlaces podiumPlace = ResultPodiumsPlaces.All, bool ignoreCurrentWinners = false);
    }
}
