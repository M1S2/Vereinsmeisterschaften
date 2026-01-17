using Vereinsmeisterschaften.Core.Helpers;

namespace Vereinsmeisterschaften.Core.Documents.DocumentPlaceholderResolvers
{
    /// <summary>
    /// Interface for a document placeholder resolver.
    /// </summary>
    /// <typeparam name="T">Data type for the items, used to resolve the placeholders.</typeparam>
    public interface IDocumentPlaceholderResolver<T>
    {
        /// <summary>
        /// Take the item and create <see cref="DocXPlaceholderHelper.TextPlaceholders"/>.
        /// </summary>
        /// <param name="item">Item to create placeholders from</param>
        /// <returns><see cref="DocXPlaceholderHelper.TextPlaceholders"/></returns>
        DocXPlaceholderHelper.TextPlaceholders ResolveTextPlaceholders(T item);

        /// <summary>
        /// Take the list of items and create <see cref="DocXPlaceholderHelper.TablePlaceholders"/>.
        /// </summary>
        /// <param name="items">List of items to create placeholders from</param>
        /// <returns><see cref="DocXPlaceholderHelper.TablePlaceholders"/></returns>
        DocXPlaceholderHelper.TablePlaceholders ResolveTablePlaceholders(IEnumerable<T> items);

        /// <summary>
        /// List of all placeholder keys that are supported by this resolver.
        /// </summary>
        List<string> SupportedPlaceholderKeys { get; }

        /// <summary>
        /// List of all postfix numbers that are supported by this resolver.
        /// </summary>
        List<int> PostfixNumbersSupported { get; }
    }
}
