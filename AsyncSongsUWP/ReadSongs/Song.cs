using AsyncSongs.Genius;
using System.Threading.Tasks;

namespace AsyncSongs.ReadSongs
{
    public class Song
    {
        public string Name { get; private set; }
        public string Artist { get; private set; }

        private Lyrics _cachedLyrics = null;

        public Song(string name, string artist)
        {
            Name = name;
            Artist = artist;
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
            return await GeniusRequests.Instance.FetchLyrics(this);
        }
    }
}
