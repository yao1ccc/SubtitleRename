using System.Globalization;
using System.Windows.Data;

namespace SubtitleRename.ViewModels
{
    /// <summary>
    /// String to string array by space
    /// </summary>
    sealed class SplitConverter : IValueConverter
    {
        public static SplitConverter Instance { get; } = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value) switch
            {
                string[] list => string.Join(" ", list),
                _ => throw new ArgumentException("List err"),
            };
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture
        )
        {
            return value switch
            {
                string Input when !string.IsNullOrWhiteSpace(Input) => Input.Split(
                    [' '],
                    StringSplitOptions.RemoveEmptyEntries
                ),
                _ => new ArgumentException("ToString[] err"),
            };
        }
    }
}
