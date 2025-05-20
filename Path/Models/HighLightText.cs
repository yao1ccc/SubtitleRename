namespace SubtitleRename.Models
{
    class HighLightText(string text, int start, int length)
    {
        public string Text = text;
        public int Start = start;
        public int Length = length;

        public static explicit operator HighLightText(FileCollectionItem fileCollectionItem)
        {
            return new(
                fileCollectionItem.FileInfo.Name,
                fileCollectionItem.MatchStart,
                fileCollectionItem.MatchLength
            );
        }
    }
}
