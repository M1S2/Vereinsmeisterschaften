using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime;
using System.Security.Cryptography;
using System.Text;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Models;
using Xceed.Words.NET;
using Xceed.Document.NET;
using System.Text.RegularExpressions;
using Vereinsmeisterschaften.Core.Helpers;
using Vereinsmeisterschaften.Core.Settings;
using System.Reflection.Emit;
using Vereinsmeisterschaften.Core.Documents;
using System.Reflection;
using System.Xml.Linq;

namespace Vereinsmeisterschaften.Core.Services
{
    /// <summary>
    /// Service used to create documents like certificates or start lists
    /// </summary>
    public class DocumentService : IDocumentService
    {
        private const string _tempFolderName = "temp";
        private const string _templatePostfix = "_Template";

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private IPersonService _personService;
        private IWorkspaceService _workspaceService;
        private IRaceService _raceService;
        private IScoreService _scoreService;
        private IServiceProvider _serviceProvider;
        
        private readonly IEnumerable<IDocumentStrategy> _documentStrategies;
        private PersonStartFilters _personStartFilter = PersonStartFilters.None;
        private object _personStartFilterParameter = null;

        public DocumentService(IPersonService personService, IWorkspaceService workspaceService, IRaceService raceService, IScoreService scoreService, IServiceProvider serviceProvider, IEnumerable<IDocumentStrategy> documentStrategies)
        {
            _personService = personService;
            _workspaceService = workspaceService;
            _raceService = raceService;
            _scoreService = scoreService;
            _serviceProvider = serviceProvider;
            _documentStrategies = documentStrategies;
        }

        private IDocumentStrategy getDocumentStrategy(DocumentCreationTypes type)
            => _documentStrategies.FirstOrDefault(s => s.DocumentType == type) ?? throw new InvalidOperationException($"No strategy found for {type}");

        public string PlaceholderMarker
            => _workspaceService?.Settings?.GetSettingValue<string>(WorkspaceSettings.GROUP_DOCUMENT_CREATION, WorkspaceSettings.SETTING_DOCUMENT_CREATION_PLACEHOLDER_MARKER) ?? string.Empty;

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Set the filters that are used for the certificate creation.
        /// </summary>
        /// <param name="personStartFilter">Filter to select only some specific <see cref="PersonStart"/> elements. Only valid for <see cref="DocumentCreationTypes.Certificates"/></param>
        /// <param name="personStartFilterParameter">Parameter for the personStartFilter. Only valid for <see cref="DocumentCreationTypes.Certificates"/></param>
        public void SetCertificateCreationFilters(PersonStartFilters personStartFilter = PersonStartFilters.None, object personStartFilterParameter = null)
        {
            _personStartFilter = personStartFilter;
            _personStartFilterParameter = personStartFilterParameter;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Create the document indicated by the document type.
        /// For the <see cref="DocumentCreationTypes.Certificates"/> type, the <see cref="SetCertificateCreationFilters"/> method must be called before this method to set the filters for the certificate creation. Otherwise the old values are used.
        /// </summary>
        /// <param name="documentType"><see cref="DocumentCreationTypes"/> used to decide which document type and <see cref="IDocumentStrategy"/> is used</param>
        /// <param name="createPdf">True to also create a .pdf file</param>
        /// <returns><see cref="Task"/> that can be used to run this async</returns>
        public Task<int> CreateDocument(DocumentCreationTypes documentType, bool createPdf = true)
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

                // For all other document types than certificates, we do not filter by person start
                PersonStartFilters personStartFilter = _personStartFilter;
                if (!(documentStrategy is DocumentStrategyCertificates))
                {
                    personStartFilter = PersonStartFilters.None;
                }

                // Create the output file name based on the filter
                string outputFileNameDocx = Path.GetFileNameWithoutExtension(documentTemplate);
                switch (personStartFilter)
                {
                    case PersonStartFilters.None: outputFileNameDocx = outputFileNameDocx.Replace(_templatePostfix, ""); break;
                    case PersonStartFilters.Person: outputFileNameDocx = outputFileNameDocx.Replace(_templatePostfix, "_" + ((Person)_personStartFilterParameter).FirstName + "_" + ((Person)_personStartFilterParameter).Name); break;
                    case PersonStartFilters.SwimmingStyle: outputFileNameDocx = outputFileNameDocx.Replace(_templatePostfix, "_" + EnumCoreToLocalizedString.Convert((SwimmingStyles)_personStartFilterParameter)); break;
                    case PersonStartFilters.CompetitionID: outputFileNameDocx = outputFileNameDocx.Replace(_templatePostfix, "_WK" + (int)_personStartFilterParameter); break;
                    default: outputFileNameDocx = outputFileNameDocx.Replace(_templatePostfix, ""); break;
                }
                outputFileNameDocx += ".docx";
                string outputFile = Path.Combine(documentOutputFolder, outputFileNameDocx);

                // Collect all items used to create the document
                object[] items = null;
                if (documentStrategy is DocumentStrategyCertificates documentStrategyCertificates)
                {
                    items = documentStrategyCertificates.GetItemsFiltered(personStartFilter, _personStartFilterParameter);
                }
                else
                {
                    items = documentStrategy.GetItems();
                }

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
                                string outputFileMulti = Path.Combine(tempFolder, Path.GetFileNameWithoutExtension(documentTemplate).Replace(_templatePostfix, "") + $"_{numCreatedPages}.docx");

                                DocXPlaceholderHelper.TablePlaceholders tablePlaceholders = documentStrategy.ResolveTablePlaceholders(items);
                                if (tablePlaceholders != null) { DocXPlaceholderHelper.ReplaceTablePlaceholders(documentTemplate, outputFileMulti, tablePlaceholders, placeholderMarker); }

                                DocXPlaceholderHelper.TextPlaceholders textPlaceholders = documentStrategy.ResolveTextPlaceholders(multiplePagesObj);
                                if (textPlaceholders != null) { DocXPlaceholderHelper.ReplaceTextPlaceholders(tablePlaceholders == null ? documentTemplate : outputFileMulti, outputFileMulti, textPlaceholders, placeholderMarker); }

                                insertCompetitionYearPlaceholderValue(outputFileMulti, outputFileMulti);

                                numCreatedPages++;
                            }

