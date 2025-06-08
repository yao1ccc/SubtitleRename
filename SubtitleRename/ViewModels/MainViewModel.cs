using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubtitleRename.Models;

namespace SubtitleRename.ViewModels
{
    sealed partial class MainViewModel : ObservableObject, IDataErrorInfo
    {
        public string[] SubtitleSuffixes
        {
            get => Subtitle.Suffixes;
            set
            {
                Subtitle.Suffixes = value;
                OnPropertyChanged(nameof(LINQBinding));
            }
        }
        public string[] VideoSuffixes
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
                Video.OnDirectoryChanged();
                Subtitle.OnDirectoryChanged();
                OnPropertyChanged(nameof(LINQBinding));
            }
        }

        public IEnumerable<HighLightText> LINQBinding =>
            Video
                .HighLightTexts.Concat(Subtitle.HighLightTexts)
                .OrderBy(static x => x.Text.Substring(x.Start, x.Length));

        private DirectoryInfo? rootFolder;
        private readonly FileManager Subtitle;
        private readonly FileManager Video;

        public string Error => string.Empty;
        public string this[string columnName] =>
            columnName switch
            {
                nameof(SubtitleSuffixes) or nameof(VideoSuffixes) => VideoSuffixes.Any(
                    SubtitleSuffixes.Contains
                )
                    ? "suffix conflict"
                    : string.Empty,
                _ => string.Empty,
            };

        [RelayCommand]
        public void SubtitleNextIndex()
        {
            Subtitle.Filter?.Next();
            OnPropertyChanged(nameof(LINQBinding));
        }

        [RelayCommand]
        public void SubtitlePreviousIndex()
        {
            Subtitle.Filter?.Previous();
            OnPropertyChanged(nameof(LINQBinding));
        }

        [RelayCommand]
        public void VideoNextIndex()
        {
            Video.Filter?.Next();
            OnPropertyChanged(nameof(LINQBinding));
        }

        [RelayCommand]
        public void VideoPreviousIndex()
        {
            Video.Filter?.Previous();
            OnPropertyChanged(nameof(LINQBinding));
        }

        [RelayCommand]
        public void FileConverter()
        {
            Subtitle.TargetNameUpdate(Video);

            foreach (var f in Subtitle.FileCollections)
            {
                if (f.TargetName is null || rootFolder is null)
                {
                    return;
                }
                f.FileInfo.MoveTo(Path.Combine(rootFolder.ToString(), f.TargetName));
            }

            Subtitle.Filter = null;
            Video.Filter = null;
            Subtitle.OnDirectoryChanged();

            OnPropertyChanged(nameof(LINQBinding));
        }

        public MainViewModel()
        {
            Subtitle = new(() => rootFolder, FileType.Subtitle);
            Video = new(() => rootFolder, FileType.Video);
            Subtitle.Suffixes = ["ass", "ssa", "srt"];
            Video.Suffixes = ["mkv", "mp4"];
            Subtitle.Filter = new RegexFilter(DefaultRegex());
            Video.Filter = new RegexFilter(DefaultRegex());
        }

        [GeneratedRegex("[^\\d](\\d\\d)[^\\d]")]
        private static partial Regex DefaultRegex();
    }
}
