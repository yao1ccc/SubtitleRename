using System.ComponentModel;
using System.IO;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using SubtitleRename.Models;

namespace SubtitleRename.ViewModels
{
    sealed partial class MainViewModel : ObservableObject, IDataErrorInfo
    {
        public List<string> SubtitleSuffixes
        {
            get => Subtitle.Suffixes;
            set
            {
                Subtitle.Suffixes = value;
                OnPropertyChanged(nameof(LINQBinding));
            }
        }

        public List<string> VideoSuffixes
        {
            get => Video.Suffixes;
            set
            {
                Video.Suffixes = value;
                OnPropertyChanged(nameof(LINQBinding));
            }
        }
        public RegexFilter? SubtitleFilter
        {
            get => Subtitle.Filter;
            set
            {
                Subtitle.Filter = value;
                OnPropertyChanged(nameof(LINQBinding));
            }
        }

        public RegexFilter? VideoFilter
        {
            get => Video.Filter;
            set
            {
                Video.Filter = value;
                OnPropertyChanged(nameof(LINQBinding));
            }
        }

        public DirectoryInfo? RootFolder
        {
            get => rootFolder;
            set
            {
                SetProperty(ref rootFolder, value);
                Video.PathUpdate();
                Subtitle.PathUpdate();
                OnPropertyChanged(nameof(LINQBinding));
            }
        }

        public IEnumerable<HighLightText> LINQBinding =>
            Subtitle.FileCollections
                .Concat(Video.FileCollections)
                .OrderBy(x => x.OrderInfo)
                .Select(x => (HighLightText)x);

        private DirectoryInfo? rootFolder;
        private readonly FileCollection Subtitle;
        private readonly FileCollection Video;
        public string Error => string.Empty;

        public string this[string columnName]
        {
            get
            {
                return columnName switch
                {
                    nameof(SubtitleSuffixes) or nameof(VideoSuffixes) => VideoSuffixes.Any(
                        SubtitleSuffixes.Contains
                    )
                        ? "suffix conflict"
                        : string.Empty,
                    nameof(RootFolder) => RootFolder is not null && !RootFolder.Exists
                        ? "folder not exist"
                        : string.Empty,
                    _ => string.Empty,
                };
            }
        }

        public MainViewModel()
        {
            Subtitle = new(() => rootFolder);
            Video = new(() => rootFolder);
            Subtitle.Suffixes = ["ass", "ssa", "srt"];
            Video.Suffixes = ["mkv", "mp4"];
        }
    }
}
