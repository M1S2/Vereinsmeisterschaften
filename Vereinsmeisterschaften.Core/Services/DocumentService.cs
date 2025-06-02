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

namespace Vereinsmeisterschaften.Core.Services
{
    /// <summary>
    /// Service used to create documents like certificates or start lists
    /// </summary>
    public class DocumentService : IDocumentService
    {
        private const string _tempFolderName = "temp";
        private const string _certificateOutputFileNameDocx = "Urkunden{0}.docx";
       
        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private IPersonService _personService;
        private IWorkspaceService _workspaceService;

        public DocumentService(IPersonService personService, IWorkspaceService workspaceService)
        {
            _personService = personService;
            _workspaceService = workspaceService;
        }

        private string getDocumentOutputFolderAbsolute()
        {
            string certificateOutputFolder = _workspaceService?.Settings?.GetSettingValue<string>(WorkspaceSettings.GROUP_DOCUMENT_CREATION, WorkspaceSettings.SETTING_DOCUMENT_CREATION_OUTPUT_FOLDER) ?? string.Empty;
            certificateOutputFolder = FilePathHelper.MakePathAbsolute(certificateOutputFolder, _workspaceService?.PersistentPath);
            return certificateOutputFolder;
        }

        private string getLibreOfficePathAbsolute()
        {
            string libreOfficePath = _workspaceService?.Settings?.GetSettingValue<string>(WorkspaceSettings.GROUP_DOCUMENT_CREATION, WorkspaceSettings.SETTING_DOCUMENT_CREATION_LIBRE_OFFICE_PATH) ?? string.Empty;
            libreOfficePath = FilePathHelper.MakePathAbsolute(libreOfficePath, _workspaceService?.PersistentPath);
            return libreOfficePath;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Certificate Creation

        public Task<int> CreateCertificates(bool createPdf = true, PersonStartFilters personStartFilter = PersonStartFilters.None, object personStartFilterParameter = null)
        {
            return Task.Run(async () =>
            {
                int numCreatedCertificates = 0;

                string documentOutputFolder = getDocumentOutputFolderAbsolute();
                string tempFolder = Path.Combine(documentOutputFolder, _tempFolderName);

                // Delete temp folder and all files in it
                if (Directory.Exists(tempFolder))
                {
                    Directory.Delete(tempFolder, true);
                }

                // Create all certificates in a temp folder as single docx files
                List<PersonStart> personStarts = _personService.GetAllPersonStarts(personStartFilter, personStartFilterParameter).Where(s => s.CompetitionObj != null).ToList();
                if (personStarts.Count > 0)
                {
                    try
                    {
                        foreach (PersonStart personStart in personStarts)
                        {
                            int resultSingle = await createSingleCertificate(personStart, false, tempFolder);
                            numCreatedCertificates += resultSingle;
                        }

                        // Create the output file name based on the filter
                        string outputFileNameDocx = string.Empty;
                        switch (personStartFilter)
                        {
                            case PersonStartFilters.None: outputFileNameDocx = string.Format(_certificateOutputFileNameDocx, ""); break;
                            case PersonStartFilters.Person: outputFileNameDocx = string.Format(_certificateOutputFileNameDocx, "_" + ((Person)personStartFilterParameter).FirstName + "_" + ((Person)personStartFilterParameter).Name); break;
#warning Add localization for "Lage"
                            case PersonStartFilters.SwimmingStyle: outputFileNameDocx = string.Format(_certificateOutputFileNameDocx, "_" + (SwimmingStyles)personStartFilterParameter); break;
                            case PersonStartFilters.CompetitionID: outputFileNameDocx = string.Format(_certificateOutputFileNameDocx, "_WK" + (int)personStartFilterParameter); break;
                            default: outputFileNameDocx = string.Format(_certificateOutputFileNameDocx, ""); break;
                        }
                        string outputFile = Path.Combine(documentOutputFolder, outputFileNameDocx);

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

                        if (createPdf)
                        {
                            // Convert the combined docx file to pdf
                            string outputFilePdf = outputFile.Replace(".docx", ".pdf");
                            File.Delete(outputFilePdf);
                            LibreOfficeDocumentConverter.Convert(outputFile, outputFilePdf, getLibreOfficePathAbsolute());
                        }
                    }
                    catch(Exception)
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
                return numCreatedCertificates;
            });
        }

        private Task<int> createSingleCertificate(PersonStart personStart, bool createPdf = true, string outputFolder = "")
        {
            return Task.Run(() =>
            {
                if (string.IsNullOrEmpty(outputFolder))
                {
                    string documentOutputFolder = getDocumentOutputFolderAbsolute();
                    outputFolder = documentOutputFolder;
                }
                if (!Directory.Exists(outputFolder))
                {
                    Directory.CreateDirectory(outputFolder);
                }

                if (personStart.CompetitionObj == null)
                {
                    return 0;
                }

                // Collect all placeholder values
                DocXPlaceholderHelper.TextPlaceholders textPlaceholders = new DocXPlaceholderHelper.TextPlaceholders();
                textPlaceholders.Add("Name", personStart.PersonObj?.FirstName + " " + personStart.PersonObj?.Name);
                textPlaceholders.Add("JG", personStart.PersonObj?.BirthYear.ToString());
                textPlaceholders.Add("Strecke", personStart.CompetitionObj.Distance.ToString() + "m");
#warning Add localization for "Lage"
                textPlaceholders.Add("Lage", personStart.Style.ToString());
                textPlaceholders.Add("WK", personStart.CompetitionObj.ID.ToString());

                string outputFile = Path.Combine(outputFolder, $"{personStart.PersonObj?.FirstName}_{personStart.PersonObj?.Name}_{personStart.Style}.docx");
                string certificateTemplatePath = getCertificateTemplatePathAbsolute();
                DocXPlaceholderHelper.ReplaceTextPlaceholders(certificateTemplatePath, outputFile, textPlaceholders);

                if (createPdf)
                {
                    // Convert the docx file to pdf
                    string outputFilePdf = outputFile.Replace(".docx", ".pdf");
                    LibreOfficeDocumentConverter.Convert(outputFile, outputFilePdf, getLibreOfficePathAbsolute());
                }
                return 1;       // This method always creates one certificate, so we return 1 to indicate success
            });
        }

        private string getCertificateTemplatePathAbsolute()
        {
            string certificateTemplatePath = _workspaceService?.Settings?.GetSettingValue<string>(WorkspaceSettings.GROUP_DOCUMENT_CREATION, WorkspaceSettings.SETTING_DOCUMENT_CREATION_CERTIFICATE_TEMPLATE_PATH) ?? string.Empty;
            certificateTemplatePath = FilePathHelper.MakePathAbsolute(certificateTemplatePath, _workspaceService?.PersistentPath);
            return certificateTemplatePath;
        }

        #endregion

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        public Task CreateOverviewList(bool createPdf = true)
        {
            return Task.Run(() =>
            {
                List<PersonStart> personStarts = _personService.GetAllPersonStarts();
                List<string> names = new List<string>();
                List<string> styles = new List<string>();
                foreach (PersonStart personStart in personStarts)
                {
                    names.Add(personStart.PersonObj?.FirstName + " " + personStart.PersonObj?.Name);
                    styles.Add(personStart.Style.ToString());
                }

                DocXPlaceholderHelper.TablePlaceholders tablePlaceholders = new DocXPlaceholderHelper.TablePlaceholders();
                tablePlaceholders.Add("Name", names);
#warning Add localization for "Lage"
                tablePlaceholders.Add("Lage", styles);

                string documentOutputFolder = getDocumentOutputFolderAbsolute();
                string overviewListTemplatePath = getOverviewListTemplatePathAbsolute();
                string outputFile = Path.Combine(documentOutputFolder, Path.GetFileNameWithoutExtension(overviewListTemplatePath).Replace("_Template", "") + ".docx");
                DocXPlaceholderHelper.ReplaceTablePlaceholders(overviewListTemplatePath, outputFile, tablePlaceholders);

                if (createPdf)
                {
                    // Convert the docx file to pdf
                    string outputFilePdf = outputFile.Replace(".docx", ".pdf");
                    File.Delete(outputFilePdf);
                    LibreOfficeDocumentConverter.Convert(outputFile, outputFilePdf, getLibreOfficePathAbsolute());
                }
            });
        }

        private string getOverviewListTemplatePathAbsolute()
        {
            string overviewListTemplatePath = _workspaceService?.Settings?.GetSettingValue<string>(WorkspaceSettings.GROUP_DOCUMENT_CREATION, WorkspaceSettings.SETTING_DOCUMENT_CREATION_OVERVIEW_LIST_TEMPLATE_PATH) ?? string.Empty;
            overviewListTemplatePath = FilePathHelper.MakePathAbsolute(overviewListTemplatePath, _workspaceService?.PersistentPath);
            return overviewListTemplatePath;
        }

    }
}
