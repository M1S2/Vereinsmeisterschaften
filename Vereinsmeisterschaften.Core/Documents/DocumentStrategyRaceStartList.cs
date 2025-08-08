using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;
using Vereinsmeisterschaften.Core.Services;
using Vereinsmeisterschaften.Core.Settings;

namespace Vereinsmeisterschaften.Core.Documents
{
    /// <summary>
    /// Document strategy for creating a race start list.
    /// </summary>
    public class DocumentStrategyRaceStartList : DocumentStrategyBase<Race>
    {
        private readonly IRaceService _raceService;

        /// <summary>
        /// Constructor for the document strategy for a race start list.
        /// </summary>
        /// <param name="raceService"><see cref="IRaceService"/> that is used to get the <see cref="Race"/> items</param>
        /// <param name="workspaceService"><see cref="IWorkspaceService"/> that can be used to e.g. get the workspace root path</param>
        /// <param name="serviceProvider"><see cref="IServiceProvider"/> used to get the correct <see cref="IDocumentPlaceholderResolver{T}"/></param>
        public DocumentStrategyRaceStartList(IRaceService raceService, IWorkspaceService workspaceService, IServiceProvider serviceProvider) : base(workspaceService, serviceProvider)
        {
            _raceService = raceService;
        }

        /// <inheritdoc/>
        public override DocumentCreationTypes DocumentType => DocumentCreationTypes.RaceStartList;

        /// <inheritdoc/>
        public override string TemplatePath => DocumentService.GetDocumentPathAbsolute(WorkspaceSettings.SETTING_DOCUMENT_CREATION_RACE_START_LIST_TEMPLATE_PATH, _workspaceService);

        /// <summary>
        /// This always returns <see langword="false"/>
        /// </summary>
        public override bool CreateMultiplePages => false;

        /// <summary>
        /// Return a list of all <see cref="Race"/> items of the <see cref="RaceService.PersistedRacesVariant"/>
        /// </summary>
        /// <returns>List of all <see cref="Race"/> items of the <see cref="RaceService.PersistedRacesVariant"/></returns>
        public override Race[] GetItems()
            => _raceService.PersistedRacesVariant?.Races?.ToArray();
    }
}
