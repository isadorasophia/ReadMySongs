using System.Threading;
using ReadMySongs.Database;

namespace ReadMySongs
{
    public class Song
    {
        public string Name { get; private set; }

        private Lyrics _cachedLyrics = null;

        public Song(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Fetch lyrics for the song. If none was found, return null.
        /// </summary>
        public Lyrics FetchLyrics()
        {
            if (_cachedLyrics == null)
            {
                string lyrics = DoLyricsRequest();
                if (lyrics != null)
                {
                    _cachedLyrics = new Lyrics(this, lyrics);
                }
            }

            return _cachedLyrics;
        }

        private string DoLyricsRequest()
        {
            string lyrics;
            TryGetLyricsWebRequest(out lyrics);

            return lyrics;
        }

        #region Web APIs

        private bool TryGetLyricsWebRequest(out string lyrics)
        {
            // Web request...
            Thread.Sleep(10);

            return LyricsDatabase.Songs.TryGetValue(Name, out lyrics);
        }

        #endregion
    }
}
