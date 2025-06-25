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
    /// Base class for document strategies that define how to create and collect data for different types of documents.
    /// </summary>
    /// <typeparam name="TData">Data type for the items handled by this <see cref="IDocumentStrategy"/></typeparam>
    public abstract class DocumentStrategyBase<TData> : IDocumentStrategy
    {
        protected readonly IWorkspaceService _workspaceService;

        /// <summary>
        /// Constructor for the document strategy base class.
        /// </summary>
        /// <param name="workspaceService"><see cref="IWorkspaceService"/> that can be used to e.g. get the workspace root path</param>
        protected DocumentStrategyBase(IWorkspaceService workspaceService)
        {
            _workspaceService = workspaceService;
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
    }
}
