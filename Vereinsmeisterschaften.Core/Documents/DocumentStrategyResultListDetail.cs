using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;
using Vereinsmeisterschaften.Core.Services;
using Vereinsmeisterschaften.Core.Settings;

namespace Vereinsmeisterschaften.Core.Documents
{
    /// <summary>
    /// Document strategy for creating a result list with details.
    /// </summary>
    public class DocumentStrategyResultListDetail : DocumentStrategyBase<Person>
    {
        private readonly IPersonService _personService;

        /// <summary>
        /// Constructor for the document strategy for a result list with details.
        /// </summary>
        /// <param name="personService"><see cref="IPersonService"/> that is used to get the <see cref="Person"/> items</param>
        /// <param name="workspaceService"><see cref="IWorkspaceService"/> that can be used to e.g. get the workspace root path</param>
        /// <param name="serviceProvider"><see cref="IServiceProvider"/> used to get the correct <see cref="IDocumentPlaceholderResolver{T}"/></param>
        public DocumentStrategyResultListDetail(IPersonService personService, IWorkspaceService workspaceService, IServiceProvider serviceProvider) : base(workspaceService, serviceProvider)
        {
            _personService = personService;
        }

        /// <inheritdoc/>
        public override DocumentCreationTypes DocumentType => DocumentCreationTypes.ResultListDetail;

        /// <inheritdoc/>
        public override string TemplatePath => DocumentService.GetDocumentPathAbsolute(WorkspaceSettings.SETTING_DOCUMENT_CREATION_RESULT_LIST_DETAIL_TEMPLATE_PATH, _workspaceService);

        /// <summary>
        /// This always returns <see langword="false"/>
        /// </summary>
        public override bool CreateMultiplePages => false;

        /// <inheritdoc/>
        public override Enum ItemOrdering { get; set; } = ItemOrderingsResultListDetail.None;

        /// <inheritdoc/>
        public override IEnumerable<Enum> AvailableItemOrderings => Enum.GetValues(typeof(ItemOrderingsResultListDetail)).Cast<Enum>();

        /// <summary>
        /// Return a list of all <see cref="Person"/> items.
        /// </summary>
        /// <returns>List of all <see cref="Person"/> items</returns>
        public override Person[] GetItems()
        {
            List<Person> persons = _personService.GetPersons().ToList();
            switch (ItemOrdering)
            {
                case ItemOrderingsResultListDetail.ByNameAscending: persons = persons.OrderBy(p => p?.Name).ToList(); break;
                case ItemOrderingsResultListDetail.ByNameDescending: persons = persons.OrderByDescending(p => p?.Name).ToList(); break;
                case ItemOrderingsResultListDetail.ByFirstNameAscending: persons = persons.OrderBy(p => p?.FirstName).ToList(); break;
                case ItemOrderingsResultListDetail.ByFirstNameDescending: persons = persons.OrderByDescending(p => p?.FirstName).ToList(); break;
                case ItemOrderingsResultListDetail.ByBestPlaceAscending: persons = persons.OrderBy(p => p?.ResultListPlace).ToList(); break;
                case ItemOrderingsResultListDetail.ByBestPlaceDescending: persons = persons.OrderByDescending(p => p?.ResultListPlace).ToList(); break;
                default: break;
            }
            return persons.ToArray();
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Enum with all available orderings for result lists with details.
        /// </summary>
        public enum ItemOrderingsResultListDetail
        {
            None,
            ByNameAscending,
            ByNameDescending,
            ByFirstNameAscending,
            ByFirstNameDescending,
            ByBestPlaceAscending,
            ByBestPlaceDescending
        }
    }
    
}
