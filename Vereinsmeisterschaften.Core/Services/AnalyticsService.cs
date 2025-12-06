using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;
using Vereinsmeisterschaften.Core.Settings;

namespace Vereinsmeisterschaften.Core.Services
{
    /// <summary>
    /// Service used to calculate different analytics
    /// </summary>
    public class AnalyticsService : IAnalyticsService
    {
        private IPersonService _personService;
        private ICompetitionService _competitionService;
        private IRaceService _raceService;
        private IWorkspaceService _workspaceService;

        /// <summary>
        /// Constructor for the <see cref="AnalyticsService"/>
        /// </summary>
        /// <param name="personService"><see cref="IPersonService"/> object</param>
        /// <param name="competitionService"><see cref="ICompetitionService"/> object</param>
        /// <param name="raceService"><see cref="IRaceService"/> object</param>
        /// <param name="workspaceService"><see cref="IWorkspaceService"/> object</param>
        public AnalyticsService(IPersonService personService, ICompetitionService competitionService, IRaceService raceService, IWorkspaceService workspaceService)
        {
            _personService = personService;
            _competitionService = competitionService;
            _raceService = raceService;
            _workspaceService = workspaceService;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Gender analytics

        /// <summary>
        /// Percentage of people that are male
        /// </summary>
        public double MalePersonPercentage => (_personService.GetPersons().Count(p => p.Gender == Genders.Male) / (double)_personService.PersonCount) * 100;

        /// <summary>
        /// Percentage of people that are female
        /// </summary>
        public double FemalePersonPercentage => (_personService.GetPersons().Count(p => p.Gender == Genders.Female) / (double)_personService.PersonCount) * 100;

        /// <summary>
        /// Percentage of starts that are male
        /// </summary>
        public double MaleStartsPercentage => (_personService.GetAllPersonStarts().Count(s => s.PersonObj?.Gender == Genders.Male) / (double)_personService.PersonStarts) * 100;

        /// <summary>
        /// Percentage of starts that are female
        /// </summary>
        public double FemaleStartsPercentage => (_personService.GetAllPersonStarts().Count(s => s.PersonObj?.Gender == Genders.Female) / (double)_personService.PersonStarts) * 100;

        #endregion
    }
}
