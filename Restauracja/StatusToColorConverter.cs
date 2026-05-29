using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Restauracja
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TableStatus status)
            {
                return status switch
                {
                    TableStatus.Wolny => Brushes.DarkGreen,
                    TableStatus.Zajety => Brushes.Firebrick,
                    TableStatus.Rezerwowany => Brushes.DarkGoldenrod,
                    _ => Brushes.Gray
                };
            }
            return Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}