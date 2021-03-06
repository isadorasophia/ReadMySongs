using AsyncSongs.ReadSongs;
using Genius.Clients.Interfaces;
using Genius.Models;
using Genius.Models.Response;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using GeniusSong = Genius.Models.Song.Song;

namespace AsyncSongs.Genius
{
    public class GeniusRequests
    {
        private User _user = new();
        private Cache _cache = new();
        private Login _login = new();

        private object @lock = new();

        public static GeniusRequests Instance = new();

        public async Task InitializeAsync()
        {
            await _user.InitializeAsync();
        }

        public Task<bool> LoginAsync()
        {
            if (!_user.IsLogged)
            {
                return _login.TryLoginAsync();
            }

            return Task.FromResult(true);
        }

        public async Task<string?> FetchLyricsAsync(Song song)
        {
            if (!_user.IsLogged)
            {
                return null;
            }

            // Try to find the best match for the song.
            GeniusSong? geniusSong = await TryFindSongAsync(song);
            if (geniusSong is null)
            {
                return null;
            }

            return await geniusSong.FetchLyricsAsync();
        }

        private async Task<GeniusSong?> TryFindSongAsync(Song song)
        {
            ISearchClient search = _user.Client!.SearchClient;
            SearchResponse response = await search.Search(song.Name + song.Artist);

            SearchHit? hit = response.Response.Hits
                .Where(h => h != null && h.Type is "song")?
                .FirstOrDefault();

            return hit?.Result;
        }

        internal Task SetLoginTokenAsync(string token) => _user.RegisterTokenAsync(token, save: true);
    }
}
