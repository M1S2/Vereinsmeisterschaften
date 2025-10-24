using CommunityToolkit.Mvvm.ComponentModel;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Helpers;

namespace Vereinsmeisterschaften.Core.Models
{
    /// <summary>
    /// Class describing a person.
    /// </summary>
    public class Person : ObservableObject, IEquatable<Person>, ICloneable
    {
        #region Constructors

        /// <summary>
        /// Constructor for a new Person object.
        /// </summary>
        public Person()
        {
            Starts = new Dictionary<SwimmingStyles, PersonStart>();
            List<SwimmingStyles> availableSwimmingStyles = Enum.GetValues(typeof(SwimmingStyles)).Cast<SwimmingStyles>().ToList();
            availableSwimmingStyles.Remove(SwimmingStyles.Unknown);
            foreach(SwimmingStyles style in availableSwimmingStyles)
            {
                Starts.Add(style, null);

                // prepare a PersonStart object for each style and store it in the available starts dictionary
                PersonStart newStart = new PersonStart() { Style = style, PersonObj = this };
                newStart.PropertyChanged += PersonStart_PropertyChanged;
                _availableStartsDict.Add(style, newStart);
            }
        }

        /// <summary>
        /// Clone constructor for a Person object.
        /// </summary>
        /// <param name="other">Object to clone</param>
        public Person(Person other) : this()
        {
            if (other == null) { return; }
            this.Name = other.Name;
            this.FirstName = other.FirstName;
            this.Gender = other.Gender;
            this.BirthYear = other.BirthYear;

            // Deep clone of the Starts dictionary
            this.Starts = other.Starts.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.Clone() as PersonStart);
        }

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Basic properties

        private string _name = string.Empty;
        /// <summary>
        /// Last name of the person.
        /// </summary>
        [FileServiceOrder]
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _firstName = string.Empty;
        /// <summary>
        /// First name of the person.
        /// </summary>
        [FileServiceOrder]
        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        private Genders _gender;
        /// <summary>
        /// Gender of the person.
        /// </summary>
        [FileServiceOrder]
        public Genders Gender
        {
            get => _gender;
            set => SetProperty(ref _gender, value);
        }

        private UInt16 _birthYear;
        /// <summary>
        /// Birth year of the person.
        /// </summary>
        [FileServiceOrder]
        public UInt16 BirthYear
        {
            get => _birthYear;
            set => SetProperty(ref _birthYear, value);
        }

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Competition related properties

        private Dictionary<SwimmingStyles, Competition> _availableCompetitions = new Dictionary<SwimmingStyles, Competition>();
        /// <summary>
        /// Dictionary with all available <see cref="Competition"/> of the person
        /// </summary>
        [FileServiceIgnore]
        public Dictionary<SwimmingStyles, Competition> AvailableCompetitions
        {
            get => _availableCompetitions;
            set
            {
                if (SetProperty(ref _availableCompetitions, value))
                {
                    OnPropertyChanged(nameof(AvailableCompetitionsFlags));
                }
            }
        }

        /// <summary>
        /// Representation for the <see cref="AvailableCompetitions"/> as boolean flags (true = when competition is available, false = when Competition is null)
        /// </summary>
        [FileServiceIgnore]
        public Dictionary<SwimmingStyles, bool> AvailableCompetitionsFlags
        {
            get
            {
                Dictionary<SwimmingStyles, bool> availableCompetitionsFlags = new Dictionary<SwimmingStyles, bool>();
                foreach (KeyValuePair<SwimmingStyles, Competition> kvp in AvailableCompetitions)
                {
                    availableCompetitionsFlags.Add(kvp.Key, kvp.Value != null);
                }
                return availableCompetitionsFlags;
            }
        }

        /// <summary>
        /// Get the competition IDs for the <see cref="AvailableCompetitions"/> (-1 = when Competition is null)
        /// </summary>
        [FileServiceIgnore]
        public Dictionary<SwimmingStyles, int> AvailableCompetitionsIDs
        {
            get
            {
                Dictionary<SwimmingStyles, int> availableCompetitionsIDs = new Dictionary<SwimmingStyles, int>();
                foreach (KeyValuePair<SwimmingStyles, Competition> kvp in AvailableCompetitions)
                {
                    availableCompetitionsIDs.Add(kvp.Key, kvp.Value?.Id ?? -1);
                }
                return availableCompetitionsIDs;
            }
        }

