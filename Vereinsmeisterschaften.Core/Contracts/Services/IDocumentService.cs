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
        /// String marker for placeholders in the template files. The placeholder must be enclosed by this marker.
        /// e.g. PlaceholderMarker = "%" --> %PLACEHOLDER%
        /// </summary>
        string PlaceholderMarker { get; }

        /// <summary>
        /// Create the document indicated by the document type.
        /// For the <see cref="DocumentCreationTypes.Certificates"/> type, the <see cref="SetCertificateCreationFilters"/> method must be called before this method to set the filters for the certificate creation. Otherwise the old values are used.
        /// </summary>
        /// <param name="documentType"><see cref="DocumentCreationTypes"/> used to decide which document type and <see cref="IDocumentStrategy"/> is used</param>
        /// <param name="createPdf">True to also create a .pdf file</param>
        /// <returns><see cref="Task"/> that can be used to run this async. The return parameter is a tuple of (number of created pages in the document, filepath to the last created document (PDF if possible))</returns>
        Task<(int, string)> CreateDocument(DocumentCreationTypes documentType, bool createPdf = true);

        /// <summary>
        /// Set the filters that are used for the certificate creation.
        /// </summary>
        /// <param name="personStartFilter">Filter to select only some specific <see cref="PersonStart"/> elements. Only valid for <see cref="DocumentCreationTypes.Certificates"/></param>
        /// <param name="personStartFilterParameter">Parameter for the personStartFilter. Only valid for <see cref="DocumentCreationTypes.Certificates"/></param>
        void SetCertificateCreationFilters(PersonStartFilters personStartFilter = PersonStartFilters.None, object personStartFilterParameter = null);
    }
}
