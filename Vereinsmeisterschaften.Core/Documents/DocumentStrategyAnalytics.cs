using Vereinsmeisterschaften.Core.Analytics;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;
using Vereinsmeisterschaften.Core.Services;
using Vereinsmeisterschaften.Core.Settings;

namespace Vereinsmeisterschaften.Core.Documents
{
    /// <summary>
    /// Document strategy for creating an analytics overview.
    /// </summary>
    public class DocumentStrategyAnalytics : DocumentStrategyBase<IAnalyticsModule>
    {
        private readonly IEnumerable<IAnalyticsModule> _analyticsModules;

        /// <summary>
        /// Constructor for the document strategy for an overview list.
        /// </summary>
        /// <param name="analyticsModules">List of <see cref="IAnalyticsModule"/></param>
        /// <param name="workspaceService"><see cref="IWorkspaceService"/> that can be used to e.g. get the workspace root path</param>
        /// <param name="serviceProvider"><see cref="IServiceProvider"/> used to get the correct <see cref="IDocumentPlaceholderResolver{T}"/></param>
        public DocumentStrategyAnalytics(IEnumerable<IAnalyticsModule> analyticsModules, IWorkspaceService workspaceService, IServiceProvider serviceProvider) : base(workspaceService, serviceProvider)
        {
            _analyticsModules = analyticsModules;
        }

        /// <inheritdoc/>
        public override DocumentCreationTypes DocumentType => DocumentCreationTypes.Analytics;

        /// <inheritdoc/>
        public override string TemplatePath => DocumentService.GetDocumentPathAbsolute(WorkspaceSettings.SETTING_DOCUMENT_CREATION_ANALYTICS_TEMPLATE_PATH, _workspaceService);

        /// <summary>
        /// This always returns <see langword="false"/>
        /// </summary>
        public override bool CreateMultiplePages => false;

        /// <inheritdoc/>
        public override bool SupportTextPlaceholders => true;

        /// <inheritdoc/>
        public override bool SupportTablePlaceholders => false;

        /// <summary>
        /// Return a list of all <see cref="IAnalyticsModule"/> items.
        /// </summary>
        /// <returns>List of all <see cref="IAnalyticsModule"/> items.</returns>
        public override IAnalyticsModule[] GetItems()
        {
            return _analyticsModules.ToArray();
        }
    }
    
}
