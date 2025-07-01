using Microsoft.International.Converters.TraditionalChineseToSimplifiedConverter;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace SubtitleRename.Models
{
    /// <summary>
    /// Main function
    /// </summary>
    /// <param name="func">
    /// Reference to directory
    /// </param>
    sealed partial class FileManager(Func<DirectoryInfo?> func, FileType fileType)
    {
        public RegexMatcher? Matcher
        {
            get => matcher;
            set
            {
                matcher = value;
                OnPropertyChanged();
            }
        }
        public string[] Suffixes
        {
            get => suffixes;
            set
            {
                suffixes = value;
                OnPropertyChanged();
            }
        }

        public List<MatchedFileItem> FileCollections { get; set; } = [];

        public IEnumerable<HighLightText> HighLightTexts =>
            FileCollections.Select(x =>
                (matcher is null)
                    ? x.ToHighLightText(fileType)
                    : x.ToHighLightText(matcher.FilterIndex, fileType)
            );

        private readonly Func<DirectoryInfo?> directoryGetter = func;
        private RegexMatcher? matcher;
        private string[] suffixes = [];

        private DirectoryInfo? DirectoryHandler => directoryGetter.Invoke();
        private readonly FileType fileType = fileType;

        /// <summary>
        /// Next regex match or capturing group
        /// </summary>
        /// <returns>
        /// Whether data changed
        /// </returns>
        public bool Next()
        {
            if (Matcher is null)
            {
                return false;
            }

            if (Matcher.Next())
            {
                foreach (var f in FileCollections)
                {
                    f.Next();
                }
            }
            return true;
        }

        /// <summary>
        /// Previous regex match or capturing group
        /// </summary>
        /// <returns>
        /// Whether data changed
        /// </returns>
        public bool Previous()
        {
            if (Matcher is null)
            {
                return false;
            }

            if (Matcher.Previous())
            {
                foreach (var f in FileCollections)
                {
                    f.Previous();
                }
            }
            return true;
        }

        /// <summary>
        /// Call when directory changed
        /// </summary>
        public void OnDirectoryChanged()
        {
            OnPropertyChanged("Directory");
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            switch (propertyName)
            {
                case "Directory":
                    goto case "Matcher";

                case "Matcher":
                    if (DirectoryHandler is null) { return; }
                    OnSuffixChanged();
                    goto case "Suffixes";

                case "Suffixes":
                    if (FileCollections.Count == 0) { return; }
                    OnRegexChanged();
                    return;
            }
        }

        private void OnSuffixChanged()
        {
            FileCollections.Clear();

            foreach (string s in suffixes)
            {
                FileCollections.AddRange(
                    DirectoryHandler!
                        .GetFiles("*." + s)
                        .ToList()
                        .ConvertAll(x => new MatchedFileItem(x))
                );
            }
        }

        private void OnRegexChanged()
        {
            if (matcher is null)
            {
                foreach (var f in FileCollections)
                {
                    f.MatchResult = null;
                }
                return;
            }

            matcher.FilterIndex = 0;

            foreach (var f in FileCollections)
            {
                if (matcher.regex.Match(f.FileInfo.Name).Success)
                {
                    f.MatchResult = matcher.regex.Matches(f.FileInfo.Name);
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
                        matcher.FilterIndex = i;
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
            if (matcher is null || other.matcher is null)
            {
                return;
            }

            foreach (MatchedFileItem f in FileCollections)
            {
                if (f.MatchText(matcher.FilterIndex) is null)
                {
                    f.TargetName = null;
                    continue;
                }

                MatchedFileItem? matchVideo = other.FileCollections.Find(x =>
                    x.MatchText(other.matcher.FilterIndex)
                    == f.MatchText(matcher.FilterIndex)
                );

                if (matchVideo is not null)
                {
                    f.TargetName =
                        Path.GetFileNameWithoutExtension(matchVideo.FileInfo.ToString())
                        + f.FileInfo.Extension;
                }
            }

        }

        public async Task ConverterAsync(CancellationToken token)
        {
            await Parallel.ForEachAsync(FileCollections, token,
                async (fileCollections, token) =>
            {
                string originalText = await File.ReadAllTextAsync(fileCollections.FileInfo.FullName, token);
                string processedText = ChineseConverter.Convert(originalText, ChineseConversionDirection.TraditionalToSimplified);
                await File.WriteAllTextAsync(fileCollections.FileInfo.FullName, processedText, CancellationToken.None);
            });
        }

        [GeneratedRegex("\\d")]
        private static partial Regex DigitRegex();
    }
}
