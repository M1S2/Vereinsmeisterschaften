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
    /// Document strategy for creating time forms.
    /// </summary>
    public class DocumentStrategyTimeForms : DocumentStrategyBase<Race>
    {
        private readonly IRaceService _raceService;

        /// <summary>
        /// Constructor for the document strategy for time forms.
        /// </summary>
        /// <param name="raceService"><see cref="IRaceService"/> that is used to get the <see cref="Race"/> items</param>
        /// <param name="workspaceService"><see cref="IWorkspaceService"/> that can be used to e.g. get the workspace root path</param>
        public DocumentStrategyTimeForms(IRaceService raceService, IWorkspaceService workspaceService) : base(workspaceService)
        {
            _raceService = raceService;
        }

        /// <inheritdoc/>
        public override DocumentCreationTypes DocumentType => DocumentCreationTypes.TimeForms;

        /// <inheritdoc/>
        public override string TemplatePath => DocumentService.GetDocumentPathAbsolute(WorkspaceSettings.SETTING_DOCUMENT_CREATION_TIME_FORMS_TEMPLATE_PATH, _workspaceService);

        /// <summary>
        /// This always returns <see langword="true"/>
        /// </summary>
        public override bool CreateMultiplePages => true;

        /// <summary>
        /// Return a list of all <see cref="Race"/> items.
        /// </summary>
        /// <returns>List of all <see cref="Race"/> items</returns>
        public override Race[] GetItems()
            => _raceService.PersistedRacesVariant?.Races?.ToArray();
    }
}
