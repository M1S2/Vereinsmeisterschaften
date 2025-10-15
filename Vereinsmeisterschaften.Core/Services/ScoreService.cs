using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;
using Vereinsmeisterschaften.Core.Settings;

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

        /// <summary>
        /// Constructor for the <see cref="ScoreService"/>
        /// </summary>
        /// <param name="personService"><see cref="IPersonService"/> object</param>
        /// <param name="competitionService"><see cref="ICompetitionService"/> object</param>
        /// <param name="workspaceService"><see cref="IWorkspaceService"/> object</param>
        public ScoreService(IPersonService personService, ICompetitionService competitionService, IWorkspaceService workspaceService)
        {
            _personService = personService;
            _personService.SetScoreServiceObj(this);        // Dependency Injection can't be used in the constructor because of circular dependency
            _competitionService = competitionService;
            _workspaceService = workspaceService;

            _workspaceService.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(IWorkspaceService.Settings))
                {
                    UpdateScoresForAllPersons();
                }
            };
        }

        /// <summary>
        /// Update all scores for this <see cref="Person"/>
        /// </summary>
        /// <param name="person"><see cref="Person"/> for which to update the scores</param>
        public void UpdateScoresForPerson(Person person)
        {
            _competitionService.UpdateAllCompetitionsForPerson(person);

            foreach (PersonStart start in person?.Starts?.Values)
            {
                if (start == null) { continue; }

                if (!start.IsActive)
                {
                    // Inactive starts have no score
                    start.Score = 0;
                }
                else
                {
                    Competition competition = start.CompetitionObj;
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

                        ushort scoreFractionalDigits = _workspaceService?.Settings?.GetSettingValue<ushort>(WorkspaceSettings.GROUP_GENERAL, WorkspaceSettings.SETTING_GENERAL_SCORE_FRACTIONAL_DIGITS) ?? 1;
                        start.Score = Math.Round(start.Score, scoreFractionalDigits);

                        // Limit score to 0
                        if (start.Score < 0) { start.Score = 0; }
                    }
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
        /// Update the result list places for all <see cref="Person"/>.
        /// </summary>
        public void UpdateResultListPlacesForAllPersons()
        {
            List<Person> sortedPersons = GetPersonsSortedByScore(ResultTypes.Overall, false);
            List<PersonStart> bestStarts = new List<PersonStart>();
            bestStarts = sortedPersons.Where(p => p.HighestScoreStyle != SwimmingStyles.Unknown && p.Starts[p.HighestScoreStyle] != null)?.Select(p => p.Starts[p.HighestScoreStyle]).ToList();

            // Group all starts by the score. It is possible to have more than one start with the same score leading to the same podium place
            List<IGrouping<double, PersonStart>> groupedStarts = bestStarts.GroupBy(s => s.Score).ToList();
            sortedPersons.ForEach(p => p.ResultListPlace = 0); // Reset all result list places
            foreach (PersonStart personStart in bestStarts)
            {
                personStart.PersonObj.ResultListPlace = groupedStarts.IndexOf(groupedStarts.FirstOrDefault(g => g.Contains(personStart))) + 1;
            }
        }

        /// <summary>
        /// Get all persons, sort them depending on the requested <see cref="ResultTypes"/> and return as new list
        /// </summary>
        /// <param name="resultType">The list with all persons is sorted depending on this parameter</param>
        /// <param name="onlyActivePersons">Only return persons with <see cref="Person.IsActive"/> true</param>
        /// <returns>List with <see cref="Person"/> sorted (descending)</returns>
        public List<Person> GetPersonsSortedByScore(ResultTypes resultType, bool onlyActivePersons)
        {
            UpdateScoresForAllPersons();
            List<Person> persons = new List<Person>(_personService.GetPersons());
            persons = onlyActivePersons ? persons.Where(p => p.IsActive).ToList() : persons;

            if (resultType == ResultTypes.Overall)
            {
                return persons.OrderByDescending(p => p.HighestScore).ToList();
            }
            else if(resultType == ResultTypes.MaxAgeCompetitions)
            {
                return persons.Where(p => p.Starts[p.HighestScoreStyle].IsUsingMaxAgeCompetition).OrderByDescending(p => p.HighestScore).ToList();
            }
            else
            {
                SwimmingStyles style = getStyleFromResultType(resultType);
                return persons.Where(p => p.Starts[style] != null).OrderByDescending(p => p.Starts[style].Score).ToList();
            }       
        }

        /// <summary>
        /// Find the best starts of all persons depending on the <see cref="ResultTypes"/> and requested <see cref="ResultPodiumsPlaces"/>
        /// This method returns a list of start because there is the possibility to have more than one person with the same score.
        /// </summary>
        /// <param name="resultType">Only regard starts that match this <see cref="ResultTypes"/></param>
        /// <param name="podiumsPlace">Return the starts for this podium place</param>
        /// <returns>List with best <see cref="PersonStart"/> or null if no elements are found</returns>
        public List<PersonStart> GetWinnersPodiumStarts(ResultTypes resultType, ResultPodiumsPlaces podiumsPlace)
        {
            List<Person> sortedPersons = GetPersonsSortedByScore(resultType, true);
            UpdateResultListPlacesForAllPersons();
            List<PersonStart> bestStarts = new List<PersonStart>();
            if (resultType == ResultTypes.Overall || resultType == ResultTypes.MaxAgeCompetitions)
            {
                bestStarts = sortedPersons.Where(p => p.HighestScoreStyle != SwimmingStyles.Unknown && p.Starts[p.HighestScoreStyle] != null)?.Select(p => p.Starts[p.HighestScoreStyle]).ToList();
            }
            else
            {
                SwimmingStyles style = getStyleFromResultType(resultType);
                bestStarts = sortedPersons.Where(p => p.Starts[style] != null)?.Select(p => p.Starts[style]).ToList();
            }
            bestStarts = bestStarts.Where(s => s.IsActive).ToList();

            // Group all starts by the score. It is possible to have more than one start with the same score leading to the same podium place
            List<IGrouping<double, PersonStart>> groupedStarts = bestStarts.GroupBy(s => s.Score).ToList();
            if(podiumsPlace == ResultPodiumsPlaces.Gold && groupedStarts.Count > 0)
            {
                return groupedStarts[0].ToList();
            }
            else if (podiumsPlace == ResultPodiumsPlaces.Silver && groupedStarts.Count > 1)
            {
                return groupedStarts[1].ToList();
            }
            else if (podiumsPlace == ResultPodiumsPlaces.Bronze && groupedStarts.Count > 2)
            {
                return groupedStarts[2].ToList();
            }
            return null;
        }

        private SwimmingStyles getStyleFromResultType(ResultTypes resultType)
        {
            switch (resultType)
            {
                case ResultTypes.Overall: return SwimmingStyles.Unknown;
                case ResultTypes.MaxAgeCompetitions: return SwimmingStyles.Unknown;
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
