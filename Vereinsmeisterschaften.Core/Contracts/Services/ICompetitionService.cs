using System;
using System.Collections.Generic;
using System.Text;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Contracts.Services
{
    /// <summary>
    /// Interface for a service used to get and store a list of Competition objects
    /// </summary>
    public interface ICompetitionService
    {
        /// <summary>
        /// Load a list of Competitions to the <see cref="CompetitionList"/>.
        /// This is using a separate Task because the file possibly can be large.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>true if importing succeeded; false if importing failed (e.g. canceled)</returns>
        Task<bool> LoadFromFile(CancellationToken cancellationToken);

        /// <summary>
        /// Save the list of Competitions to a file
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>true if saving succeeded; false if saving failed (e.g. canceled)</returns>
        Task<bool> SaveToFile(CancellationToken cancellationToken);

        /// <summary>
        /// Event that is raised when the file operation progress changes
        /// </summary>
        event ProgressDelegate OnFileProgress;

        /// <summary>
        /// Event that is raised when the file operation is finished.
        /// </summary>
        event EventHandler OnFileFinished;

        /// <summary>
        /// Path to the competition file
        /// </summary>
        string CompetitionFilePath { get; set; }

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
    }
}
