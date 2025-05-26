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
        public HighLightText ToHighLightText(int i, FileType fileType)
        {
            if (MatchResult is null)
            {
                return ToHighLightText(fileType);
            }
            return (
                new HighLightText(
                    FileInfo.Name,
                    MatchResult.Groups[i].Index,
                    MatchResult.Groups[i].Length,
                    fileType
                )
            );
        }
        public HighLightText ToHighLightText(FileType fileType)
        {
            return new(FileInfo.Name, 0, 0, fileType);
        }
    }
}
