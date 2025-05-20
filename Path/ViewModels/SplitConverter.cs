using System.Globalization;
using System.Windows.Data;

namespace SubtitleRename.ViewModels
{
    sealed class SplitConverter : IValueConverter
    {
        public static SplitConverter Instance { get; } = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value) switch
            {
                List<string> list => string.Join(" ", list),
                _ => throw new ArgumentException("List err"),
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                string Input when !string.IsNullOrWhiteSpace(Input) => Input
                    .Split([' '], StringSplitOptions.RemoveEmptyEntries)
                    .ToList(),
                _ => new ArgumentException("ToList err"),
            };
        }
    }
}