        private Dictionary<SwimmingStyles, bool> _isUsingMaxAgeCompetitionDict = new Dictionary<SwimmingStyles, bool>();
        /// <summary>
        /// Dictionary with flags if the max age competition will be used for the person and the style
        /// </summary>
        [FileServiceIgnore]
        public Dictionary<SwimmingStyles, bool> IsUsingMaxAgeCompetitionDict
        {
            get => _isUsingMaxAgeCompetitionDict;
            set => SetProperty(ref _isUsingMaxAgeCompetitionDict, value);
        }

        private Dictionary<SwimmingStyles, bool> _isUsingExactAgeCompetitionDict = new Dictionary<SwimmingStyles, bool>();
        /// <summary>
        /// Dictionary with flags if the exact age competition will be used for the person and the style
        /// </summary>
        [FileServiceIgnore]
        public Dictionary<SwimmingStyles, bool> IsUsingExactAgeCompetitionDict
        {
            get => _isUsingExactAgeCompetitionDict;
            set => SetProperty(ref _isUsingExactAgeCompetitionDict, value);
        }

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Starts management

        /// <summary>
        /// This dictionary holds all available starts of the person (no matter if assigned or not).
        /// The <see cref="Starts"/> dictionary holds only the assigned starts."/>
        /// </summary>
        private Dictionary<SwimmingStyles, PersonStart> _availableStartsDict = new Dictionary<SwimmingStyles, PersonStart>();

        private Dictionary<SwimmingStyles, PersonStart> _starts = new Dictionary<SwimmingStyles, PersonStart>();
        /// <summary>
        /// Dictionary with all starts of the person
        /// </summary>
        [FileServiceIgnore]
        public Dictionary<SwimmingStyles, PersonStart> Starts
        {
            get => _starts;
            set => SetProperty(ref _starts, value);
        }

        /// <summary>
        /// True if at least one of the starts in the <see cref="Starts"/> dictionary is not null
        /// </summary>
        [FileServiceIgnore]
        public bool HasStarts => Starts?.Values?.Any(s => s != null) ?? false;

        /// <summary>
        /// Get the first start in the list of starts of the person that matches the style
        /// </summary>
        /// <param name="style">Requested <see cref="SwimmingStyles"/></param>
        /// <returns>Matching <see cref="PersonStart"/> or <see langword="null"/></returns>
        public PersonStart GetStartByStyle(SwimmingStyles style)
        {
            return Starts[style];
        }

        /// <summary>
        /// Set the flag if the start is available
        /// </summary>
        /// <param name="style">Requested <see cref="SwimmingStyles"/></param>
        /// <param name="available">If true and the style isn't available yet, add it; If false and the style is already available, remove it</param>
        public void SetStartAvailable(SwimmingStyles style, bool available)
        {
            PersonStart start = GetStartByStyle(style);
            if (start == null && available)
            {
                Starts[style] = _availableStartsDict[style];
            }
            else if (start != null && !available)
            {
                Starts[style] = null;
            }
            OnPropertyChanged(nameof(Starts));
            OnPropertyChanged(nameof(HasStarts));
            OnPropertyChanged(nameof(IsActive));
        }

        /// <summary>
        /// Set the time for the matching start if available
        /// </summary>
        /// <param name="style">Requested <see cref="SwimmingStyles"/></param>
        /// <param name="time">Time to set for the start</param>
        public void SetStartTime(SwimmingStyles style, TimeSpan time)
        {
            PersonStart start = GetStartByStyle(style);
            if (start == null) { return; }
            start.Time = time;
        }

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Properties derived from the starts

        /// <summary>
        /// This is the start in the <see cref="Starts"/> dictionary with the highest score value
        /// </summary>
        [FileServiceIgnore]
        public double HighestScore => Starts?.Values?.Where(s => s != null && s.IsActive)?.OrderByDescending(s => s.Score).FirstOrDefault()?.Score ?? 0;

        /// <summary>
        /// This is the start in the <see cref="Starts"/> dictionary for which the highest score value was reached
        /// </summary>
        [FileServiceIgnore]
        public SwimmingStyles HighestScoreStyle => Starts?.Values?.Where(s => s != null && s.IsActive)?.OrderByDescending(s => s.Score).FirstOrDefault()?.Style ?? SwimmingStyles.Unknown;

