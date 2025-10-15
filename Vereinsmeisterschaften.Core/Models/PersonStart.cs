using CommunityToolkit.Mvvm.ComponentModel;

namespace Vereinsmeisterschaften.Core.Models
{
    /// <summary>
    /// Class describing a start of a person
    /// </summary>
    public partial class PersonStart : ObservableObject, IEquatable<PersonStart>, ICloneable
    {
        /// <summary>
        /// Constructor for a new PersonStart object
        /// </summary>
        public PersonStart()
        {
        }

        /// <summary>
        /// Clone constructor for a new PersonStart object
        /// </summary>
        /// <param name="other">Object to clone</param>
        public PersonStart(PersonStart other) : this()
        {
            if (other == null) { return; }
            this.PersonObj = other.PersonObj;
            this.CompetitionObj = other.CompetitionObj;
            this.Style = other.Style;
            this.Time = other.Time;
            this.Score = other.Score;
            this.IsHighlighted = other.IsHighlighted;
            this.IsActive = other.IsActive;
        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Reference to the person object to which the start belongs
        /// </summary>
        [ObservableProperty]
        private Person _personObj;
        
        /// <summary>
        /// <see cref="SwimmingStyles"/> of the start
        /// </summary>
        [ObservableProperty]
        private SwimmingStyles _style;

        /// <summary>
        /// Time the person needed during the race of this start
        /// </summary>
        [ObservableProperty]
        private TimeSpan _time;

        /// <summary>
        /// Score of the person for this start
        /// </summary>
        [ObservableProperty]
        private double _score;

        /// <summary>
        /// Indicates if this start is currently active. You can disable starts to exclude them from races and start rankings.
        /// </summary>
        [ObservableProperty]
        private bool _isActive = true;

        /// <summary>
        /// Flag indicating if this start should be highlighted in the UI
        /// </summary>
        [ObservableProperty]
        private bool _isHighlighted;

        private Competition _competitionObj;
        /// <summary>
        /// Reference to the competition object to which the start belongs
        /// </summary>
        public Competition CompetitionObj
        {
            get => _competitionObj;
            set
            {
                if (SetProperty(ref _competitionObj, value))
                {
                    OnPropertyChanged(nameof(IsCompetitionObjAssigned));
                }
            }
        }

        /// <summary>
        /// Flag indicating if the <see cref="CompetitionObj"/> is assigned (<see cref="CompetitionObj"/> != null).
        /// </summary>
        public bool IsCompetitionObjAssigned => CompetitionObj != null;

        /// <summary>
        /// Flag indicating if this start is using a competition that was found by the max available age.
        /// E.g. if competitions up to an age of 18 are defined, but the person is 20 years old, the competition for 18 years is used.
        /// </summary>
        [ObservableProperty]
        private bool _isUsingMaxAgeCompetition;

        /// <summary>
        /// Flag indicating if this start is using a competition for which the age of the person matches the competition age.
        /// E.g. if a competition for age 18 is defined, and the person is 18 years old, this is an exact age competition.
        /// </summary>
        [ObservableProperty]
        private bool _isUsingExactAgeCompetition;

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Compare if two PersonStarts are equal
        /// </summary>
        /// <param name="obj">Other PersonStart to compare against this instance.</param>
        /// <returns>true if both instances are equal; false if not equal or obj isn't of type <see cref="PersonStart"/></returns>
        public override bool Equals(object obj)
            => obj is PersonStart s && (s.PersonObj, s.Style, s.Time, s.IsActive).Equals((PersonObj, Style, Time, IsActive));

        /// <summary>
        /// Indicates wheather the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">Other object to compare.</param>
        /// <returns>true if the current object is equal to the other parameter; otherwise false.</returns>
        public bool Equals(PersonStart other)
            => Equals((object)other);

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
            => (PersonObj, Style, Time, IsActive).GetHashCode();

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
            => $"{(!IsActive ? "Inactive " : "")}Start for {PersonObj} in {Style}" + (CompetitionObj != null ? $" (Distance: {CompetitionObj.Distance}m)" : "");

        /// <summary>
        /// Create a new object that has the same property values than this one
        /// </summary>
        /// <returns>Cloned object of type</returns>
        public object Clone()
            => new PersonStart(this);
    }
}
