using Xceed.Words.NET;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;
using Vereinsmeisterschaften.Core.Helpers;
using Vereinsmeisterschaften.Core.Settings;
using Vereinsmeisterschaften.Core.Documents;
using Vereinsmeisterschaften.Core.Documents.DocumentFileConverters;
using Vereinsmeisterschaften.Core.Documents.DocumentStrategies;

namespace Vereinsmeisterschaften.Core.Services
{
    /// <summary>
    /// Service used to create documents like certificates or start lists
    /// </summary>
    public class DocumentService : IDocumentService
    {
        private const string _tempFolderName = "temp";

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private IWorkspaceService _workspaceService;        
        private readonly IEnumerable<IDocumentStrategy> _documentStrategies;
        private IDocumentFileConverterSelector _documentFileConverterSelector;

        /// <summary>
        /// Constructor for the DocumentService.
        /// </summary>
        /// <param name="workspaceService"><see cref="IWorkspaceService"/> object</param>
        /// <param name="documentStrategies">List of <see cref="IDocumentStrategy"/> objects</param>
        /// <param name="documentFileConverterSelector"><see cref="IDocumentFileConverterSelector"/> object</param>
        public DocumentService(IWorkspaceService workspaceService, IEnumerable<IDocumentStrategy> documentStrategies, IDocumentFileConverterSelector documentFileConverterSelector)
        {
            _workspaceService = workspaceService;
            _documentStrategies = documentStrategies;
            _documentFileConverterSelector = documentFileConverterSelector;
        }

        private IDocumentStrategy getDocumentStrategy(DocumentCreationTypes type)
            => _documentStrategies.FirstOrDefault(s => s.DocumentType == type) ?? throw new InvalidOperationException($"No strategy found for {type}");

