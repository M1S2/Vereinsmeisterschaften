using CommunityToolkit.Mvvm.ComponentModel;
using Vereinsmeisterschaften.Core.Contracts.Services;

namespace Vereinsmeisterschaften.Core.Models
{
    /// <summary>
    /// Class describing one element of the winner history
    /// </summary>
    public partial class WinnerHistoryElement : ObservableObject, IEquatable<WinnerHistoryElement>, ICloneable, IComparable<WinnerHistoryElement>
    {
        #region Constructors

        /// <summary>
        /// Constructor for a new WinnerHistoryElement object
        /// </summary>
        public WinnerHistoryElement()
        {
        }

        /// <summary>
        /// Constructor for a new WinnerHistoryElement object using name strings
        /// </summary>
        /// <param name="timestamp">Timestamp value</param>
        /// <param name="winnersGold">List with names of gold winners</param>
        /// <param name="winnersSilver">List with names of silver winners</param>
        /// <param name="winnersBronze">List with names of bronze winners</param>
        public WinnerHistoryElement(DateTime timestamp, List<WinnerName> winnersGold, List<WinnerName> winnersSilver, List<WinnerName> winnersBronze)
        {
            Timestamp = timestamp;
            WinnersGold = winnersGold;
            WinnersSilver = winnersSilver;
            WinnersBronze = winnersBronze;
        }

        /// <summary>
        /// Constructor for a new WinnerHistoryElement object using <see cref="PersonStart"/> objects
        /// </summary>
        /// <param name="timestamp">Timestamp value</param>
        /// <param name="winnersGold">List with <see cref="PersonStart"> of gold winners</param>
        /// <param name="winnersSilver">List with <see cref="PersonStart"> of silver winners</param>
        /// <param name="winnersBronze">List with <see cref="PersonStart"> of bronze winners</param>
        public WinnerHistoryElement(DateTime timestamp, List<PersonStart> winnersGold, List<PersonStart> winnersSilver, List<PersonStart> winnersBronze)
        {
            Timestamp = timestamp;
            WinnersGold = winnersGold?.Select(s => new WinnerName(s?.PersonObj?.FirstName, s?.PersonObj?.Name) { IsPartOfCurrentWinners = false }).ToList();
            WinnersSilver = winnersSilver?.Select(s => new WinnerName(s?.PersonObj?.FirstName, s?.PersonObj?.Name) { IsPartOfCurrentWinners = false }).ToList();
            WinnersBronze = winnersBronze?.Select(s => new WinnerName(s?.PersonObj?.FirstName, s?.PersonObj?.Name) { IsPartOfCurrentWinners = false }).ToList();
        }

        /// <summary>
        /// Clone constructor for a WinnerHistoryElement object.
        /// </summary>
        /// <param name="other">Object to clone</param>
        public WinnerHistoryElement(WinnerHistoryElement other) : this()
        {
            if (other == null) { return; }
            this.Timestamp = other.Timestamp;

            // Deep clone of the winner lists
            this.WinnersGold = new List<WinnerName>(other.WinnersGold ?? Enumerable.Empty<WinnerName>());
            this.WinnersSilver = new List<WinnerName>(other.WinnersSilver ?? Enumerable.Empty<WinnerName>());
            this.WinnersBronze = new List<WinnerName>(other.WinnersBronze ?? Enumerable.Empty<WinnerName>());

            this.MatchesCurrentWinnersElement = other.MatchesCurrentWinnersElement;
        }

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Basic properties

        /// <summary>
        /// Timestamp for the history element
        /// </summary>
        [ObservableProperty]
        private DateTime _timestamp;
        
        /// <summary>
        /// All gold winners of the history element
        /// </summary>
        [ObservableProperty]
        private List<WinnerName> _winnersGold;

        /// <summary>
        /// All silver winners of the history element
        /// </summary>
        [ObservableProperty]
        private List<WinnerName> _winnersSilver;

        /// <summary>
        /// All bronze winners of the history element
        /// </summary>
        [ObservableProperty]
        private List<WinnerName> _winnersBronze;

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Helper properties for previous elements

