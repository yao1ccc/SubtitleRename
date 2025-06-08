using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace SubtitleRename
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindowKeyEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                itemsControl.Focus();
                e.Handled = true;
            }
        }

        private void WindowDragEnter(object sender, DragEventArgs e)
        {
            DragMask.Background = new SolidColorBrush(
                new Color()
                {
                    A = 20,
                    R = 255,
                    G = 255,
                    B = 255,
                }
            );
        }

        private void MaskDrag(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var value = e.Data.GetData(DataFormats.FileDrop) as string[];
                RootText.Focus();
                RootText.Text = value![0];
                itemsControl.Focus();
            }
            DragMask.Background = null;
        }

        private void MaskLeave(object sender, DragEventArgs e)
        {
            {
                DragMask.Background = null;
            }
        }
    }
}
