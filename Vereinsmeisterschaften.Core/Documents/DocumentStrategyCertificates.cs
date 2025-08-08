using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;
using Vereinsmeisterschaften.Core.Services;
using Vereinsmeisterschaften.Core.Settings;

namespace Vereinsmeisterschaften.Core.Documents
{
    /// <summary>
    /// Document strategy for creating certificates.
    /// </summary>
    public class DocumentStrategyCertificates : DocumentStrategyBase<PersonStart>
    {
        private readonly IPersonService _personService;

        /// <summary>
        /// Constructor for the document strategy for certificates.
        /// </summary>
        /// <param name="personService"><see cref="IPersonService"/> that is used to get the <see cref="PersonStart"/> items</param>
        /// <param name="workspaceService"><see cref="IWorkspaceService"/> that can be used to e.g. get the workspace root path</param>
        /// <param name="serviceProvider"><see cref="IServiceProvider"/> used to get the correct <see cref="IDocumentPlaceholderResolver{T}"/></param>
        public DocumentStrategyCertificates(IPersonService personService, IWorkspaceService workspaceService, IServiceProvider serviceProvider) : base(workspaceService, serviceProvider)
        {
            _personService = personService;
        }

        /// <inheritdoc/>
        public override DocumentCreationTypes DocumentType => DocumentCreationTypes.Certificates;

        /// <inheritdoc/>
        public override string TemplatePath => DocumentService.GetDocumentPathAbsolute(WorkspaceSettings.SETTING_DOCUMENT_CREATION_CERTIFICATE_TEMPLATE_PATH, _workspaceService);

        /// <summary>
        /// This always returns <see langword="true"/>
        /// </summary>
        public override bool CreateMultiplePages => true;

        /// <summary>
        /// Return a list of all <see cref="PersonStart"/> items which have <see cref="PersonStart.CompetitionObj"/> assigned.
        /// </summary>
        /// <returns>List of all <see cref="PersonStart"/> items which have <see cref="PersonStart.CompetitionObj"/> assigned</returns>
        public override PersonStart[] GetItems()
            => _personService.GetAllPersonStarts().Where(s => s.CompetitionObj != null).ToArray();

        /// <summary>
        /// Return a list of all <see cref="PersonStart"/> items which have <see cref="PersonStart.CompetitionObj"/> assigned and that match the filter criteria.
        /// </summary>
        /// <param name="personStartFilter">Filter to select only some specific <see cref="PersonStart"/> elements.</param>
        /// <param name="personStartFilterParameter">Parameter for the personStartFilter</param>
        /// <returns>List of all <see cref="PersonStart"/> items which have <see cref="PersonStart.CompetitionObj"/> assigned</returns>
        public PersonStart[] GetItemsFiltered(PersonStartFilters personStartFilter = PersonStartFilters.None, object personStartFilterParameter = null)
            => _personService.GetAllPersonStarts(personStartFilter, personStartFilterParameter).Where(s => s.CompetitionObj != null).ToArray();
    }
}
