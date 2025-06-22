using System;
using System.Collections.Generic;
using System.Text;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Contracts.Services
{
    /// <summary>
    /// Interface for a service used to create documents like certificates or start lists
    /// </summary>
    public interface IDocumentService
    {
        /// <summary>
        /// Create certificates for all person starts in the database based on the given filter.
        /// </summary>
        /// <param name="createPdf">True to also create a .pdf file</param>
        /// <param name="personStartFilter">Filter to select only some specific person starts</param>
        /// <param name="personStartFilterParameter">Parameter for the personStartFilter</param>
        /// <returns>Number of created certificates</returns>
        Task<int> CreateCertificates(bool createPdf = true, PersonStartFilters personStartFilter = PersonStartFilters.None, object personStartFilterParameter = null);

        /// <summary>
        /// Create an overview list of all person starts in the database and save it to the output folder.
        /// </summary>
        /// <param name="createPdf">True to also create a .pdf file</param>
        /// <returns><see cref="Task"/> that can be used to run this async</returns>
        Task CreateOverviewList(bool createPdf = true);

        /// <summary>
        /// Create a list with all races and the planned order.
        /// </summary>
        /// <param name="createPdf">True to also create a .pdf file</param>
        /// <returns><see cref="Task"/> that can be used to run this async</returns>
        Task CreateRaceStartList(bool createPdf = true);

        /// <summary>
        /// Create a list with the overall result.
        /// </summary>
        /// <param name="createPdf">True to also create a .pdf file</param>
        /// <returns><see cref="Task"/> that can be used to run this async</returns>
        Task CreateResultList(bool createPdf = true);
    }
}
