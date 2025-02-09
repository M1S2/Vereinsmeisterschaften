using System;
using System.Collections.Generic;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Vereinsmeisterschaften.Core.Models
{
    /// <summary>
    /// Class describing a person.
    /// </summary>
    public class Person : ObservableObject, IEquatable<Person>
    {
        private int _personId = 0;
        /// <summary>
        /// Person ID assigned when adding this person to the data source
        /// </summary>
        public int PersonID
        {
            get => _personId;
            set => SetProperty(ref _personId, value);
        }

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

        private bool _breaststroke;
        /// <summary>
        /// Starting with Breaststroke
        /// </summary>
        public bool Breaststroke
        {
            get => _breaststroke;
            set => SetProperty(ref _breaststroke, value);
        }

        private bool _freestyle;
        /// <summary>
        /// Starting with Freestyle
        /// </summary>
        public bool Freestyle
        {
            get => _freestyle;
            set => SetProperty(ref _freestyle, value);
        }

        private bool _backstroke;
        /// <summary>
        /// Starting with Backstroke
        /// </summary>
        public bool Backstroke
        {
            get => _backstroke;
            set => SetProperty(ref _backstroke, value);
        }

        private bool _butterfly;
        /// <summary>
        /// Starting with Butterfly
        /// </summary>
        public bool Butterfly
        {
            get => _butterfly;
            set => SetProperty(ref _butterfly, value);
        }

        private bool _medley;
        /// <summary>
        /// Starting with Medley
        /// </summary>
        public bool Medley
        {
            get => _medley;
            set => SetProperty(ref _medley, value);
        }

        private bool _waterFlea;
        /// <summary>
        /// Starting with WaterFlea
        /// </summary>
        public bool WaterFlea
        {
            get => _waterFlea;
            set => SetProperty(ref _waterFlea, value);
        }

        //private List<SwimmingStyles> _swimmingStyles;
        ///// <summary>
        ///// List with all swimming styles, for which the person wants to start
        ///// </summary>
        //public List<SwimmingStyles> SwimmingStyles
        //{
        //    get => _swimmingStyles;
        //    set => SetProperty(ref _swimmingStyles, value);
        //}

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        public void SetPropertyFromString(string propertyName, string value)
        {
            switch (propertyName)
            {
                case nameof(PersonID): PersonID = int.Parse(value); break;
                case nameof(FirstName): FirstName = value; break;
                case nameof(Name): Name = value; break;
                case nameof(Gender): Gender = (Genders)Enum.Parse(typeof(Genders), value); break;
                case nameof(BirthYear): BirthYear = UInt16.Parse(value); break;
                case nameof(Breaststroke): Breaststroke = !string.IsNullOrEmpty(value); break;
                case nameof(Freestyle): Freestyle = !string.IsNullOrEmpty(value); break;
                case nameof(Backstroke): Backstroke = !string.IsNullOrEmpty(value); break;
                case nameof(Butterfly): Butterfly = !string.IsNullOrEmpty(value); break;
                case nameof(Medley): Medley = !string.IsNullOrEmpty(value); break;
                case nameof(WaterFlea): WaterFlea = !string.IsNullOrEmpty(value); break;
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
            return FirstName + ", " + Name + " (Birth Year: " + BirthYear.ToString() + ")";
        }
    }
}
