using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Vereinsmeisterschaften.Core.Models
{
    /// <summary>
    /// Class describing one issue during the validation of the <see cref="CompetitionDistanceRule"/>
    /// </summary>
    public class CompetitionDistanceRuleValidationIssue
    {
        #region Issue Types

        /// <summary>
        /// Enum with available issue types
        /// </summary>
        public enum CompetitionDistanceRuleValidationIssueType
        {
            AgeGap,
            Overlap,
            UnreachableRule
        }

        #endregion

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// <see cref="CompetitionDistanceRuleValidationIssueType"/> for this <see cref="CompetitionDistanceRuleValidationIssue"/>
        /// </summary>
        public CompetitionDistanceRuleValidationIssueType IssueType { get; set; }

        /// <summary>
        /// <see cref="SwimmingStyles"/> for this <see cref="CompetitionDistanceRuleValidationIssue"/>
        /// </summary>
        public SwimmingStyles SwimmingStyle { get; set; }

        /// <summary>
        /// Minimum age for this <see cref="CompetitionDistanceRuleValidationIssue"/>
        /// </summary>
        public byte MinAge { get; set; }

        /// <summary>
        /// Maximum age for this <see cref="CompetitionDistanceRuleValidationIssue"/>
        /// </summary>
        public byte MaxAge { get; set; }

        /// <summary>
        /// First involved <see cref="CompetitionDistanceRule"/>
        /// </summary>
        public CompetitionDistanceRule Rule1 { get; set; }

        /// <summary>
        /// Second involved <see cref="CompetitionDistanceRule"/>
        /// </summary>
        public CompetitionDistanceRule Rule2 { get; set; }

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Equality, HashCode

        /// <summary>
        /// Compare if two <see cref="CompetitionDistanceRuleValidationIssue"> are equal
        /// </summary>
        /// <param name="obj">Other <see cref="CompetitionDistanceRuleValidationIssue"> to compare against this instance.</param>
        /// <returns>true if both instances are equal; false if not equal or obj isn't of type <see cref="CompetitionDistanceRuleValidationIssue"/></returns>
        public override bool Equals(object obj)
            => obj is CompetitionDistanceRuleValidationIssue i && (i.IssueType, i.SwimmingStyle, i.MinAge, i.MaxAge).Equals((IssueType, SwimmingStyle, MinAge, MaxAge));

        /// <summary>
        /// Indicates wheather the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">Other object to compare.</param>
        /// <returns>true if the current object is equal to the other parameter; otherwise false.</returns>
        public bool Equals(CompetitionDistanceRuleValidationIssue other)
            => Equals((object)other);

        /// <summary>
        /// Serves as the default hash function.
        /// Use the default <see cref="GetHashCode"/> method here.
        /// Otherwise the application crashed sometimes with the "An item with the same key has already been added. Key: System.Windows.Controls.ItemsControl+ItemInfo" error when the person is used inside a DataGrid.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
            => base.GetHashCode();
            //=> (IssueType, SwimmingStyle, MinAge, MaxAge).GetHashCode();

        #endregion
    }
}
