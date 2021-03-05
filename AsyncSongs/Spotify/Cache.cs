using SpotifyAPI.Web;
using System.Collections.Generic;

namespace AsyncSongs.Spotify
{
    internal class Cache
    {
        public Dictionary<string, FullPlaylist> Playlists = new();
    }
}
