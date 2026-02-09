using System.Collections.ObjectModel;
using System.ComponentModel;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Contracts.Services
{
    /// <summary>
    /// Interface for a service used to get and store a list of <see cref="Competition"> objects
    /// </summary>
    public interface ICompetitionService : INotifyPropertyChanged, ISaveable
    {
        /// <summary>
        /// Save the reference to the <see cref="IWorkspaceService"/> object.
        /// Dependency Injection in the constructor can't be used here because there would be a circular dependency.
        /// </summary>
        /// <param name="workspaceService">Reference to the <see cref="IWorkspaceService"/> implementation</param>
        void SetWorkspaceServiceObj(IWorkspaceService workspaceService);

        /// <summary>
        /// Return all available Competitions
        /// </summary>
        /// <returns>List of <see cref="Competition"/> objects</returns>
        ObservableCollection<Competition> GetCompetitions();

        /// <summary>
        /// Clear all Competitions
        /// </summary>
        void ClearAll();

        /// <summary>
        /// Reset the list of Competitions to the state when the <see cref="Load(string, CancellationToken)"/> method was called.
        /// This will clear all Competitions and add the Competitions that were loaded at that time.
        /// </summary>
        void ResetToLoadedState();

        /// <summary>
        /// Add a new <see cref="Competition"/> to the list of Competitions.
        /// </summary>
        /// <param name="person"><see cref="Competition"/> to add</param>
        void AddCompetition(Competition competition);

        /// <summary>
        /// Remove the given <see cref="Competition"/> from the list of Competitions
        /// </summary>
        /// <param name="competition">Competition to remove</param>
        void RemoveCompetition(Competition competition);

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
        /// <param name="isUsingMaxAgeCompetition">Flag indicating if this start is using a competition that was found by the max available age.</param>
        /// <param name="isUsingExactAgeCompetition">Flag indicating if this start is using a competition for which the age of the person matches the competition age.</param>
        /// <returns>Found <see cref="Competition"/> or <see langword="null"/></returns>
        Competition GetCompetitionForPerson(Person person, SwimmingStyles swimmingStyle, out bool isUsingMaxAgeCompetition, out bool isUsingExactAgeCompetition);

        /// <summary>
        /// Update all <see cref="PersonStart"/> objects and the <see cref="Person.AvailableCompetitions"/> for the given <see cref="Person"/> with the corresponding <see cref="Competition"/> objects
        /// </summary>
        /// <param name="person"><see cref="Person"/> to update</param>
        void UpdateAllCompetitionsForPerson(Person person);

        /// <summary>
        /// Update all <see cref="PersonStart"/> and the <see cref="Person.AvailableCompetitions"/> objects with the corresponding <see cref="Competition"/> objects
        /// </summary>
        void UpdateAllCompetitionsForPerson();

        /// <summary>
        /// Update the <see cref="Competition.Distance"/> from the matching <see cref="CompetitionDistanceRule"/>
        /// </summary>
        /// <param name="competition"><see cref="Competition"/> that is updated</param>
        /// <param name="keepRudolphTableFlags">Set this to true to make sure that the rudolph table related flags aren't changed (otherwise changing the Distance will reset the flags)</param>
        void UpdateCompetitionDistanceFromDistanceRules(Competition competition, bool keepRudolphTableFlags = false);

        /// <summary>
        /// Update the <see cref="Competition.Distance"/> from the matching <see cref="CompetitionDistanceRule"/> for all <see cref="Competition"/> objects.
        /// </summary>
        /// <param name="keepRudolphTableFlags">Set this to true to make sure that the rudolph table related flags aren't changed (otherwise changing the Distance will reset the flags)</param>
        void UpdateAllCompetitionDistancesFromDistanceRules(bool keepRudolphTableFlags = false);

        /// <summary>
        /// Update all <see cref="Competition.BestTime"/> properties from the given rudolph table.
        /// </summary>
        /// <param name="rudolphTableCsvFile">CSV file for the rudolph table</param>
        /// <param name="rudolphScore">Rudolph score used to identify the row in the table to use</param>
        void UpdateAllCompetitionTimesFromRudolphTable(string rudolphTableCsvFile, byte rudolphScore);

        /// <summary>
        /// Create the <see cref="Competition"/> objects from the given rudolph table.
        /// The lines from the given rudolph score are used.
        /// To find the correct columns (distances), the <see cref="CompetitionDistanceRules"/> are used.
        /// CAUTION: All competitions are removed!
        /// </summary>
        /// <param name="rudolphTableCsvFile">CSV file for the rudolph table</param>
        /// <param name="rudolphScore">Rudolph score used to identify the row in the table to use</param>
        void CreateCompetitionsFromRudolphTable(string rudolphTableCsvFile, byte rudolphScore);

        /// <summary>
        /// For each swimming style, gender and distance take all competitions with times from the rudolph table and interpolate these times to find values for the competitions without times from the rudolph table.
        /// </summary>
        /// <exception cref="Exception">An exception is thrown, when the interpolation fails for at least one group of <see cref="Competition"/></exception>
        void InterpolateMissingCompetitionTimesFromRudolphTable();

    }
}
