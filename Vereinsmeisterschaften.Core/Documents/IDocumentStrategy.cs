﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vereinsmeisterschaften.Core.Helpers;
using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.Core.Documents
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
    }
}
