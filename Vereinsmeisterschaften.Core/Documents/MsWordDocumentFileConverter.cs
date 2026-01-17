using System.IO.Compression;
using System.Xml.Linq;
using Microsoft.Win32;

namespace Vereinsmeisterschaften.Core.Documents
{
    /// <summary>
    /// Class that can be used to convert e.g. .docx files to .pdf files via Microsoft word.
    /// </summary>
    public class MsWordDocumentFileConverter : IDocumentFileConverter
    {
        /// <summary>
        /// Check if Microsoft Word is available.
        /// This is done by trying to read the current version from the registry.
        /// </summary>
        /// <see href="https://stackoverflow.com/questions/18820434/quick-command-or-batch-script-to-determine-windows-and-office-version"/>
        public bool IsAvailable
        {
            get
            {
                object regValue = Registry.GetValue(@"HKEY_CLASSES_ROOT\Word.Application\CurVer", "", null);        // Format will be e.g. "Word.Application.14"
                return regValue != null;
            }
        }

        /// <summary>
        /// Convert a document using Word
        /// </summary>
        /// <param name="inputFile">Input document to convert. This must be .docx </param>
        /// <param name="outputFile">Output document. This must be .pdf or .docx</param>
        /// <exception cref="Exception">Thrown when the MS Word application isn't found or the conversion fails</exception>
        public bool Convert(string inputFile, string outputFile)
        {
            if (!IsAvailable) { return false; }
            
            string convertedFile = "";

            //Create tmp folder
            string tmpFolder = Path.Combine(Path.GetDirectoryName(outputFile), "MSWordDocumentConverter_" + Guid.NewGuid().ToString().Substring(0, 10));
            if (!Directory.Exists(tmpFolder))
            {
                Directory.CreateDirectory(tmpFolder);
            }

            dynamic wordApp = null;
            dynamic document = null;

            try
            {
                if (inputFile.EndsWith(".docx") && outputFile.EndsWith(".docx"))
                {
                    // No conversion necessary. Only copy the inputFile to the outputFile.
                    File.Copy(inputFile, outputFile, true);
                    return true;
                }
                else if (inputFile.EndsWith(".docx") && outputFile.EndsWith(".pdf"))
                {
                    convertedFile = Path.Combine(tmpFolder, Path.GetFileNameWithoutExtension(inputFile) + ".pdf");

                    // Microsoft.Office.Interop.Word doesn't work correct. So use a dynamic Instance as workaround.
                    Type t = Type.GetTypeFromProgID("Word.Application", true);
                    wordApp = Activator.CreateInstance(t);
                    wordApp.Visible = false;
                    document = wordApp.Documents.Open(inputFile);
                    document.ExportAsFixedFormat(convertedFile, 17); // 17 = WdExportFormat.wdExportFormatPDF

                    if (File.Exists(outputFile))
                    {
                        File.Delete(outputFile);
                    }
                    if (File.Exists(convertedFile))
                    {
                        File.Move(convertedFile, outputFile);
                    }
                    return true;
                }
                else
                {
                    // conversion of these file types isn't supported.
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(Properties.Resources.Error_MSWord_ConversionFailed, ex.Message));
            }
            finally
            {
                Directory.Delete(tmpFolder, true);
                document?.Close(false);
                wordApp?.Quit(false);
            }
        }

        /// <summary>
        /// Check if the given .docx file was created by Microsoft Word.
        /// </summary>
        /// <param name="docxFile">File to check</param>
        /// <returns><see langword="true"/> when the .docx file was created by Microsoft word; otherwise <see langword="false"/></returns>
        public bool IsDocxCreateWithThisConverter(string docxFile)
        {
            ZipArchive zip = ZipFile.OpenRead(docxFile);
            ZipArchiveEntry zipEntry = zip?.GetEntry("docProps/app.xml");
            if (zipEntry == null) { return false; }

            Stream zipEntryStream = zipEntry.Open();
            XDocument zipEntryXml = XDocument.Load(zipEntryStream);
            XElement zipEntryAppElement = zipEntryXml.Root?.Elements().FirstOrDefault(e => e.Name.LocalName == "Application");
            string appName = zipEntryAppElement?.Value;
            return appName.Contains("Microsoft Office Word");
        }
    }
}
