using System.Text.RegularExpressions;

namespace SubtitleRename.Models
{
    sealed class RegexFilter(Regex re)
    {
        public readonly Regex regex = re;
        public int FilterIndex { get; set; }

        private int Length => regex.GetGroupNumbers().Length;

        public bool Next()
        {
            if (FilterIndex < Length - 1)
            {
                FilterIndex++;
                return false;
            }
            else
            {
                FilterIndex = 0;
                return true;
            }
        }

        public bool Previous()
        {
            if (FilterIndex > 0)
            {
                FilterIndex--;
                return false;
            }
            else
            {
                FilterIndex = Length - 1;
                return true;
            }
        }

        public override string ToString() => regex.ToString();
    }
}
