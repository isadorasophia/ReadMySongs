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

        private object @lock = new();

        public static GeniusRequests Instance = new();

        Task _pendingOperations;

        private GeniusRequests()
        {
            _pendingOperations = Task.Run(_user.RegisterAsync);
        }

        public async Task<string> FetchLyricsAsync(Song song)
        {
            if (_pendingOperations is not null)
            {
                await _pendingOperations;
                _pendingOperations = null;
            }

            ISearchClient search = _user.Client.SearchClient;
            SearchResponse response = await search.Search(song.Name + song.Artist);

            Debug.Fail("we never reach this line lalala");
            return "";
        }
    }
}
