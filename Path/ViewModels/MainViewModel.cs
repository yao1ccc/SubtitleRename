using System.IO;
using SubtitleRename.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace SubtitleRename.ViewModels
{
    sealed partial class MainViewModel : ObservableObject, IDataErrorInfo
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
        public ObservableCollection<FileCollectionItem> SubtitleCollectionShows { get; set; } = [];
        public ObservableCollection<FileCollectionItem> VideoCollectionShows { get; set; } = [];
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

            if (VideoCollectionShows.Count == 0 || videoFilter is null)
            {
                return;
            }

            videoFilter.GroupIndex = 0;

            foreach (var f in VideoCollectionShows)
            {
                f.MatchResult = videoFilter.regex.Match(f.FileInfo.Name);
            }

            var sample = VideoCollectionShows.FirstOrDefault();
            if (sample is not null && sample.MatchResult is not null && sample.MatchResult.Success)
            {
                for (int i = 1; i < sample.MatchResult.Groups.Count; i++)
                {
                    var match = DigitRegex().Match(sample.MatchResult.Groups[i].Value);
                    if (match.Success)
                    {
                        videoFilter.GroupIndex = i;
                        Debug.WriteLine($"Index:{i} all:{sample.MatchResult.Value} value:{sample.MatchResult.Groups[i].Value}");
                        break;
                    }
                }
            }

            VideoIndexUpdate();
        }

        private void SubtitleFilterUpdate() {

            if (SubtitleCollectionShows.Count == 0 || subtitleFilter is null)
            {
                return;
            }

            subtitleFilter.GroupIndex = 0;

            foreach (var f in SubtitleCollectionShows)
            {
                f.MatchResult = subtitleFilter.regex.Match(f.FileInfo.Name);
            }

            var sample = SubtitleCollectionShows.FirstOrDefault();
            if (sample is not null && sample.MatchResult is not null && sample.MatchResult.Success)
            {
                for (int i = 1; i < sample.MatchResult.Groups.Count; i++)
                {
                    var match = DigitRegex().Match(sample.MatchResult.Groups[i].Value);
                    if (match.Success)
                    {
                        subtitleFilter.GroupIndex = i;
                        Debug.WriteLine($"Index:{i} all:{sample.MatchResult.Value} value:{sample.MatchResult.Groups[i].Value}");
                        break;
                    }
                }
            }

            SubtitleIndexUpdate();
        }

        private void PathUpdate() {

            SubtitleCollectionShows.Clear();
            VideoCollectionShows.Clear();

            if (RootFolder is null)
            {
                return;
            }

            foreach (string suffix in SubtitleSuffixes)
            {
                foreach (var f in RootFolder.GetFiles("*." + suffix))
                {
                    SubtitleCollectionShows.Add(new FileCollectionItem(f));
                    Debug.WriteLine(f.Name);
                }
            }

            foreach (string suffix in VideoSuffixes)
            {
                foreach (var f in RootFolder.GetFiles("*." + suffix))
                {
                    VideoCollectionShows.Add(new FileCollectionItem(f));
                    Debug.WriteLine(f.Name);
                }
            }

            VideoFilterUpdate();
            SubtitleFilterUpdate();
        }
        private void SubtitleIndexUpdate() {

            if (subtitleFilter is null) { return; }

            foreach (var f in SubtitleCollectionShows)
            {
                if (f.MatchResult is null) { continue; }
                var match = f.MatchResult.Groups[subtitleFilter.GroupIndex];
                f.MatchStart = match.Index;
                f.MatchLength = match.Length;
                Debug.WriteLine(match.Value);
            }
        }
        private void VideoIndexUpdate() {

            if (videoFilter is null) { return; }

            foreach (var f in VideoCollectionShows)
            {
                if (f.MatchResult is null || !f.MatchResult.Success) { continue; }
                var match = f.MatchResult.Groups[videoFilter.GroupIndex];
                f.MatchStart = match.Index;
                f.MatchLength = match.Length;
                Debug.WriteLine(match.Value);
            }
        }

        [GeneratedRegex("\\d")]
        private static partial Regex DigitRegex();
    }
}
