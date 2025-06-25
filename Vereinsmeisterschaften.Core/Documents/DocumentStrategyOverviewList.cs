using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Helpers;
using Vereinsmeisterschaften.Core.Models;
using Vereinsmeisterschaften.Core.Services;
using Vereinsmeisterschaften.Core.Settings;

namespace Vereinsmeisterschaften.Core.Documents
{
    /// <summary>
    /// Document strategy for creating an overview list.
    /// </summary>
    public class DocumentStrategyOverviewList : DocumentStrategyBase<PersonStart>
    {
        private readonly IPersonService _personService;

        /// <summary>
        /// Constructor for the document strategy for an overview list.
        /// </summary>
        /// <param name="personService"><see cref="IPersonService"/> that is used to get the <see cref="PersonStart"/> items</param>
        /// <param name="workspaceService"><see cref="IWorkspaceService"/> that can be used to e.g. get the workspace root path</param>
        public DocumentStrategyOverviewList(IPersonService personService, IWorkspaceService workspaceService) : base(workspaceService)
        {
            _personService = personService;
        }

        /// <inheritdoc/>
        public override DocumentCreationTypes DocumentType => DocumentCreationTypes.OverviewList;

        /// <inheritdoc/>
        public override string TemplatePath => DocumentService.GetDocumentPathAbsolute(WorkspaceSettings.SETTING_DOCUMENT_CREATION_OVERVIEW_LIST_TEMPLATE_PATH, _workspaceService);

        /// <summary>
        /// This always returns <see langword="false"/>
        /// </summary>
        public override bool CreateMultiplePages => false;

        /// <summary>
        /// Return a list of all <see cref="PersonStart"/> items.
        /// </summary>
        /// <returns>List of all <see cref="PersonStart"/> items.</returns>
        public override PersonStart[] GetItems()
            => _personService.GetAllPersonStarts().ToArray();
    }
}
