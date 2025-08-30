using System.Globalization;
using System.Windows.Data;
using Vereinsmeisterschaften.Core.Models;
using Vereinsmeisterschaften.Properties;

namespace Vereinsmeisterschaften.Converters;

/// <summary>
/// Converts the requested competition of a Person to a competition ID string representation.
/// </summary>
[ValueConversion(typeof(Person), typeof(string))]
public class PersonStartToCompetitionIDStringConverter : IValueConverter
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
        if(value != null && value is Person personVal && parameter is SwimmingStyles swimmingStyle)
        {
            int competitionID = personVal.AvailableCompetitionsIDs[swimmingStyle];
            return competitionID == -1 ? Resources.CompetitionMissingString : ($"{Resources.CompetitionIDString} : {competitionID}");
        }
        return null;
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
