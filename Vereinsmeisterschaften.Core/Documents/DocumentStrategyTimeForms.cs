using Vereinsmeisterschaften.Core.Contracts.Services;
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
        /// <param name="serviceProvider"><see cref="IServiceProvider"/> used to get the correct <see cref="IDocumentPlaceholderResolver{T}"/></param>
        public DocumentStrategyTimeForms(IRaceService raceService, IWorkspaceService workspaceService, IServiceProvider serviceProvider) : base(workspaceService, serviceProvider)
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
        {
            // Return a list of all races but only with the active starts
            List<Race> raceClones = new List<Race>();
            foreach(Race originalRace in _raceService.PersistedRacesVariant?.Races)
            {
                Race newRace = new Race(originalRace, true);
                newRace.Starts = new System.Collections.ObjectModel.ObservableCollection<PersonStart>(newRace.Starts.Where(s => s.IsActive));
                raceClones.Add(newRace);
            }
            return raceClones.ToArray();
        }
    }
}