        /// <summary>
        /// This is the start in the <see cref="Starts"/> dictionary for which the highest score value was reached
        /// </summary>
        [FileServiceIgnore]
        public Competition HighestScoreCompetition => Starts?.Values?.Where(s => s != null && s.IsActive)?.OrderByDescending(s => s.Score).FirstOrDefault()?.CompetitionObj;

        /// <summary>
        /// Check if at least one of the starts in the <see cref="Starts"/> dictionary is active.
        /// Or set all starts to active/inactive.
        /// </summary>
        [FileServiceIgnore]
        public bool IsActive
        {
            get => Starts?.Values?.Where(s => s != null)?.Any(s => s.IsActive) ?? false;
            set
            {
                if (Starts == null) { return; }
                Starts?.Values?.Where(s => s != null)?.ToList()?.ForEach(s => s.IsActive = value);
                OnPropertyChanged(nameof(IsActive));
            }
        }

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Other properties

        private bool _hasDuplicates = false;
        /// <summary>
        /// This flag indicates if there are duplicates of this person in the person list.
        /// It is updated by the <see cref="Services.PersonService"/> when loading or modifying the person list.
        /// </summary>
        [FileServiceIgnore]
        public bool HasDuplicates
        {
            get => _hasDuplicates;
            set => SetProperty(ref _hasDuplicates, value);
        }

        private int _resultListPlace = 0;
        /// <summary>
        /// This is the place in the overall result list of the person.
        /// This is 1-based. 0 means not assigned yet or not available.
        /// </summary>
        [FileServiceIgnore]
        public int ResultListPlace
        {
            get => _resultListPlace;
            set => SetProperty(ref _resultListPlace, value);
        }

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region PersonStart PropertyChanged event handler