        /// <summary>
        /// Get the placeholder marker string used in the document templates.
        /// </summary>
        public string PlaceholderMarker
            => _workspaceService?.Settings?.GetSettingValue<string>(WorkspaceSettings.GROUP_DOCUMENT_CREATION, WorkspaceSettings.SETTING_DOCUMENT_CREATION_PLACEHOLDER_MARKER) ?? string.Empty;

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Create the document indicated by the document type.
        /// </summary>
        /// <param name="documentType"><see cref="DocumentCreationTypes"/> used to decide which document type and <see cref="IDocumentStrategy"/> is used</param>
        /// <param name="createPdf">True to also create a .pdf file</param>
        /// <returns><see cref="Task"/> that can be used to run this async. The return parameter is a tuple of (number of created pages in the document, filepath to the last created document (PDF if possible))</returns>
        public Task<(int, string)> CreateDocument(DocumentCreationTypes documentType, bool createPdf = true)
        {
            return Task.Run(() =>
            {
                int numCreatedPages = 0;

                string documentOutputFolder = GetDocumentPathAbsolute(WorkspaceSettings.SETTING_DOCUMENT_CREATION_OUTPUT_FOLDER, _workspaceService);
                if (!Directory.Exists(documentOutputFolder))
                {
                    Directory.CreateDirectory(documentOutputFolder);
                }

                IDocumentStrategy documentStrategy = getDocumentStrategy(documentType);
                string documentTemplate = documentStrategy.TemplatePath;
                if (Path.GetExtension(documentTemplate) != ".docx")
                {
                    throw new InvalidOperationException($"Document template \"{documentTemplate}\" is not a .docx file.");
                }

                // Create the output file name. The TemplateFileNamePostfixReplaceStr is created based on the filter.
                string outputFileNameDocx = Path.GetFileNameWithoutExtension(documentTemplate);
                string templateFileNamePostfix = _workspaceService?.Settings?.GetSettingValue<string>(WorkspaceSettings.GROUP_DOCUMENT_CREATION, WorkspaceSettings.SETTING_DOCUMENT_CREATION_TEMPLATE_FILENAME_POSTFIX) ?? string.Empty;
                string templateFileNamePostfixReplaceStr = documentStrategy.TemplateFileNamePostfixReplaceStr;
                if (outputFileNameDocx.Contains(templateFileNamePostfix))
                {
                    outputFileNameDocx = outputFileNameDocx.Replace(templateFileNamePostfix, templateFileNamePostfixReplaceStr);
                }
                else
                {
                    outputFileNameDocx += templateFileNamePostfixReplaceStr;
                }
                outputFileNameDocx += ".docx";
                string outputFile = Path.Combine(documentOutputFolder, outputFileNameDocx);

                // Collect all items used to create the document
                object[] items = documentStrategy.GetItems();

                string placeholderMarker = PlaceholderMarker;

                // Decide whether to create multiple pages or not
                if (documentStrategy.CreateMultiplePages)
                {
                    // Create a temp folder to store all pages as single documents
                    string tempFolder = Path.Combine(documentOutputFolder, _tempFolderName);
                    try
                    {
                        // Delete temp folder and all files in it
                        if (Directory.Exists(tempFolder))
                        {
                            Directory.Delete(tempFolder, true);
                        }
                        Directory.CreateDirectory(tempFolder);
                                                
                        if (items != null && items.Length > 0)
                        {
                            foreach (object multiplePagesObj in items)
                            {
                                // Create the filename for the temporary file containing one page of the final multi pages document. Make sure the postfix number always contain an equal number of digits (prefixed by 0). This is important later for Directory.GetFiles() ordering.
                                string outputFileMulti = Path.Combine(tempFolder, Path.GetFileNameWithoutExtension(documentTemplate).Replace(templateFileNamePostfix, "") + $"_{numCreatedPages:0000}.docx");

                                DocXPlaceholderHelper.TablePlaceholders tablePlaceholders = documentStrategy.ResolveTablePlaceholders(items);
                                if (tablePlaceholders != null) { DocXPlaceholderHelper.ReplaceTablePlaceholders(documentTemplate, outputFileMulti, tablePlaceholders, placeholderMarker); }

                                DocXPlaceholderHelper.TextPlaceholders textPlaceholders = documentStrategy.ResolveTextPlaceholders(multiplePagesObj);
                                if (textPlaceholders != null) { DocXPlaceholderHelper.ReplaceTextPlaceholders(tablePlaceholders == null ? documentTemplate : outputFileMulti, outputFileMulti, textPlaceholders, placeholderMarker); }

                                insertAlwaysSupportedPlaceholderValues(outputFileMulti, outputFileMulti);

                                numCreatedPages++;
                            }

                            // Combine all docx files in the temp folder into one docx file
                            string[] files = Directory.GetFiles(tempFolder);
                            if (files.Length > 0)
                            {
                                // Copy the first document as the base for the output file
                                File.Copy(files[0], outputFile, true);
                                using (DocX document = DocX.Load(outputFile))
                                {
                                    // Append all other documents to the first one
                                    for (int i = 1; i < files.Length; i++)
                                    {
                                        using (DocX tempDocument = DocX.Load(files[i]))
                                        {
                                            document.InsertDocument(tempDocument, true);
                                        }
                                    }
                                    document.Save();
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        // Delete temp folder and all files in it
                        if (Directory.Exists(tempFolder))
                        {
                            Directory.Delete(tempFolder, true);
                        }
                    }
                }
                else
                {
                    // Create a single page document
                    string inputFile = documentTemplate;

                    DocXPlaceholderHelper.TablePlaceholders tablePlaceholders = documentStrategy.ResolveTablePlaceholders(items);
                    if (tablePlaceholders != null)
                    {
                        DocXPlaceholderHelper.ReplaceTablePlaceholders(inputFile, outputFile, tablePlaceholders, placeholderMarker);
                        inputFile = outputFile;
                    }

                    foreach (object item in items)
                    {
                        DocXPlaceholderHelper.TextPlaceholders textPlaceholders = documentStrategy.ResolveTextPlaceholders(item);
                        if (textPlaceholders != null)
                        {
                            DocXPlaceholderHelper.ReplaceTextPlaceholders(inputFile, outputFile, textPlaceholders, placeholderMarker);
                            inputFile = outputFile;
                        }
                    }
                    insertAlwaysSupportedPlaceholderValues(outputFile, outputFile);

                    numCreatedPages = 1; // We created one page for the document
                }

                DocumentCreationFileTypes fileTypes = _workspaceService?.Settings?.GetSettingValue<DocumentCreationFileTypes>(WorkspaceSettings.GROUP_DOCUMENT_CREATION, WorkspaceSettings.SETTING_DOCUMENT_CREATION_FILE_TYPES) ?? DocumentCreationFileTypes.DOCX_AND_PDF;

                string returnFilePath = outputFile;

                // Create PDF if requested and file type is PDF or DOCX_AND_PDF
                if (createPdf && (fileTypes == DocumentCreationFileTypes.PDF || fileTypes == DocumentCreationFileTypes.DOCX_AND_PDF))
                {
                    returnFilePath = convertToPdf(outputFile);
                }

                // Delete the .docx file if file type is PDF
                if (fileTypes == DocumentCreationFileTypes.PDF)
                {
                    if (File.Exists(outputFile))
                    {
                        File.Delete(outputFile);
                    }
                }

                return (numCreatedPages, returnFilePath);
            });
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Helper methods

        /// <summary>
        /// Get the absolute path for the requested document creation setting.
        /// Caution: Only use keys here that refer to string parameters for paths!
        /// </summary>
        /// <param name="documentCreationSettingKey">Key for the setting inside the <see cref="WorkspaceSettings.GROUP_DOCUMENT_CREATION"/></param>
        /// <returns>Absolute path</returns>
        public static string GetDocumentPathAbsolute(string documentCreationSettingKey, IWorkspaceService workspaceService)
        {
            string path = workspaceService?.Settings?.GetSettingValue<string>(WorkspaceSettings.GROUP_DOCUMENT_CREATION, documentCreationSettingKey) ?? string.Empty;
            path = FilePathHelper.MakePathAbsolute(path, workspaceService?.PersistentPath);
            return path;
        }

        /// <summary>
        /// Insert the values for the <see cref="DocumentStrategyBase{TData}.AlwaysSupportedPlaceholderKeys"/> into the template file and save it to the output file.
        /// </summary>
        /// <param name="templateFile">File in which to insert the always supported placeholder values to the corresponding placeholders.</param>
        /// <param name="outputFile">Destination file location.</param>
        private void insertAlwaysSupportedPlaceholderValues(string templateFile, string outputFile)
        {
            // Make sure to have logic here for all values in DocumentStrategyBase<TData>.AlwaysSupportedPlaceholderKeys

            ushort competitionYear = _workspaceService?.Settings?.GetSettingValue<ushort>(WorkspaceSettings.GROUP_GENERAL, WorkspaceSettings.SETTING_GENERAL_COMPETITIONYEAR) ?? 0;
            DateTime competitionDate = _workspaceService?.Settings?.GetSettingValue<DateTime>(WorkspaceSettings.GROUP_GENERAL, WorkspaceSettings.SETTING_GENERAL_COMPETITIONDATE) ?? DateTime.Today;
            string appVersion = System.Reflection.Assembly.GetEntryAssembly().GetName().Version?.ToString() ?? "?";
            string workspacePath = _workspaceService?.PersistentPath ?? string.Empty;
            
            DocXPlaceholderHelper.TextPlaceholders textPlaceholders = new DocXPlaceholderHelper.TextPlaceholders();
            foreach (string placeholder in Placeholders.Placeholders_CompetitionYear) { textPlaceholders.Add(placeholder, competitionYear.ToString()); }
            foreach (string placeholder in Placeholders.Placeholders_CompetitionDate) { textPlaceholders.Add(placeholder, competitionDate.ToShortDateString()); }
            foreach (string placeholder in Placeholders.Placeholders_AppVersion) { textPlaceholders.Add(placeholder, appVersion); }
            foreach (string placeholder in Placeholders.Placeholders_WorkspacePath) { textPlaceholders.Add(placeholder, workspacePath); }

            DocXPlaceholderHelper.ReplaceTextPlaceholders(templateFile, outputFile, textPlaceholders, PlaceholderMarker);
        }

        /// <summary>
        /// Convert a .docx file to a .pdf file using LibreOffice.
        /// </summary>
        /// <param name="docxFile">.docx file to convert. The same name with the extension .pdf instead of .docx is used.</param>
        /// <returns>.pdf file path</returns>
        private string convertToPdf(string docxFile)
        {
            // Convert the docx file to pdf
            string outputFilePdf = docxFile.Replace(".docx", ".pdf");
            File.Delete(outputFilePdf);

            List<IDocumentFileConverter> ignoreConverters = new List<IDocumentFileConverter>();
            IDocumentFileConverter converter = null;
            bool conversionResult = false;
            Exception firstException = null;
            // Loop until no converter is found anymore or the conversion is finished.
            do
            {
                converter = _documentFileConverterSelector.GetConverter(docxFile, ignoreConverters);
                try
                {
                    conversionResult = converter?.Convert(docxFile, outputFilePdf) ?? false;
                }
                catch(Exception ex)
                {
                    if(firstException == null)
                    {
                        firstException = ex;
                    }
                    conversionResult = false;
                }
                if(!conversionResult)
                {
                    ignoreConverters.Add(converter);
                }
            } while (converter != null && !conversionResult);

            if(!conversionResult && firstException != null)
            {
                throw firstException;
            }
            return outputFilePdf;
        }

        #endregion

    }
}
