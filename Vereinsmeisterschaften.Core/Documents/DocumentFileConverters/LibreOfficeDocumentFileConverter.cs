using Microsoft.Win32;
using System.Diagnostics;
using System.IO.Compression;
using System.Xml.Linq;

namespace Vereinsmeisterschaften.Core.Documents.DocumentFileConverters
{
    /// <summary>
    /// Class that can be used to convert e.g. .docx files to .pdf files via Libre Office.
    /// </summary>
    public class LibreOfficeDocumentFileConverter : IDocumentFileConverter
    {
        private string _libreOfficePath;

        /// <summary>
        /// Check if Libre Office is available.
        /// If Libre Office is installed, the path is get from the registry.
        /// Otherwise the soffice.exe file must be located at one of these paths:
        /// - C:\Program Files\LibreOffice\program\soffice.exe
        /// - C:\Program Files (x86)\LibreOffice\program\soffice.exe
        /// </summary>
        public bool IsAvailable
        {
            get
            {
                string tmpFile = findLibreOfficePath();
                if(tmpFile != null)
                {
                    _libreOfficePath = tmpFile;
                    return true;
                }
                return false;
            }            
        }

        /// <summary>
        /// Check the registry for a installation of libre office. If not found check some default directories.
        /// </summary>
        /// <returns>Filename of the soffice.exe when found; otherwise <see langword="null"/></returns>
        private string? findLibreOfficePath()
        {
            object regValue = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\LibreOffice\UNO\InstallPath", "", null);
            if(regValue == null)
            {
                regValue = Registry.GetValue(@"HKEY_LOCAL_MACHINE\WOW6432Node\LibreOffice\UNO\InstallPath", "", null);
            }

            string sofficePath = Path.Combine(regValue.ToString(), "soffice.exe");
            if (File.Exists(sofficePath))
            {
                return sofficePath;
            }
            
            sofficePath = @"C:\Program Files\LibreOffice\program\soffice.exe";
            if (File.Exists(sofficePath))
            {
                return sofficePath;
            }

            sofficePath = @"C:\Program Files (x86)\LibreOffice\program\soffice.exe";
            if (File.Exists(sofficePath))
            {
                return sofficePath;
            }
            return null;
        }

        /// <summary>
        /// Convert a document using LibreOffice
        /// </summary>
        /// <param name="inputFile">Input document to convert. This can be .html, .htm or .docx </param>
        /// <param name="outputFile">Output document. This can be .pdf, .docx, .html or .htm</param>
        /// <exception cref="Exception">Thrown when the LibreOffice application path is invalid or the conversion fails</exception>
        /// <see href="https://github.com/smartinmedia/Net-Core-DocX-HTML-To-PDF-Converter/blob/master/DocXToPdfConverter/DocXToPdfHandlers/ConvertWithLibreOffice.cs"/>
        public bool Convert(string inputFile, string outputFile)
        {
            if(!IsAvailable) { return false; }

            List<string> commandArgs = new List<string>();
            string convertedFile = "";

            if (string.IsNullOrEmpty(_libreOfficePath) || !File.Exists(_libreOfficePath))
            {
                throw new Exception(Properties.Resources.Error_LibreOffice_ApplicationPathError + (string.IsNullOrEmpty(_libreOfficePath) ? "" : (Environment.NewLine + _libreOfficePath)));
            }

            //Create tmp folder
            string tmpFolder = Path.Combine(Path.GetDirectoryName(outputFile), "LibreOfficeDocumentConverter_" + Guid.NewGuid().ToString().Substring(0, 10));
            if (!Directory.Exists(tmpFolder))
            {
                Directory.CreateDirectory(tmpFolder);
            }

            commandArgs.Add("--convert-to");

            if ((inputFile.EndsWith(".html") || inputFile.EndsWith(".htm")) && outputFile.EndsWith(".pdf"))
            {
                commandArgs.Add("pdf:writer_pdf_Export");
                convertedFile = Path.Combine(tmpFolder, Path.GetFileNameWithoutExtension(inputFile) + ".pdf");
            }
            else if (inputFile.EndsWith(".docx") && outputFile.EndsWith(".pdf"))
            {
                commandArgs.Add("pdf:writer_pdf_Export");
                convertedFile = Path.Combine(tmpFolder, Path.GetFileNameWithoutExtension(inputFile) + ".pdf");
            }
            else if (inputFile.EndsWith(".docx") && (outputFile.EndsWith(".html") || outputFile.EndsWith(".htm")))
            {
                commandArgs.Add("html:HTML:EmbedImages");
                convertedFile = Path.Combine(tmpFolder, Path.GetFileNameWithoutExtension(inputFile) + ".html");
            }
            else if ((inputFile.EndsWith(".html") || inputFile.EndsWith(".htm")) && outputFile.EndsWith(".docx"))
            {
                commandArgs.Add("docx:\"Office Open XML Text\"");
                convertedFile = Path.Combine(tmpFolder, Path.GetFileNameWithoutExtension(inputFile) + ".docx");
            }

            commandArgs.AddRange(new[] { inputFile, "--norestore", "--writer", "--headless", "--outdir", tmpFolder });

            ProcessStartInfo procStartInfo = new ProcessStartInfo(_libreOfficePath);
            string arguments = "";
            foreach (var arg in commandArgs) { arguments += arg + " "; }
            procStartInfo.Arguments = arguments.Trim();
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = true;
            procStartInfo.WorkingDirectory = Environment.CurrentDirectory;

            Process process = new Process() { StartInfo = procStartInfo };
            process.Start();
            process.WaitForExit();

            // Check for failed exit code.
            if (process.ExitCode != 0)
            {
                Directory.Delete(tmpFolder, true);

                throw new Exception(string.Format(Properties.Resources.Error_LibreOffice_ConversionFailed, process.ExitCode));
            }
            else
            {
                if (File.Exists(outputFile))
                {
                    File.Delete(outputFile);
                }
                if (File.Exists(convertedFile))
                {
                    File.Move(convertedFile, outputFile);
                }
                Directory.Delete(tmpFolder, true);
                return true;
            }
        }

        /// <summary>
        /// Check if the given .docx file was created by Libre Office.
        /// </summary>
        /// <param name="docxFile">File to check</param>
        /// <returns><see langword="true"/> when the .docx file was created by Libre Office; otherwise <see langword="false"/></returns>
        public bool IsDocxCreateWithThisConverter(string docxFile)
        {
            ZipArchive zip = ZipFile.OpenRead(docxFile);
            ZipArchiveEntry zipEntry = zip?.GetEntry("docProps/app.xml");
            if (zipEntry == null) { return false; }

            Stream zipEntryStream = zipEntry.Open();
            XDocument zipEntryXml = XDocument.Load(zipEntryStream);
            XElement zipEntryAppElement = zipEntryXml.Root?.Elements().FirstOrDefault(e => e.Name.LocalName == "Application");
            string appName = zipEntryAppElement?.Value;
            return appName.StartsWith("LibreOffice");
        }
    }
}
