using AsyncSongs.ReadSongs;
using Genius.Clients.Interfaces;
using Genius.Models.Response;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AsyncSongs.Genius
{
    public class GeniusRequests
    {
        private User _user = new();
        private Cache _cache = new();
        private Login _login = new();

        private object @lock = new();

        public static GeniusRequests Instance = new();

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

            ISearchClient search = _user.Client!.SearchClient;
            SearchResponse response = await search.Search(song.Name + song.Artist);

            Debug.Fail("we never reach this line lalala");
            return "";
        }
    }
}
