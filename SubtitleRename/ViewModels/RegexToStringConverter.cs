using SubtitleRename.Models;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace SubtitleRename.ViewModels
{
    sealed class RegexToStringConverter : IValueConverter
    {
        public static RegexToStringConverter Instance { get; } = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                RegexMatcher regex => regex.ToString(),
                _ => string.Empty,
            };
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s && !string.IsNullOrEmpty(s))
            {
                try
                {
                    Regex regex = new(s);
                    return new RegexMatcher(regex);
                }
                catch
                {
                    return new ArgumentException("Regex err");
                }
            }
            return null;
        }
    }
}
