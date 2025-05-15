using SubtitleRename.Models;
using System.Globalization;
using System.Windows.Data;

namespace SubtitleRename.ViewModels
{
    class TextPartConverter : IValueConverter
    {
        public static TextPartConverter Instance { get; } = new();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            if (value is not FileCollectionItem item || parameter is not string partType)
            {
                return string.Empty;
            }

            return item.MatchResult switch
            {
                null when partType == "before" => item.FileInfo.Name,
                null => string.Empty,
                _ => partType switch
                {
                    "before" => item.FileInfo.Name[..item.MatchStart],
                    "highlight" => item.FileInfo.Name.Substring(item.MatchStart, item.MatchLength),
                    "after" => item.FileInfo.Name[(item.MatchStart + item.MatchLength)..],
                    _ => string.Empty,
                },
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return new NotImplementedException();
        }
    }
}
