using System.Globalization;
using System.Text.RegularExpressions;

namespace Vereinsmeisterschaften.Core.Models
{
    /// <summary>
    /// Class that offers methods that can be used to parse a rudolph table to objects.
    /// The rudolph table is developed by Klaus Rudolph and contains times for different ages, genders, swimming styles and distances that can be used to compare swimmers.
    /// 
    /// The table can be downloaded from: <see href="https://www.dsv.de/de/leistungs--und-wettkampfsport/schwimmen/wettkampf-national/punktetabellen/"/>
    /// To convert the .pdf to a .csv use the following online converter: <see href="https://pdfcandy.com/de/pdf-to-csv.html"/>
    /// </summary>
    public class RudolphTable
    {
        #region Constants

        public const string FEMALE_MARKER = "weiblich";
        public const string MALE_MARKER = "männlich";
        public const string AGE_MARKER = "Jahre";
        public const string AGE_MARKER_ALTERNATIVE = "Altersklasse";
        public const string OPEN_AGE_MARKER = "offen";
        public const string END_PAGE_MARKER = "Klaus Rudolph";
        public const string TIME_FORMAT_REGEX = @"\d{2}:\d{2},\d{2}";
        public const string TIME_FORMAT_STRING = @"mm\:ss\,ff";
        public const int NUMBER_TIME_COLUMNS_RUDOLPH_TABLE = 17;
        public static readonly List<SwimmingStyles> SWIMMINGSTYLES_PER_TIME_COLUMN = new List<SwimmingStyles>()
        {
            SwimmingStyles.Freestyle,
            SwimmingStyles.Freestyle,
            SwimmingStyles.Freestyle,
            SwimmingStyles.Freestyle,
            SwimmingStyles.Freestyle,
            SwimmingStyles.Freestyle,
            SwimmingStyles.Breaststroke,
            SwimmingStyles.Breaststroke,
            SwimmingStyles.Breaststroke,
            SwimmingStyles.Butterfly,
            SwimmingStyles.Butterfly,
            SwimmingStyles.Butterfly,
            SwimmingStyles.Backstroke,
            SwimmingStyles.Backstroke,
            SwimmingStyles.Backstroke,
            SwimmingStyles.Medley,
            SwimmingStyles.Medley
        };
        public static readonly List<ushort> DISTANCES_PER_TIME_COLUMN = new List<ushort>()
        {
            50,
            100,
            200,
            400,
            800,
            1500,
            50,
            100,
            200,
            50,
            100,
            200,
            50,
            100,
            200,
            200,
            400
        };

        #endregion

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Properties

        /// <summary>
        /// List with all entries of the table parsed from the .csv
        /// </summary>
        public List<RudolphTableEntry> Entries { get; private set; }

        #endregion

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Constructors

        /// <summary>
        /// Empty Constructor for the <see cref="RudolphTable"/>. You must call <see cref="ParseFromCsv(string)"/> later.
        /// </summary>
        public RudolphTable()
        {
        }

        /// <summary>
        /// Constructor for the <see cref="RudolphTable"/> that immediatelly parses the given table .csv
        /// </summary>
        /// <param name="csvFile">Filepath to the .csv file</param>
        public RudolphTable(string csvFile)
        {
            ParseFromCsv(csvFile);
        }

