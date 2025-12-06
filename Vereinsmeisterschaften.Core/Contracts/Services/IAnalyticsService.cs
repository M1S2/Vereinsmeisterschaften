using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Contracts.Services
{
    /// <summary>
    /// Interface for a service used to calculate different analytics
    /// </summary>
    public interface IAnalyticsService
    {
        #region Gender analytics

        /// <summary>
        /// Percentage of people that are male
        /// </summary>
        double MalePersonPercentage { get; }

        /// <summary>
        /// Percentage of people that are female
        /// </summary>
        double FemalePersonPercentage { get; }

        /// <summary>
        /// Percentage of starts that are male
        /// </summary>
        double MaleStartsPercentage { get; }

        /// <summary>
        /// Percentage of starts that are female
        /// </summary>
        double FemaleStartsPercentage { get; }

        #endregion
    }
}
