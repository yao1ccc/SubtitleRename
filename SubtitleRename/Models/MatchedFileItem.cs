using System.IO;
using System.Text.RegularExpressions;

namespace SubtitleRename.Models
{
    sealed class MatchedFileItem(FileInfo file)
    {
        public FileInfo FileInfo { get; } = file;
        public string? TargetName { get; set; }
        public MatchCollection? MatchResult { get; set; }
        public int MatchIndex { get; set; }

        /// <param name="i">
        /// Capturing group index
        /// </param>
        public string? MatchText(int i) => MatchResult?[MatchIndex].Groups[i].Value;

        /// <summary>
        /// Next regex match
        /// </summary>
        public void Next()
        {
            if (MatchResult is null)
            {
                return;
            }

            if (MatchIndex < MatchResult.Count - 1)
            {
                MatchIndex++;
            }
            else
            {
                MatchIndex = 0;
            }
        }


        /// <summary>
        /// Previous regex match
        /// </summary>
        public void Previous()
        {
            if (MatchResult is null)
            {
                return;
            }

            if (MatchIndex > 0)
            {
                MatchIndex--;
            }
            else
            {
                MatchIndex = MatchResult.Count - 1;
            }
        }

        public HighLightText ToHighLightText(int i, FileType fileType)
        {
            if (MatchResult is null)
            {
                return ToHighLightText(fileType);
            }
            return new HighLightText(
                FileInfo.Name,
                MatchResult[MatchIndex].Groups[i].Index,
                MatchResult[MatchIndex].Groups[i].Length,
                fileType
            );
        }

        public HighLightText ToHighLightText(FileType fileType)
        {
            return new(FileInfo.Name, 0, 0, fileType);
        }
    }
}