        #endregion

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Parse a .csv file to <see cref="RudolphTableEntry"/> objects.
        /// The .csv must have the following format:
        /// 
        /// GENDER:                     "weiblich","Strecke","Pkt"
        /// AGE:                        "8 Jahre","Punkte","501002004008001500501002005010020050100200200400"
        /// RUDOLPH SCORE 1st TIME:     "20"
        /// TIMES:                      "00:35,12 01:18,79 02:57,1306:13,45 12:56,00 24:44,1000:44,38 01:39,79 03:43,61 00:39,8801:42,82 03:52,8000:40,50 01:30,1303:12,3103:21,8107:16,50"
        /// RUDOLPH SCORE 2nd TIME:     "20"
        /// RUDOLPH SCORE 1st TIME:     "19"
        /// TIMES:                      "00:35,67 01:20,01 02:59,8706:19,23 13:08,00 25:07,0500:45,06 01:41,34 03:47,07 00:40,4901:44,41 03:56,4000:41,12 01:31,5303:15,2903:24,9307:23,25"
        /// RUDOLPH SCORE 2nd TIME:     "19"
        ///                             ...
        /// RUDOLPH SCORE 1st TIME:     "1"
        /// TIMES:                      "00:45,44 01:41,94 03:49,1808:03,18 16:44,00 32:00,1500:57,42 02:09,11 04:49,32 00:51,5902:13,03 05:01,2000:52,40 01:56,6104:08,8204:21,1009:24,75"
        /// RUDOLPH SCORE 2nd TIME:     "1"
        /// END PAGE LINE:              "© Dr.Klaus Rudolph 2020/Basis 20192025/Basis 2024Die Disziplinen 400-1500F, 100/200S, 200R, 400L sind statistisch unzureichend gesichert und sollten zur Leistungseinschätzung nicht herangezogen werden."
        ///                             "Freestyle","Breaststroke","Butterfly","Backstroke","Medley"
        ///                             ...
        /// GENDER:                     "männlich","Strecke","Pkt"
        /// AGE:                        "8 Jahre","Punkte","501002004008001500501002005010020050100200200400"
        /// RUDOLPH SCORE 1st TIME:     "20"
        /// TIMES:                      "00:33,68 01:17,71 02:51,20"
        /// TIMES:                      "06:18,30 13:05,70 24:58,65"
        /// TIMES:                      "00:45,18 01:41,49 03:46,01 00:39,41"
        /// TIMES:                      "01:32,15 04:12,20"
        /// TIMES:                      "00:39,61 01:27,36"
        /// TIMES:                      "03:13,48"
        /// TIMES:                      "03:22,73"
        /// TIMES:                      "07:21,35"
        /// RUDOLPH SCORE 2nd TIME:     "20"
        ///                             ...
        /// RUDOLPH SCORE 1st TIME:     "1"
        /// TIMES:                      "00:43,57 01:40,54 03:41,51"
        /// TIMES:                      "08:09,45 16:56,55 32:18,97"
        /// TIMES:                      "00:58,46 02:11,31 04:52,42 00:50,99"
        /// TIMES:                      "01:59,23 05:26,30"
        /// TIMES:                      "00:51,24 01:53,03"
        /// TIMES:                      "04:10,32"
        /// TIMES:                      "04:22,29"
        /// TIMES:                      "09:31,02"
        /// RUDOLPH SCORE 2nd TIME:     "1"
        ///                             "Die Disziplinen 400-1500F, 100/200S, 200R, 400L sind statistisch unzureichend gesichert und sollten zur Leistungseinschätzung nicht herangezogen werden."
        ///                             "Freestyle","Breaststroke","Butterfly","Backstroke","Medley"
        /// END PAGE LINE:              "© Dr Klaus Rudolph 2025/Basis 2024"
        ///                             ...
        /// </summary>
        /// <param name="csvFile">Filepath to the .csv file</param>
        public void ParseFromCsv(string csvFile)
        {
            Entries = new List<RudolphTableEntry>();

            using(StreamReader reader = new StreamReader(csvFile))
            {
                // Read the .csv content and split it to lines. Remove empty lines. Also remove " from the line entries.
                string csvContent = reader.ReadToEnd();
                List<string> csvLines = csvContent.Split("\n").Select(line => line.Replace("\"", "").Trim()).Where(line => !string.IsNullOrEmpty(line)).ToList();

                RudolphTableEntry baseEntry = new RudolphTableEntry();
                bool wasGenderOrAgeFound = false;
                bool collectRudolphScoreLines = false;
                string currentRudolphScoreTimesStr = "";

                foreach (string line in csvLines)
                {
                    // This is the line that contains the gender
                    if(line.Contains(FEMALE_MARKER) || line.Contains(MALE_MARKER))
                    {
                        baseEntry.Gender = line.Contains(FEMALE_MARKER) ? Genders.Female : Genders.Male;
                        wasGenderOrAgeFound = true;
                    }
                    // This is the line that contains the age
                    else if (line.Contains(AGE_MARKER) || line.Contains(AGE_MARKER_ALTERNATIVE))
                    {
                        string ageStr = "";
                        byte age = 0;
                        Match match = Regex.Match(line, $"(.*) {AGE_MARKER}");
                        Match matchAlternative = Regex.Match(line, $"{AGE_MARKER_ALTERNATIVE} (.*)");
                        if ((match.Success && match.Groups.Count >= 2) || (matchAlternative.Success && matchAlternative.Groups.Count >= 2))
                        {
                            ageStr = match.Success ? match.Groups[1].Value : matchAlternative.Groups[1].Value;
                            if(byte.TryParse(ageStr, out age))
                            {
                                baseEntry.Age = age;
                                wasGenderOrAgeFound = true;
                            }
                        }
                    }
                    // This is the line that contains the open age marker
                    else if (line.Contains(OPEN_AGE_MARKER))
                    {
                        baseEntry.Age = 0;
                        baseEntry.IsOpenAge = true;
                        wasGenderOrAgeFound = true;
                    }
                    // This is a line that contains the end page marker
                    else if(line.Contains(END_PAGE_MARKER))
                    {
                        baseEntry = new RudolphTableEntry() { Gender = baseEntry.Gender, RudolphScore = 0 };      // reuse last gender (if it's not contained on the page)
                        wasGenderOrAgeFound = false;
                    }
                    // This is a line that contains a rudolph score (only a single number between 1 and 20)
                    else if(line.Trim().Length <= 2 && wasGenderOrAgeFound)
                    {
                        byte score = 0;
                        if (byte.TryParse(line.Trim(), out score))
                        {
                            if(baseEntry.RudolphScore != score)
                            {
                                // start of block reached. Start collecting the time strings.
                                collectRudolphScoreLines = true;
                                baseEntry.RudolphScore = score;
                            }
                            else
                            {
                                // end of block reached. Parse the times.
                                collectRudolphScoreLines = false;
                                Entries.AddRange(parseEntriesFromRudolphScoreTimesStr(currentRudolphScoreTimesStr, baseEntry));
                                currentRudolphScoreTimesStr = "";
                            }
                        }
                    }
                    // This is a line that contains times for the current rudolph score
                    else if(collectRudolphScoreLines && wasGenderOrAgeFound)
                    {
                        currentRudolphScoreTimesStr += " " + line;
                    }
                }
            }
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Parse a string containing times to <see cref="RudolphTableEntry"/> objects.
        /// </summary>
        /// <param name="rudolphScoreTimesStr">String with times that must match the <see cref="TIME_FORMAT_REGEX"/></param>
        /// <param name="baseEntry"><see cref="RudolphTableEntry"/> describing the parameters for the current rudolph score</param>
        /// <returns>List with <see cref="RudolphTableEntry"/> objects for the current rudolph score</returns>
        /// <exception cref="Exception">Exception is thrown when the number of times doesn't match <see cref="NUMBER_TIME_COLUMNS_RUDOLPH_TABLE"/></exception>
        private static List<RudolphTableEntry> parseEntriesFromRudolphScoreTimesStr(string rudolphScoreTimesStr, RudolphTableEntry baseEntry)
        {
            List<RudolphTableEntry> rudolphScoreEntries = new List<RudolphTableEntry>();

            MatchCollection timeMatches = Regex.Matches(rudolphScoreTimesStr, TIME_FORMAT_REGEX);
            if (timeMatches.Count != NUMBER_TIME_COLUMNS_RUDOLPH_TABLE)
            {
                throw new Exception("Error while parsing rudolph table: line encountered that contains a wrong number of times.");
            }
            int timeColumnIndex = 0;
            foreach (Match match in timeMatches)
            {
                RudolphTableEntry singleEntry = (RudolphTableEntry)baseEntry.Clone();
                TimeSpan time;
                if (TimeSpan.TryParseExact(match.Value, TIME_FORMAT_STRING, CultureInfo.InvariantCulture, out time))
                {
                    singleEntry.Time = time;
                    singleEntry.SwimmingStyle = SWIMMINGSTYLES_PER_TIME_COLUMN[timeColumnIndex];
                    singleEntry.Distance = DISTANCES_PER_TIME_COLUMN[timeColumnIndex];
                    rudolphScoreEntries.Add(singleEntry);
                }
                timeColumnIndex++;
            }

            return rudolphScoreEntries;
        }

        // ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Try to get a specific <see cref="RudolphTableEntry"/>.
        /// The input is the list returned by <see cref="ParseFromCsv(string)"/>
        /// When the <paramref name="age"/> is too big, return the open age table entry.
        /// </summary>
        /// <param name="rudolphTable">Complete rudolph table with all entries</param>
        /// <param name="gender"><see cref="Genders"/> to search</param>
        /// <param name="age">Age to search</param>
        /// <param name="swimmingStyle"><see cref="SwimmingStyles"/> to search</param>
        /// <param name="distance">Distance to search</param>
        /// <param name="rudolphScore">Rudolph Score to search</param>
        /// <returns>Found <see cref="RudolphTableEntry"/> if found or <see langword="null"/></returns>
        public RudolphTableEntry GetEntryByParameters(Genders gender, byte age, SwimmingStyles swimmingStyle, ushort distance, byte rudolphScore)
        {
            List<RudolphTableEntry> entries = Entries.Where(e => e.Gender == gender && e.SwimmingStyle == swimmingStyle && e.Distance == distance && e.RudolphScore == rudolphScore).ToList();
            if(entries == null || entries.Count == 0) { return null; }

            byte maxAgeEntries = entries.Max(e => e.Age);
            if (age > maxAgeEntries)
            {
                return entries.FirstOrDefault(e => e.IsOpenAge);
            }
            else
            {
                return entries.FirstOrDefault(e => e.Age == age);
            }
        }
    }
}
