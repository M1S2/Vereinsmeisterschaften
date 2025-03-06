using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Contracts.Services
{
    /// <summary>
    /// Interface for a service used to get and store a list of Person objects
    /// </summary>
    public interface IPersonService : INotifyPropertyChanged, ISaveable
    {
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
        public List<Person> CheckForDuplicatePerson();
    }
}
