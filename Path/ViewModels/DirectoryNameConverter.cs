using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace LearnPath.ViewModels
{
    class DirectoryNameConverter : IValueConverter
    {
        public static DirectoryNameConverter Instance = new();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return value switch
            {
                DirectoryInfo directoryInfo => directoryInfo.Name,
                _ => string.Empty,
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new ArgumentException();
        }
    }
}
