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
        private IPersonService _personService;
        private ICompetitionService _competitionService;

        public ScoreService(IPersonService personService, ICompetitionService competitionService)
        {
            _personService = personService;
            _competitionService = competitionService;
        }

        public void UpdateScoresForPerson(Person person, ushort competitionYear)
        {
            foreach (PersonStart start in person.Starts)
            {
                Competition competition = _competitionService.GetCompetitionForPerson(person, start.Style, competitionYear);
                if (competition == null) { continue; }
                start.Score = (competition.BestTime.TotalMilliseconds / start.Time.TotalMilliseconds) * 100;
            }
        }

        public void UpdateScoresForAllPersons(ushort competitionYear)
        {
            foreach(Person person in _personService.GetPersons())
            {
                UpdateScoresForPerson(person, competitionYear);
            }
        }
    }
}
