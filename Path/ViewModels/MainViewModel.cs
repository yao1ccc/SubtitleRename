using System.IO;
using LearnPath.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace LearnPath.ViewModels
{
    partial class MainViewModel : ObservableObject, IDataErrorInfo
    {
        public List<string> SubtitleSuffixes
        {
            get => subtitleSuffixes;
            set
            {
                subtitleSuffixes = value;
                Model.PathUpdate(this);
                OnPropertyChanged(nameof(FileCollectionShows));
            }
        }
        public List<string> VideoSuffixes
        {
            get => videoSuffixes;
            set
            {
                videoSuffixes = value;
                Model.PathUpdate(this);
                OnPropertyChanged(nameof(FileCollectionShows));
            }
        }
        public Regex? SubtitleFilter
        {
            get => subtitleFilter;
            set
            {
                subtitleFilter = value;

                if (value is null)
                {
                    SubtitleFilterGroups = [];
                    SubtitleFilterIndex = 0;
                    return;
                }

                SubtitleFilterGroups = value.GetGroupNumbers();

                if (FileCollectionShows.Count == 0)
                {
                    return;
                }

                foreach (var fileCollectionItem in from FileCollectionItem fileCollectionItem in FileCollectionShows
                                                   where fileCollectionItem.FileType == FileType.Subtitle
                                                   select fileCollectionItem)
                {
                    fileCollectionItem.MatchResult = value.Match(fileCollectionItem.FileInfo.Name);
                }
            }
        }
        public Regex? VideoFilter
        {
            get => videoFilter;
            set
            {
                videoFilter = value;
                if (value is not null)
                {
                    VideoFilterGroups = value.GetGroupNumbers();
                    foreach (var fileCollectionItem in from FileCollectionItem fileCollectionItem in FileCollectionShows
                                                       where fileCollectionItem.FileType == FileType.Video
                                                       select fileCollectionItem)
                    {
                        fileCollectionItem.MatchResult = value.Match(fileCollectionItem.FileInfo.Name);
                    }
                }
            }
        }
        public DirectoryInfo? RootFolder
        {
            get => rootFolder;
            set
            {
                SetProperty(ref rootFolder, value);
                Model.PathUpdate(this);
                OnPropertyChanged(nameof(FileCollectionShows));
            }
        }
        public int[] VideoFilterGroups { get; set; } = [];
        public int[] SubtitleFilterGroups { get; set; } = [];
        public int VideoFilterIndex { get; set; } = 0;
        public int SubtitleFilterIndex { get; set; } = 0;
        public List<FileCollectionItem> FileCollectionShows { set; get; } = [];

        public string Error => string.Empty;

        public string this[string columnName]
        {
            get
            {
                return columnName switch
                {
                    nameof(SubtitleSuffixes) or nameof(VideoSuffixes) =>
                        VideoSuffixes.Any(SubtitleSuffixes.Contains) ?
                        "suffix conflict" : string.Empty,
                    nameof(RootFolder) =>
                        RootFolder is not null && !RootFolder.Exists ?
                        "folder not exist" : string.Empty,
                    _ => string.Empty,
                };
            }
        }

        private DirectoryInfo? rootFolder;
        private Regex? subtitleFilter;
        private Regex? videoFilter;
        private List<string> subtitleSuffixes = ["srt", "ass", "ssa"];
        private List<string> videoSuffixes = ["mkv", "mp4"];
    }

}
