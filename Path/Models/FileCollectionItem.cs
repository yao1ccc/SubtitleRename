using System.IO;
using System.Text.RegularExpressions;

namespace SubtitleRename.Models
{
    sealed class FileCollectionItem(FileInfo file)
    {
        public FileInfo FileInfo { get; } = file;
        public string? TargetName { get; set; }
        public Match? MatchResult { get; set; }
        public string? MatchText(int i) => MatchResult?.Groups[i].Value;
        public HighLightText ToHighLightText(int i)
        {
            if (MatchResult is null)
            {
                return ToHighLightText();
            }
            return (
                new HighLightText(
                    FileInfo.Name,
                    MatchResult.Groups[i].Index,
                    MatchResult.Groups[i].Length
                )
            );
        }
        public HighLightText ToHighLightText()
        {
            return new(FileInfo.Name, 0, 0);
        }
    }
}
