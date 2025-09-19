﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Contracts.Services
{
    /// <summary>
    /// Interface for a service used to get and store a list of Competition objects
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
        /// <returns>Found <see cref="Competition"/> or <see langword="null"/></returns>
        Competition GetCompetitionForPerson(Person person, SwimmingStyles swimmingStyle, out bool isUsingMaxAgeCompetition);

        /// <summary>
        /// Update all <see cref="PersonStart"/> objects and the <see cref="Person.AvailableCompetitions"/> for the given <see cref="Person"/> with the corresponding <see cref="Competition"/> objects
        /// </summary>
        /// <param name="person"><see cref="Person"/> to update</param>
        void UpdateAllCompetitionsForPerson(Person person);

        /// <summary>
        /// Update all <see cref="PersonStart"/> and the <see cref="Person.AvailableCompetitions"/> objects with the corresponding <see cref="Competition"/> objects
        /// </summary>
        void UpdateAllCompetitionsForPerson();
    }
}
