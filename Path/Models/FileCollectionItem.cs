using System.IO;
using System.Text.RegularExpressions;


namespace SubtitleRename.Models
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
        public string ShowInfo
        {
            get
            {
                return (TargetFileInfo is null) ? FileInfo.Name : TargetFileInfo.Name;
            }
        }
    }
}
