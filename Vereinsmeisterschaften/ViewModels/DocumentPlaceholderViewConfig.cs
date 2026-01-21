using Vereinsmeisterschaften.Core.Models;

namespace Vereinsmeisterschaften.ViewModels
{
    /// <summary>
    /// Configuration for document placeholders used in the UI.
    /// </summary>
    public class DocumentPlaceholderViewConfig
    {
        /// <summary>
        /// Group for the placeholder
        /// </summary>
        public string Group { get; set; }
        /// <summary>
        /// Key for the placeholder, used to identify it
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// Name of the placeholder, displayed in the UI
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Info text for the placeholder, providing additional context or instructions
        /// </summary>
        public string Info { get; set; }
        /// <summary>
        /// Placeholders that can be used in the document, formatted as a string
        /// </summary>
        public string Placeholders { get; set; }
        /// <summary>
        /// Indicates whether this placeholder is supported for the document type
        /// </summary>
        public Dictionary<DocumentCreationTypes, bool> IsSupportedForDocumentType { get; set; }
        /// <summary>
        /// Indicates whether this placeholder is supported for text placeholders
        /// </summary>
        public Dictionary<DocumentCreationTypes, bool> IsSupportedForTextPlaceholders { get; set; }
        /// <summary>
        /// Indicates whether this placeholder is supported for table placeholders
        /// </summary>
        public Dictionary<DocumentCreationTypes, bool> IsSupportedForTablePlaceholders { get; set; }
        /// <summary>
        /// Postfix numbers that can be used for the placeholder, formatted as a string
        /// </summary>
        public Dictionary<DocumentCreationTypes, string> PostfixNumbersSupportedForDocumentType { get; set; }
    }
}
