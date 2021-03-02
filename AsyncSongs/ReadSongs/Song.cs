using AsyncSongs.WebRequests;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncSongs
{
    class Song
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
            return TryGetLyricsWebRequest();
        }

        #region Web APIs

        private string TryGetLyricsWebRequest()
        {
            // Web request...
            Thread.Sleep(10_000);

            if (MockLyricsDatabase.Songs.TryGetValue(Name, out string lyrics))
            {
                return lyrics;
            }

            return null;
        }

        #endregion
    }
}
