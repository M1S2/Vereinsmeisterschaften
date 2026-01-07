using Vereinsmeisterschaften.Core.Helpers;

namespace Vereinsmeisterschaften.Core.Analytics
{
    /// <summary>
    /// Interface for an analytics module
    /// </summary>
    public interface IAnalyticsModule
    {
        /// <summary>
        /// Flag indicating if the analytics module has data.
        /// </summary>
        bool AnalyticsAvailable { get; }

        /// <summary>
        /// Collect the values for all document placeholders that are supported by this <see cref="IAnalyticsModule"/>.
        /// If the <see cref="IAnalyticsModule"> doesn't support any placeholders for the document creation return <see langword="null"/>
        /// </summary>
        /// <returns><see cref="DocXPlaceholderHelper.TextPlaceholders"/> or <see langword="null"/> if no placeholders are supported</returns>
        DocXPlaceholderHelper.TextPlaceholders CollectDocumentPlaceholderContents();

        /// <summary>
        /// List of all document placeholder keys that are supported by this analytics module.
        /// If the <see cref="IAnalyticsModule"> doesn't support any placeholders for the document creation return <see langword="null"/>
        /// </summary>
        List<string> SupportedDocumentPlaceholderKeys { get; }
    }
}
