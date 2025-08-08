using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Helpers;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Documents
{
    /// <summary>
    /// Base class for document strategies that define how to create and collect data for different types of documents.
    /// </summary>
    /// <typeparam name="TData">Data type for the items handled by this <see cref="IDocumentStrategy"/></typeparam>
    public abstract class DocumentStrategyBase<TData> : IDocumentStrategy
    {
        protected readonly IWorkspaceService _workspaceService;
        protected readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Constructor for the document strategy base class.
        /// </summary>
        /// <param name="workspaceService"><see cref="IWorkspaceService"/> that can be used to e.g. get the workspace root path</param>
        /// <param name="serviceProvider"><see cref="IServiceProvider"/> used to get the correct <see cref="IDocumentPlaceholderResolver{T}"/></param>
        protected DocumentStrategyBase(IWorkspaceService workspaceService, IServiceProvider serviceProvider)
        {
            _workspaceService = workspaceService;
            _serviceProvider = serviceProvider;

            Type resolverType = typeof(IDocumentPlaceholderResolver<>).MakeGenericType(ItemType);
            PlaceholderResolver = _serviceProvider.GetService(resolverType) as IDocumentPlaceholderResolver<TData>;
            if (PlaceholderResolver is null) throw new InvalidOperationException($"No Resolver found for type {ItemType.Name}.");
        }

        /// <inheritdoc/>
        public abstract DocumentCreationTypes DocumentType { get; }

        /// <inheritdoc/>
        public abstract string TemplatePath { get; }

        /// <inheritdoc/>
        public abstract bool CreateMultiplePages { get; }

        /// <inheritdoc/>
        public abstract object[] GetItems();

        /// <summary>
        /// Gets the type of items that this strategy will handle.
        /// This is retrieved from the typeparam <see cref="TData"/>
        /// </summary>
        public Type ItemType => typeof(TData);

        /// <inheritdoc/>
        public virtual bool SupportTextPlaceholders => CreateMultiplePages;

        /// <inheritdoc/>
        public virtual bool SupportTablePlaceholders => !CreateMultiplePages;

        /// <summary>
        /// Placeholder resolver used to resolve placeholders in the document.
        /// </summary>
        public IDocumentPlaceholderResolver<TData> PlaceholderResolver { get; }

        /// <summary>
        /// List of all placeholder keys that are supported by the <see cref="PlaceholderResolver"/>.
        /// The key for the competition year (<see cref="Placeholders.PLACEHOLDER_KEY_COMPETITION_YEAR"/>) is always supported.
        /// </summary>
        public List<string> SupportedPlaceholderKeys => new List<string>(PlaceholderResolver.SupportedPlaceholderKeys) { Placeholders.PLACEHOLDER_KEY_COMPETITION_YEAR };

        /// <inheritdoc/>
        public List<int> PostfixNumbersSupported => PlaceholderResolver.PostfixNumbersSupported ?? Enumerable.Repeat(0, PlaceholderResolver?.SupportedPlaceholderKeys?.Count ?? 0).ToList();

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <inheritdoc/>
        public DocXPlaceholderHelper.TextPlaceholders ResolveTextPlaceholders(object item = null)
        {
            if (!SupportTextPlaceholders || item == null) { return null; }

            return PlaceholderResolver.ResolveTextPlaceholders((TData)item);
        }

        /// <inheritdoc/>
        public DocXPlaceholderHelper.TablePlaceholders ResolveTablePlaceholders(object[] items)
        {
            if (!SupportTablePlaceholders || items == null) { return null; }

            return PlaceholderResolver.ResolveTablePlaceholders(items.Cast<TData>());
        }
    }
}
