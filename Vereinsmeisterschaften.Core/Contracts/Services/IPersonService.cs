using System;
using System.Collections.Generic;
using System.Text;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Contracts.Services
{
    /// <summary>
    /// Delegate void for progress changes
    /// </summary>
    /// <param name="sender">Progress sender</param>
    /// <param name="progress">Progress 0 .. 100 </param>
    public delegate void ProgressDelegate(object sender, float progress);


    /// <summary>
    /// Interface for a service used to get and store a list of Person objects
    /// </summary>
    public interface IPersonService
    {
        /// <summary>
        /// Load a list of Persons to the <see cref="PersonList"/>.
        /// This is using a separate Task because the file possibly can be large.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>true if importing succeeded; false if importing failed (e.g. canceled)</returns>
        Task<bool> LoadFromFile(CancellationToken cancellationToken);

        /// <summary>
        /// Save the list of Persons to a file
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
        /// Path to the person file
        /// </summary>
        string PersonFilePath { get; set; }

        /// <summary>
        /// Return all available Persons.
        /// </summary>
        /// <returns>List of <see cref="Person"/> objects</returns>
        List<Person> GetPersons();

        /// <summary>
        /// Clear all Persons.
        /// </summary>
        void ClearPersons();

        /// <summary>
        /// Add a new <see cref="Person"/> to the list of Persons.
        /// </summary>
        /// <param name="person"><see cref="Person"/> to add</param>
        void AddPerson(Person person);

        /// <summary>
        /// Return the number of <see cref="Person"/>
        /// </summary>
        /// <returns>Number of <see cref="Person"/></returns>
        int PersonCount { get; }
    }
}
