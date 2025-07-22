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
        /// String marker for placeholders in the template files. The placeholder must be enclosed by this marker.
        /// e.g. PlaceholderMarker = "%" --> %PLACEHOLDER%
        /// </summary>
        public const string PlaceholderMarker = "%";

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Placeholder Lists

        /// <summary>
        /// List with all placeholders that can be used in the template to insert the competition year.
        /// </summary>
        public static List<string> Placeholders_CompetitionYear = new List<string>() { "Jahr", "J", "CompetitionYear", "Year", "Y" };
        /// <summary>
        /// List with all placeholders that can be used in the template to insert the name of a person.
        /// </summary>
        public static List<string> Placeholders_Name = new List<string>() { "Name", "N" };
        /// <summary>
        /// List with all placeholders that can be used in the template to insert the gender of a person as string.
        /// </summary>
        public static List<string> Placeholders_Gender = new List<string>() { "Geschlecht", "Gender", "G" };
        /// <summary>
        /// List with all placeholders that can be used in the template to insert the gender of a person as symbol.
        /// </summary>
        public static List<string> Placeholders_GenderSymbol = new List<string>() { "GeschlechtS", "GenderS", "GS" };
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

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Placeholder Keys

        /// <summary>
        /// Key for the competition year placeholders
        /// </summary>
        public const string PLACEHOLDER_KEY_COMPETITION_YEAR = "CompetitionYear";
        /// <summary>
        /// Key for the name placeholders
        /// </summary>
        public const string PLACEHOLDER_KEY_NAME = "Name";
        /// <summary>
        /// Key for the gender placeholders
        /// </summary>
        public const string PLACEHOLDER_KEY_GENDER = "Gender";
        /// <summary>
        /// Key for the gender symbol placeholders
        /// </summary>
        public const string PLACEHOLDER_KEY_GENDER_SYMBOL = "GenderSymbol";
        /// <summary>
        /// Key for the birth year placeholders
        /// </summary>
        public const string PLACEHOLDER_KEY_BIRTH_YEAR = "BirthYear";
        /// <summary>
        /// Key for the distance placeholders
        /// </summary>
        public const string PLACEHOLDER_KEY_DISTANCE = "Distance";
        /// <summary>
        /// Key for the swimming style placeholders
        /// </summary>
        public const string PLACEHOLDER_KEY_SWIMMING_STYLE = "SwimmingStyle";
        /// <summary>
        /// Key for the competition ID placeholders
        /// </summary>
        public const string PLACEHOLDER_KEY_COMPETITION_ID = "CompetitionID";
        /// <summary>
        /// Key for the score placeholders
        /// </summary>
        public const string PLACEHOLDER_KEY_SCORE = "Score";
        /// <summary>
        /// Key for the result list place placeholders
        /// </summary>
        public const string PLACEHOLDER_KEY_RESULT_LIST_PLACE = "ResultListPlace";

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Dictionary that maps placeholder keys to their corresponding lists of placeholders.
        /// </summary>
        public static Dictionary<string, List<string>> PlaceholderDict = new Dictionary<string, List<string>>()
        {
            { PLACEHOLDER_KEY_COMPETITION_YEAR, Placeholders_CompetitionYear },
            { PLACEHOLDER_KEY_NAME, Placeholders_Name },
            { PLACEHOLDER_KEY_GENDER, Placeholders_Gender },
            { PLACEHOLDER_KEY_GENDER_SYMBOL, Placeholders_GenderSymbol },
            { PLACEHOLDER_KEY_BIRTH_YEAR, Placeholders_BirthYear },
            { PLACEHOLDER_KEY_DISTANCE, Placeholders_Distance },
            { PLACEHOLDER_KEY_SWIMMING_STYLE, Placeholders_SwimmingStyle },
            { PLACEHOLDER_KEY_COMPETITION_ID, Placeholders_CompetitionID },
            { PLACEHOLDER_KEY_SCORE, Placeholders_Score },
            { PLACEHOLDER_KEY_RESULT_LIST_PLACE, Placeholders_ResultListPlace }
        };
    }
}
