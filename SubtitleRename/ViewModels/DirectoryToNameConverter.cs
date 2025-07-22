using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace SubtitleRename.ViewModels
{
    sealed class DirectoryToNameConverter : IValueConverter
    {
        public static DirectoryToNameConverter Instance { get; } = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                DirectoryInfo directoryInfo => directoryInfo.Name,
                _ => string.Empty,
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("DirectoryNameConverter");
        }
    }
}
