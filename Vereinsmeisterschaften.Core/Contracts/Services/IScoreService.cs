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
        /// Update the result list places for all <see cref="Person"/>.
        /// </summary>
        void UpdateResultListPlacesForAllPersons();

        /// <summary>
        /// Get all persons, sort them depending on the requested <see cref="ResultTypes"/> and return as new list
        /// </summary>
        /// <param name="resultType">The list with all persons is sorted depending on this parameter</param>
        /// <returns>List with <see cref="Person"/> sorted (descending)</returns>
        List<Person> GetPersonsSortedByScore(ResultTypes resultType = ResultTypes.Overall);

        /// <summary>
        /// Find the best starts of all persons depending on the <see cref="ResultTypes"/> and requested <see cref="ResultPodiumsPlaces"/>
        /// This method returns a list of start because there is the possibility to have more than one person with the same score.
        /// </summary>
        /// <param name="resultType">Only regard starts that match this <see cref="ResultTypes"/></param>
        /// <param name="podiumsPlace">Return the starts for this podium place</param>
        /// <returns>List with best <see cref="PersonStart"/> or null if no elements are found</returns>
        List<PersonStart> GetWinnersPodiumStarts(ResultTypes resultType, ResultPodiumsPlaces podiumsPlace);
    }
}
