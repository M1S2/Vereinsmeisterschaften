using System;
using System.Collections.Generic;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Services;

namespace Vereinsmeisterschaften.Core.Models
{
    /// <summary>
    /// Class describing a person.
    /// </summary>
    public class Person : ObservableObject, IEquatable<Person>
    {
        private string _name = string.Empty;
        /// <summary>
        /// Last name of the person.
        /// </summary>
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _firstName = string.Empty;
        /// <summary>
        /// First name of the person.
        /// </summary>
        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        private Genders _gender;
        /// <summary>
        /// Gender of the person.
        /// </summary>
        public Genders Gender
        {
            get => _gender;
            set => SetProperty(ref _gender, value);
        }

        private UInt16 _birthYear;
        /// <summary>
        /// Birth year of the person.
        /// </summary>
        public UInt16 BirthYear
        {
            get => _birthYear;
            set => SetProperty(ref _birthYear, value);
        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

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
        /// This is the start in the <see cref="Starts"/> dictionary with the highest score value
        /// </summary>
        [FileServiceIgnore]
        public double HighestScore => Starts?.Values.OrderByDescending(s => s.Score).FirstOrDefault()?.Score ?? 0;

        /// <summary>
        /// This is the start in the <see cref="Starts"/> dictionary for which the highest score value was reached
        /// </summary>
        [FileServiceIgnore]
        public SwimmingStyles HighestScoreStyle => Starts?.Values.OrderByDescending(s => s.Score).FirstOrDefault()?.Style ?? SwimmingStyles.Unknown;

        /// <summary>
        /// Get the first start in the list of starts of the person that matches the style
        /// </summary>
        /// <param name="style">Requested <see cref="SwimmingStyles"/></param>
        /// <returns>Matching <see cref="PersonStart"/> or <see langword="null"/></returns>
        public PersonStart GetStartByStyle(SwimmingStyles style)
        {
            if (Starts == null) { Starts = new Dictionary<SwimmingStyles, PersonStart>(); return null; }
            if (Starts.Count == 0 || !Starts.ContainsKey(style)) { return null; }
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
                Starts.Add(style, new PersonStart() { Style = style, PersonObj = this });
            }
            else if (start != null && !available)
            {
                Starts.Remove(style);
            }
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

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // PROPERTIES FOR FASTER ACCESS AND FILE SAVING

        /// <summary>
        /// Starting with Breaststroke
        /// </summary>
        public bool Breaststroke
        {
            get => GetStartByStyle(SwimmingStyles.Breaststroke) != null;
            set { SetStartAvailable(SwimmingStyles.Breaststroke, value); OnPropertyChanged(); }
        }

        /// <summary>
        /// Starting with Freestyle
        /// </summary>
        public bool Freestyle
        {
            get => GetStartByStyle(SwimmingStyles.Freestyle) != null;
            set { SetStartAvailable(SwimmingStyles.Freestyle, value); OnPropertyChanged(); }
        }

        /// <summary>
        /// Starting with Backstroke
        /// </summary>
        public bool Backstroke
        {
            get => GetStartByStyle(SwimmingStyles.Backstroke) != null;
            set { SetStartAvailable(SwimmingStyles.Backstroke, value); OnPropertyChanged(); }
        }

        /// <summary>
        /// Starting with Butterfly
        /// </summary>
        public bool Butterfly
        {
            get => GetStartByStyle(SwimmingStyles.Butterfly) != null;
            set { SetStartAvailable(SwimmingStyles.Butterfly, value); OnPropertyChanged(); }
        }

        /// <summary>
        /// Starting with Medley
        /// </summary>
        public bool Medley
        {
            get => GetStartByStyle(SwimmingStyles.Medley) != null;
            set { SetStartAvailable(SwimmingStyles.Medley, value); OnPropertyChanged(); }
        }

        /// <summary>
        /// Starting with WaterFlea
        /// </summary>
        public bool WaterFlea
        {
            get => GetStartByStyle(SwimmingStyles.WaterFlea) != null;
            set { SetStartAvailable(SwimmingStyles.WaterFlea, value); OnPropertyChanged(); }
        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Time for Breaststroke
        /// </summary>
        public TimeSpan BreaststrokeTime
        {
            get => GetStartByStyle(SwimmingStyles.Breaststroke)?.Time ?? new TimeSpan();
            set { SetStartTime(SwimmingStyles.Breaststroke, value); OnPropertyChanged(); }
        }

        /// <summary>
        /// Time for Freestyle
        /// </summary>
        public TimeSpan FreestyleTime
        {
            get => GetStartByStyle(SwimmingStyles.Freestyle)?.Time ?? new TimeSpan();
            set { SetStartTime(SwimmingStyles.Freestyle, value); OnPropertyChanged(); }
        }

        /// <summary>
        /// Time for Backstroke
        /// </summary>
        public TimeSpan BackstrokeTime
        {
            get => GetStartByStyle(SwimmingStyles.Backstroke)?.Time ?? new TimeSpan();
            set { SetStartTime(SwimmingStyles.Backstroke, value); OnPropertyChanged(); }
        }

        /// <summary>
        /// Time for Butterfly
        /// </summary>
        public TimeSpan ButterflyTime
        {
            get => GetStartByStyle(SwimmingStyles.Butterfly)?.Time ?? new TimeSpan();
            set { SetStartTime(SwimmingStyles.Butterfly, value); OnPropertyChanged(); }
        }

        /// <summary>
        /// Time for Medley
        /// </summary>
        public TimeSpan MedleyTime
        {
            get => GetStartByStyle(SwimmingStyles.Medley)?.Time ?? new TimeSpan();
            set { SetStartTime(SwimmingStyles.Medley, value); OnPropertyChanged(); }
        }

        /// <summary>
        /// Time for WaterFlea
        /// </summary>
        public TimeSpan WaterFleaTime
        {
            get => GetStartByStyle(SwimmingStyles.WaterFlea)?.Time ?? new TimeSpan();
            set { SetStartTime(SwimmingStyles.WaterFlea, value); OnPropertyChanged(); }
        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

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
                case nameof(Gender): dataObj.Gender = (Genders)Enum.Parse(typeof(Genders), value); break;
                case nameof(BirthYear): dataObj.BirthYear = UInt16.Parse(value); break;
                case nameof(Breaststroke): dataObj.Breaststroke = !string.IsNullOrEmpty(value); break;
                case nameof(Freestyle): dataObj.Freestyle = !string.IsNullOrEmpty(value); break;
                case nameof(Backstroke): dataObj.Backstroke = !string.IsNullOrEmpty(value); break;
                case nameof(Butterfly): dataObj.Butterfly = !string.IsNullOrEmpty(value); break;
                case nameof(Medley): dataObj.Medley = !string.IsNullOrEmpty(value); break;
                case nameof(WaterFlea): dataObj.WaterFlea = !string.IsNullOrEmpty(value); break;
                case nameof(BreaststrokeTime): dataObj.BreaststrokeTime = TimeSpan.Parse(value); break;
                case nameof(FreestyleTime): dataObj.FreestyleTime = TimeSpan.Parse(value); break;
                case nameof(BackstrokeTime): dataObj.BackstrokeTime = TimeSpan.Parse(value); break;
                case nameof(ButterflyTime): dataObj.ButterflyTime = TimeSpan.Parse(value); break;
                case nameof(MedleyTime): dataObj.MedleyTime = TimeSpan.Parse(value); break;
                case nameof(WaterFleaTime): dataObj.WaterFleaTime = TimeSpan.Parse(value); break;
                default: break;
            }
        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Compare if two Persons are equal
        /// </summary>
        /// <param name="obj">Other Person to compare against this instance.</param>
        /// <returns>true if both instances are equal; false if not equal or obj isn't of type <see cref="Person"/></returns>
        public override bool Equals(object obj)
        {
            Person other = obj as Person;
            if (other == null) return false;

            return Name.Equals(other.Name) &&
                FirstName.Equals(other.FirstName) &&
                Gender.Equals(other.Gender) &&
                BirthYear.Equals(other.BirthYear);
        }

        /// <summary>
        /// Indicates wheather the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">Other object to compare.</param>
        /// <returns>true if the current object is equal to the other parameter; otherwise false.</returns>
        public bool Equals(Person other)
        {
            return Equals((object)other);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return Name + ", " + FirstName + " (" + BirthYear.ToString() + ")";
        }
    }
}
