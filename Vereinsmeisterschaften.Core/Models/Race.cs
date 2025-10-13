using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using Vereinsmeisterschaften.Core.Contracts.Services;

namespace Vereinsmeisterschaften.Core.Models
{
    /// <summary>
    /// Class that represents a single race.
    /// </summary>
    public class Race : ObservableObject, IEquatable<Race>, ICloneable
    {
        /// <summary>
        /// <see cref="SwimmingStyles"> for this <see cref="Race"/>. This is the <see cref="SwimmingStyles"/>> from the first <see cref="PersonStart"/> in the <see cref="Starts"/> collection.
        /// </summary>
        [FileServiceOrder]
        public SwimmingStyles Style => Starts?.FirstOrDefault()?.Style ?? SwimmingStyles.Unknown;

        /// <summary>
        /// Distance for this <see cref="Race"/>. This is the distance from the first <see cref="PersonStart"/> in the <see cref="Starts"/> collection.
        /// </summary>
        [FileServiceOrder]
        public int Distance => Starts?.FirstOrDefault()?.CompetitionObj?.Distance ?? 0;

        private ObservableCollection<PersonStart> _starts;
        /// <summary>
        /// List with all starts of this <see cref="Race"/>
        /// </summary>
        [FileServiceOrder]
        public ObservableCollection<PersonStart> Starts
        {
            get => _starts;
            set => SetProperty(ref _starts, value);
        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// True when all <see cref="PersonStart"/> in the <see cref="Starts"/> collection have the same distance
        /// </summary>
        [FileServiceIgnore]
        public bool IsValid_SameDistances => Starts?.GroupBy(s => s?.CompetitionObj?.Distance).Count() <= 1;

        /// <summary>
        /// True when all <see cref="PersonStart"/> in the <see cref="Starts"/> collection have the same <see cref="SwimmingStyles"/>
        /// </summary>
        [FileServiceIgnore]
        public bool IsValid_SameStyles => Starts?.GroupBy(s => s?.Style).Count() <= 1;

        /// <summary>
        /// True when the starts are not empty
        /// </summary>
        [FileServiceIgnore]
        public bool IsValid_StartsAvailable => Starts?.Count > 0;

        /// <summary>
        /// A <see cref="Race"/> is considered as valid, when:
        /// - all <see cref="PersonStart"/> in the <see cref="Starts"/> collection have the same <see cref="SwimmingStyles"/>
        /// - all <see cref="PersonStart"/> in the <see cref="Starts"/> collection have the same distance
        /// - the starts are not empty
        /// </summary>
        [FileServiceIgnore]
        public bool IsValid => IsValid_SameStyles && IsValid_SameDistances && IsValid_StartsAvailable;

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private int _raceID;
        /// <summary>
        /// Number for this <see cref="Race"/>
        /// </summary>
        [FileServiceIgnore]
        public int RaceID
        {
            get => _raceID;
            set => SetProperty(ref _raceID, value);
        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Constructor for the Race class (create an empty <see cref="Starts"/> collection).
        /// </summary>
        public Race()
        {
            Starts = new ObservableCollection<PersonStart>();
            Starts.CollectionChanged += Starts_CollectionChanged;
        }

        /// <summary>
        /// Constructor for the Race class (copy an <see cref="Starts"/> collection).
        /// </summary>
        /// <param name="starts">List of <see cref="PersonStart"/> to copy</param>
        public Race(List<PersonStart> starts)
        {
            Starts = starts == null ? null : new ObservableCollection<PersonStart>(starts);
            if(Starts != null)
            {
                Starts.CollectionChanged += Starts_CollectionChanged;
            }
        }

        /// <summary>
        /// Constructor for the Race class (copy an <see cref="Starts"/> collection).
        /// </summary>
        /// <param name="starts">Observable collection of <see cref="PersonStart"/> to copy</param>
        public Race(ObservableCollection<PersonStart> starts)
        {
            Starts = starts;
            if (Starts != null)
            {
                Starts.CollectionChanged += Starts_CollectionChanged;
            }
        }

        /// <summary>
        /// Create a new object that has the same property values than this one
        /// </summary>
        /// <param name="other"><see cref="Race"/> object to clone</param>
        /// <param name="deepClone">If true, the <see cref="Starts"/> are also cloned. Otherwise the same <see cref="Starts"> references are used.</param>
        public Race(Race other, bool deepClone = true) : this()
        {
            if (other == null) { return; }
            if (deepClone)
            {
                // Create a deep copy of the list
                Starts = new ObservableCollection<PersonStart>(other.Starts.Select(item => (PersonStart)item.Clone()));
            }
            else
            {
                // Create a new list but keep the references to the <see cref="PersonStart"/> objects
                Starts = new ObservableCollection<PersonStart>(other.Starts);
            }
            Starts.CollectionChanged += Starts_CollectionChanged;
        }

        private void Starts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Style));
            OnPropertyChanged(nameof(Distance));
            OnPropertyChanged(nameof(IsValid_SameStyles));
            OnPropertyChanged(nameof(IsValid_SameDistances));
            OnPropertyChanged(nameof(IsValid_StartsAvailable));
            OnPropertyChanged(nameof(IsValid));
        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Compare if two <see cref="Race"/> are equal
        /// </summary>
        /// <param name="obj">Other <see cref="Race"> to compare against this instance.</param>
        /// <returns>true if both instances are equal; false if not equal or obj isn't of type <see cref="Race"/></returns>
        public override bool Equals(object obj)
            => obj is Race r && r.Starts.SequenceEqual(Starts, new PersonStartWithoutIsActiveEqualityComparer());

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
            => new Race(this, true);
    }
}
