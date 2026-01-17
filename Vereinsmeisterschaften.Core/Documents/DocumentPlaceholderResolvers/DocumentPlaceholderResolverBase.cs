using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Helpers;

namespace Vereinsmeisterschaften.Core.Documents.DocumentPlaceholderResolvers
{
    /// <summary>
    /// Base class for a document placeholder resolver.
    /// </summary>
    /// <typeparam name="T">Data type for the items, used to resolve the placeholders.</typeparam>
    public abstract class DocumentPlaceholderResolverBase<T> : IDocumentPlaceholderResolver<T>
    {
        protected IWorkspaceService _workspaceService;

        /// <summary>
        /// Base constructor for a document placeholder resolver.
        /// </summary>
        /// <param name="workspaceService"><see cref="IWorkspaceService"/> that can be used to access e.g. <see cref="Settings.WorkspaceSettings"/></param>
        public DocumentPlaceholderResolverBase(IWorkspaceService workspaceService)
        {
            _workspaceService = workspaceService;
        }

        /// <summary>
        /// Take the <see cref="T"/> item and create <see cref="DocXPlaceholderHelper.TextPlaceholders"/>.
        /// </summary>
        /// <param name="item">Item to create placeholders from</param>
        /// <param name="workspaceService"><see cref="IWorkspaceService"/> that can be used to access e.g. <see cref="Settings.WorkspaceSettings"/></param>
        /// <returns><see cref="DocXPlaceholderHelper.TextPlaceholders"/></returns>
        public abstract DocXPlaceholderHelper.TextPlaceholders ResolveTextPlaceholders(T item);

        /// <summary>
        /// Take the list of <see cref="T"/> items and create <see cref="DocXPlaceholderHelper.TablePlaceholders"/>.
        /// </summary>
        /// <param name="items">List of items to create placeholders from</param>
        /// <param name="workspaceService"><see cref="IWorkspaceService"/> that can be used to access e.g. <see cref="Settings.WorkspaceSettings"/></param>
        /// <returns><see cref="DocXPlaceholderHelper.TablePlaceholders"/></returns>
        public virtual DocXPlaceholderHelper.TablePlaceholders ResolveTablePlaceholders(IEnumerable<T> items)
        {
            List<DocXPlaceholderHelper.TextPlaceholders> textPlaceholdersList = new List<DocXPlaceholderHelper.TextPlaceholders>();
            foreach (T item in items)
            {
                DocXPlaceholderHelper.TextPlaceholders textPlaceholders = ResolveTextPlaceholders(item);
                textPlaceholdersList.Add(textPlaceholders);
            }
            return DocXPlaceholderHelper.ConvertTextToTablePlaceholders(textPlaceholdersList);
        }

        /// <inheritdoc/>
        public abstract List<string> SupportedPlaceholderKeys { get; }

        /// <inheritdoc/>
        public virtual List<int> PostfixNumbersSupported { get; } = null;
    }
}