                            // Combine all docx files in the temp folder into one docx file
                            using (DocX document = DocX.Create(outputFile))
                            {
                                bool firstDocument = true;
                                foreach (string file in Directory.GetFiles(tempFolder))
                                {
                                    using (DocX tempDocument = DocX.Load(file))
                                    {
                                        document.InsertDocument(tempDocument, !firstDocument);
                                    }
                                    firstDocument = false;
                                }
                                document.Save();
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
                    DocXPlaceholderHelper.TablePlaceholders tablePlaceholders = documentStrategy.ResolveTablePlaceholders(items);
                    if (tablePlaceholders != null) { DocXPlaceholderHelper.ReplaceTablePlaceholders(documentTemplate, outputFile, tablePlaceholders, placeholderMarker); }

                    DocXPlaceholderHelper.TextPlaceholders textPlaceholders = documentStrategy.ResolveTextPlaceholders();
                    if (textPlaceholders != null) { DocXPlaceholderHelper.ReplaceTextPlaceholders(tablePlaceholders == null ? documentTemplate : outputFile, outputFile, textPlaceholders, placeholderMarker); }

                    insertCompetitionYearPlaceholderValue(outputFile, outputFile);

                    numCreatedPages = 1; // We created one page for the document
                }

                DocumentCreationFileTypes fileTypes = _workspaceService?.Settings?.GetSettingValue<DocumentCreationFileTypes>(WorkspaceSettings.GROUP_DOCUMENT_CREATION, WorkspaceSettings.SETTING_DOCUMENT_CREATION_FILE_TYPES) ?? DocumentCreationFileTypes.DOCX_AND_PDF;

                // Create PDF if requested and file type is PDF or DOCX_AND_PDF
                if (createPdf && (fileTypes == DocumentCreationFileTypes.PDF || fileTypes == DocumentCreationFileTypes.DOCX_AND_PDF))
                {
                    convertToPdf(outputFile);
                }

                // Delete the .docx file if file type is PDF
                if (fileTypes == DocumentCreationFileTypes.PDF)
                {
                    if (File.Exists(outputFile))
                    {
                        File.Delete(outputFile);
                    }
                }

                return numCreatedPages;
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
        /// Insert the competition year placeholder value into the template file and save it to the output file.
        /// </summary>
        /// <param name="templateFile">File in which to insert the competition year to the corresponding placeholder.</param>
        /// <param name="outputFile">Destination file location.</param>
        private void insertCompetitionYearPlaceholderValue(string templateFile, string outputFile)
        {
            ushort competitionYear = _workspaceService?.Settings?.GetSettingValue<ushort>(WorkspaceSettings.GROUP_GENERAL, WorkspaceSettings.SETTING_GENERAL_COMPETITIONYEAR) ?? 0;
            DocXPlaceholderHelper.TextPlaceholders textPlaceholders = new DocXPlaceholderHelper.TextPlaceholders();
            foreach (string placeholder in Placeholders.Placeholders_CompetitionYear) { textPlaceholders.Add(placeholder, competitionYear.ToString()); }
            DocXPlaceholderHelper.ReplaceTextPlaceholders(templateFile, outputFile, textPlaceholders, PlaceholderMarker);
        }

        /// <summary>
        /// Convert a .docx file to a .pdf file using LibreOffice.
        /// </summary>
        /// <param name="docxFile">.docx file to convert. The same name with the extension .pdf instead of .docx is used.</param>
        private void convertToPdf(string docxFile)
        {
            // Convert the docx file to pdf
            string outputFilePdf = docxFile.Replace(".docx", ".pdf");
            File.Delete(outputFilePdf);
            LibreOfficeDocumentConverter.Convert(docxFile, outputFilePdf, GetDocumentPathAbsolute(WorkspaceSettings.SETTING_DOCUMENT_CREATION_LIBRE_OFFICE_PATH, _workspaceService));
        }

        #endregion

    }
}
