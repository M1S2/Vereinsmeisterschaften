using System.Drawing;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Vereinsmeisterschaften.Converters;

public class PersonCompetitionStatusToBrushConverter : IMultiValueConverter
{
    public SolidColorBrush BrushCompetitionMissingAndSelected { get; set; } = new SolidColorBrush(Colors.Red);
    public SolidColorBrush BrushCompetitionMissing { get; set; } = new SolidColorBrush(Colors.Gray);
    public SolidColorBrush BrushCompetitionAvailable { get; set; } = new SolidColorBrush(Colors.Transparent);

    // values[0] = AvailableCompetitionsBool[Key]
    // values[1] = SwimmingStyle
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

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
