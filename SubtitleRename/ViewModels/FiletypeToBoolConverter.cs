using SubtitleRename.Models;
using System.Globalization;
using System.Windows.Data;

namespace SubtitleRename.ViewModels
{
    /// <summary>
    /// Video to true, subtitle to false
    /// </summary>
    internal class FiletypeToBoolConverter : IValueConverter
    {
        public static FiletypeToBoolConverter Instance { get; } = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((HighLightText)value).FileType switch
            {
                FileType.Video => true,
                FileType.Subtitle => false,
                _ => throw new Exception(),
            };
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture
        )
        {
            throw new Exception();
        }
    }
}
