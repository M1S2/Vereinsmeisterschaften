using CommunityToolkit.Mvvm.ComponentModel;

namespace Vereinsmeisterschaften.Core.Models
{
    /// <summary>
    /// Class describing a single entry in the rudolph table (one single cell containing one time).
    /// </summary>
    public partial class RudolphTableEntry : ObservableObject, IEquatable<RudolphTableEntry>, ICloneable
    {
        #region Constructors

        /// <summary>
        /// Constructor for a new <see cref="RudolphTableEntry"/> object.
        /// </summary>
        public RudolphTableEntry()
        {
        }

        /// <summary>
        /// Clone constructor for a <see cref="RudolphTableEntry"/> object.
        /// </summary>
        /// <param name="other">Object to clone</param>
        public RudolphTableEntry(RudolphTableEntry other) : this()
        {
            if (other == null) { return; }
            this.Gender = other.Gender;
            this.Age = other.Age;
            this.SwimmingStyle = other.SwimmingStyle;            
            this.Distance = other.Distance;
            this.RudolphScore = other.RudolphScore;
            this.Time = other.Time;
        }

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Basic properties

        /// <summary>
        /// <see cref="Genders"/> for this <see cref="RudolphTableEntry"/>
        /// </summary>
        [ObservableProperty]
        private Genders _gender;

        /// <summary>
        /// Age for this <see cref="RudolphTableEntry"/>
        /// </summary>
        [ObservableProperty]
        private byte _age;

        /// <summary>
        /// Is open age attribute for this <see cref="RudolphTableEntry"/>. The <see cref="Age"/> can be ignored when this is <see langword="true"/>
        /// </summary>
        [ObservableProperty]
        private bool _isOpenAge = false;

        /// <summary>
        /// <see cref="SwimmingStyles"/> for this <see cref="RudolphTableEntry"/>
        /// </summary>
        [ObservableProperty]
        private SwimmingStyles _swimmingStyle;

        /// <summary>
        /// Distance in meters for this <see cref="RudolphTableEntry"/> (e.g. 50, 100, 200, 400, 800, 1500)
        /// </summary>
        [ObservableProperty]
        private ushort _distance;

        /// <summary>
        /// Rudolph score for this <see cref="RudolphTableEntry"/> (between 1 and 20)
        /// </summary>
        [ObservableProperty]
        private byte _rudolphScore;

        /// <summary>
        /// Time for this <see cref="RudolphTableEntry"/>
        /// </summary>
        [ObservableProperty]
        private TimeSpan _time;

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Equality, HashCode, ToString, Clone

        /// <summary>
        /// Compare if two RudolphTableEntry are equal
        /// </summary>
        /// <param name="obj">Other RudolphTableEntry to compare against this instance.</param>
        /// <returns>true if both instances are equal; false if not equal or obj isn't of type <see cref="RudolphTableEntry"/></returns>
        public override bool Equals(object obj)
            => obj is RudolphTableEntry c && (c.Gender, c.Age, c.SwimmingStyle, c.Distance, c.RudolphScore, c.Time).Equals((Gender, Age, SwimmingStyle, Distance, RudolphScore, Time));

        /// <summary>
        /// Indicates wheather the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">Other object to compare.</param>
        /// <returns>true if the current object is equal to the other parameter; otherwise false.</returns>
        public bool Equals(RudolphTableEntry other)
            => Equals((object)other);

        /// <summary>
        /// Serves as the default hash function.
        /// Use the default <see cref="GetHashCode"/> method here.
        /// Otherwise the application crashed sometimes with the "An item with the same key has already been added. Key: System.Windows.Controls.ItemsControl+ItemInfo" error when the competition is used inside a DataGrid.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
            => base.GetHashCode();

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
            => $"{Gender}, Age: {(IsOpenAge ? "open age" : Age)}, {Distance}m {SwimmingStyle}, Rudolph score: {RudolphScore}, Time: {Time}";

        /// <summary>
        /// Create a new object that has the same property values than this one
        /// </summary>
        /// <returns>Cloned object of type</returns>
        public object Clone()
            => new RudolphTableEntry(this);

        #endregion
    }
}
