using CommunityToolkit.Mvvm.ComponentModel;
using Vereinsmeisterschaften.Core.Documents.DocumentStrategies;

namespace Vereinsmeisterschaften.Models
{
    /// <summary>
    /// Class combining configuration infos used while document creation.
    /// </summary>
    public partial class DocumentCreationConfig : ObservableObject
    {
        /// <summary>
        /// <see cref="IDocumentStrategy"/> used by the document creation
        /// </summary>
        [ObservableProperty]
        private IDocumentStrategy _documentStrategy;

        /// <summary>
        /// Available item orderings
        /// </summary>
        [ObservableProperty]
        private IEnumerable<Enum> _availableItemOrderings;

        /// <summary>
        /// Current item ordering
        /// </summary>
        [ObservableProperty]
        private Enum _currentItemOrdering;

        /// <summary>
        /// Available item filters
        /// </summary>
        [ObservableProperty]
        private IEnumerable<Enum> _availableItemFilters;

        /// <summary>
        /// Current item filter
        /// </summary>
        [ObservableProperty]
        private Enum _currentItemFilter;

        /// <summary>
        /// Current item filter parameter
        /// </summary>
        [ObservableProperty]
        private object _currentItemFilterParameter;
    }
}
