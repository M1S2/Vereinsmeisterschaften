using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Vereinsmeisterschaften.Core.Models
{
    public class PersonStart : ObservableObject, IEquatable<PersonStart>, ICloneable
    {
        public PersonStart()
        {
        }

        public PersonStart(PersonStart other) : this()
        {
            if (other == null) { return; }
            this.PersonObj = other.PersonObj;
            this.Style = other.Style;
            this.Time = other.Time;
            this.Score = other.Score;
            this.IsHighlighted = other.IsHighlighted;
        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private Person _personObj;
        public Person PersonObj
        {
            get => _personObj;
            set => SetProperty(ref _personObj, value);
        }

        private SwimmingStyles _style;
        public SwimmingStyles Style
        {
            get => _style;
            set => SetProperty(ref _style, value);
        }

        private TimeSpan _time;
        public TimeSpan Time
        {
            get => _time;
            set => SetProperty(ref _time, value);
        }

        private double _score;
        public double Score
        {
            get => _score;
            set => SetProperty(ref _score, value);
        }

        private bool _isHighlighted;
        public bool IsHighlighted
        {
            get => _isHighlighted;
            set => SetProperty(ref _isHighlighted, value);
        }

        private Competition _competitionObj;
        public Competition CompetitionObj
        {
            get => _competitionObj;
            set => SetProperty(ref _competitionObj, value);
        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Compare if two PersonStarts are equal
        /// </summary>
        /// <param name="obj">Other PersonStart to compare against this instance.</param>
        /// <returns>true if both instances are equal; false if not equal or obj isn't of type <see cref="PersonStart"/></returns>
        public override bool Equals(object obj)
            => obj is PersonStart s && (s.PersonObj, s.Style, s.Time, s.Score).Equals((PersonObj, Style, Time, Score));

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
            => (PersonObj, Style, Time, Score).GetHashCode();

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
            => $"Start for {PersonObj} in {Style}" + (CompetitionObj != null ? $" (Distance: {CompetitionObj.Distance}m)" : "");

        /// <summary>
        /// Create a new object that has the same property values than this one
        /// </summary>
        /// <returns>Cloned object of type</returns>
        public object Clone()
            => new PersonStart(this);
    }
}
