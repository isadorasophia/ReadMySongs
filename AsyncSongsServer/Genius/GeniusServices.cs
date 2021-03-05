using Genius.Clients.Interfaces;
using Genius.Models.Response;
using System.Threading.Tasks;

namespace AsyncSongsServer.Genius
{
    public class GeniusServices
    {
        private User _user = null;
        private Cache _cache = new();

        private object @lock = new();

        public static GeniusServices Instance = new();

        private GeniusServices()
        {
            _user.Register();
        }

        public async Task<string> FetchLyrics(Song song)
        {
            ISearchClient search = _user.Client.SearchClient;
            SearchResponse response = await search.Search(song.Name + song.Artist);

            return "what happens here Isa? what do I return???? :'( ";
        }
    }
}
