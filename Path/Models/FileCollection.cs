using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace SubtitleRename.Models
{
    sealed partial class FileCollection(Func<DirectoryInfo?> func)
    {
        public RegexFilter? Filter
        {
            get => regexFilter;
            set
            {
                regexFilter = value;
                OnRegexChanged();
            }
        }
        public List<string> Suffixes
        {
            get => suffixes;
            set
            {
                suffixes = value;
                OnSuffixChanged();
            }
        }
        public List<FileCollectionItem> FileCollections
        {
            get => fileCollections;
            set => fileCollections = value;
        }

        public IEnumerable<HighLightText> HighLightTexts =>
            fileCollections.Select(x =>
                (regexFilter is null)
                    ? x.ToHighLightText()
                    : x.ToHighLightText(regexFilter.GroupIndex)
            );

        private readonly Func<DirectoryInfo?> directoryGetter = func;
        private RegexFilter? regexFilter;
        private List<string> suffixes = [];
        private List<FileCollectionItem> fileCollections = [];
        private DirectoryInfo? DirectoryHandler => directoryGetter.Invoke();

        public void OnDirectoryChanged()
        {
            OnSuffixChanged();
        }

        private void OnSuffixChanged()
        {
            fileCollections.Clear();

            if (DirectoryHandler is null)
            {
                return;
            }

            foreach (string s in suffixes)
            {
                fileCollections.AddRange(
                    DirectoryHandler
                        .GetFiles("*." + s)
                        .ToList()
                        .ConvertAll(x => new FileCollectionItem(x))
                );
            }

            OnRegexChanged();
        }

        private void OnRegexChanged()
        {
            if (regexFilter is null)
            {
                foreach (var f in fileCollections)
                {
                    f.MatchResult = null;
                }
                return;
            }

            if (FileCollections.Count == 0)
            {
                return;
            }

            regexFilter.GroupIndex = 0;

            foreach (var f in FileCollections)
            {
                if (regexFilter.regex.Match(f.FileInfo.Name).Success)
                {
                    f.MatchResult = regexFilter.regex.Match(f.FileInfo.Name);
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
                        regexFilter.GroupIndex = i;
                        Debug.WriteLine(
                            $"Index:{i} all:{sample.MatchResult.Value} value:{sample.MatchResult.Groups[i].Value}"
                        );
                        break;
                    }
                }
            }
        }

        public void OnTargetFileUpdate(FileCollection other)
        {
            if (regexFilter is null || other.regexFilter is null)
            {
                return;
            }

            foreach (FileCollectionItem f in fileCollections)
            {
                if (f.MatchText(regexFilter.GroupIndex) is null)
                {
                    f.TargetName = null;
                    continue;
                }

                FileCollectionItem? matchVideo = other.fileCollections.Find(x =>
                    x.MatchText(other.regexFilter.GroupIndex) == f.MatchText(regexFilter.GroupIndex)
                );

                if (matchVideo is not null)
                {
                    f.TargetName =
                        Path.GetFileNameWithoutExtension(matchVideo.FileInfo.ToString())
                        + f.FileInfo.Extension;
                }
            }
        }

        [GeneratedRegex("\\d")]
        private static partial Regex DigitRegex();
    }
}
