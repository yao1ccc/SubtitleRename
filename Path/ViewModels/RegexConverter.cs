using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace LearnPath.ViewModels
{
    public class RegexConverter : IValueConverter
    {
        public static RegexConverter Instance = new();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return value switch
            {
                Regex regex => regex.ToString(),
                _ => string.Empty,
            };
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return value switch
            {
                string regex when !string.IsNullOrEmpty(regex) => new Regex(regex),
                _ => null,
            };
        }
    }
}
