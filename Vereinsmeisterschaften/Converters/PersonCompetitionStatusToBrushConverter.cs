using System.Drawing;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Vereinsmeisterschaften.Converters;

/// <summary>
/// Converter that converts the competition status of a person to a corresponding brush color.
/// values[0] = isCompetitionAvailable
/// values[1] = isCompetitionSelected
/// </summary>
[ValueConversion(typeof(bool), typeof(SolidColorBrush))]
public class PersonCompetitionStatusToBrushConverter : IMultiValueConverter
{
    /// <summary>
    /// Brush used when the competition is missing and selected.
    /// </summary>
    public SolidColorBrush BrushCompetitionMissingAndSelected { get; set; } = new SolidColorBrush(Colors.Red);

    /// <summary>
    /// Brush used when the competition is missing but not selected.
    /// </summary>
    public SolidColorBrush BrushCompetitionMissing { get; set; } = new SolidColorBrush(Colors.Gray);

    /// <summary>
    /// Brush used when the competition is available.
    /// </summary>
    public SolidColorBrush BrushCompetitionAvailable { get; set; } = new SolidColorBrush(Colors.Transparent);

    /// <summary>
    /// Conversion method.
    /// </summary>
    /// <param name="values">Values used for conversion</param>
    /// <param name="targetType">Target <see cref="Type"/></param>
    /// <param name="parameter">ConverterParameter</param>
    /// <param name="culture"><see cref="CultureInfo"/></param>
    /// <returns>Converted object</returns>
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 2 || !(values[0] is bool isCompetitionAvailable) || !(values[1] is bool isCompetitionSelected))
            return BrushCompetitionAvailable;

        if (!isCompetitionAvailable && isCompetitionSelected)
            return BrushCompetitionMissingAndSelected;
        if (!isCompetitionAvailable && !isCompetitionSelected)
            return BrushCompetitionMissing;

        return BrushCompetitionAvailable;
    }

    /// <summary>
    /// Back conversion method. Not implemented for this converter.
    /// </summary>
    /// <param name="value">Value used for conversion</param>
    /// <param name="targetTypes">Target <see cref="Type"/> array</param>
    /// <param name="parameter">ConverterParameter</param>
    /// <param name="culture"><see cref="CultureInfo"/></param>
    /// <returns>Back conversion result</returns>
    /// <exception cref="NotImplementedException"></exception>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
