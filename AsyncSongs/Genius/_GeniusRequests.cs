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

        private GeniusRequests()
        {
            _user.Register();
        }

        public async Task<string> FetchLyricsAsync(Song song)
        {
            ISearchClient search = _user.Client.SearchClient;
            SearchResponse response = await search.Search(song.Name + song.Artist);

            Debug.Fail("why ;_;");
            return "";
        }
    }
}
