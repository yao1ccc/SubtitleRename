using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace SubtitleRename.Models
{
    sealed partial class FileCollection(Func<DirectoryInfo?> func)
    {
        public RegexFilter? Filter
        {
            get => filter;
            set
            {
                filter = value;
                FilterUpdate();
            }
        }
        public List<string> Suffixes
        {
            get => suffixes;
            set
            {
                suffixes = value;
                PathUpdate();
            }
        }
        public List<FileCollectionItem> FileCollections
        {
            get => fileCollections;
            set => fileCollections = value;
        }

        private readonly Func<DirectoryInfo?> directoryGetter = func;
        private RegexFilter? filter;
        private List<string> suffixes = [];
        private List<FileCollectionItem> fileCollections = [];
        private DirectoryInfo? DirectoryHandler => directoryGetter.Invoke();

        public void PathUpdate()
        {
            fileCollections.Clear();

            if (DirectoryHandler is null)
            {
                return;
            }

            foreach (string s in suffixes)
            {
                foreach (var f in DirectoryHandler.GetFiles("*." + s))
                {
                    FileCollections.Add(new FileCollectionItem(f));
                    Debug.WriteLine(f.Name);
                }
            }

            FilterUpdate();
        }

        private void FilterUpdate()
        {
            if (filter is null)
            {
                foreach (var f in fileCollections)
                {
                    f.MatchResult = null;
                }
                IndexUpdate();
                return;
            }

            if (FileCollections.Count == 0)
            {
                return;
            }

            filter.GroupIndex = 0;

            foreach (var f in FileCollections)
            {
                if (filter.regex.Match(f.FileInfo.Name).Success)
                {
                    f.MatchResult = filter.regex.Match(f.FileInfo.Name);
                }
            }

            var sample = FileCollections.FirstOrDefault();
            if (sample?.MatchResult is not null)
            {
                for (int i = 1; i < sample.MatchResult.Groups.Count; i++)
                {
                    var match = DigitRegex().Match(sample.MatchResult.Groups[i].Value);
                    if (match.Success)
                    {
                        filter.GroupIndex = i;
                        Debug.WriteLine(
                            $"Index:{i} all:{sample.MatchResult.Value} value:{sample.MatchResult.Groups[i].Value}"
                        );
                        break;
                    }
                }
            }
            IndexUpdate();
        }

        private void IndexUpdate()
        {
            foreach (var f in fileCollections)
            {
                if (f.MatchResult is null)
                {
                    f.MatchStart = 0;
                    f.MatchLength = 0;
                    continue;
                }
                var match = f.MatchResult.Groups[filter!.GroupIndex];
                f.MatchStart = match.Index;
                f.MatchLength = match.Length;
                Debug.WriteLine(match.Value);
            }
        }

        [GeneratedRegex("\\d")]
        private static partial Regex DigitRegex();
    }
}
