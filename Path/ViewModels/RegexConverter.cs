using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;
using SubtitleRename.Models;

namespace SubtitleRename.ViewModels
{
    sealed class RegexConverter : IValueConverter
    {
        public static RegexConverter Instance { get; } = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                RegexFilter regex => regex.ToString(),
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
                    return new RegexFilter(regex);
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
