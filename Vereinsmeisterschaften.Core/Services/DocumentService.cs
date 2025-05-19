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
using static Vereinsmeisterschaften.Core.Services.CompetitionService;
using DocXToPdfConverter;

namespace Vereinsmeisterschaften.Core.Services
{
    /// <summary>
    /// Service used to create documents like certificates or start lists
    /// </summary>
    public class DocumentService : IDocumentService
    {
#warning TODO: Replace by more dynamic path
        public string CertificateTemplatePath = @"C:\Users\Markus\Desktop\VM_TestData\Data1\Urkunde_Template.docx";
        public string CertificateOutputFolder = @"C:\Users\Markus\Desktop\VM_TestData\Data1\Urkunden";
        public string OverviewlistTemplatePath = @"C:\Users\Markus\Desktop\VM_TestData\Data1\Gesamtliste_Template.docx";
        public string OverviewlistOutputFolder = @"C:\Users\Markus\Desktop\VM_TestData\Data1";

#warning TODO: Replace by more dynamic path
        private string _libreOfficePath = @"C:\Program Files\LibreOffice\program\soffice.exe";
        private ReportGenerator _reportGenerator;
        
        private Placeholders _placeholders = new Placeholders()
        {
            NewLineTag = "<br/>",
            TextPlaceholderStartTag = "%",
            TextPlaceholderEndTag = "%",
            TablePlaceholderStartTag = "==",
            TablePlaceholderEndTag = "=="
        };

        private IPersonService _personService;

        public DocumentService(IPersonService personService)
        {
            _personService = personService;
            _reportGenerator = new ReportGenerator(_libreOfficePath);
        }

        public void CreateCertificates()
        {
            List<PersonStart> personStarts = _personService.GetAllPersonStarts();
            foreach (PersonStart personStart in personStarts)
            {
                string outputPathPdf = Path.Combine(CertificateOutputFolder, $"{personStart.PersonObj?.FirstName}_{personStart.PersonObj?.Name}_{personStart.Style}.pdf");
                CreateSingleCertificate(personStart, outputPathPdf);
            }
        }
        public void CreateSingleCertificate(PersonStart personStart, string outputFileName)
        {
            _placeholders.TextPlaceholders = new Dictionary<string, string>
            {
                {"Name", personStart.PersonObj?.FirstName + " " + personStart.PersonObj?.Name },
                {"JG", personStart.PersonObj?.BirthYear.ToString() },
                {"Strecke", personStart.CompetitionObj == null ? "?" : personStart.CompetitionObj?.Distance.ToString()  },
                {"Lage", personStart.Style.ToString() },
                {"WK", personStart.CompetitionObj == null ? "?" : personStart.CompetitionObj?.ID.ToString() }
            };

            if (!Directory.Exists(CertificateOutputFolder))
            {
                Directory.CreateDirectory(CertificateOutputFolder);
            }
            _reportGenerator.Convert(CertificateTemplatePath, outputFileName, _placeholders);
        }

        public void CreateOverviewlist()
        {
            List<PersonStart> personStarts = _personService.GetAllPersonStarts();
            List<string> names = new List<string>();
            List<string> styles = new List<string>();
            foreach (PersonStart personStart in personStarts)
            {
                names.Add(personStart.PersonObj?.FirstName + " " + personStart.PersonObj?.Name);
                styles.Add(personStart.Style.ToString());
            }

            _placeholders.TablePlaceholders = new List<Dictionary<string, string[]>>()
            {
                new Dictionary<string, string[]>()
                {
                    {"Name", names.ToArray() },
                    {"Stil", styles.ToArray() }
                }
            };

            string outputPathPdf = Path.Combine(OverviewlistOutputFolder, Path.GetFileNameWithoutExtension(OverviewlistTemplatePath).Replace("_Template", "") + ".pdf");
            _reportGenerator.Convert(OverviewlistTemplatePath, outputPathPdf, _placeholders);
        }

    }
}
