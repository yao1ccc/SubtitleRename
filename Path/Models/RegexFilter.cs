using System.Text.RegularExpressions;

namespace SubtitleRename.Models
{
    class RegexFilter
    {
        public readonly Regex regex;
        private readonly int[] groups;
        private int index = 0;

        public RegexFilter(Regex re) {
            regex = re;
            groups = regex.GetGroupNumbers();
        }

        public void Next() {
            index = (index + 1) % groups.Length;
        }

        public void Previous() {
            index = (index - 1) % groups.Length;
        }

        public void SetIndex(int i) {
            index = i;
        }

        public override string ToString() => regex.ToString();
    }
}
