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
        /// Get all persons, sort them depending on the requested <see cref="ResultTypes"/> and return as new list
        /// </summary>
        /// <param name="resultType">The list with all persons is sorted depending on this parameter</param>
        /// <returns>List with <see cref="Person"/> sorted (descending)</returns>
        List<Person> GetPersonsSortedByScore(ResultTypes resultType = ResultTypes.Overall);

        /// <summary>
        /// Find the best starts of all persons depending on the <see cref="ResultTypes"/>
        /// </summary>
        /// <param name="resultType">Only regard starts that match this <see cref="ResultTypes"/></param>
        /// <param name="numberOfStartsToReturn">Maximum number of starts to return</param>
        /// <returns>List with best <see cref="PersonStart"/></returns>
        List<PersonStart> GetBestPersonStarts(ResultTypes resultType = ResultTypes.Overall, int numberOfStartsToReturn = 3);
    }
}
