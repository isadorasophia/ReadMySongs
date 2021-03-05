using AsyncSongs.Spotify;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncSongs.ReadSongs
{
    public class Playlist
    {
        /// <summary>
        /// Name which has been passed down by user.
        /// </summary>
        private readonly string _name;

        private List<Song> _cachedSongs;
        private string _id;

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
        public async Task<Song> TryFindSong(string text)
        {
            List<Task<Song>> songsToSearch = new();

            foreach (Song song in await FetchSongs())
            {
                Task<Song> compareLyrics = Task.Run(async delegate
                {
                    Lyrics lyrics = await song.FetchLyrics();
                    if (lyrics != null && lyrics.HasText(text))
                    {
                        return song;
                    }

                    return null;
                });

                songsToSearch.Add(compareLyrics);
            }

            // Run and grab the first valid song!
            Song[] searchedSongs = await Task.WhenAll(songsToSearch);
            return searchedSongs.FirstOrDefault(song => song != null);
        }

        public async Task<List<Song>> FetchSongs()
        {
            if (_cachedSongs == null)
            {
                _cachedSongs = await DoSongsRequest();
                await Task.Run(NotifyUser);
            }

            return _cachedSongs;
        }

        private async Task<List<Song>> DoSongsRequest()
        {
            // Best match
            if (_id == null)
            {
                _id = await SpotifyRequests.Instance.GetPlaylistIdAsync(_name);
            }

            return await SpotifyRequests.Instance.GetTracksAsync(_id);
        }

        private Task NotifyUser()
        {
            return new Task(() =>
            {
                Thread.Sleep(10);
            });
        }
    }
}
