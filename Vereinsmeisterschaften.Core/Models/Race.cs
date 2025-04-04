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
        /// <summary>
        /// List with all starts of this <see cref="Race"/>
        /// </summary>
        public ObservableCollection<PersonStart> Starts { get; set; }

        /// <summary>
        /// <see cref="SwimmingStyles"> for this <see cref="Race"/>. This is the <see cref="SwimmingStyles"/>> from the first <see cref="PersonStart"/> in the <see cref="Starts"/> collection.
        /// </summary>
        public SwimmingStyles Style => Starts?.FirstOrDefault()?.Style ?? SwimmingStyles.Unknown;

        /// <summary>
        /// Distance for this <see cref="Race"/>. This is the distance from the first <see cref="PersonStart"/> in the <see cref="Starts"/> collection.
        /// </summary>
        public int Distance => Starts?.FirstOrDefault()?.CompetitionObj?.Distance ?? 0;

        /// <summary>
        /// A <see cref="Race"/> is considered as valid, when:
        /// - all <see cref="PersonStart"/> in the <see cref="Starts"/> collection have the same <see cref="SwimmingStyles"/>
        /// - all <see cref="PersonStart"/> in the <see cref="Starts"/> collection have the same distance
        /// </summary>
        public bool IsValid => Starts?.GroupBy(s => s?.CompetitionObj?.Distance).Count() == 1 &&
                               Starts?.GroupBy(s => s?.Style).Count() == 1;

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        public Race()
        {
            Starts = new ObservableCollection<PersonStart>();
            Starts.CollectionChanged += (sender, e) => { OnPropertyChanged(nameof(Style)); OnPropertyChanged(nameof(Distance)); OnPropertyChanged(nameof(IsValid)); };
        }

        public Race(List<PersonStart> starts)
        {
            Starts = starts == null ? null : new ObservableCollection<PersonStart>(starts);
            if(Starts != null)
            {
                Starts.CollectionChanged += (sender, e) => { OnPropertyChanged(nameof(Style)); OnPropertyChanged(nameof(Distance)); OnPropertyChanged(nameof(IsValid)); };
            }
        }

        public Race(ObservableCollection<PersonStart> starts)
        {
            Starts = starts;
            if (Starts != null)
            {
                Starts.CollectionChanged += (sender, e) => { OnPropertyChanged(nameof(Style)); OnPropertyChanged(nameof(Distance)); OnPropertyChanged(nameof(IsValid)); };
            }
        }

        public Race(Race other) : this()
        {
            if (other == null) { return; }
            // Create a deep copy of the list
            Starts = new ObservableCollection<PersonStart>(other.Starts.Select(item => (PersonStart)item.Clone()));
            Starts.CollectionChanged += (sender, e) => { OnPropertyChanged(nameof(Style)); OnPropertyChanged(nameof(Distance)); OnPropertyChanged(nameof(IsValid)); };
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
