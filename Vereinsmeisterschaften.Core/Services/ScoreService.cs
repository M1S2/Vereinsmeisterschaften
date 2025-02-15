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
        private IWorkspaceService _workspaceService;

        public ScoreService(IPersonService personService, ICompetitionService competitionService, IWorkspaceService workspaceService)
        {
            _personService = personService;
            _competitionService = competitionService;
            _workspaceService = workspaceService;
        }

        /// <summary>
        /// Update all scores for this <see cref="Person"/>
        /// </summary>
        /// <param name="person"><see cref="Person"/> for which to update the scores</param>
        public void UpdateScoresForPerson(Person person)
        {
            foreach (PersonStart start in person?.Starts?.Values)
            {
                Competition competition = _competitionService.GetCompetitionForPerson(person, start.Style, _workspaceService.Settings.CompetitionYear);
                if (competition == null) { continue; }
                // If the start time equals the competition best time the score will be 100
                // If the person swims faster, the score is higher
                if (start.Time.TotalMilliseconds == 0)
                {
                    start.Score = 0;
                }
                else
                {
                    // Original formula from old Vereinsmeisterschaften tool:
                    // score = 100 * (1 - ((start_time - competition_best_time) / competition_best_time)) = 100 * (2 - (start_time / competition_best_time))
                    // if startTime == bestTime  =>  BEST_TIME_SCORE
                    // linear equation around this point
                    // zero points if startTime >= 2* bestTime
                    start.Score = (2 - (start.Time.TotalMilliseconds / competition.BestTime.TotalMilliseconds)) * BEST_TIME_SCORE;

                    // Limit score to 0
                    if(start.Score < 0) { start.Score = 0; }
                }
            }
        }

        /// <summary>
        /// Update all scores for all <see cref="Person"/>
        /// </summary>
        public void UpdateScoresForAllPersons()
        {
            foreach(Person person in _personService.GetPersons())
            {
                UpdateScoresForPerson(person);
            }
        }

        /// <summary>
        /// Get all persons, sort them by their highest scores and return as new list
        /// </summary>
        /// <returns>List with <see cref="Person"/> sorted by <see cref="Person.HighestScore"/> (descending)</returns>
        public List<Person> GetPersonsSortedByScore()
        {
            UpdateScoresForAllPersons();
            List<Person> persons = new List<Person>(_personService.GetPersons());
            return persons.OrderByDescending(p => p.HighestScore).ToList();
        }
    }
}
