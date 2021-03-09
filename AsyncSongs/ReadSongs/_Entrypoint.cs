using System.Collections.Generic;
using System.Linq;
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
            return songs.FirstOrDefault(s => s != null);
        }
    }
}
