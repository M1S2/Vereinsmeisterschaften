namespace Vereinsmeisterschaften.Core.Documents.DocumentFileConverters
{
    /// <summary>
    /// Interface for a class used to select the correct <see cref="IDocumentFileConverter"/>
    /// </summary>
    public interface IDocumentFileConverterSelector
    {
        /// <summary>
        /// Select the correct <see cref="IDocumentFileConverter"/> depending on the .docx file creator and the available converter applications.
        /// </summary>
        /// <param name="docxFile">.docx file used to select the correct converter</param>
        /// <param name="ignoreConverters">List with converters that are ignored</param>
        /// <returns><see cref="IDocumentFileConverter"/> or <see langword="null"/> when no converter is available or all converters are in ignore list</returns>
        IDocumentFileConverter GetConverter(string docxFile, List<IDocumentFileConverter> ignoreConverters);
    }
}
