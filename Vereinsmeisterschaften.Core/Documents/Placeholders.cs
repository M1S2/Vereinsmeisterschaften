namespace Vereinsmeisterschaften.Core.Documents
{
    /// <summary>
    /// Static class that contains lists of placeholders that can be used in templates for documents.
    /// </summary>
    public static class Placeholders
    {
        #region Placeholder Lists

        /// <summary>
        /// List with all placeholders that can be used in the template to insert the competition year.
        /// </summary>
        public static List<string> Placeholders_CompetitionYear = new List<string>() { "Jahr", "J", "CompetitionYear", "Year", "Y" };
        /// <summary>
        /// List with all placeholders that can be used in the template to insert the competition date.
        /// </summary>
        public static List<string> Placeholders_CompetitionDate = new List<string>() { "Datum", "WK-Datum", "WKD", "CompetitionDate", "Date" };
        /// <summary>
        /// List with all placeholders that can be used in the template to insert the application version.
        /// </summary>
        public static List<string> Placeholders_AppVersion = new List<string>() { "AppVersion", "Version", "V" };
        /// <summary>
        /// List with all placeholders that can be used in the template to insert the path of the workspace.
        /// </summary>
        public static List<string> Placeholders_WorkspacePath = new List<string>() { "Arbeitsbereich", "WorkspacePath", "Workspace", "A", "WP" };
        /// <summary>
        /// List with all placeholders that can be used in the template to insert the complete name of a person.
        /// </summary>
        public static List<string> Placeholders_Name = new List<string>() { "Name", "N" };
        /// <summary>
        /// List with all placeholders that can be used in the template to insert the first name of a person.
        /// </summary>
        public static List<string> Placeholders_FirstName = new List<string>() { "Vorname", "FirstName", "VN", "FN" };
        /// <summary>
        /// List with all placeholders that can be used in the template to insert the last name of a person.
        /// </summary>
        public static List<string> Placeholders_LastName = new List<string>() { "Nachname", "LastName", "NN", "LN" };
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
        /// <summary>
        /// List with all placeholders that can be used in the template to insert the style of the start with the highest score of a person.
        /// </summary>
        public static List<string> Placeholders_BestStyle = new List<string>() { "BesterStil", "BestStyle", "BS" };
        /// <summary>
        /// List with all placeholders that can be used in the template to insert the race number.
        /// </summary>
        public static List<string> Placeholders_RaceNumber = new List<string>() { "Rennen", "Renn Nr.", "Race", "Race Number", "RNo.", "R" };

        /// <summary>
        /// List with all placeholders that can be used in the template to insert the breaststroke score of a person.
        /// </summary>
        public static List<string> Placeholders_ScoreBreaststroke = new List<string>() { "PunkteBrust", "ScoreBreaststroke", "PB", "SBr" };
        /// <summary>
        /// List with all placeholders that can be used in the template to insert the freestyle score of a person.
        /// </summary>
        public static List<string> Placeholders_ScoreFreestyle = new List<string>() { "PunkteKraul", "ScoreFreestyle", "PK", "SF" };
        /// <summary>
        /// List with all placeholders that can be used in the template to insert the backstroke score of a person.
        /// </summary>
        public static List<string> Placeholders_ScoreBackstroke = new List<string>() { "PunkteRücken", "ScoreBackstroke", "PR", "SBa" };
        /// <summary>
        /// List with all placeholders that can be used in the template to insert the butterfly score of a person.
        /// </summary>
        public static List<string> Placeholders_ScoreButterfly = new List<string>() { "PunkteDelphin", "ScoreButterfly", "PD", "SBu" };
        /// <summary>
        /// List with all placeholders that can be used in the template to insert the medley score of a person.
        /// </summary>
        public static List<string> Placeholders_ScoreMedley = new List<string>() { "PunkteLagen", "ScoreMedley", "PL", "SM" };
        /// <summary>
        /// List with all placeholders that can be used in the template to insert the waterflea score of a person.
        /// </summary>
        public static List<string> Placeholders_ScoreWaterflea = new List<string>() { "PunkteWasserfloh", "ScoreWaterflea", "PW", "SW" };
        /// <summary>
        /// List with all placeholders that can be used in the template to insert the breaststroke time of a person.
        /// </summary>
        public static List<string> Placeholders_TimeBreaststroke = new List<string>() { "ZeitBrust", "TimeBreaststroke", "ZB", "TBr" };
        /// <summary>
        /// List with all placeholders that can be used in the template to insert the freestyle time of a person.
        /// </summary>
        public static List<string> Placeholders_TimeFreestyle = new List<string>() { "ZeitKraul", "TimeFreestyle", "ZK", "TF" };
        /// <summary>
        /// List with all placeholders that can be used in the template to insert the backstroke time of a person.
        /// </summary>
        public static List<string> Placeholders_TimeBackstroke = new List<string>() { "ZeitRücken", "TimeBackstroke", "ZR", "TBa" };
        /// <summary>
        /// List with all placeholders that can be used in the template to insert the butterfly time of a person.
        /// </summary>
        public static List<string> Placeholders_TimeButterfly = new List<string>() { "ZeitDelphin", "TimeButterfly", "ZD", "TBu" };
        /// <summary>
        /// List with all placeholders that can be used in the template to insert the medley time of a person.
        /// </summary>
        public static List<string> Placeholders_TimeMedley = new List<string>() { "ZeitLagen", "TimeMedley", "ZL", "TM" };
        /// <summary>
        /// List with all placeholders that can be used in the template to insert the waterflea time of a person.
        /// </summary>
        public static List<string> Placeholders_TimeWaterflea = new List<string>() { "ZeitWasserfloh", "TimeWaterflea", "ZW", "TW" };

        /// <summary>
        /// List with all placeholders that can be used in the template to insert the analytics gender person male count.
        /// </summary>
        public static List<string> Placeholders_AnalyticsGenderPersonsMaleCount = new List<string>() { "NumberMale", "AnzahlMännlich", "A_NM", "A_AM" };
        /// <summary>
        /// List with all placeholders that can be used in the template to insert the analytics gender person male percentage.
        /// </summary>
        public static List<string> Placeholders_AnalyticsGenderPersonsMalePercentage = new List<string>() { "PercentMale", "ProzentMännlich", "A_PM" };
        /// <summary>
        /// List with all placeholders that can be used in the template to insert the analytics gender person female count.
        /// </summary>
        public static List<string> Placeholders_AnalyticsGenderPersonsFemaleCount = new List<string>() { "NumberFemale", "AnzahlWeiblich", "A_NF", "A_AW" };
        /// <summary>
        /// List with all placeholders that can be used in the template to insert the analytics gender person female percentage.
        /// </summary>
        public static List<string> Placeholders_AnalyticsGenderPersonsFemalePercentage = new List<string>() { "PercentFemale", "ProzentWeiblich", "A_PF", "A_PW" };

        /// <summary>
        /// List with all placeholders that can be used in the template to insert the analytics gender starts male count.
        /// </summary>
        public static List<string> Placeholders_AnalyticsGenderStartsMaleCount = new List<string>() { "NumberStartsMale", "AnzahlStartsMännlich", "A_NSM", "A_ASM" };
        /// <summary>
        /// List with all placeholders that can be used in the template to insert the analytics gender starts male percentage.
        /// </summary>
        public static List<string> Placeholders_AnalyticsGenderStartsMalePercentage = new List<string>() { "PercentStartsMale", "ProzentStartsMännlich", "A_PSM" };
        /// <summary>
        /// List with all placeholders that can be used in the template to insert the analytics gender starts female count.
        /// </summary>
        public static List<string> Placeholders_AnalyticsGenderStartsFemaleCount = new List<string>() { "NumberStartsFemale", "AnzahlStartsWeiblich", "A_NSF", "A_ASW" };
        /// <summary>
        /// List with all placeholders that can be used in the template to insert the analytics gender starts female percentage.
        /// </summary>
        public static List<string> Placeholders_AnalyticsGenderStartsFemalePercentage = new List<string>() { "PercentStartsFemale", "ProzentStartsWeiblich", "A_PSF", "A_PSW" };

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Placeholder Groups

        public const string PLACEHOLDER_GROUP_GENERAL = "General";
        public const string PLACEHOLDER_GROUP_PERSONDETAILS = "Persondetails";
        public const string PLACEHOLDER_GROUP_SCORES = "Scores";
        public const string PLACEHOLDER_GROUP_TIMES = "Times";
        public const string PLACEHOLDER_GROUP_ANALYTICS = "Analytics";

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Placeholder Keys

        public const string PLACEHOLDER_KEY_COMPETITION_YEAR = "CompetitionYear";
        public const string PLACEHOLDER_KEY_COMPETITION_DATE = "CompetitionDate";
        public const string PLACEHOLDER_KEY_APP_VERSION = "AppVersion";
        public const string PLACEHOLDER_KEY_WORKSPACE_PATH = "WorkspacePath";
        public const string PLACEHOLDER_KEY_DISTANCE = "Distance";
        public const string PLACEHOLDER_KEY_SWIMMING_STYLE = "SwimmingStyle";
        public const string PLACEHOLDER_KEY_COMPETITION_ID = "CompetitionID";
        public const string PLACEHOLDER_KEY_RESULT_LIST_PLACE = "ResultListPlace";
        public const string PLACEHOLDER_KEY_BEST_STYLE = "BestStyle";
        public const string PLACEHOLDER_KEY_RACE_NUMBER = "RaceNumber";

        public const string PLACEHOLDER_KEY_NAME = "Name";
        public const string PLACEHOLDER_KEY_FIRSTNAME = "FirstName";
        public const string PLACEHOLDER_KEY_LASTNAME = "LastName";
        public const string PLACEHOLDER_KEY_GENDER = "Gender";
        public const string PLACEHOLDER_KEY_GENDER_SYMBOL = "GenderSymbol";
        public const string PLACEHOLDER_KEY_BIRTH_YEAR = "BirthYear";

        public const string PLACEHOLDER_KEY_SCORE = "Score";
        public const string PLACEHOLDER_KEY_SCOREBREASTSTROKE = "ScoreBreaststroke";
        public const string PLACEHOLDER_KEY_SCOREFREESTYLE = "ScoreFreestyle";
        public const string PLACEHOLDER_KEY_SCOREBACKSTROKE = "ScoreBackstroke";
        public const string PLACEHOLDER_KEY_SCOREBUTTERFLY = "ScoreButterfly";
        public const string PLACEHOLDER_KEY_SCOREMEDLEY = "ScoreMedley";
        public const string PLACEHOLDER_KEY_SCOREWATERFLEA = "ScoreWaterflea";

        public const string PLACEHOLDER_KEY_TIMEBREASTSTROKE = "TimeBreaststroke";
        public const string PLACEHOLDER_KEY_TIMEFREESTYLE = "TimeFreestyle";
        public const string PLACEHOLDER_KEY_TIMEBACKSTROKE = "TimeBackstroke";
        public const string PLACEHOLDER_KEY_TIMEBUTTERFLY = "TimeButterfly";
        public const string PLACEHOLDER_KEY_TIMEMEDLEY = "TimeMedley";
        public const string PLACEHOLDER_KEY_TIMEWATERFLEA = "TimeWaterflea";

        public const string PLACEHOLDER_KEY_ANALYTICS_GENDER_PERSONS_MALE_COUNT = "AnalyticsGenderPersonsMaleCount";
        public const string PLACEHOLDER_KEY_ANALYTICS_GENDER_PERSONS_MALE_PERCENTAGE = "AnalyticsGenderPersonsMalePercentage";
        public const string PLACEHOLDER_KEY_ANALYTICS_GENDER_PERSONS_FEMALE_COUNT = "AnalyticsGenderPersonsFemaleCount";
        public const string PLACEHOLDER_KEY_ANALYTICS_GENDER_PERSONS_FEMALE_PERCENTAGE = "AnalyticsGenderPersonsFemalePercentage";
        public const string PLACEHOLDER_KEY_ANALYTICS_GENDER_STARTS_MALE_COUNT = "AnalyticsGenderStartsMaleCount";
        public const string PLACEHOLDER_KEY_ANALYTICS_GENDER_STARTS_MALE_PERCENTAGE = "AnalyticsGenderStartsMalePercentage";
        public const string PLACEHOLDER_KEY_ANALYTICS_GENDER_STARTS_FEMALE_COUNT = "AnalyticsGenderStartsFemaleCount";
        public const string PLACEHOLDER_KEY_ANALYTICS_GENDER_STARTS_FEMALE_PERCENTAGE = "AnalyticsGenderStartsFemalePercentage";

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Dictionary that maps placeholder keys to a tuple of the group and their corresponding lists of placeholders.
        /// </summary>
        public static Dictionary<string, (string groupName, List<string> placeholders)> PlaceholderDict = new Dictionary<string, (string, List<string>)>()
        {
            { PLACEHOLDER_KEY_COMPETITION_YEAR, (PLACEHOLDER_GROUP_GENERAL, Placeholders_CompetitionYear) },
            { PLACEHOLDER_KEY_COMPETITION_DATE, (PLACEHOLDER_GROUP_GENERAL, Placeholders_CompetitionDate) },
            { PLACEHOLDER_KEY_APP_VERSION, (PLACEHOLDER_GROUP_GENERAL, Placeholders_AppVersion) },
            { PLACEHOLDER_KEY_WORKSPACE_PATH, (PLACEHOLDER_GROUP_GENERAL, Placeholders_WorkspacePath) },
            { PLACEHOLDER_KEY_DISTANCE, (PLACEHOLDER_GROUP_GENERAL, Placeholders_Distance) },
            { PLACEHOLDER_KEY_SWIMMING_STYLE, (PLACEHOLDER_GROUP_GENERAL, Placeholders_SwimmingStyle) },
            { PLACEHOLDER_KEY_COMPETITION_ID, (PLACEHOLDER_GROUP_GENERAL, Placeholders_CompetitionID) },
            { PLACEHOLDER_KEY_BEST_STYLE, (PLACEHOLDER_GROUP_GENERAL, Placeholders_BestStyle) },
            { PLACEHOLDER_KEY_RESULT_LIST_PLACE, (PLACEHOLDER_GROUP_GENERAL, Placeholders_ResultListPlace) },
            { PLACEHOLDER_KEY_RACE_NUMBER, (PLACEHOLDER_GROUP_GENERAL, Placeholders_RaceNumber) },

            { PLACEHOLDER_KEY_NAME, (PLACEHOLDER_GROUP_PERSONDETAILS, Placeholders_Name) },
            { PLACEHOLDER_KEY_FIRSTNAME, (PLACEHOLDER_GROUP_PERSONDETAILS, Placeholders_FirstName) },
            { PLACEHOLDER_KEY_LASTNAME, (PLACEHOLDER_GROUP_PERSONDETAILS, Placeholders_LastName) },
            { PLACEHOLDER_KEY_GENDER, (PLACEHOLDER_GROUP_PERSONDETAILS, Placeholders_Gender) },
            { PLACEHOLDER_KEY_GENDER_SYMBOL, (PLACEHOLDER_GROUP_PERSONDETAILS, Placeholders_GenderSymbol) },
            { PLACEHOLDER_KEY_BIRTH_YEAR, (PLACEHOLDER_GROUP_PERSONDETAILS, Placeholders_BirthYear) },

            { PLACEHOLDER_KEY_SCORE, (PLACEHOLDER_GROUP_SCORES, Placeholders_Score) },
            { PLACEHOLDER_KEY_SCOREBREASTSTROKE, (PLACEHOLDER_GROUP_SCORES, Placeholders_ScoreBreaststroke) },
            { PLACEHOLDER_KEY_SCOREFREESTYLE, (PLACEHOLDER_GROUP_SCORES, Placeholders_ScoreFreestyle) },
            { PLACEHOLDER_KEY_SCOREBACKSTROKE, (PLACEHOLDER_GROUP_SCORES, Placeholders_ScoreBackstroke) },
            { PLACEHOLDER_KEY_SCOREBUTTERFLY, (PLACEHOLDER_GROUP_SCORES, Placeholders_ScoreButterfly) },
            { PLACEHOLDER_KEY_SCOREMEDLEY, (PLACEHOLDER_GROUP_SCORES, Placeholders_ScoreMedley) },
            { PLACEHOLDER_KEY_SCOREWATERFLEA, (PLACEHOLDER_GROUP_SCORES, Placeholders_ScoreWaterflea) },

            { PLACEHOLDER_KEY_TIMEBREASTSTROKE, (PLACEHOLDER_GROUP_TIMES, Placeholders_TimeBreaststroke) },
            { PLACEHOLDER_KEY_TIMEFREESTYLE, (PLACEHOLDER_GROUP_TIMES, Placeholders_TimeFreestyle) },
            { PLACEHOLDER_KEY_TIMEBACKSTROKE, (PLACEHOLDER_GROUP_TIMES, Placeholders_TimeBackstroke) },
            { PLACEHOLDER_KEY_TIMEBUTTERFLY, (PLACEHOLDER_GROUP_TIMES, Placeholders_TimeButterfly) },
            { PLACEHOLDER_KEY_TIMEMEDLEY, (PLACEHOLDER_GROUP_TIMES, Placeholders_TimeMedley) },
            { PLACEHOLDER_KEY_TIMEWATERFLEA, (PLACEHOLDER_GROUP_TIMES, Placeholders_TimeWaterflea) },

            { PLACEHOLDER_KEY_ANALYTICS_GENDER_PERSONS_MALE_COUNT, (PLACEHOLDER_GROUP_ANALYTICS, Placeholders_AnalyticsGenderPersonsMaleCount) },
            { PLACEHOLDER_KEY_ANALYTICS_GENDER_PERSONS_MALE_PERCENTAGE, (PLACEHOLDER_GROUP_ANALYTICS, Placeholders_AnalyticsGenderPersonsMalePercentage) },
            { PLACEHOLDER_KEY_ANALYTICS_GENDER_PERSONS_FEMALE_COUNT, (PLACEHOLDER_GROUP_ANALYTICS, Placeholders_AnalyticsGenderPersonsFemaleCount) },
            { PLACEHOLDER_KEY_ANALYTICS_GENDER_PERSONS_FEMALE_PERCENTAGE, (PLACEHOLDER_GROUP_ANALYTICS, Placeholders_AnalyticsGenderPersonsFemalePercentage) },
            { PLACEHOLDER_KEY_ANALYTICS_GENDER_STARTS_MALE_COUNT, (PLACEHOLDER_GROUP_ANALYTICS, Placeholders_AnalyticsGenderStartsMaleCount) },
            { PLACEHOLDER_KEY_ANALYTICS_GENDER_STARTS_MALE_PERCENTAGE, (PLACEHOLDER_GROUP_ANALYTICS, Placeholders_AnalyticsGenderStartsMalePercentage) },
            { PLACEHOLDER_KEY_ANALYTICS_GENDER_STARTS_FEMALE_COUNT, (PLACEHOLDER_GROUP_ANALYTICS, Placeholders_AnalyticsGenderStartsFemaleCount) },
            { PLACEHOLDER_KEY_ANALYTICS_GENDER_STARTS_FEMALE_PERCENTAGE, (PLACEHOLDER_GROUP_ANALYTICS, Placeholders_AnalyticsGenderStartsFemalePercentage) }
        };
    }
}