        private void PersonStart_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Starts));
            OnPropertyChanged(nameof(HighestScore));
            OnPropertyChanged(nameof(HighestScoreStyle));
            OnPropertyChanged(nameof(HighestScoreCompetition));
            switch(e.PropertyName)
            {
                case nameof(PersonStart.IsActive):
                    OnPropertyChanged(nameof(IsActive));
                    break;
                default: break;
            }
        }

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Style Properties for faster access and file saving

        /// <summary>
        /// Starting with Breaststroke
        /// </summary>
        [FileServiceOrder]
        public bool Breaststroke
        {
            get => GetStartByStyle(SwimmingStyles.Breaststroke) != null;
            set { SetStartAvailable(SwimmingStyles.Breaststroke, value); OnPropertyChanged(); }
        }

        /// <summary>
        /// Starting with Freestyle
        /// </summary>
        [FileServiceOrder]
        public bool Freestyle
        {
            get => GetStartByStyle(SwimmingStyles.Freestyle) != null;
            set { SetStartAvailable(SwimmingStyles.Freestyle, value); OnPropertyChanged(); }
        }

        /// <summary>
        /// Starting with Backstroke
        /// </summary>
        [FileServiceOrder]
        public bool Backstroke
        {
            get => GetStartByStyle(SwimmingStyles.Backstroke) != null;
            set { SetStartAvailable(SwimmingStyles.Backstroke, value); OnPropertyChanged(); }
        }

        /// <summary>
        /// Starting with Butterfly
        /// </summary>
        [FileServiceOrder]
        public bool Butterfly
        {
            get => GetStartByStyle(SwimmingStyles.Butterfly) != null;
            set { SetStartAvailable(SwimmingStyles.Butterfly, value); OnPropertyChanged(); }
        }

        /// <summary>
        /// Starting with Medley
        /// </summary>
        [FileServiceOrder]
        public bool Medley
        {
            get => GetStartByStyle(SwimmingStyles.Medley) != null;
            set { SetStartAvailable(SwimmingStyles.Medley, value); OnPropertyChanged(); }
        }

        /// <summary>
        /// Starting with WaterFlea
        /// </summary>
        [FileServiceOrder]
        public bool WaterFlea
        {
            get => GetStartByStyle(SwimmingStyles.WaterFlea) != null;
            set { SetStartAvailable(SwimmingStyles.WaterFlea, value); OnPropertyChanged(); }
        }

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Time Properties for faster access and file saving

        /// <summary>
        /// Time for Breaststroke
        /// </summary>
        [FileServiceOrder]
        public TimeSpan BreaststrokeTime
        {
            get => GetStartByStyle(SwimmingStyles.Breaststroke)?.Time ?? new TimeSpan();
            set { SetStartTime(SwimmingStyles.Breaststroke, value); OnPropertyChanged(); }
        }

        /// <summary>
        /// Time for Freestyle
        /// </summary>
        [FileServiceOrder]
        public TimeSpan FreestyleTime
        {
            get => GetStartByStyle(SwimmingStyles.Freestyle)?.Time ?? new TimeSpan();
            set { SetStartTime(SwimmingStyles.Freestyle, value); OnPropertyChanged(); }
        }

        /// <summary>
        /// Time for Backstroke
        /// </summary>
        [FileServiceOrder]
        public TimeSpan BackstrokeTime
        {
            get => GetStartByStyle(SwimmingStyles.Backstroke)?.Time ?? new TimeSpan();
            set { SetStartTime(SwimmingStyles.Backstroke, value); OnPropertyChanged(); }
        }

        /// <summary>
        /// Time for Butterfly
        /// </summary>
        [FileServiceOrder]
        public TimeSpan ButterflyTime
        {
            get => GetStartByStyle(SwimmingStyles.Butterfly)?.Time ?? new TimeSpan();
            set { SetStartTime(SwimmingStyles.Butterfly, value); OnPropertyChanged(); }
        }

        /// <summary>
        /// Time for Medley
        /// </summary>
        [FileServiceOrder]
        public TimeSpan MedleyTime
        {
            get => GetStartByStyle(SwimmingStyles.Medley)?.Time ?? new TimeSpan();
            set { SetStartTime(SwimmingStyles.Medley, value); OnPropertyChanged(); }
        }

        /// <summary>
        /// Time for WaterFlea
        /// </summary>
        [FileServiceOrder]
        public TimeSpan WaterFleaTime
        {
            get => GetStartByStyle(SwimmingStyles.WaterFlea)?.Time ?? new TimeSpan();
            set { SetStartTime(SwimmingStyles.WaterFlea, value); OnPropertyChanged(); }
        }

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region SetPropertyFromString helper

        /// <summary>
        /// String used to mark a start as inactive in the file (case-insensitive)
        /// </summary>
        public const string START_INACTIVE_MARKER_STRING = "i";

        /// <summary>
        /// Set the requested property in the <see cref="Person"/> object by parsing the given string value
        /// </summary>
        /// <param name="dataObj"><see cref="Person"/> in which to set the property</param>
        /// <param name="propertyName">Name of the property to set</param>
        /// <param name="value">String value that will be parsed and set to the property</param>
        public static void SetPropertyFromString(Person dataObj, string propertyName, string value)
        {
            if(string.IsNullOrEmpty(value)) return;

            switch (propertyName)
            {
                case nameof(FirstName): dataObj.FirstName = value; break;
                case nameof(Name): dataObj.Name = value; break;
                case nameof(Gender): if (EnumCoreLocalizedStringHelper.TryParse(value, out Genders parsedGender)) { dataObj.Gender = parsedGender; } break;
                case nameof(BirthYear): dataObj.BirthYear = UInt16.Parse(value); break;
                case nameof(Breaststroke):
                    dataObj.Breaststroke = !string.IsNullOrEmpty(value);
                    if (dataObj.Starts[SwimmingStyles.Breaststroke] != null) { dataObj.Starts[SwimmingStyles.Breaststroke].IsActive = !value.Equals(START_INACTIVE_MARKER_STRING, StringComparison.OrdinalIgnoreCase); }
                    break;
                case nameof(Freestyle): 
                    dataObj.Freestyle = !string.IsNullOrEmpty(value);
                    if (dataObj.Starts[SwimmingStyles.Freestyle] != null) { dataObj.Starts[SwimmingStyles.Freestyle].IsActive = !value.Equals(START_INACTIVE_MARKER_STRING, StringComparison.OrdinalIgnoreCase); }
                    break;
                case nameof(Backstroke): 
                    dataObj.Backstroke = !string.IsNullOrEmpty(value);
                    if (dataObj.Starts[SwimmingStyles.Backstroke] != null) { dataObj.Starts[SwimmingStyles.Backstroke].IsActive = !value.Equals(START_INACTIVE_MARKER_STRING, StringComparison.OrdinalIgnoreCase); }
                    break;
                case nameof(Butterfly): 
                    dataObj.Butterfly = !string.IsNullOrEmpty(value);
                    if (dataObj.Starts[SwimmingStyles.Butterfly] != null) { dataObj.Starts[SwimmingStyles.Butterfly].IsActive = !value.Equals(START_INACTIVE_MARKER_STRING, StringComparison.OrdinalIgnoreCase); }
                    break;
                case nameof(Medley):
                    dataObj.Medley = !string.IsNullOrEmpty(value);
                    if (dataObj.Starts[SwimmingStyles.Medley] != null) { dataObj.Starts[SwimmingStyles.Medley].IsActive = !value.Equals(START_INACTIVE_MARKER_STRING, StringComparison.OrdinalIgnoreCase); }
                    break;
                case nameof(WaterFlea): 
                    dataObj.WaterFlea = !string.IsNullOrEmpty(value);
                    if (dataObj.Starts[SwimmingStyles.WaterFlea] != null) { dataObj.Starts[SwimmingStyles.WaterFlea].IsActive = !value.Equals(START_INACTIVE_MARKER_STRING, StringComparison.OrdinalIgnoreCase); }
                    break;
                case nameof(BreaststrokeTime): dataObj.BreaststrokeTime = TimeSpan.Parse(value); break;
                case nameof(FreestyleTime): dataObj.FreestyleTime = TimeSpan.Parse(value); break;
                case nameof(BackstrokeTime): dataObj.BackstrokeTime = TimeSpan.Parse(value); break;
                case nameof(ButterflyTime): dataObj.ButterflyTime = TimeSpan.Parse(value); break;
                case nameof(MedleyTime): dataObj.MedleyTime = TimeSpan.Parse(value); break;
                case nameof(WaterFleaTime): dataObj.WaterFleaTime = TimeSpan.Parse(value); break;
                default: break;
            }
        }

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Equality, HashCode, ToString, Clone

        /// <summary>
        /// Compare if two Persons are equal
        /// </summary>
        /// <param name="obj">Other Person to compare against this instance.</param>
        /// <returns>true if both instances are equal; false if not equal or obj isn't of type <see cref="Person"/></returns>
        public override bool Equals(object obj)
            => obj is Person p && (p.Name.ToUpper(), p.FirstName.ToUpper(), p.Gender, p.BirthYear).Equals((Name.ToUpper(), FirstName.ToUpper(), Gender, BirthYear)) && CompareDictionaries(Starts, p.Starts);
        
        /// <summary>
        /// Indicates wheather the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">Other object to compare.</param>
        /// <returns>true if the current object is equal to the other parameter; otherwise false.</returns>
        public bool Equals(Person other)
            => Equals((object)other);

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
            => (Name.ToUpper(), FirstName.ToUpper(), Gender, BirthYear, Starts).GetHashCode();

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
            => Name + ", " + FirstName + " (" + BirthYear.ToString() + ")";

        /// <summary>
        /// Create a new object that has the same property values than this one
        /// </summary>
        /// <returns>Cloned object of type</returns>
        public object Clone()
            => new Person(this);

        /// <summary>
        /// Compare if two dictionaries are equal (same keys and same values)
        /// </summary>
        /// <typeparam name="TKey">Type of the keys</typeparam>
        /// <typeparam name="TValue">Type of the values</typeparam>
        /// <param name="dict1">Dictionary 1</param>
        /// <param name="dict2">Dictionary 2</param>
        /// <returns>Are the dictionaries equal?</returns>
        /// <see cref="https://stackoverflow.com/questions/3804367/testing-for-equality-between-dictionaries-in-c-sharp"/>
        private bool CompareDictionaries<TKey, TValue>(Dictionary<TKey, TValue> dict1, Dictionary<TKey, TValue> dict2)
        {
            if (dict1 == dict2) return true;
            if ((dict1 == null) || (dict2 == null)) return false;
            if (dict1.Count != dict2.Count) return false;

            EqualityComparer<TValue> valueComparer = EqualityComparer<TValue>.Default;

            foreach (KeyValuePair<TKey, TValue> kvp in dict1)
            {
                TValue value2;
                if (!dict2.TryGetValue(kvp.Key, out value2)) return false;
                if (!valueComparer.Equals(kvp.Value, value2)) return false;
            }
            return true;
        }

        #endregion
    }
}
