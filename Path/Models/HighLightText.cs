namespace SubtitleRename.Models
{
    struct HighLightText(string text, int start, int length)
    {
        public string Text = text;
        public int Start = start;
        public int Length = length;
    }
}
