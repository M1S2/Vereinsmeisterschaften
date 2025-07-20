using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Helpers;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Documents
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

        List<int> PostfixNumbersSupported { get; }
    }
}
