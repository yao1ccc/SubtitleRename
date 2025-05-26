namespace SubtitleRename.Models
{
    record HighLightText(string Text, int Start, int Length, FileType FileType)
    {
        public string Text = Text;
        public int Start = Start;
        public int Length = Length;
        public FileType FileType = FileType;
    }
}
