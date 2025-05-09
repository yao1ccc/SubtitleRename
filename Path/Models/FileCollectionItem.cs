using System.IO;
using System.Text.RegularExpressions;


namespace LearnPath.Models
{
    enum FileType
    {
        Video,
        Subtitle,
    }

    class FileCollectionItem(FileInfo file, FileType fileType)
    {
        public FileInfo FileInfo { get; } = file;
        public FileInfo? TargetFileInfo { get; set; }
        public FileType FileType { get; } = fileType;
        public Match? MatchResult { get; set; }
    }
}
