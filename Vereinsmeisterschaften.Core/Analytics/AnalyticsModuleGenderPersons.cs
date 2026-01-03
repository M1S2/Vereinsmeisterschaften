using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Analytics
{
    /// <summary>
    /// Analytics module to calculate the gender deviation for the persons
    /// </summary>
    public class AnalyticsModuleGenderPersons : IAnalyticsModule
    {
        private IPersonService _personService;

        /// <summary>
        /// Constructor for the <see cref="AnalyticsModuleGenderPersons"/>
        /// </summary>
        /// <param name="personService"><see cref="IPersonService"/> object</param>
        public AnalyticsModuleGenderPersons(IPersonService personService)
        {
            _personService = personService;
        }

        /// <inheritdoc/>
        public bool AnalyticsAvailable => _personService.PersonCount > 0;

        /// <summary>
        /// Number of people that are male
        /// </summary>
        public int MalePersonCount => _personService.GetPersons().Count(p => p.IsActive && p.Gender == Genders.Male);

        /// <summary>
        /// Percentage of people that are male
        /// </summary>
        public double MalePersonPercentage => (MalePersonCount / (double)_personService.GetPersons().Count(p => p.IsActive)) * 100;

        /// <summary>
        /// Number of people that are female
        /// </summary>
        public int FemalePersonCount => _personService.GetPersons().Count(p => p.IsActive && p.Gender == Genders.Female);

        /// <summary>
        /// Percentage of people that are female
        /// </summary>
        public double FemalePersonPercentage => (FemalePersonCount / (double)_personService.GetPersons().Count(p => p.IsActive)) * 100;
    }
}
