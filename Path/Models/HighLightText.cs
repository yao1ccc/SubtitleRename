namespace SubtitleRename.Models
{
    record HighLightText(string Text, int Start, int Length)
    {
        public string Text = Text;
        public int Start = Start;
        public int Length = Length;
    }
}
