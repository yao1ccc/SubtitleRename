using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;
using LearnPath.Models;

namespace LearnPath.ViewModels
{
    public class RegexConverter : IValueConverter
    {
        public static RegexConverter Instance = new();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return value switch
            {
                RegexFilter regex => regex.ToString(),
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
