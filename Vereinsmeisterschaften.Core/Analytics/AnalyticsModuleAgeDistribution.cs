using Vereinsmeisterschaften.Core.Contracts.Services;

namespace Vereinsmeisterschaften.Core.Analytics
{
    /// <summary>
    /// Analytics module to calculate the age distribution of all persons
    /// </summary>
    public class AnalyticsModuleAgeDistribution : IAnalyticsModule
    {
        private IPersonService _personService;

        /// <summary>
        /// Constructor for the <see cref="AnalyticsModuleAgeDistribution"/>
        /// </summary>
        /// <param name="personService"><see cref="IPersonService"/> object</param>
        public AnalyticsModuleAgeDistribution(IPersonService personService)
        {
            _personService = personService;
        }

        /// <inheritdoc/>
        public bool AnalyticsAvailable => true;

        /// <summary>
        /// Dictionary that counts the number of persons (value) per birth year (key)
        /// </summary>
        public Dictionary<UInt16, int> NumberPersonsPerBirthYear => _personService.GetPersons()
                                                                                  .Where(p => p.IsActive)
                                                                                  .GroupBy(p => p.BirthYear)
                                                                                  .OrderBy(g => g.Key)
                                                                                  .ToDictionary(g => g.Key, g => g.Count());
    }
}
