using CommunityToolkit.Mvvm.ComponentModel;
using Vereinsmeisterschaften.Core.Contracts.Services;

namespace Vereinsmeisterschaften.Core.Models
{
    /// <summary>
    /// Class describing the name of one winner
    /// </summary>
    public partial class WinnerName : ObservableObject, IEquatable<WinnerName>, ICloneable
    {
        #region Constructors

        /// <summary>
        /// Constructor for a new <see cref="WinnerName"> object
        /// </summary>
        public WinnerName()
        {
        }

        /// <summary>
        /// Constructor for a new <see cref="WinnerName"> object
        /// </summary>
        /// <param name="firstName">First name of the winner</param>
        /// <param name="name">Last name of the winner</param>
        public WinnerName(string firstName, string name)
        {
            FirstName = firstName;
            Name = name;
        }

        /// <summary>
        /// Constructor for a new <see cref="WinnerName"/> object.
        /// The given name is split to the <see cref="FirstName"/> and <see cref="Name"/> properties.
        /// </summary>
        /// <param name="completeName">Complete name in the format "{FirstName}, {Name}"</param>
        public WinnerName(string completeName)
        {
            string[] nameParts = completeName.Split(",");
            FirstName = nameParts.Count() > 0 ? nameParts[0].Trim() : "";
            Name = nameParts.Count() > 1 ? nameParts[1].Trim() : "";
        }

        /// <summary>
        /// Clone constructor for a <see cref="WinnerName"> object.
        /// </summary>
        /// <param name="other">Object to clone</param>
        public WinnerName(WinnerName other) : this()
        {
            if (other == null) { return; }
            this.FirstName = other.FirstName;
            this.Name = other.Name;
        }

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Basic properties
        
        /// <summary>
        /// First name of the winner
        /// </summary>
        [ObservableProperty]
        private string _firstName;

        /// <summary>
        /// Last name of the winner
        /// </summary>
        [ObservableProperty]
        private string _name;

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Helper properties for previous elements

        private bool _isPartOfCurrentWinners = false;
        /// <summary>
        /// <see langword="true"/> when this <see cref="WinnerName"/> is part of the current winners (but the name is used in a previous winners history element)
        /// </summary>
        [FileServiceIgnore]
        public bool IsPartOfCurrentWinners
        {
            get => _isPartOfCurrentWinners;
            set => SetProperty(ref _isPartOfCurrentWinners, value);
        }

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Equality, HashCode, ToString, Clone

        /// <summary>
        /// Compare if two <see cref="WinnerName"> are equal
        /// </summary>
        /// <param name="obj">Other <see cref="WinnerName"> to compare against this instance.</param>
        /// <returns>true if both instances are equal; false if not equal or obj isn't of type <see cref="WinnerName"/></returns>
        public override bool Equals(object obj)
            => obj is WinnerName w && w.FirstName.Equals(FirstName) && w.Name.Equals(Name);

        /// <summary>
        /// Indicates wheather the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">Other object to compare.</param>
        /// <returns>true if the current object is equal to the other parameter; otherwise false.</returns>
        public bool Equals(WinnerName other)
            => Equals((object)other);

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
            => (FirstName, Name).GetHashCode();

        /// <summary>
        /// String representation for the <see cref="WinnerName">
        /// </summary>
        /// <returns>String containing the <see cref="FirstName"/> and <see cref="Name"/></returns>
        public override string ToString()
            => $"{FirstName}, {Name}";

        /// <summary>
        /// Create a new object that has the same property values than this one
        /// </summary>
        /// <returns>Cloned object of type</returns>
        public object Clone()
            => new WinnerName(this);

        #endregion
    }
}
