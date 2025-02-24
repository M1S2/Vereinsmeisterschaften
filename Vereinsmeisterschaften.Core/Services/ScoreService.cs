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
        /// Get all persons, sort them depending on the requested <see cref="ResultTypes"/> and return as new list
        /// </summary>
        /// <param name="resultType">The list with all persons is sorted depending on this parameter</param>
        /// <returns>List with <see cref="Person"/> sorted (descending)</returns>
        public List<Person> GetPersonsSortedByScore(ResultTypes resultType)
        {
            UpdateScoresForAllPersons();
            List<Person> persons = new List<Person>(_personService.GetPersons());

            if (resultType == ResultTypes.Overall)
            {
                return persons.OrderByDescending(p => p.HighestScore).ToList();
            }
            else
            {
                SwimmingStyles style = getStyleFromResultType(resultType);
                return persons.Where(p => p.Starts.ContainsKey(style)).OrderByDescending(p => p.Starts[style].Score).ToList();
            }       
        }

        /// <summary>
        /// Find the best starts of all persons depending on the <see cref="ResultTypes"/>
        /// </summary>
        /// <param name="resultType">Only regard starts that match this <see cref="ResultTypes"/></param>
        /// <param name="numberOfStartsToReturn">Maximum number of starts to return</param>
        /// <returns>List with best <see cref="PersonStart"/></returns>
        public List<PersonStart> GetBestPersonStarts(ResultTypes resultType, int numberOfStartsToReturn)
        {
            List<Person> sortedPersons = GetPersonsSortedByScore(resultType);
            List<PersonStart> bestStarts = new List<PersonStart>();
            if(resultType == ResultTypes.Overall)
            {
                bestStarts = sortedPersons.Where(p => p.Starts.ContainsKey(p.HighestScoreStyle))?.Select(p => p.Starts[p.HighestScoreStyle]).ToList();
            }
            else
            {
                SwimmingStyles style = getStyleFromResultType(resultType);
                bestStarts = sortedPersons.Where(p => p.Starts.ContainsKey(style))?.Select(p => p.Starts[style]).ToList();
            }
            bestStarts = bestStarts.Take(numberOfStartsToReturn).ToList();  // Limit the number of starts to maximum numberOfStartsToReturn (less elements possible)
            bestStarts.AddRange(Enumerable.Repeat<PersonStart>(null, numberOfStartsToReturn - bestStarts.Count));   // Add null until the numberOfStartsToReturn elements are reached
            return bestStarts.Take(numberOfStartsToReturn).ToList();
        }

        private SwimmingStyles getStyleFromResultType(ResultTypes resultType)
        {
            switch (resultType)
            {
                case ResultTypes.Overall: return SwimmingStyles.Unknown;
                case ResultTypes.Breaststroke: return SwimmingStyles.Breaststroke;
                case ResultTypes.Freestyle: return SwimmingStyles.Freestyle;
                case ResultTypes.Backstroke: return SwimmingStyles.Backstroke;
                case ResultTypes.Butterfly: return SwimmingStyles.Butterfly;
                case ResultTypes.Medley: return SwimmingStyles.Medley;
                case ResultTypes.WaterFlea: return SwimmingStyles.WaterFlea;
                default: return SwimmingStyles.Unknown;
            }
        }
    }
}
