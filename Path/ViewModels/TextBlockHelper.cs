using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using SubtitleRename.Models;

namespace SubtitleRename.ViewModels
{
    sealed class TextBlockHelper
    {
        public static HighLightText GetRichText(DependencyObject obj)
        {
            return (HighLightText)obj.GetValue(RichText);
        }

        public static void SetRichText(DependencyObject obj, HighLightText value)
        {
            if (obj is TextBlock textBlock)
            {
                textBlock.SetValue(RichText, value);
            }
        }

        public static void OnRichTextChanged(
            DependencyObject obj,
            DependencyPropertyChangedEventArgs e
        )
        {
            if (obj is TextBlock textBlock)
            {
                var NewHighLightText = (HighLightText)e.NewValue;
                var OldHighLightText = (HighLightText)e.OldValue;

                if (NewHighLightText.Text != OldHighLightText.Text)
                {
                    textBlock.Text = NewHighLightText.Text;
                }

                if (NewHighLightText.Start == 0 || NewHighLightText.Length == 0)
                {
                    textBlock.Background = null;
                }

                TextPointer Start = textBlock.ContentStart.GetPositionAtOffset(
                    NewHighLightText.Start + 1
                );
                TextPointer End = Start.GetPositionAtOffset(NewHighLightText.Length);
                new TextRange(Start, End).ApplyPropertyValue(
                    TextElement.BackgroundProperty,
                    new SolidColorBrush(Colors.CornflowerBlue)
                );
            }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RichText = DependencyProperty.RegisterAttached(
            "RichText",
            typeof(HighLightText),
            typeof(TextBlockHelper),
            new PropertyMetadata(new HighLightText("", 0, 0, FileType.Video), OnRichTextChanged)
        );
    }
}
