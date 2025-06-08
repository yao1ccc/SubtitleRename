using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace SubtitleRename.ViewModels
{
    internal sealed class DirectoryInfoConverter : IValueConverter
    {
        public static DirectoryInfoConverter Instance { get; } = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                DirectoryInfo directoryInfo => directoryInfo.FullName,
                null => "",
                _ => throw new ArgumentException("path transforms err"),
            };
        }

        public object? ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture
        )
        {
            if (value is string path)
            {
                if (string.IsNullOrEmpty(path))
                {
                    return Binding.DoNothing;
                }

                if (Directory.Exists(path))
                {
                    return new DirectoryInfo(path);
                }
                else if (File.Exists(path))
                {
                    return new FileInfo(path).Directory;
                }
            }

            return new ArgumentException("directory");
        }
    }
}
