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
                    new List<string> { "Island Song", "Those Were the Days", "What's New Scooby" }
            },
            {
                "Romantic songs",
                    new List<string> { "Luck Be A Lady" }
            },
            {
                "Spanish songs",
                    new List<string> { "Julieta" }
            }
        };
    }
}
