using AsyncSongsUWP.ReadSongs.WebRequests;
using System.Threading.Tasks;

namespace AsyncSongsUWP.ReadSongs
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
        public async Task<Lyrics> FetchLyrics()
        {
            if (_cachedLyrics == null)
            {
                string lyrics = await DoLyricsRequest();
                if (lyrics != null)
                {
                    _cachedLyrics = new Lyrics(this, lyrics);
                }
            }

            return _cachedLyrics;
        }

        private async Task<string> DoLyricsRequest()
        {
            return await TryGetLyricsWebRequest();
        }

        #region Web APIs

        private async Task<string> TryGetLyricsWebRequest()
        {
            // Web request...
            await Task.Delay(100_000);

            if (MockLyricsDatabase.Songs.TryGetValue(Name, out string lyrics))
            {
                return lyrics;
            }

            return null;
        }

        #endregion
    }
}
