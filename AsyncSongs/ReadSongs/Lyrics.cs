using System;
using System.Collections.Generic;
using System.Linq;

namespace AsyncSongs.ReadSongs
{
    public class Lyrics
    {
        public readonly Song Song;
        public string Content { get; private set; }

        public Lyrics(Song song, string content)
        {
            Song = song;
            Content = content;
        }

        /// <summary>
        /// Get an exerpt of six lines.
        /// </summary>
        public IEnumerable<string> GetExerpt(string text, int resultLines = 7)
        {
            int index = Content.IndexOf(text, StringComparison.OrdinalIgnoreCase);

            int firstIndex = Math.Max(index - 100, 0);
            int lastIndex = Math.Min(index + 100, Content.Length);

            string excerpt = Content.Substring(firstIndex, lastIndex - firstIndex);
            string[] lines = excerpt.Split("\n");

            if (lines.Length <= resultLines)
            {
                return lines;
            }

            // Otherwise, take these lines from the result.
            int middle = lines.Length / 2;
            firstIndex = middle - resultLines / 2;

            return lines.Skip(firstIndex).Take(resultLines);
        }

        public bool HasText(string text)
        {
            return Content.Contains(text, StringComparison.OrdinalIgnoreCase);
        }
    }
}
