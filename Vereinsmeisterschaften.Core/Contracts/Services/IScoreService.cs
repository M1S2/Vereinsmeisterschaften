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
        /// <param name="competitionYear">Year in which the competition takes place</param>
        void UpdateScoresForPerson(Person person, ushort competitionYear);

        /// <summary>
        /// Update all scores for all <see cref="Person"/>
        /// </summary>
        /// <param name="competitionYear">Year in which the competition takes place</param>
        void UpdateScoresForAllPersons(ushort competitionYear);
    }
}
