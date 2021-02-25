using System.Collections.Generic;
using System.Threading.Tasks;

namespace AsyncSongs
{
    static class DataProvider
    {
        private static async Task<string> FetchLyricsFromWebAsync(string songName)
        {
            // Web request...
            await Task.Delay(300);

            // TODO: return result from web api.
            return string.Empty;
        }

        public static async Task<List<string>> FetchPlaylistsNamesAsync()
        {
            // Web request...
            await Task.Delay(300);

            return new List<string>();
        }
    }
}
