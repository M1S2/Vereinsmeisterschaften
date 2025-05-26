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
        Task<int> CreateCertificates(bool createPdf = true, PersonStartFilters personStartFilter = PersonStartFilters.None, object personStartFilterParameter = null);
        Task CreateOverviewList(bool createPdf = true);
    }
}
