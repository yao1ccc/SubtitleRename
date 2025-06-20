using System.Text.RegularExpressions;

namespace SubtitleRename.Models
{
    /// <summary>
    /// Regex and capturing group index
    /// </summary>
    sealed class RegexMatcher(Regex re)
    {
        public readonly Regex regex = re;
        public int FilterIndex { get; set; }

        private int Length => regex.GetGroupNumbers().Length;
        /// <summary>
        /// Next capturing group
        /// </summary>
        /// <returns>
        /// Whether loop cycled
        /// </returns>
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

        /// <summary>
        /// Previous capturing group
        /// </summary>
        /// <returns>
        /// Whether loop cycled
        /// </returns>
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
