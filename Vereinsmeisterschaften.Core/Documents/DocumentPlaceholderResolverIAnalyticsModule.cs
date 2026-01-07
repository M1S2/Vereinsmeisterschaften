using System.Diagnostics.Metrics;
using System.Reflection.Emit;
using Vereinsmeisterschaften.Core.Analytics;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Helpers;
using Vereinsmeisterschaften.Core.Models;
using Vereinsmeisterschaften.Core.Settings;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Vereinsmeisterschaften.Core.Documents
{
    /// <summary>
    /// Document placeholder resolver for the items of type <see cref="IAnalyticsModule"/>.
    /// </summary>
    public class DocumentPlaceholderResolverIAnalyticsModule : DocumentPlaceholderResolverBase<IAnalyticsModule>
    {
        private IEnumerable<IAnalyticsModule> _analyticsModules;

        /// <summary>
        /// Constructor for a document placeholder resolver.
        /// </summary>
        /// <param name="workspaceService"><see cref="IWorkspaceService"/> that can be used to access e.g. <see cref="Settings.WorkspaceSettings"/></param>
        /// <param name="analyticsModules">List of <see cref="IAnalyticsModule"/></param>
        public DocumentPlaceholderResolverIAnalyticsModule(IWorkspaceService workspaceService, IEnumerable<IAnalyticsModule> analyticsModules) : base(workspaceService)
        {
            _analyticsModules = analyticsModules;
        }

        /// <summary>
        /// Take the <see cref="IAnalyticsModule"/> item and create <see cref="DocXPlaceholderHelper.TextPlaceholders"/>.
        /// </summary>
        /// <param name="item">Item to create placeholders from</param>
        /// <returns><see cref="DocXPlaceholderHelper.TextPlaceholders"/></returns>
        public override DocXPlaceholderHelper.TextPlaceholders ResolveTextPlaceholders(IAnalyticsModule item)
            => item.CollectDocumentPlaceholderContents();

        /// <summary>
        /// <inheritdoc/>
        /// The list is collected from all available <see cref="IAnalyticsModule"/> objects.
        /// </summary>
        public override List<string> SupportedPlaceholderKeys
        {
            get
            {
                List<string> placeholderKeys = new List<string>();
                foreach (IAnalyticsModule item in _analyticsModules)
                {
                    if (item.SupportedDocumentPlaceholderKeys != null)
                    {
                        placeholderKeys.AddRange(item.SupportedDocumentPlaceholderKeys);
                    }
                }
                return placeholderKeys;
            }
        }

    }
}
