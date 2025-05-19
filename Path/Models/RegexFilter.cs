using System.Text.RegularExpressions;

namespace SubtitleRename.Models
{
    sealed class RegexFilter
    {
        public readonly Regex regex;
        private readonly int[] groups;
        private int groupIndex = 0;

        public RegexFilter(Regex re)
        {
            regex = re;
            groups = regex.GetGroupNumbers();
        }

        public void Next()
        {
            GroupIndex = (GroupIndex + 1) % groups.Length;
        }

        public void Previous()
        {
            GroupIndex = (GroupIndex - 1) % groups.Length;
        }

        public int GroupIndex { get => groupIndex; set => groupIndex = value; }
        public override string ToString() => regex.ToString();
    }
}
