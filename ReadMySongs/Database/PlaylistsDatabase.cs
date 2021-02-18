using System.Collections.Generic;

namespace ReadMySongs.Database
{
    static class PlaylistsDatabase
    {
        public static Dictionary<string, List<string>> Playlists = new Dictionary<string, List<string>>
        {
            {
                "cute background songs", 
                    new List<string> { "Island Song" }
            }
        };
    }
}
