using System.Collections.ObjectModel;
using System.ComponentModel;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Contracts.Services
{
    /// <summary>
    /// Interface for a service used to get and store a list of Person objects
    /// </summary>
    public interface IPersonService : INotifyPropertyChanged, ISaveable
    {
        /// <summary>
        /// Save the reference to the <see cref="IScoreService"/> object.
        /// Dependency Injection in the constructor can't be used here because there would be a circular dependency.
        /// </summary>
        /// <param name="scoreService">Reference to the <see cref="IScoreService"/> implementation</param>
        void SetScoreServiceObj(IScoreService scoreService);

        /// <summary>
        /// Save the reference to the <see cref="ICompetitionService"/> object.
        /// Dependency Injection in the constructor can't be used here because there would be a circular dependency.
        /// </summary>
        /// <param name="competitionService">Reference to the <see cref="ICompetitionService"/> implementation</param>
        void SetCompetitionServiceObj(ICompetitionService competitionService);

        /// <summary>
        /// Save the reference to the <see cref="IRaceService"/> object.
        /// Dependency Injection in the constructor can't be used here because there would be a circular dependency.
        /// </summary>
        /// <param name="raceService">Reference to the <see cref="IRaceService"/> implementation</param>
        void SetRaceServiceObj(IRaceService raceService);

        /// <summary>
        /// Return all available Persons
        /// </summary>
        /// <returns>List of <see cref="Person"/> objects</returns>
        ObservableCollection<Person> GetPersons();

        /// <summary>
        /// Clear all Persons
        /// </summary>
        void ClearAll();

        /// <summary>
        /// Reset the list of Persons to the state when the <see cref="Load(string, CancellationToken)"/> method was called.
        /// This will clear all Persons and add the Persons that were loaded at that time.
        /// </summary>
        void ResetToLoadedState();

        /// <summary>
        /// Add a new <see cref="Person"/> to the list of Persons.
        /// </summary>
        /// <param name="person"><see cref="Person"/> to add</param>
        void AddPerson(Person person);

        /// <summary>
        /// Remove the given <see cref="Person"/> from the list of Persons
        /// </summary>
        /// <param name="person">Person to remove</param>
        void RemovePerson(Person person);

        /// <summary>
        /// Return the number of <see cref="Person"/>
        /// </summary>
        /// <returns>Number of <see cref="Person"/></returns>
        int PersonCount { get; }

        /// <summary>
        /// Return the total number of starts of all persons
        /// </summary>
        int PersonStarts { get; }

        /// <summary>
        /// Find all duplicate <see cref="Person"/> objects.
        /// </summary>
        /// <returns>List with duplicate <see cref="Person"/></returns>
        List<Person> CheckForDuplicatePerson();

        /// <summary>
        /// Get all <see cref="PersonStart"/> objects for all <see cref="Person"/> objects that are not <see langword="null"/>.
        /// </summary>
        /// <param name="filter">Filter used to only return a subset of all <see cref="PersonStart"/> objects</param>
        /// <param name="filterParameter">Parameter used depending on the selected filter</param>
        /// <returns>List with <see cref="PersonStart"/> objects</returns>
        List<PersonStart> GetAllPersonStarts(PersonStartFilters filter = PersonStartFilters.None, object filterParameter = null);
    }
}
