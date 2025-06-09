using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace SubtitleRename.Models
{
    sealed partial class FileManager(Func<DirectoryInfo?> func, FileType fileType)
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
        public string[] Suffixes
        {
            get => suffixes;
            set
            {
                suffixes = value;
                OnSuffixChanged();
            }
        }
        public List<FileCollectionItem> FileCollections { get; set; } = [];

        public IEnumerable<HighLightText> HighLightTexts =>
            FileCollections.Select(x =>
                (regexFilter is null)
                    ? x.ToHighLightText(fileType)
                    : x.ToHighLightText(regexFilter.FilterIndex, fileType)
            );

        private readonly Func<DirectoryInfo?> directoryGetter = func;
        private RegexFilter? regexFilter;
        private string[] suffixes = [];

        private DirectoryInfo? DirectoryHandler => directoryGetter.Invoke();
        private readonly FileType fileType = fileType;

        public bool Next()
        {
            if (Filter is null)
            {
                return false;
            }

            if (Filter.Next())
            {
                foreach (var f in FileCollections)
                {
                    f.Next();
                }
            }
            return true;
        }

        public bool Previous()
        {
            if (Filter is null)
            {
                return false;
            }

            if (Filter.Previous())
            {
                foreach (var f in FileCollections)
                {
                    f.Previous();
                }
            }
            return true;
        }

        public void OnDirectoryChanged()
        {
            OnSuffixChanged();
        }

        private void OnSuffixChanged()
        {
            FileCollections.Clear();

            if (DirectoryHandler is null)
            {
                return;
            }

            foreach (string s in suffixes)
            {
                FileCollections.AddRange(
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
                foreach (var f in FileCollections)
                {
                    f.MatchResult = null;
                }
                return;
            }

            if (FileCollections.Count == 0)
            {
                return;
            }

            regexFilter.FilterIndex = 0;

            foreach (var f in FileCollections)
            {
                if (regexFilter.regex.Match(f.FileInfo.Name).Success)
                {
                    f.MatchResult = regexFilter.regex.Matches(f.FileInfo.Name);
                }
            }

            var sample = FileCollections.FirstOrDefault();
            if (sample?.MatchResult is not null)
            {
                for (int i = 1; i < sample.MatchResult[0].Groups.Count; i++)
                {
                    var match = DigitRegex().Match(sample.MatchResult[0].Groups[i].Value);
                    if (match.Success)
                    {
                        regexFilter.FilterIndex = i;
                        Debug.WriteLine(
                            $"Index:{i} all:{sample.MatchResult[0].Value} value:{sample.MatchResult[0].Groups[i].Value}"
                        );
                        break;
                    }
                }
            }
        }

        public void TargetNameUpdate(FileManager other)
        {
            if (regexFilter is null || other.regexFilter is null)
            {
                return;
            }

            foreach (FileCollectionItem f in FileCollections)
            {
                if (f.MatchText(regexFilter.FilterIndex) is null)
                {
                    f.TargetName = null;
                    continue;
                }

                FileCollectionItem? matchVideo = other.FileCollections.Find(x =>
                    x.MatchText(other.regexFilter.FilterIndex)
                    == f.MatchText(regexFilter.FilterIndex)
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
