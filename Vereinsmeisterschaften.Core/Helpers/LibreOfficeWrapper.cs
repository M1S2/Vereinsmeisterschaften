using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vereinsmeisterschaften.Core.Helpers
{
    /// <summary>
    /// Wrapper for LibreOffice to convert documents
    /// </summary>
    /// <see href="https://github.com/smartinmedia/Net-Core-DocX-HTML-To-PDF-Converter/blob/master/DocXToPdfConverter/DocXToPdfHandlers/ConvertWithLibreOffice.cs"/>
    public static class LibreOfficeDocumentConverter
    {
        /// <summary>
        /// Convert a document using LibreOffice
        /// </summary>
        /// <param name="inputFile">Input document to convert. This can be .html, .htm or .docx </param>
        /// <param name="outputFile">Output document. This can be .pdf, .docx, .html or .htm</param>
        /// <param name="libreOfficePath">Path to the soffice.exe of LibreOffice</param>
        /// <exception cref="Exception">Thrown when the LibreOffice application path is invalid or the conversion fails</exception>
        public static void Convert(string inputFile, string outputFile, string libreOfficePath)
        {
            List<string> commandArgs = new List<string>();
            string convertedFile = "";

            if(string.IsNullOrEmpty(libreOfficePath) || !File.Exists(libreOfficePath))
            {
                throw new Exception(Properties.Resources.Error_LibreOffice_ApplicationPathError + (string.IsNullOrEmpty(libreOfficePath) ? "" : (Environment.NewLine + libreOfficePath)));
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

            ProcessStartInfo procStartInfo = new ProcessStartInfo(libreOfficePath);
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
            }
        }
    }
}
