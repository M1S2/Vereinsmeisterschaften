using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Vereinsmeisterschaften.Core.Models
{
    /// <summary>
    /// Class that represents a single race in the complete competition race.
    /// </summary>
    public class Race : ObservableObject, IEquatable<Race>, ICloneable
    {
        public ObservableCollection<PersonStart> Starts { get; set; }

        public SwimmingStyles Style => Starts?.FirstOrDefault()?.Style ?? SwimmingStyles.Unknown;
        public int Distance => Starts?.FirstOrDefault()?.CompetitionObj?.Distance ?? 0;

        public Race()
        {
            Starts = new ObservableCollection<PersonStart>();
        }

        public Race(List<PersonStart> starts)
        {
            Starts = starts == null ? null : new ObservableCollection<PersonStart>(starts);
        }

        public Race(ObservableCollection<PersonStart> starts)
        {
            Starts = starts;
        }

        public Race(Race other) : this()
        {
            if (other == null) { return; }
            // Create a deep copy of the list
            Starts = new ObservableCollection<PersonStart>(other.Starts.Select(item => (PersonStart)item.Clone()));
        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Compare if two <see cref="Race"/> are equal
        /// </summary>
        /// <param name="obj">Other <see cref="Race"> to compare against this instance.</param>
        /// <returns>true if both instances are equal; false if not equal or obj isn't of type <see cref="Race"/></returns>
        public override bool Equals(object obj)
            => obj is Race r && r.Starts.SequenceEqual(Starts);

        /// <summary>
        /// Indicates wheather the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">Other object to compare.</param>
        /// <returns>true if the current object is equal to the other parameter; otherwise false.</returns>
        public bool Equals(Race other)
            => Equals((object)other);

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
            => Starts.GetHashCode();

        /// <summary>
        /// Create a new object that has the same property values than this one
        /// </summary>
        /// <returns>Cloned object of type</returns>
        public object Clone()
            => new Race(this);
    }
}
