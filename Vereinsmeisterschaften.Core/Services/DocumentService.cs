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
using static Vereinsmeisterschaften.Core.Helpers.DocXPlaceholderHelper;

namespace Vereinsmeisterschaften.Core.Services
{
    /// <summary>
    /// Service used to create documents like certificates or start lists
    /// </summary>
    public class DocumentService : IDocumentService
    {
        private const string _tempFolderName = "temp";
        private const string _templatePostfix = "_Template";


        public static List<string> Placeholders_CompetitionYear = new List<string>() { "Jahr", "J", "CompetitionYear", "Year", "Y" };
        public static List<string> Placeholders_Name = new List<string>() { "Name", "N" };
        public static List<string> Placeholders_BirthYear = new List<string>() { "Jahrgang", "JG", "BirthYear" };
        public static List<string> Placeholders_Distance = new List<string>() { "Strecke", "S", "Distance", "D" };
        public static List<string> Placeholders_SwimmingStyle = new List<string>() { "Lage", "L", "Style", "SwimmingStyle" };
        public static List<string> Placeholders_CompetitionID = new List<string>() { "WK", "Wettkampf", "Competition", "C" };       

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private IPersonService _personService;
        private IWorkspaceService _workspaceService;
        private IRaceService _raceService;

        public DocumentService(IPersonService personService, IWorkspaceService workspaceService, IRaceService raceService)
        {
            _personService = personService;
            _workspaceService = workspaceService;
            _raceService = raceService;
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

        private void insertCompetitionYearPlaceholderValue(string templateFile, string outputFile)
        {
            ushort competitionYear = _workspaceService?.Settings?.GetSettingValue<ushort>(WorkspaceSettings.GROUP_GENERAL, WorkspaceSettings.SETTING_GENERAL_COMPETITIONYEAR) ?? 0;
            DocXPlaceholderHelper.TextPlaceholders textPlaceholders = new DocXPlaceholderHelper.TextPlaceholders();
            foreach (string placeholder in Placeholders_CompetitionYear) { textPlaceholders.Add(placeholder, competitionYear.ToString()); }
            DocXPlaceholderHelper.ReplaceTextPlaceholders(templateFile, outputFile, textPlaceholders);
        }

        private void convertToPdf(string docxFile)
        {
            // Convert the docx file to pdf
            string outputFilePdf = docxFile.Replace(".docx", ".pdf");
            File.Delete(outputFilePdf);
            LibreOfficeDocumentConverter.Convert(docxFile, outputFilePdf, getLibreOfficePathAbsolute());
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
                        string certificateTemplatePath = getCertificateTemplatePathAbsolute();
                        string outputFileNameDocx = Path.GetFileNameWithoutExtension(certificateTemplatePath);
                        switch (personStartFilter)
                        {
                            case PersonStartFilters.None: outputFileNameDocx = outputFileNameDocx.Replace(_templatePostfix, ""); break;
                            case PersonStartFilters.Person: outputFileNameDocx = outputFileNameDocx.Replace(_templatePostfix, "_" + ((Person)personStartFilterParameter).FirstName + "_" + ((Person)personStartFilterParameter).Name); break;
#warning Add localization for "Lage"
                            case PersonStartFilters.SwimmingStyle: outputFileNameDocx = outputFileNameDocx.Replace(_templatePostfix, "_" + (SwimmingStyles)personStartFilterParameter); break;
                            case PersonStartFilters.CompetitionID: outputFileNameDocx = outputFileNameDocx.Replace(_templatePostfix, "_WK" + (int)personStartFilterParameter); break;
                            default: outputFileNameDocx = outputFileNameDocx.Replace(_templatePostfix, ""); break;
                        }
                        outputFileNameDocx += ".docx";

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
                            convertToPdf(outputFile);
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
                foreach (string placeholder in Placeholders_Name) { textPlaceholders.Add(placeholder, personStart.PersonObj?.FirstName + " " + personStart.PersonObj?.Name); }
                foreach (string placeholder in Placeholders_BirthYear) { textPlaceholders.Add(placeholder, personStart.PersonObj?.BirthYear.ToString()); }
                foreach (string placeholder in Placeholders_Distance) { textPlaceholders.Add(placeholder, personStart.CompetitionObj.Distance.ToString() + "m"); }
#warning Add localization for "Lage"
                foreach (string placeholder in Placeholders_SwimmingStyle) { textPlaceholders.Add(placeholder, personStart.Style.ToString()); }
                foreach (string placeholder in Placeholders_CompetitionID) { textPlaceholders.Add(placeholder, personStart.CompetitionObj.ID.ToString()); }

                string outputFile = Path.Combine(outputFolder, $"{personStart.PersonObj?.FirstName}_{personStart.PersonObj?.Name}_{personStart.Style}.docx");
                string certificateTemplatePath = getCertificateTemplatePathAbsolute();
                DocXPlaceholderHelper.ReplaceTextPlaceholders(certificateTemplatePath, outputFile, textPlaceholders);

                insertCompetitionYearPlaceholderValue(outputFile, outputFile);

                if (createPdf)
                {
                    convertToPdf(outputFile);
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

        #region Overview List Creation

        public Task CreateOverviewList(bool createPdf = true)
        {
            return Task.Run(() =>
            {
                List<PersonStart> personStarts = _personService.GetAllPersonStarts();
                List<string> names = new List<string>();
                List<string> birthYears = new List<string>();
                List<string> styles = new List<string>();
                List<string> distances = new List<string>();
                List<string> competitions = new List<string>();
                foreach (PersonStart personStart in personStarts)
                {
                    names.Add(personStart.PersonObj?.FirstName + " " + personStart.PersonObj?.Name);
                    birthYears.Add(personStart.PersonObj?.BirthYear.ToString() ?? string.Empty);
                    styles.Add(personStart.Style.ToString());
                    distances.Add(personStart.CompetitionObj?.Distance.ToString() + "m" ?? string.Empty);
                    competitions.Add(personStart.CompetitionObj?.ID.ToString() ?? string.Empty);
                }

                DocXPlaceholderHelper.TablePlaceholders tablePlaceholders = new DocXPlaceholderHelper.TablePlaceholders();
                foreach (string placeholder in Placeholders_Name) { tablePlaceholders.Add(placeholder, names); }
                foreach (string placeholder in Placeholders_BirthYear) { tablePlaceholders.Add(placeholder, birthYears); }
#warning Add localization for "Lage"
                foreach (string placeholder in Placeholders_SwimmingStyle) { tablePlaceholders.Add(placeholder, styles); }
                foreach (string placeholder in Placeholders_Distance) { tablePlaceholders.Add(placeholder, distances); }
                foreach (string placeholder in Placeholders_CompetitionID) { tablePlaceholders.Add(placeholder, competitions); }

                string documentOutputFolder = getDocumentOutputFolderAbsolute();
                if (!Directory.Exists(documentOutputFolder))
                {
                    Directory.CreateDirectory(documentOutputFolder);
                }

                string overviewListTemplatePath = getOverviewListTemplatePathAbsolute();
                string outputFile = Path.Combine(documentOutputFolder, Path.GetFileNameWithoutExtension(overviewListTemplatePath).Replace(_templatePostfix, "") + ".docx");
                DocXPlaceholderHelper.ReplaceTablePlaceholders(overviewListTemplatePath, outputFile, tablePlaceholders);

                insertCompetitionYearPlaceholderValue(outputFile, outputFile);

                if (createPdf)
                {
                    convertToPdf(outputFile);
                }
            });
        }

        private string getOverviewListTemplatePathAbsolute()
        {
            string overviewListTemplatePath = _workspaceService?.Settings?.GetSettingValue<string>(WorkspaceSettings.GROUP_DOCUMENT_CREATION, WorkspaceSettings.SETTING_DOCUMENT_CREATION_OVERVIEW_LIST_TEMPLATE_PATH) ?? string.Empty;
            overviewListTemplatePath = FilePathHelper.MakePathAbsolute(overviewListTemplatePath, _workspaceService?.PersistentPath);
            return overviewListTemplatePath;
        }

        #endregion

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Race Start List Creation

        public Task CreateRaceStartList(bool createPdf = true)
        {
            return Task.Run(() =>
            {
                RacesVariant racesVariant = _raceService.PersistedRacesVariant;
                if(racesVariant == null) { return; }

                List<string> competitions = new List<string>();
                List<string> styles = new List<string>();
                List<string> distances = new List<string>();
                List<List<string>> names = new List<List<string>>();
                List<List<string>> birthYears = new List<List<string>>();
                int maxPersonsInRace = 0;
                foreach (Race race in racesVariant.Races)
                {
                    competitions.Add(string.Join(", ", race.Starts.Select(s => s.CompetitionObj?.ID.ToString() ?? "?")).TrimEnd(',', ' '));
                    styles.Add(race.Style.ToString());
                    distances.Add(race.Distance.ToString());

                    List<string> personNames = race.Starts.Select(s => s.PersonObj?.FirstName + " " + s.PersonObj?.Name).ToList();
                    maxPersonsInRace = Math.Max(maxPersonsInRace, personNames.Count);
                    names.Add(personNames);

                    List<string> personBirthDates = race.Starts.Select(s => s.PersonObj?.BirthYear.ToString()).ToList();
                    birthYears.Add(personBirthDates);
                }

                ushort numSwimLanes = _workspaceService?.Settings?.GetSettingValue<ushort>(WorkspaceSettings.GROUP_RACE_CALCULATION, WorkspaceSettings.SETTING_RACE_CALCULATION_NUMBER_OF_SWIM_LANES) ?? 3;

                DocXPlaceholderHelper.TablePlaceholders tablePlaceholders = new DocXPlaceholderHelper.TablePlaceholders();
                foreach (string placeholder in Placeholders_CompetitionID) { tablePlaceholders.Add(placeholder, competitions); }
#warning Add localization for "Lage"
                foreach (string placeholder in Placeholders_SwimmingStyle) { tablePlaceholders.Add(placeholder, styles); }
                foreach (string placeholder in Placeholders_Distance) { tablePlaceholders.Add(placeholder, distances); }
                for (int i = 0; i < Math.Max(numSwimLanes, maxPersonsInRace); i++)
                {
                    foreach (string placeholder in Placeholders_Name) { tablePlaceholders.Add(placeholder + (i + 1), names.Select(innerNames => innerNames.Count > i ? innerNames[i] : "").ToList()); }
                    foreach (string placeholder in Placeholders_BirthYear) { tablePlaceholders.Add(placeholder + (i + 1), birthYears.Select(innerBirthYear => innerBirthYear.Count > i ? innerBirthYear[i] : "").ToList()); }
                }

                string documentOutputFolder = getDocumentOutputFolderAbsolute();
                if (!Directory.Exists(documentOutputFolder))
                {
                    Directory.CreateDirectory(documentOutputFolder);
                }

                string raceStartListTemplatePath = getRaceStartListTemplatePathAbsolute();
                string outputFile = Path.Combine(documentOutputFolder, Path.GetFileNameWithoutExtension(raceStartListTemplatePath).Replace(_templatePostfix, "") + ".docx");
                DocXPlaceholderHelper.ReplaceTablePlaceholders(raceStartListTemplatePath, outputFile, tablePlaceholders);

                insertCompetitionYearPlaceholderValue(outputFile, outputFile);

                if (createPdf)
                {
                    convertToPdf(outputFile);
                }
            });
        }

        private string getRaceStartListTemplatePathAbsolute()
        {
            string raceStartListTemplatePath = _workspaceService?.Settings?.GetSettingValue<string>(WorkspaceSettings.GROUP_DOCUMENT_CREATION, WorkspaceSettings.SETTING_DOCUMENT_CREATION_RACE_START_LIST_TEMPLATE_PATH) ?? string.Empty;
            raceStartListTemplatePath = FilePathHelper.MakePathAbsolute(raceStartListTemplatePath, _workspaceService?.PersistentPath);
            return raceStartListTemplatePath;
        }

        #endregion

    }
}
