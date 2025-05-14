using System.IO;
using SubtitleRename.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;

namespace SubtitleRename.ViewModels
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
                SubtitleFilterUpdate();
            }
        }


        public RegexFilter? VideoFilter
        {
            get { return videoFilter; }
            set
            {
                videoFilter = value;
                VideoFilterUpdate();
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

        public ObservableCollection<FileCollectionItem> SubtitleCollectionShows { set; get; } = [];
        public ObservableCollection<FileCollectionItem> VideoCollectionShows { set; get; } = [];

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

        private void VideoFilterUpdate() {

            if (VideoCollectionShows.Count == 0 || videoFilter is null) { return; }

            foreach (var fileCollectionItem in VideoCollectionShows)
            {
                fileCollectionItem.MatchResult = videoFilter.regex.Match(fileCollectionItem.FileInfo.Name);
            }

            var sample = VideoCollectionShows.FirstOrDefault();
            if (sample is not null && sample.MatchResult is not null && sample.MatchResult.Success)
            {
                for (int i = 1; i < sample.MatchResult.Groups.Count; i++)
                {
                    var match = Regex.Match(sample.MatchResult.Groups[i].Value, "\\d");
                    if (match.Success)
                    {
                        videoFilter.SetIndex(i);
                        return;
                    }
                }
            }
        }

        private void SubtitleFilterUpdate() {

            if (SubtitleCollectionShows.Count == 0 || subtitleFilter is null) { return; }

            foreach (var fileCollectionItem in SubtitleCollectionShows)
            {
                fileCollectionItem.MatchResult = subtitleFilter.regex.Match(fileCollectionItem.FileInfo.Name);
            }

            var sample = SubtitleCollectionShows.FirstOrDefault();
            if (sample is not null && sample.MatchResult is not null && sample.MatchResult.Success)
            {
                for (int i = 1; i < sample.MatchResult.Groups.Count; i++)
                {
                    var match = Regex.Match(sample.MatchResult.Groups[i].Value, "\\d");
                    if (match.Success)
                    {
                        subtitleFilter.SetIndex(i);
                        return;
                    }
                }
            }
        }

        private void PathUpdate() {
            SubtitleCollectionShows.Clear();
            VideoCollectionShows.Clear();

            if (RootFolder == null)
            {
                return;
            }

            foreach (string suffix in SubtitleSuffixes)
            {
                foreach (var subtitleFile in RootFolder.GetFiles("*." + suffix))
                {
                    SubtitleCollectionShows.Add(new FileCollectionItem(subtitleFile, FileType.Subtitle));
                }
            }

            foreach (string suffix in VideoSuffixes)
            {
                foreach (var subtitleFile in RootFolder.GetFiles("*." + suffix))
                {
                    VideoCollectionShows.Add(new FileCollectionItem(subtitleFile, FileType.Video));
                }
            }
        }
    }

}
