using System.Collections.Generic;
using System.Threading.Tasks;

namespace AsyncSongs
{
    class Playlist
    {
        /// <summary>
        /// Name which has been passed down by user
        /// </summary>
        private readonly string _name;

        private string _completeName;

        public Playlist(string name)
        {
            _name = name;
        }

        /// <summary>
        /// Find the first song that contains <paramref name="text"/> in its lyrics.
        /// </summary>
        /// <returns>
        /// The song that its lyrics has <paramref name="text"/>. Otherwise, return null.
        /// </returns>
        public async Task<Song> TryFindSongAsync(string text)
        {
            // Best match
            if (_completeName == null)
            {
                _completeName = await TryToGetActualPlaylistNameAsync();
            }

            await foreach (Song song in DataProvider.FetchPlaylistSongsFromWebAsync(_completeName))
            {
                if (song.Lyrics.HasText(text))
                {
                    return song;
                }
            }

            return null;
        }

        private async Task<string> TryToGetActualPlaylistNameAsync()
        {
            List<string> playlistNames = await DataProvider.FetchPlaylistsNamesAsync();

            return playlistNames.Find(actualName => actualName.Contains(_name, System.StringComparison.OrdinalIgnoreCase));
        }
    }
}
