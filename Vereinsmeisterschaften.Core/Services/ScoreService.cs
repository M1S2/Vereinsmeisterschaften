using System;
using System.Collections.Generic;
using System.Runtime;
using System.Text;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Services
{
    /// <summary>
    /// Service used to calculate the scores for all persons
    /// </summary>
    public class ScoreService : IScoreService
    {
        /// <summary>
        /// Score that a person gets if the start time equals the competition best time
        /// </summary>
        public const int BEST_TIME_SCORE = 100;

        private IPersonService _personService;
        private ICompetitionService _competitionService;

        public ScoreService(IPersonService personService, ICompetitionService competitionService)
        {
            _personService = personService;
            _competitionService = competitionService;
        }

        /// <summary>
        /// Update all scores for this <see cref="Person"/>
        /// </summary>
        /// <param name="person"><see cref="Person"/> for which to update the scores</param>
        /// <param name="competitionYear">Year in which the competition takes place</param>
        public void UpdateScoresForPerson(Person person, ushort competitionYear)
        {
            foreach (PersonStart start in person?.Starts?.Values)
            {
                Competition competition = _competitionService.GetCompetitionForPerson(person, start.Style, competitionYear);
                if (competition == null) { continue; }
                // If the start time equals the competition best time the score will be 100
                // If the person swims faster, the score is higher
                start.Score = (competition.BestTime.TotalMilliseconds / start.Time.TotalMilliseconds) * BEST_TIME_SCORE;
            }
        }

        /// <summary>
        /// Update all scores for all <see cref="Person"/>
        /// </summary>
        /// <param name="competitionYear">Year in which the competition takes place</param>
        public void UpdateScoresForAllPersons(ushort competitionYear)
        {
            foreach(Person person in _personService.GetPersons())
            {
                UpdateScoresForPerson(person, competitionYear);
            }
        }
    }
}
