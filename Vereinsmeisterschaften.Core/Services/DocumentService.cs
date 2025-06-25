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
        private const string _templatePostfix = "_Template";

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Placeholders

        /// <summary>
        /// List with all placeholders that can be used in the template to insert the competition year.
        /// </summary>
        public static List<string> Placeholders_CompetitionYear = new List<string>() { "Jahr", "J", "CompetitionYear", "Year", "Y" };
        /// <summary>
        /// List with all placeholders that can be used in the template to insert the name of a person.
        /// </summary>
        public static List<string> Placeholders_Name = new List<string>() { "Name", "N" };
        /// <summary>
        /// List with all placeholders that can be used in the template to insert the birth year of a person.
        /// </summary>
        public static List<string> Placeholders_BirthYear = new List<string>() { "Jahrgang", "JG", "BirthYear" };
        /// <summary>
        /// List with all placeholders that can be used in the template to insert the distance of a competition.
        /// </summary>
        public static List<string> Placeholders_Distance = new List<string>() { "Strecke", "S", "Distance", "D" };
        /// <summary>
        /// List with all placeholders that can be used in the template to insert the swimming style of a person.
        /// </summary>
        public static List<string> Placeholders_SwimmingStyle = new List<string>() { "Lage", "L", "Style", "SwimmingStyle" };
        /// <summary>
        /// List with all placeholders that can be used in the template to insert the competition ID of a competition.
        /// </summary>
        public static List<string> Placeholders_CompetitionID = new List<string>() { "WK", "Wettkampf", "Competition", "C" };
        /// <summary>
        /// List with all placeholders that can be used in the template to insert the highest score of a person.
        /// </summary>
        public static List<string> Placeholders_Score = new List<string>() { "Punkte", "Score", "Pkt" };
        /// <summary>
        /// List with all placeholders that can be used in the template to insert the place in the overall result list of a person.
        /// </summary>
        public static List<string> Placeholders_ResultListPlace = new List<string>() { "Platzierung", "Platz", "Result", "Place", "P" };

        #endregion

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private IPersonService _personService;
        private IWorkspaceService _workspaceService;
        private IRaceService _raceService;
        private IScoreService _scoreService;

        public DocumentService(IPersonService personService, IWorkspaceService workspaceService, IRaceService raceService, IScoreService scoreService)
        {
            _personService = personService;
            _workspaceService = workspaceService;
            _raceService = raceService;
            _scoreService = scoreService;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Helper methods

        /// <summary>
        /// Get the absolute path for the requested document creation setting.
        /// Caution: Only use keys here that refer to string parameters for paths!
        /// </summary>
        /// <param name="documentCreationSettingKey">Key for the setting inside the <see cref="WorkspaceSettings.GROUP_DOCUMENT_CREATION"/></param>
        /// <returns>Absolute path</returns>
        private string getDocumentPathAbsolute(string documentCreationSettingKey)
        {
            string path = _workspaceService?.Settings?.GetSettingValue<string>(WorkspaceSettings.GROUP_DOCUMENT_CREATION, documentCreationSettingKey) ?? string.Empty;
            path = FilePathHelper.MakePathAbsolute(path, _workspaceService?.PersistentPath);
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
            foreach (string placeholder in Placeholders_CompetitionYear) { textPlaceholders.Add(placeholder, competitionYear.ToString()); }
            DocXPlaceholderHelper.ReplaceTextPlaceholders(templateFile, outputFile, textPlaceholders);
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
            LibreOfficeDocumentConverter.Convert(docxFile, outputFilePdf, getDocumentPathAbsolute(WorkspaceSettings.SETTING_DOCUMENT_CREATION_LIBRE_OFFICE_PATH));
        }

#endregion

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Certificate Creation

        /// <summary>
        /// Create certificates for all person starts in the database based on the given filter.
        /// </summary>
        /// <param name="createPdf">True to also create a .pdf file</param>
        /// <param name="personStartFilter">Filter to select only some specific person starts</param>
        /// <param name="personStartFilterParameter">Parameter for the personStartFilter</param>
        /// <returns>Number of created certificates</returns>
        public Task<int> CreateCertificates(bool createPdf = true, PersonStartFilters personStartFilter = PersonStartFilters.None, object personStartFilterParameter = null)
        {
            return Task.Run(async () =>
            {
                int numCreatedCertificates = 0;

                string documentOutputFolder = getDocumentPathAbsolute(WorkspaceSettings.SETTING_DOCUMENT_CREATION_OUTPUT_FOLDER);
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
                        string certificateTemplatePath = getDocumentPathAbsolute(WorkspaceSettings.SETTING_DOCUMENT_CREATION_CERTIFICATE_TEMPLATE_PATH);
                        string outputFileNameDocx = Path.GetFileNameWithoutExtension(certificateTemplatePath);
                        switch (personStartFilter)
                        {
                            case PersonStartFilters.None: outputFileNameDocx = outputFileNameDocx.Replace(_templatePostfix, ""); break;
                            case PersonStartFilters.Person: outputFileNameDocx = outputFileNameDocx.Replace(_templatePostfix, "_" + ((Person)personStartFilterParameter).FirstName + "_" + ((Person)personStartFilterParameter).Name); break;
                            case PersonStartFilters.SwimmingStyle: outputFileNameDocx = outputFileNameDocx.Replace(_templatePostfix, "_" + EnumCoreToLocalizedString.Convert((SwimmingStyles)personStartFilterParameter)); break;
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

        /// <summary>
        /// Create a single certificate for a person start and save it to the output folder.
        /// </summary>
        /// <param name="personStart"><see cref="PersonStart"/> for which a certificate is created</param>
        /// <param name="createPdf">True to also create a .pdf file</param>
        /// <param name="outputFolder">Folder in which the certificate will be created</param>
        /// <returns>1 if creation was successful; otherwise 0</returns>
        private Task<int> createSingleCertificate(PersonStart personStart, bool createPdf = true, string outputFolder = "")
        {
            return Task.Run(() =>
            {
                if (string.IsNullOrEmpty(outputFolder))
                {
                    string documentOutputFolder = getDocumentPathAbsolute(WorkspaceSettings.SETTING_DOCUMENT_CREATION_OUTPUT_FOLDER);
                    outputFolder = documentOutputFolder;
                }
                if (!Directory.Exists(outputFolder))
                {
                    Directory.CreateDirectory(outputFolder);
                }
                string certificateTemplatePath = getDocumentPathAbsolute(WorkspaceSettings.SETTING_DOCUMENT_CREATION_CERTIFICATE_TEMPLATE_PATH);
                string outputFile = Path.Combine(outputFolder, $"{personStart.PersonObj?.FirstName}_{personStart.PersonObj?.Name}_{personStart.Style}.docx");

                if (personStart.CompetitionObj == null)
                {
                    return 0;
                }

                DocXPlaceholderHelper.TextPlaceholders textPlaceholders = createTextPlaceholdersFromPersonStart(personStart);                
                DocXPlaceholderHelper.ReplaceTextPlaceholders(certificateTemplatePath, outputFile, textPlaceholders);
                insertCompetitionYearPlaceholderValue(outputFile, outputFile);

                if (createPdf)
                {
                    convertToPdf(outputFile);
                }
                return 1;       // This method always creates one certificate, so we return 1 to indicate success
            });
        }

        /// <summary>
        /// Create text placeholders from a <see cref="PersonStart"/> object.
        /// </summary>
        /// <param name="personStarts"><see cref="PersonStart"/> object</param>
        /// <returns><see cref="DocXPlaceholderHelper.TextPlaceholders"/></returns>
        private DocXPlaceholderHelper.TextPlaceholders createTextPlaceholdersFromPersonStart(PersonStart personStart)
        {
            DocXPlaceholderHelper.TextPlaceholders textPlaceholders = new DocXPlaceholderHelper.TextPlaceholders();
            foreach (string placeholder in Placeholders_Name) { textPlaceholders.Add(placeholder, personStart.PersonObj?.FirstName + " " + personStart.PersonObj?.Name); }
            foreach (string placeholder in Placeholders_BirthYear) { textPlaceholders.Add(placeholder, personStart.PersonObj?.BirthYear.ToString()); }
            foreach (string placeholder in Placeholders_Distance) { textPlaceholders.Add(placeholder, personStart.CompetitionObj.Distance.ToString() + "m"); }
            foreach (string placeholder in Placeholders_SwimmingStyle) { textPlaceholders.Add(placeholder, EnumCoreToLocalizedString.Convert(personStart.Style)); }
            foreach (string placeholder in Placeholders_CompetitionID) { textPlaceholders.Add(placeholder, personStart.CompetitionObj.ID.ToString()); }
            foreach (string placeholder in Placeholders_Score) { textPlaceholders.Add(placeholder, personStart.Score.ToString("F2")); }
            return textPlaceholders;
        }

        #endregion

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Overview List Creation

        /// <summary>
        /// Create an overview list of all person starts in the database and save it to the output folder.
        /// </summary>
        /// <param name="createPdf">True to also create a .pdf file</param>
        /// <returns><see cref="Task"/> that can be used to run this async</returns>
        public Task CreateOverviewList(bool createPdf = true)
        {
            return Task.Run(() =>
            {
                string overviewListTemplatePath = getDocumentPathAbsolute(WorkspaceSettings.SETTING_DOCUMENT_CREATION_OVERVIEW_LIST_TEMPLATE_PATH);
                string documentOutputFolder = getDocumentPathAbsolute(WorkspaceSettings.SETTING_DOCUMENT_CREATION_OUTPUT_FOLDER);
                if (!Directory.Exists(documentOutputFolder))
                {
                    Directory.CreateDirectory(documentOutputFolder);
                }
                string outputFile = Path.Combine(documentOutputFolder, Path.GetFileNameWithoutExtension(overviewListTemplatePath).Replace(_templatePostfix, "") + ".docx");

                List<PersonStart> personStarts = _personService.GetAllPersonStarts();
                DocXPlaceholderHelper.TablePlaceholders tablePlaceholders = createTablePlaceholdersFromPersonStarts(personStarts);                                
                DocXPlaceholderHelper.ReplaceTablePlaceholders(overviewListTemplatePath, outputFile, tablePlaceholders);

                insertCompetitionYearPlaceholderValue(outputFile, outputFile);

                if (createPdf)
                {
                    convertToPdf(outputFile);
                }
            });
        }

        /// <summary>
        /// Create table placeholders from a list of <see cref="PersonStart"/> objects.
        /// </summary>
        /// <param name="personStarts"><see cref="PersonStart"/> objects</param>
        /// <returns><see cref="DocXPlaceholderHelper.TablePlaceholders"/></returns>
        private DocXPlaceholderHelper.TablePlaceholders createTablePlaceholdersFromPersonStarts(List<PersonStart> personStarts)
        {
            List<string> names = new List<string>();
            List<string> birthYears = new List<string>();
            List<string> styles = new List<string>();
            List<string> distances = new List<string>();
            List<string> competitions = new List<string>();
            List<string> scores = new List<string>();
            foreach (PersonStart personStart in personStarts)
            {
                names.Add(personStart.PersonObj?.FirstName + " " + personStart.PersonObj?.Name);
                birthYears.Add(personStart.PersonObj?.BirthYear.ToString() ?? "?");
                styles.Add(EnumCoreToLocalizedString.Convert(personStart.Style));
                distances.Add(personStart.CompetitionObj?.Distance.ToString() + "m" ?? "?");
                competitions.Add(personStart.CompetitionObj?.ID.ToString() ?? "?");
                scores.Add(personStart.Score.ToString("F2"));
            }

            DocXPlaceholderHelper.TablePlaceholders tablePlaceholders = new DocXPlaceholderHelper.TablePlaceholders();
            foreach (string placeholder in Placeholders_Name) { tablePlaceholders.Add(placeholder, names); }
            foreach (string placeholder in Placeholders_BirthYear) { tablePlaceholders.Add(placeholder, birthYears); }
            foreach (string placeholder in Placeholders_SwimmingStyle) { tablePlaceholders.Add(placeholder, styles); }
            foreach (string placeholder in Placeholders_Distance) { tablePlaceholders.Add(placeholder, distances); }
            foreach (string placeholder in Placeholders_CompetitionID) { tablePlaceholders.Add(placeholder, competitions); }
            foreach (string placeholder in Placeholders_Score) { tablePlaceholders.Add(placeholder, scores); }
            return tablePlaceholders;
        }

        #endregion

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Race Start List Creation

        /// <summary>
        /// Create a list with all races and the planned order.
        /// </summary>
        /// <param name="createPdf">True to also create a .pdf file</param>
        /// <returns><see cref="Task"/> that can be used to run this async</returns>
        public Task CreateRaceStartList(bool createPdf = true)
        {
            return Task.Run(() =>
            {
                RacesVariant racesVariant = _raceService.PersistedRacesVariant;
                if (racesVariant == null) { return; }

                string raceStartListTemplatePath = getDocumentPathAbsolute(WorkspaceSettings.SETTING_DOCUMENT_CREATION_RACE_START_LIST_TEMPLATE_PATH);
                string documentOutputFolder = getDocumentPathAbsolute(WorkspaceSettings.SETTING_DOCUMENT_CREATION_OUTPUT_FOLDER);
                if (!Directory.Exists(documentOutputFolder))
                {
                    Directory.CreateDirectory(documentOutputFolder);
                }
                string outputFile = Path.Combine(documentOutputFolder, Path.GetFileNameWithoutExtension(raceStartListTemplatePath).Replace(_templatePostfix, "") + ".docx");

                DocXPlaceholderHelper.TablePlaceholders tablePlaceholders = createTablePlaceholdersFromRaceVariant(racesVariant);
                DocXPlaceholderHelper.ReplaceTablePlaceholders(raceStartListTemplatePath, outputFile, tablePlaceholders);
                insertCompetitionYearPlaceholderValue(outputFile, outputFile);

                if (createPdf)
                {
                    convertToPdf(outputFile);
                }
            });
        }

        /// <summary>
        /// Create table placeholders from a <see cref="RacesVariant"/>.
        /// </summary>
        /// <param name="racesVariant"><see cref="RacesVariant"/> object</param>
        /// <returns><see cref="DocXPlaceholderHelper.TablePlaceholders"/></returns>
        private DocXPlaceholderHelper.TablePlaceholders createTablePlaceholdersFromRaceVariant(RacesVariant racesVariant)
        {
            List<string> competitions = new List<string>();
            List<string> styles = new List<string>();
            List<string> distances = new List<string>();
            List<List<string>> names = new List<List<string>>();
            List<List<string>> birthYears = new List<List<string>>();
            int maxPersonsInRace = 0;
            foreach (Race race in racesVariant.Races)
            {
                competitions.Add(string.Join(", ", race.Starts.Select(s => s.CompetitionObj?.ID.ToString() ?? "?")).TrimEnd(',', ' '));
                styles.Add(EnumCoreToLocalizedString.Convert(race.Style));
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
            foreach (string placeholder in Placeholders_SwimmingStyle) { tablePlaceholders.Add(placeholder, styles); }
            foreach (string placeholder in Placeholders_Distance) { tablePlaceholders.Add(placeholder, distances); }
            for (int i = 0; i < Math.Max(numSwimLanes, maxPersonsInRace); i++)
            {
                foreach (string placeholder in Placeholders_Name) { tablePlaceholders.Add(placeholder + (i + 1), names.Select(innerNames => innerNames.Count > i ? innerNames[i] : "").ToList()); }
                foreach (string placeholder in Placeholders_BirthYear) { tablePlaceholders.Add(placeholder + (i + 1), birthYears.Select(innerBirthYear => innerBirthYear.Count > i ? innerBirthYear[i] : "").ToList()); }
            }
            return tablePlaceholders;
        }

        #endregion

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Result List Creation

        /// <summary>
        /// Create a list with the overall result.
        /// </summary>
        /// <param name="createPdf">True to also create a .pdf file</param>
        /// <returns><see cref="Task"/> that can be used to run this async</returns>
        public Task CreateResultList(bool createPdf = true)
        {
            return Task.Run(() =>
            {
                string resultTemplatePath = getDocumentPathAbsolute(WorkspaceSettings.SETTING_DOCUMENT_CREATION_RESULT_LIST_TEMPLATE_PATH);
                string documentOutputFolder = getDocumentPathAbsolute(WorkspaceSettings.SETTING_DOCUMENT_CREATION_OUTPUT_FOLDER);
                if (!Directory.Exists(documentOutputFolder))
                {
                    Directory.CreateDirectory(documentOutputFolder);
                }
                string outputFile = Path.Combine(documentOutputFolder, Path.GetFileNameWithoutExtension(resultTemplatePath).Replace(_templatePostfix, "") + ".docx");

                List<Person> sortedPersons = _scoreService.GetPersonsSortedByScore(ResultTypes.Overall);
                _scoreService.UpdateResultListPlacesForAllPersons();
                DocXPlaceholderHelper.TablePlaceholders tablePlaceholders = createTablePlaceholdersFromPersons(sortedPersons);
                DocXPlaceholderHelper.ReplaceTablePlaceholders(resultTemplatePath, outputFile, tablePlaceholders);

                insertCompetitionYearPlaceholderValue(outputFile, outputFile);

                if (createPdf)
                {
                    convertToPdf(outputFile);
                }
            });
        }

        /// <summary>
        /// Create table placeholders from a list of <see cref="Person"/> objects.
        /// </summary>
        /// <param name="persons"><see cref="Person"/> objects</param>
        /// <returns><see cref="DocXPlaceholderHelper.TablePlaceholders"/></returns>
        private DocXPlaceholderHelper.TablePlaceholders createTablePlaceholdersFromPersons(List<Person> persons)
        {
            List<string> names = new List<string>();
            List<string> birthYears = new List<string>();
            List<string> styles = new List<string>();
            List<string> distances = new List<string>();
            List<string> competitions = new List<string>();
            List<string> scores = new List<string>();
            List<string> resultListPlaces = new List<string>();
            foreach (Person person in persons)
            {
                names.Add(person.FirstName + " " + person.Name);
                birthYears.Add(person.BirthYear.ToString() ?? "?");
                styles.Add(EnumCoreToLocalizedString.Convert(person.HighestScoreStyle));
                distances.Add(person.HighestScoreCompetition?.Distance.ToString() + "m" ?? "?");
                competitions.Add(person.HighestScoreCompetition?.ID.ToString() ?? "?");
                scores.Add(person.HighestScore.ToString("F2"));
                resultListPlaces.Add(person?.ResultListPlace == 0 ? "-" : person.ResultListPlace.ToString() ?? "?");
            }

            DocXPlaceholderHelper.TablePlaceholders tablePlaceholders = new DocXPlaceholderHelper.TablePlaceholders();
            foreach (string placeholder in Placeholders_Name) { tablePlaceholders.Add(placeholder, names); }
            foreach (string placeholder in Placeholders_BirthYear) { tablePlaceholders.Add(placeholder, birthYears); }
            foreach (string placeholder in Placeholders_SwimmingStyle) { tablePlaceholders.Add(placeholder, styles); }
            foreach (string placeholder in Placeholders_Distance) { tablePlaceholders.Add(placeholder, distances); }
            foreach (string placeholder in Placeholders_CompetitionID) { tablePlaceholders.Add(placeholder, competitions); }
            foreach (string placeholder in Placeholders_Score) { tablePlaceholders.Add(placeholder, scores); } 
            foreach (string placeholder in Placeholders_ResultListPlace) { tablePlaceholders.Add(placeholder, resultListPlaces); }
            return tablePlaceholders;
        }

        #endregion
    }
}
