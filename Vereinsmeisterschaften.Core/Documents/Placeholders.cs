using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vereinsmeisterschaften.Core.Documents
{
    /// <summary>
    /// Static class that contains lists of placeholders that can be used in templates for documents.
    /// </summary>
    public static class Placeholders
    {
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
    }
}
