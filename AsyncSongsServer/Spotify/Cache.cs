using SpotifyAPI.Web;
using System.Collections.Generic;

namespace AsyncSongsServer.Spotify
{
    public class Cache
    {
        public Dictionary<string, FullPlaylist> Playlists = new();
    }
}
