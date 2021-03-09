using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AsyncSongs.ReadSongs
{
    class ReadSongsService
    {
        public static async Task<Song?> SearchSongAsync(string text, string? playlistName = null)
        {
            List<Playlist> playlists = new();
            if (playlistName is null)
            {
                // TODO? Search through all playlists instead.
                // playlists = await FetchPlaylists();
            }
            else
            {
                playlists.Add(new Playlist(playlistName));
            }

            List<Task<Song?>> tasks = new();
            foreach (Playlist playlist in playlists)
            {
                tasks.Add(playlist.TryFindSongAsync(text));
            }

            Song?[] songs = await Task.WhenAll(tasks);

            //For purposes of the demo, after the initial search, we send a custom request under the covers to
            //tell the server that it should hang when the next request occurs.
            await MakeServerHangOnNextSearchAsync();

            return songs.FirstOrDefault(s => s != null);
        }

        private static async Task MakeServerHangOnNextSearchAsync()
        {
            // We need to repeat a request to the server so force the application to make a new request.
            Utilities.CacheUtils.IsCacheInvalid = true;

            string uri = $"https://localhost:44305/api/lyrics/MakeServerHang";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

            // This call does not return anything.
            await request.GetResponseAsync();
        }
    }
}
