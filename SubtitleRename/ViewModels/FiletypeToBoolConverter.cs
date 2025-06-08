using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using SubtitleRename.Models;

namespace SubtitleRename.ViewModels
{
    internal class FiletypeToBoolConverter : IValueConverter
    {
        public static FiletypeToBoolConverter Instance { get; } = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((HighLightText)value).FileType switch
            {
                FileType.Video => true,
                FileType.Subtitle => false,
                _ => throw new NotSupportedException(),
            };
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture
        )
        {
            throw new NotImplementedException();
        }
    }
}
