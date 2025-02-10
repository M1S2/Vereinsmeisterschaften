using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Vereinsmeisterschaften.Core.Models
{
    /// <summary>
    /// Class describing a competition.
    /// </summary>
    public class Competition : ObservableObject, IEquatable<Person>
    {
        private int _id = 0;
        /// <summary>
        /// Competition ID
        /// </summary>
        public int ID
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        private Genders _gender;
        /// <summary>
        /// Gender for this competition
        /// </summary>
        public Genders Gender
        {
            get => _gender;
            set => SetProperty(ref _gender, value);
        }

        private SwimmingStyles _swimmingStyle;
        /// <summary>
        /// Swimming style for this competition
        /// </summary>
        public SwimmingStyles SwimmingStyle
        {
            get => _swimmingStyle;
            set => SetProperty(ref _swimmingStyle, value);
        }

        private byte _age = 0;
        /// <summary>
        /// Age for the person that is assigned for this competition
        /// </summary>
        public byte Age
        {
            get => _age;
            set => SetProperty(ref _age, value);
        }

        private ushort _distance = 0;
        /// <summary>
        /// Distance in meters for this competition (e.g. 25, 50, 100, 200)
        /// </summary>
        public ushort Distance
        {
            get => _distance;
            set => SetProperty(ref _distance, value);
        }

        private TimeSpan _bestTime;
        /// <summary>
        /// Time for this competition to reach the maximum points
        /// </summary>
        public TimeSpan BestTime
        {
            get => _bestTime;
            set => SetProperty(ref _bestTime, value);
        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        public static void SetPropertyFromString(Competition dataObj, string propertyName, string value)
        {
            switch (propertyName)
            {
                case nameof(ID): dataObj.ID = int.Parse(value); break;
                case nameof(Gender): dataObj.Gender = (Genders)Enum.Parse(typeof(Genders), value); break;
                case nameof(SwimmingStyle): dataObj.SwimmingStyle = (SwimmingStyles)Enum.Parse(typeof(SwimmingStyles), value); break;
                case nameof(Age): dataObj.Age = byte.Parse(value); break;
                case nameof(Distance): dataObj.Distance = ushort.Parse(value); break;
                case nameof(BestTime): dataObj.BestTime = TimeSpan.Parse(value); break;
                default: break;
            }
        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Compare if two Competitions are equal
        /// </summary>
        /// <param name="obj">Other Competition to compare against this instance.</param>
        /// <returns>true if both instances are equal; false if not equal or obj isn't of type <see cref="Competition"/></returns>
        public override bool Equals(object obj)
        {
            Competition other = obj as Competition;
            if (other == null) return false;

            return ID.Equals(other.ID) &&
                Gender.Equals(other.Gender) &&
                SwimmingStyle.Equals(other.SwimmingStyle) &&
                Age.Equals(other.Age) &&
                Distance.Equals(other.Distance) &&
                BestTime.Equals(other.BestTime);
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
            return $"{ID}: {Distance}m {SwimmingStyle} {Gender} (Age: {Age})";
        }
    }
}
