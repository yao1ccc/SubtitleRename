using System.IO;
using System.Windows.Data;

namespace LearnPath.ViewModels
{
    public class DirectoryInfoConverter : IValueConverter
    {
        public static DirectoryInfoConverter Instance = new();
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            return value switch
            {
                DirectoryInfo directoryInfo => directoryInfo.FullName,
                _ => throw new ArgumentException("path transforms err")
            };
        }

        public object? ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            return value switch
            {
                string path when !string.IsNullOrWhiteSpace(path) => new DirectoryInfo(path),
                _ => null
            };
        }
    }

}
