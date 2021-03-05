using SpotifyAPI.Web;
using System.Collections.Generic;

namespace AsyncSongsServer.Genius
{
    public class Cache
    {
        public Dictionary<string, FullPlaylist> Playlists = new();
    }
}
