using Vereinsmeisterschaften.Core.Helpers;
using Vereinsmeisterschaften.Core.Models;
using Vereinsmeisterschaften.Core.Documents.DocumentPlaceholderResolvers;

namespace Vereinsmeisterschaften.Core.Documents.DocumentStrategies
{
    /// <summary>
    /// Interface for document strategies that define how to create and collect data for different types of documents.
    /// </summary>
    public interface IDocumentStrategy
    {
        /// <summary>
        /// Gets the type of document this strategy creates.
        /// </summary>
        DocumentCreationTypes DocumentType { get; }

        /// <summary>
        /// Gets the path to the template used for creating the document.
        /// </summary>
        string TemplatePath { get; }

        /// <summary>
        /// Use this string instead of the template file name postfix during document creation.
        /// e.g. Template = "Certificate_Template.docx" and TemplateFileNamePostfix = "_Template"
        /// - with this Property = "" the output file becomes "Certificate.docx"
        /// - with this Property = "_WK1" the output file becomes "Certificate_WK1.docx"
        /// - with this Property = "_Name" the output file becomes "Certificate_Name.docx"
        /// </summary>
        string TemplateFileNamePostfixReplaceStr { get; }

        /// <summary>
        /// Gets a value indicating whether this strategy supports creating multiple pages in the document.
        /// Each element returned by <see cref="GetItems"/> will be used for a separate page if this is true.
        /// </summary>
        bool CreateMultiplePages { get; }

        /// <summary>
        /// Gets the type of items that this strategy will handle.
        /// </summary>
        Type ItemType { get; }

        /// <summary>
        /// Retrieves the items that will be used to create the document.
        /// </summary>
        /// <returns>List with items</returns>
        object[] GetItems();

        /// <summary>
        /// Indicating if <see cref="DocXPlaceholderHelper.TextPlaceholders"/> are supported.
        /// If <see cref="CreateMultiplePages"/> is true, this will also return true, because each element of the <see cref="GetItems"/> is used for one page.
        /// </summary>
        bool SupportTextPlaceholders { get; }

        /// <summary>
        /// Indicating if <see cref="DocXPlaceholderHelper.TablePlaceholders"/> are supported.
        /// If <see cref="CreateMultiplePages"/> is false, this will return true, because all element of the <see cref="GetItems"/> are used for one page.
        /// </summary>
        bool SupportTablePlaceholders { get; }

        /// <summary>
        /// List of placeholder keys that are supported by this strategy.
        /// </summary>
        List<string> SupportedPlaceholderKeys { get; }

        /// <summary>
        /// List with placeholder keys that are always supported, no matter what the <see cref="IDocumentPlaceholderResolver{T}"/> supports.
        /// </summary>
        List<string> AlwaysSupportedPlaceholderKeys { get; }

        /// <summary>
        /// Number of postfix numbers that are supported for the placeholders.
        /// If this is e.g. 2, the placeholders %Name%, %Name1% and %Name2% will be allowed
        /// If 0, only the placeholder without postfix will be allowed, e.g. %Name% only.
        /// </summary>
        List<int> PostfixNumbersSupported { get; }

        /// <summary>
        /// Resolve text placeholders for the given item.
        /// </summary>
        /// <param name="item">Item to generate <see cref="DocXPlaceholderHelper.TextPlaceholders"/> from</param>
        /// <returns><see cref="DocXPlaceholderHelper.TextPlaceholders"/></returns>
        DocXPlaceholderHelper.TextPlaceholders ResolveTextPlaceholders(object item = null);

        /// <summary>
        /// Resolve table placeholders for the given items.
        /// </summary>
        /// <param name="items">List of items to generate <see cref="DocXPlaceholderHelper.TablePlaceholders"/> from</param>
        /// <returns><see cref="DocXPlaceholderHelper.TablePlaceholders"/></returns>
        DocXPlaceholderHelper.TablePlaceholders ResolveTablePlaceholders(object[] items);

        /// <summary>
        /// Current ordering for the items. If no ordering is supported, this will be <see langword="null"/>.
        /// </summary>
        Enum ItemOrdering { get; set; }

        /// <summary>
        /// Array with all available orderings for the items. If no ordering is supported, this will be <see langword="null"/>.
        /// </summary>
        IEnumerable<Enum> AvailableItemOrderings { get; }

        /// <summary>
        /// Array with all available filters for the items. If no filtering is supported, this will be <see langword="null"/>.
        /// </summary>
        IEnumerable<Enum> AvailableItemFilters { get; }

        /// <summary>
        /// Current filter for the items. If no filtering is supported, this will be <see langword="null"/>.
        /// </summary>
        Enum ItemFilter { get; set; }

        /// <summary>
        /// Filter parameter that can be used together with <see cref="ItemFiltering"/>. The type and usage of this parameter depends on the selected <see cref="ItemFilter"/>.
        /// </summary>
        object ItemFilterParameter { get; set; }
    }
}
