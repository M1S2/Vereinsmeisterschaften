using CommunityToolkit.Mvvm.ComponentModel;
using Vereinsmeisterschaften.Core.Contracts.Services;
using Vereinsmeisterschaften.Core.Helpers;

namespace Vereinsmeisterschaften.Core.Models
{
    /// <summary>
    /// Class describing a competition.
    /// </summary>
    public partial class Competition : ObservableObject, IEquatable<Competition>, ICloneable
    {
        /// <summary>
        /// Constructor for a new Competition object.
        /// </summary>
        public Competition()
        {
        }

        /// <summary>
        /// Clone constructor for a Competition object.
        /// </summary>
        /// <param name="other">Object to clone</param>
        public Competition(Competition other) : this()
        {
            if (other == null) { return; }
            this.Id = other.Id;
            this.Gender = other.Gender;
            this.SwimmingStyle = other.SwimmingStyle;
            this.Age = other.Age;
            this.Distance = other.Distance;
            this.BestTime = other.BestTime;
        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        private int _id = 0;
        /// <summary>
        /// Competition ID
        /// </summary>
        [FileServiceOrder]
        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        private Genders _gender;
        /// <summary>
        /// Gender for this competition
        /// </summary>
        [FileServiceOrder]
        public Genders Gender
        {
            get => _gender;
            set => SetProperty(ref _gender, value);
        }

        private SwimmingStyles _swimmingStyle;
        /// <summary>
        /// Swimming style for this competition
        /// </summary>
        [FileServiceOrder]
        public SwimmingStyles SwimmingStyle
        {
            get => _swimmingStyle;
            set => SetProperty(ref _swimmingStyle, value);
        }

        private byte _age = 0;
        /// <summary>
        /// Age for the person that is assigned for this competition
        /// </summary>
        [FileServiceOrder]
        public byte Age
        {
            get => _age;
            set => SetProperty(ref _age, value);
        }

        private ushort _distance = 0;
        /// <summary>
        /// Distance in meters for this competition (e.g. 25, 50, 100, 200)
        /// </summary>
        [FileServiceOrder]
        public ushort Distance
        {
            get => _distance;
            set => SetProperty(ref _distance, value);
        }

        private TimeSpan _bestTime;
        /// <summary>
        /// Time for this competition to reach the maximum points
        /// </summary>
        [FileServiceOrder]
        public TimeSpan BestTime
        {
            get => _bestTime;
            set => SetProperty(ref _bestTime, value);
        }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// Set the requested property in the <see cref="Competition"/> object by parsing the given string value
        /// </summary>
        /// <param name="dataObj"><see cref="Competition"/> in which to set the property</param>
        /// <param name="propertyName">Name of the property to set</param>
        /// <param name="value">String value that will be parsed and set to the property</param>
        public static void SetPropertyFromString(Competition dataObj, string propertyName, string value)
        {
            switch (propertyName)
            {
                case nameof(Id): dataObj.Id = int.Parse(value); break;
                case nameof(Gender): if (EnumCoreLocalizedStringHelper.TryParse(value, out Genders parsedGender)) { dataObj.Gender = parsedGender; } break;
                case nameof(SwimmingStyle): if (EnumCoreLocalizedStringHelper.TryParse(value, out SwimmingStyles parsedStyle)) { dataObj.SwimmingStyle = parsedStyle; } break;
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
            => obj is Competition c && (c.Id, c.Gender, c.SwimmingStyle, c.Age, c.Distance, c.BestTime).Equals((Id, Gender, SwimmingStyle, Age, Distance, BestTime));

        /// <summary>
        /// Indicates wheather the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">Other object to compare.</param>
        /// <returns>true if the current object is equal to the other parameter; otherwise false.</returns>
        public bool Equals(Competition other)
            => Equals((object)other);

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
            => (Id, Gender, SwimmingStyle, Age, Distance, BestTime).GetHashCode();

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
            => $"{Id}: {Distance}m {SwimmingStyle} {Gender} (Age: {Age})";

        /// <summary>
        /// Create a new object that has the same property values than this one
        /// </summary>
        /// <returns>Cloned object of type</returns>
        public object Clone()
            => new Competition(this);
    }
}
