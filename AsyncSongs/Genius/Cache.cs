using SpotifyAPI.Web;
using System.Collections.Generic;

namespace AsyncSongs.Genius
{
    internal class Cache
    {
        public Dictionary<string, FullPlaylist> Playlists = new();
    }
}
