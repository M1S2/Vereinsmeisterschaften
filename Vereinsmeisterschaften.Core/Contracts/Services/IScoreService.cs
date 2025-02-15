using System;
using System.Collections.Generic;
using System.Text;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Contracts.Services
{
    /// <summary>
    /// Interface for a service used to calculate the scores for all persons
    /// </summary>
    public interface IScoreService
    {
        /// <summary>
        /// Update all scores for this <see cref="Person"/>
        /// </summary>
        /// <param name="person"><see cref="Person"/> for which to update the scores</param>
        void UpdateScoresForPerson(Person person);

        /// <summary>
        /// Update all scores for all <see cref="Person"/>
        /// </summary>
        void UpdateScoresForAllPersons();

        /// <summary>
        /// Get all persons, sort them by their highest scores and return as new list
        /// </summary>
        /// <returns>List with <see cref="Person"/> sorted by <see cref="Person.HighestScore"/> (descending)</returns>
        List<Person> GetPersonsSortedByScore();
    }
}
