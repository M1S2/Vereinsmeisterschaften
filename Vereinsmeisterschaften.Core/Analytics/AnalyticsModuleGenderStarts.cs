using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Analytics
{
    /// <summary>
    /// Analytics module to calculate the gender deviation for the starts
    /// </summary>
    public class AnalyticsModuleGenderStarts : IAnalyticsModule
    {
        private IPersonService _personService;

        /// <summary>
        /// Constructor for the <see cref="AnalyticsModuleGenderStarts"/>
        /// </summary>
        /// <param name="personService"><see cref="IPersonService"/> object</param>
        public AnalyticsModuleGenderStarts(IPersonService personService)
        {
            _personService = personService;
        }

        /// <inheritdoc/>
        public bool AnalyticsAvailable => true;

        /// <summary>
        /// Number of starts that are male
        /// </summary>
        public double MaleStartsCount => _personService.GetAllPersonStarts().Count(s => s.IsActive && s.IsCompetitionObjAssigned && s.PersonObj?.Gender == Genders.Male);

        /// <summary>
        /// Percentage of starts that are male
        /// </summary>
        public double MaleStartsPercentage => (MaleStartsCount / (double)_personService.GetAllPersonStarts().Count(s => s.IsActive && s.IsCompetitionObjAssigned)) * 100;

        /// <summary>
        /// Number of starts that are female
        /// </summary>
        public double FemaleStartsCount => _personService.GetAllPersonStarts().Count(s => s.IsActive && s.IsCompetitionObjAssigned && s.PersonObj?.Gender == Genders.Female);

        /// <summary>
        /// Percentage of starts that are female
        /// </summary>
        public double FemaleStartsPercentage => (FemaleStartsCount / (double)_personService.GetAllPersonStarts().Count(s => s.IsActive && s.IsCompetitionObjAssigned)) * 100;
    }
}
