using System.Globalization;
using System.Windows.Data;
using Vereinsmeisterschaften.Core.Models;
using Vereinsmeisterschaften.Properties;

namespace Vereinsmeisterschaften.Converters;

/// <summary>
/// Converts the contents of a <see cref="CompetitionDistanceRuleValidationIssue"/> to a describing string
/// </summary>
[ValueConversion(typeof(CompetitionDistanceRuleValidationIssue), typeof(string))]
public class CompetitionDistanceRuleValidationIssueToStringConverter : IValueConverter
{
    /// <summary>
    /// Conversion method.
    /// </summary>
    /// <param name="value">Value used for conversion</param>
    /// <param name="targetType">Target <see cref="Type"/></param>
    /// <param name="parameter">ConverterParameter</param>
    /// <param name="culture"><see cref="CultureInfo"/></param>
    /// <returns>Converted object</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if(value != null && value is CompetitionDistanceRuleValidationIssue issue)
        {
            string formatedString = "";
            switch (issue.IssueType)
            {
                case CompetitionDistanceRuleValidationIssue.CompetitionDistanceRuleValidationIssueType.AgeGap:
                    formatedString = string.Format(Resources.CompetitionDistanceRuleValidationIssueFormat_AgeGap,
                                                    issue.MinAge,
                                                    issue.MaxAge);
                    break;
                case CompetitionDistanceRuleValidationIssue.CompetitionDistanceRuleValidationIssueType.Overlap:
                    formatedString = string.Format(Resources.CompetitionDistanceRuleValidationIssueFormat_Overlap,
                                                    issue.MinAge,
                                                    issue.MaxAge);
                    break;
                case CompetitionDistanceRuleValidationIssue.CompetitionDistanceRuleValidationIssueType.UnreachableRule:
                    formatedString = string.Format(Resources.CompetitionDistanceRuleValidationIssueFormat_UnreachableRule,
                                                    issue.Rule1!.MinAge,
                                                    issue.Rule1.MaxAge);
                    break;
                default:
                    formatedString = issue.IssueType.ToString();
                    break;
            }
            return formatedString;
        }
        return "???";
    }

    /// <summary>
    /// Back conversion method. Not implemented for this converter.
    /// </summary>
    /// <param name="value">Value used for conversion</param>
    /// <param name="targetType">Target <see cref="Type"/></param>
    /// <param name="parameter">ConverterParameter</param>
    /// <param name="culture"><see cref="CultureInfo"/></param>
    /// <returns>Back conversion result</returns>
    /// <exception cref="NotImplementedException"></exception>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
