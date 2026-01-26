using CommunityToolkit.Mvvm.ComponentModel;
using Vereinsmeisterschaften.Core.Helpers;

namespace Vereinsmeisterschaften.Core.Models
{
    /// <summary>
    /// Class describing a competition distance rule.
    /// </summary>
    public partial class CompetitionDistanceRule : ObservableObject, IEquatable<CompetitionDistanceRule>, ICloneable
    {
        #region Constructors

        /// <summary>
        /// Constructor for a new <see cref="CompetitionDistanceRule"> object.
        /// </summary>
        public CompetitionDistanceRule()
        {
        }

        /// <summary>
        /// Clone constructor for a <see cref="CompetitionDistanceRule"> object.
        /// </summary>
        /// <param name="other">Object to clone</param>
        public CompetitionDistanceRule(CompetitionDistanceRule other) : this()
        {
            if (other == null) { return; }
            this.MinAge = other.MinAge;
            this.MaxAge = other.MaxAge;
            this.SwimmingStyle = other.SwimmingStyle;
            this.Distance = other.Distance;
        }

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Properties

        /// <summary>
        /// Minimum age for this rule (inclusive)
        /// </summary>
        [ObservableProperty]
        private byte _minAge;

        /// <summary>
        /// Maximum age for this rule (inclusive)
        /// </summary>
        [ObservableProperty]
        private byte _maxAge;

        /// <summary>
        /// <see cref="SwimmingStyles"/> for this rule. Use <see langword="null"/> to use the rule for all styles.
        /// </summary>
        [ObservableProperty]
        private SwimmingStyles? _swimmingStyle = null;

        /// <summary>
        /// Distance in meters for this rule.
        /// </summary>
        [ObservableProperty]
        private ushort _distance;

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region SetPropertyFromString helper

        /// <summary>
        /// Set the requested property in the <see cref="CompetitionDistanceRule"/> object by parsing the given string value
        /// </summary>
        /// <param name="dataObj"><see cref="CompetitionDistanceRule"/> in which to set the property</param>
        /// <param name="propertyName">Name of the property to set</param>
        /// <param name="value">String value that will be parsed and set to the property</param>
        public static void SetPropertyFromString(CompetitionDistanceRule dataObj, string propertyName, string value)
        {
            switch (propertyName)
            {
                case nameof(MinAge): dataObj.MinAge = byte.Parse(value); break;
                case nameof(MaxAge): dataObj.MaxAge = byte.Parse(value); break;
                case nameof(SwimmingStyle):
                    if (EnumCoreLocalizedStringHelper.TryParse(value, out SwimmingStyles parsedStyle))
                    {
                        dataObj.SwimmingStyle = parsedStyle;
                    }
                    else
                    {
                        dataObj.SwimmingStyle = null;   // null means all styles allowed
                    }
                    break;
                case nameof(Distance): dataObj.Distance = ushort.Parse(value); break;
                default: break;
            }
        }

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Equality, HashCode, ToString, Clone

        /// <summary>
        /// Compare if two <see cref="CompetitionDistanceRule"> are equal
        /// </summary>
        /// <param name="obj">Other <see cref="CompetitionDistanceRule"> to compare against this instance.</param>
        /// <returns>true if both instances are equal; false if not equal or obj isn't of type <see cref="CompetitionDistanceRule"/></returns>
        public override bool Equals(object obj)
            => obj is CompetitionDistanceRule c && (c.MinAge, c.MaxAge, c.SwimmingStyle, c.Distance).Equals((MinAge, MaxAge, SwimmingStyle, Distance));

        /// <summary>
        /// Indicates wheather the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">Other object to compare.</param>
        /// <returns>true if the current object is equal to the other parameter; otherwise false.</returns>
        public bool Equals(CompetitionDistanceRule other)
            => Equals((object)other);

        /// <summary>
        /// Serves as the default hash function.
        /// Use the default <see cref="GetHashCode"/> method here.
        /// Otherwise the application crashed sometimes with the "An item with the same key has already been added. Key: System.Windows.Controls.ItemsControl+ItemInfo" error when the competition is used inside a DataGrid.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
            => base.GetHashCode();
        //=> (MinAge, MaxAge, SwimmingStyle, Distance).GetHashCode();

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
            => $"MinAge: {MinAge}, MaxAge: {MaxAge}, SwimmingStyle: {(SwimmingStyle == null ? "all" : SwimmingStyle)} --> Distance: {Distance}m";

        /// <summary>
        /// Create a new object that has the same property values than this one
        /// </summary>
        /// <returns>Cloned object of type</returns>
        public object Clone()
            => new CompetitionDistanceRule(this);

        #endregion

    }
}
