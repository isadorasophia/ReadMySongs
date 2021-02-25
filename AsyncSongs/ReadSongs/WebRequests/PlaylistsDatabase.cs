using System.Collections.Generic;

namespace AsyncSongs.WebRequests
{
    static class MockPlaylistsDatabase
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
