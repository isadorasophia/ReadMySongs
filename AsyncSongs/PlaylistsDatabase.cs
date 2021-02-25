using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSongs
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
