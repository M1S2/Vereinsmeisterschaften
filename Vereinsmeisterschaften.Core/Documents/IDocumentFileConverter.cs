
namespace Vereinsmeisterschaften.Core.Documents
{
    /// <summary>
    /// Interface for a class that can be used to convert e.g. .docx files to .pdf files
    /// </summary>
    public interface IDocumentFileConverter
    {
        /// <summary>
        /// Check if the converter application is available.
        /// </summary>
        bool IsAvailable { get; }

        /// <summary>
        /// Convert a document.
        /// </summary>
        /// <param name="inputFile">Input document to convert. Usually it's a .docx </param>
        /// <param name="outputFile">Output document. Usually it's a .pdf</param>
        /// <returns><see langword="true"/> when the conversion was successful; otherwise <see langword="false"/></returns>
        bool Convert(string inputFile, string outputFile);

        /// <summary>
        /// Check if the given .docx file was created by this converter application.
        /// </summary>
        /// <param name="docxFile">File to check</param>
        /// <returns><see langword="true"/> when the .docx file was created by this converter application; otherwise <see langword="false"/></returns>
        bool IsDocxCreateWithThisConverter(string docxFile);
    }
}
