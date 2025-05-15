using System.Text.RegularExpressions;

namespace SubtitleRename.Models
{
    sealed class RegexFilter
    {
        public readonly Regex regex;
        private readonly int[] groups;

        public RegexFilter(Regex re) {
            regex = re;
            groups = regex.GetGroupNumbers();
        }

        public void Next() {
            GroupIndex = (GroupIndex + 1) % groups.Length;
        }

        public void Previous() {
            GroupIndex = (GroupIndex - 1) % groups.Length;
        }

        public int GroupIndex { get; set; } = 0;
        public override string ToString() => regex.ToString();
    }
}
