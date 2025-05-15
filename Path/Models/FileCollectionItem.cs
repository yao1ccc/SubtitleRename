using System.IO;
using System.Text.RegularExpressions;


namespace SubtitleRename.Models
{
    sealed class FileCollectionItem(FileInfo file)
    {
        public FileInfo FileInfo { get; } = file;
        public FileInfo? TargetFileInfo { get; set; }
        public Match? MatchResult { get; set; }
        public string ShowInfo => (TargetFileInfo is null) ? FileInfo.Name : TargetFileInfo.Name;
        public int MatchStart {  get; set; } = 0;
        public int MatchLength { get; set; } = 0;
    }
}
