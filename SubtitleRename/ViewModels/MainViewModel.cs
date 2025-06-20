using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SubtitleRename.Models;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;

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
        public RegexMatcher? SubtitleFilter
        {
            get => Subtitle.Matcher;
            set
            {
                Subtitle.Matcher = value;
                OnPropertyChanged(nameof(LINQBinding));
            }
        }
        public RegexMatcher? VideoFilter
        {
            get => Video.Matcher;
            set
            {
                Video.Matcher = value;
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
        public void SubtitleNext()
        {
            if (Subtitle.Next())
            {
                OnPropertyChanged(nameof(LINQBinding));
            }
        }

        [RelayCommand]
        public void SubtitlePrevious()
        {
            if (Subtitle.Previous())
            {
                OnPropertyChanged(nameof(LINQBinding));
            }
        }

        [RelayCommand]
        public void VideoNext()
        {
            if (Video.Next())
            {
                OnPropertyChanged(nameof(LINQBinding));
            }
        }

        [RelayCommand]
        public void VideoPrevious()
        {
            if (Video.Previous())
            {
                OnPropertyChanged(nameof(LINQBinding));
            }
        }

        [RelayCommand]
        public void ChangeName()
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

            Subtitle.Matcher = null;
            Video.Matcher = null;
            Subtitle.OnDirectoryChanged();

            OnPropertyChanged(nameof(LINQBinding));
        }

        public MainViewModel()
        {
            Subtitle = new(() => rootFolder, FileType.Subtitle);
            Video = new(() => rootFolder, FileType.Video);
            Subtitle.Suffixes = ["ass", "ssa", "srt"];
            Video.Suffixes = ["mkv", "mp4"];
            Subtitle.Matcher = new RegexMatcher(DefaultRegex());
            Video.Matcher = new RegexMatcher(DefaultRegex());
        }

        [GeneratedRegex("[^\\d](\\d\\d)[^\\d]")]
        private static partial Regex DefaultRegex();
    }
}
