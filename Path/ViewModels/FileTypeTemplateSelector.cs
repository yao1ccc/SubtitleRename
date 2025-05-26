using System.Windows;
using System.Windows.Controls;
using SubtitleRename.Models;

namespace SubtitleRename.ViewModels
{
    class FileTypeTemplateSelector : DataTemplateSelector
    {
        public static FileTypeTemplateSelector Instance { get; } = new();

        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            return container is FrameworkElement framework && item is HighLightText highLightText
                ? highLightText.FileType switch
                {
                    FileType.Subtitle => framework.FindResource("Subtitle") as DataTemplate,
                    FileType.Video => framework.FindResource("Video") as DataTemplate,
                    _ => null,
                }
                : null;
        }
    }
}
