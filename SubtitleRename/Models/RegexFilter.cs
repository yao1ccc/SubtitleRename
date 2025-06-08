using System.Text.RegularExpressions;

namespace SubtitleRename.Models
{
    sealed class RegexFilter(Regex re)
    {
        public readonly Regex regex = re;
        public int GroupIndex { get; set; } = 0;

        private int Length => regex.GetGroupNumbers().Length;

        public void Next()
        {
            GroupIndex = (GroupIndex + 1) % Length;
        }

        public void Previous()
        {
            GroupIndex = (GroupIndex - 1) % Length;
        }

        public override string ToString() => regex.ToString();
    }
}