        private bool _matchesCurrentWinnersElement = false;
        /// <summary>
        /// <see langword="true"/> when this <see cref="WinnerHistoryElement"/> matches the current one (but is in the list of previous winners)
        /// </summary>
        [FileServiceIgnore]
        public bool MatchesCurrentWinnersElement
        {
            get => _matchesCurrentWinnersElement;
            set => SetProperty(ref _matchesCurrentWinnersElement, value);
        }

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Helper properties for current element

        private bool _isCurrentWinnersElement = false;
        /// <summary>
        /// <see langword="true"/> when this <see cref="WinnerHistoryElement"/> is the current winners element
        /// </summary>
        [FileServiceIgnore]
        public bool IsCurrentWinnersElement
        {
            get => _isCurrentWinnersElement;
            set => SetProperty(ref _isCurrentWinnersElement, value);
        }

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region SetPropertyFromString helper

        /// <summary>
        /// Set the requested property in the <see cref="WinnerHistoryElement"/> object by parsing the given string value
        /// </summary>
        /// <param name="dataObj"><see cref="WinnerHistoryElement"/> in which to set the property</param>
        /// <param name="propertyName">Name of the property to set</param>
        /// <param name="value">String value that will be parsed and set to the property</param>
        public static void SetPropertyFromString(WinnerHistoryElement dataObj, string propertyName, string value)
        {
            if (string.IsNullOrEmpty(value)) return;

            switch (propertyName)
            {
                case nameof(Timestamp): dataObj.Timestamp = DateTime.Parse(value); break;
                case nameof(WinnersGold): dataObj.WinnersGold = new List<WinnerName>(value.Split('|').Select(s => new WinnerName(s.Trim()))); break;
                case nameof(WinnersSilver): dataObj.WinnersSilver = new List<WinnerName>(value.Split('|').Select(s => new WinnerName(s.Trim()))); break;
                case nameof(WinnersBronze): dataObj.WinnersBronze = new List<WinnerName>(value.Split('|').Select(s => new WinnerName(s.Trim()))); break;
                default: break;
            }
        }

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Equality, HashCode, ToString, Clone, Comparable

        /// <summary>
        /// Compare if two WinnerHistoryElements are equal
        /// </summary>
        /// <param name="obj">Other WinnerHistoryElement to compare against this instance.</param>
        /// <returns>true if both instances are equal; false if not equal or obj isn't of type <see cref="WinnerHistoryElement"/></returns>
        public override bool Equals(object obj)
            => obj is WinnerHistoryElement h && 
                h.Timestamp.Equals(Timestamp) &&
                (h.WinnersGold == null || WinnersGold == null || h.WinnersGold.SequenceEqual(WinnersGold)) &&
                (h.WinnersSilver == null || WinnersSilver == null || h.WinnersSilver.SequenceEqual(WinnersSilver)) &&
                (h.WinnersBronze == null || WinnersBronze == null || h.WinnersBronze.SequenceEqual(WinnersBronze));

        /// <summary>
        /// Indicates wheather the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">Other object to compare.</param>
        /// <returns>true if the current object is equal to the other parameter; otherwise false.</returns>
        public bool Equals(WinnerHistoryElement other)
            => Equals((object)other);

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
            => (Timestamp, WinnersGold, WinnersSilver, WinnersBronze).GetHashCode();

        /// <summary>
        /// Create a new object that has the same property values than this one
        /// </summary>
        /// <returns>Cloned object of type</returns>
        public object Clone()
            => new WinnerHistoryElement(this);

        /// <summary>
        /// Compare this <see cref="WinnerHistoryElement"/> to another one.
        /// The comparison is based on the <see cref="Timestamp"/>.
        /// </summary>
        /// <param name="other">Other <see cref="WinnerHistoryElement"/> instance</param>
        /// <returns>
        /// smaller 0 (e.g. -1): the current object is in front of the other object.
        /// equal 0: both objects are on the same position.
        /// greater 0 (e.g. 1): the current object is after the other object.
        /// </returns>
        public int CompareTo(WinnerHistoryElement other)
        {
            if (other == null) return 1;    // Every object is greater than null
            return this.Timestamp.CompareTo(other.Timestamp);
        }

        #endregion
    }
}
