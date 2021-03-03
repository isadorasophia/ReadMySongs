using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsyncSongsUWP.ReadSongs
{
    class ReadSongsService
    {
        public static async Task<Song> SearchSong(string text, string playlistName = null)
        {
            List<Playlist> playlists = new List<Playlist>();
            if (playlistName is null)
            {
                // TODO? Search through all playlists instead.
                // playlists = await FetchPlaylists();
            }
            else
            {
                playlists.Add(new Playlist(playlistName));
            }

            List<Task<Song>> tasks = new List<Task<Song>>();
            foreach (Playlist playlist in playlists)
            {
                tasks.Add(playlist.TryFindSong(text));
            }

            Song[] songs = await Task.WhenAll(tasks);
            return songs.FirstOrDefault(s => s != null);
        }
    }
}
