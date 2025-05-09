using System.IO;
using LearnPath.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;

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
                PathUpdate();
            }
        }
        public List<string> VideoSuffixes
        {
            get => videoSuffixes;
            set
            {
                videoSuffixes = value;
                PathUpdate();
            }
        }
        public RegexFilter? SubtitleFilter
        {
            get { return subtitleFilter; }
            set
            {
                subtitleFilter = value;

                if (subtitleFilter is not null)
                {
                    FilterUpdate(ref subtitleFilter, FileType.Subtitle);
                }
            }
        }


        public RegexFilter? VideoFilter
        {
            get { return videoFilter; }
            set 
            {
                videoFilter = value;

                if (videoFilter is not null)
                {
                    FilterUpdate(ref videoFilter, FileType.Video);
                }  
            }
        }

        public DirectoryInfo? RootFolder
        {
            get => rootFolder;
            set
            {
                SetProperty(ref rootFolder, value);
                PathUpdate();
            }
        }

        public ObservableCollection<FileCollectionItem> FileCollectionShows { set; get; } = [];

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
        private List<string> subtitleSuffixes = ["srt", "ass", "ssa"];
        private List<string> videoSuffixes = ["mkv", "mp4"];
        private RegexFilter? videoFilter;
        private RegexFilter? subtitleFilter;

        private void FilterUpdate(ref RegexFilter filter, FileType type) {
            if (FileCollectionShows.Count == 0)
            {
                return;
            }

            foreach (var f in from FileCollectionItem fileCollectionItem in FileCollectionShows
                                               where fileCollectionItem.FileType == type
                                               select fileCollectionItem)
            {
                f.MatchResult = filter.regex.Match(f.FileInfo.Name);
            }

            var sample = FileCollectionShows.FirstOrDefault(f => f.FileType == type);
            if (sample is not null && sample.MatchResult is not null && sample.MatchResult.Success)
            {
                for (int i = 1; i < sample.MatchResult.Groups.Count; i++)
                {
                    var match = Regex.Match(sample.MatchResult.Groups[i].Value, "\\d");
                    if (match.Success) 
                    {
                        filter.SetIndex(i);
                        break;
                    }
                }
            }
        }

        private void PathUpdate() {
            FileCollectionShows.Clear();

            if (RootFolder == null)
            {
                return;
            }

            foreach (string suffix in SubtitleSuffixes)
            {
                foreach (var subtitleFile in RootFolder.GetFiles("*." + suffix))
                {
                    FileCollectionShows.Add(new FileCollectionItem(subtitleFile, FileType.Subtitle));
                }
            }

            foreach (string suffix in VideoSuffixes)
            {
                foreach (var subtitleFile in RootFolder.GetFiles("*." + suffix))
                {
                    FileCollectionShows.Add(new FileCollectionItem(subtitleFile, FileType.Video));
                }
            }
        }
    }

}
