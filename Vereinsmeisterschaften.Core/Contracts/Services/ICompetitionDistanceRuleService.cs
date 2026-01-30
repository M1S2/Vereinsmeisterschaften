using System.Collections.ObjectModel;
using System.ComponentModel;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Contracts.Services
{
    /// <summary>
    /// Interface for a service used to get and store a list of <see cref="CompetitionDistanceRule"/> objects
    /// </summary>
    public interface ICompetitionDistanceRuleService : INotifyPropertyChanged, ISaveable
    {
        /// <summary>
        /// Save the reference to the <see cref="ICompetitionService"/> object.
        /// Dependency Injection in the constructor can't be used here because there would be a circular dependency.
        /// </summary>
        /// <param name="competitionService">Reference to the <see cref="ICompetitionService"/> implementation</param>
        void SetCompetitionServiceObj(ICompetitionService competitionService);

        /// <summary>
        /// Return all available <see cref="CompetitionDistanceRule"> objects.
        /// </summary>
        /// <returns>List of <see cref="CompetitionDistanceRule"/> objects</returns>
        ObservableCollection<CompetitionDistanceRule> GetCompetitionDistanceRules();

        /// <summary>
        /// Clear all <see cref="CompetitionDistanceRule">
        /// </summary>
        void ClearAll();

        /// <summary>
        /// Reset the list of <see cref="CompetitionDistanceRule"> to the state when the <see cref="Load(string, CancellationToken)"/> method was called.
        /// This will clear all <see cref="CompetitionDistanceRule"> and add the <see cref="CompetitionDistanceRule"> that were loaded at that time.
        /// </summary>
        void ResetToLoadedState();

        /// <summary>
        /// Add a new <see cref="CompetitionDistanceRule"/> to the <see cref="CompetitionDistanceRules"/> list.
        /// </summary>
        /// <param name="distanceRule"><see cref="CompetitionDistanceRule"/> to add</param>
        void AddDistanceRule(CompetitionDistanceRule distanceRule);

        /// <summary>
        /// Remove the given <see cref="CompetitionDistanceRule"/> from the <see cref="Competition"/> list.
        /// </summary>
        /// <param name="distanceRule"><see cref="CompetitionDistanceRule"/> to remove</param>
        void RemoveDistanceRule(CompetitionDistanceRule distanceRule);

        /// <summary>
        /// Try to find a matching rule for the requested age and swimming style.
        /// </summary>
        /// <param name="age">Age to find</param>
        /// <param name="swimmingStyle"><see cref="SwimmingStyles"/> to find</param>
        /// <returns>Found distance or 0 if not found</returns>
        ushort GetCompetitionDistanceFromRules(byte age, SwimmingStyles swimmingStyle);
    }
}
