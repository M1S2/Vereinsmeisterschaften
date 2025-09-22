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

        /// <inheritdoc/>
        public override Enum ItemOrdering { get; set; } = ItemOrderingsCertificate.None;

        /// <inheritdoc/>
        public override IEnumerable<Enum> AvailableItemOrderings => Enum.GetValues(typeof(ItemOrderingsCertificate)).Cast<Enum>();

        /// <summary>
        /// Return a list of all <see cref="PersonStart"/> items which have <see cref="PersonStart.CompetitionObj"/> assigned.
        /// </summary>
        /// <returns>List of all <see cref="PersonStart"/> items which have <see cref="PersonStart.CompetitionObj"/> assigned</returns>
        public override PersonStart[] GetItems()
        {
            List<PersonStart> starts = _personService.GetAllPersonStarts().Where(s => s.CompetitionObj != null).ToList();
            switch (ItemOrdering)
            {
                case ItemOrderingsCertificate.ByNameAscending: starts = starts.OrderBy(s => s.PersonObj?.Name).ToList(); break;
                case ItemOrderingsCertificate.ByNameDescending: starts = starts.OrderByDescending(s => s.PersonObj?.Name).ToList(); break;
                case ItemOrderingsCertificate.ByFirstNameAscending: starts = starts.OrderBy(s => s.PersonObj?.FirstName).ToList(); break;
                case ItemOrderingsCertificate.ByFirstNameDescending: starts = starts.OrderBy(s => s.PersonObj?.FirstName).ToList(); break;
                case ItemOrderingsCertificate.ByCompetitionAscending: starts = starts.OrderBy(s => s.CompetitionObj?.ID).ToList(); break;
                case ItemOrderingsCertificate.ByCompetitionDescending: starts = starts.OrderBy(s => s.CompetitionObj?.ID).ToList(); break;
                default: break;
            }
            return starts.ToArray();
        }

        /// <summary>
        /// Return a list of all <see cref="PersonStart"/> items which have <see cref="PersonStart.CompetitionObj"/> assigned and that match the filter criteria.
        /// </summary>
        /// <param name="personStartFilter">Filter to select only some specific <see cref="PersonStart"/> elements.</param>
        /// <param name="personStartFilterParameter">Parameter for the personStartFilter</param>
        /// <returns>List of all <see cref="PersonStart"/> items which have <see cref="PersonStart.CompetitionObj"/> assigned</returns>
        public PersonStart[] GetItemsFiltered(PersonStartFilters personStartFilter = PersonStartFilters.None, object personStartFilterParameter = null)
        {
            List<PersonStart> starts = _personService.GetAllPersonStarts(personStartFilter, personStartFilterParameter).Where(s => s.CompetitionObj != null).ToList();
            switch (ItemOrdering)
            {
                case ItemOrderingsCertificate.ByNameAscending: starts = starts.OrderBy(s => s.PersonObj?.Name).ToList(); break;
                case ItemOrderingsCertificate.ByNameDescending: starts = starts.OrderByDescending(s => s.PersonObj?.Name).ToList(); break;
                case ItemOrderingsCertificate.ByFirstNameAscending: starts = starts.OrderBy(s => s.PersonObj?.FirstName).ToList(); break;
                case ItemOrderingsCertificate.ByFirstNameDescending: starts = starts.OrderBy(s => s.PersonObj?.FirstName).ToList(); break;
                case ItemOrderingsCertificate.ByCompetitionAscending: starts = starts.OrderBy(s => s.CompetitionObj?.ID).ToList(); break;
                case ItemOrderingsCertificate.ByCompetitionDescending: starts = starts.OrderBy(s => s.CompetitionObj?.ID).ToList(); break;
                default: break;
            }
            return starts.ToArray();
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Enum with all available orderings for certificates.
        /// </summary>
        public enum ItemOrderingsCertificate
        {
            None,
            ByNameAscending,
            ByNameDescending,
            ByFirstNameAscending,
            ByFirstNameDescending,
            ByCompetitionAscending,
            ByCompetitionDescending
        }
    }

}
