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

        public static async IAsyncEnumerable<Song> FetchPlaylistSongsFromWebAsync(string playlistName)
        {
            // Web request...
            await Task.Delay(300);
            List<string> returnedSongs = new List<string>();

            foreach(string songName in returnedSongs)
            {
                string lyrics = await FetchLyricsFromWebAsync(songName);
                yield return new Song(songName, lyrics);
            }
        }

        public static async Task<List<string>> FetchPlaylistsNamesAsync()
        {
            // Web request...
            await Task.Delay(300);

            return new List<string>();
        }
    }
}
