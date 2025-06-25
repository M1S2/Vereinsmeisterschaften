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
using Xceed.Document.NET;

namespace Vereinsmeisterschaften.Core.Documents
{
    /// <summary>
    /// Document strategy for creating a result list.
    /// </summary>
    public class DocumentStrategyResultList : DocumentStrategyBase<Person>
    {
        private readonly IScoreService _scoreService;

        /// <summary>
        /// Constructor for the document strategy for a result list.
        /// </summary>
        /// <param name="scoreService"><see cref="IScoreService"/> that is used to get the <see cref="Person"/> items</param>
        /// <param name="workspaceService"><see cref="IWorkspaceService"/> that can be used to e.g. get the workspace root path</param>
        public DocumentStrategyResultList(IScoreService scoreService, IWorkspaceService workspaceService) : base(workspaceService)
        {
            _scoreService = scoreService;
        }

        /// <inheritdoc/>
        public override DocumentCreationTypes DocumentType => DocumentCreationTypes.ResultList;

        /// <inheritdoc/>
        public override string TemplatePath => DocumentService.GetDocumentPathAbsolute(WorkspaceSettings.SETTING_DOCUMENT_CREATION_RESULT_LIST_TEMPLATE_PATH, _workspaceService);

        /// <summary>
        /// This always returns <see langword="false"/>
        /// </summary>
        public override bool CreateMultiplePages => false;

        /// <summary>
        /// Return a list of all <see cref="Person"/> items sorted by their scores.
        /// </summary>
        /// <returns>List of all <see cref="Person"/> items sorted by their scores.</returns>
        public override Person[] GetItems()
        {
            List<Person> sortedPersons = _scoreService.GetPersonsSortedByScore(ResultTypes.Overall);
            _scoreService.UpdateResultListPlacesForAllPersons();
            return sortedPersons.ToArray();
        }
    }
}
