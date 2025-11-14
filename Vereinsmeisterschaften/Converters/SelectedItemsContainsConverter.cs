using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace Vereinsmeisterschaften.Converters
{
    public class SelectedItemsContainsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 2 &&
                values[0] is IList list &&
                values[1] != null)
            {
                return list.Contains(values[1]);
            }

            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
