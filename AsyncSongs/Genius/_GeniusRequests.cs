using AsyncSongs.ReadSongs;
using Genius.Clients.Interfaces;
using Genius.Models.Response;
using System;
using System.Collections.Generic;
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

            SearchResponse response;
            while (true)
            {
                try
                {
                    response = await search.Search(song.Name + song.Artist);
                    break;
                }
                catch
                {
                    // We don't what to block the UI, so we let other tasks continue their work.
                    await Task.Yield();
                }
            }

            IEnumerable<GeniusSong> songs = response.Response.Hits
                .Where(h => h != null && h.Type is "song")
                .Select(h => h.Result);

            // Return immediately if nothing was found...
            if (songs == null || !songs.Any()) return null;

            // Try our best guess based on whether we find a matching artist on our results.
            var bestMatch = songs.FirstOrDefault(s => s.PrimaryArtist.Name.Contains(song.Artist, StringComparison.OrdinalIgnoreCase));

            return bestMatch ?? songs.First();
        }

        internal Task SetLoginTokenAsync(string token) => _user.RegisterTokenAsync(token, save: true);
    }
}
