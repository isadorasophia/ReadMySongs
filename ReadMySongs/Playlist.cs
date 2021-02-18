using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ReadMySongs.Database;
using ReadMySongs.Utilities;

namespace ReadMySongs
{
    public class Playlist
    {
        /// <summary>
        /// Name which has been passed down by user.
        /// </summary>
        private readonly string _name;

        private List<Song> _cachedSongs;
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
        public Song TryFindSong(string text)
        {
            foreach (Song song in FetchSongs())
            {
                Lyrics lyrics = song.FetchLyrics();

                if (lyrics != null && lyrics.HasText(text))
                {
                    return song;
                }
            }

            return null;
        }

        public List<Song> FetchSongs()
        {
            if (_cachedSongs == null)
            {
                _cachedSongs = DoSongsRequest();
            }

            return _cachedSongs;
        }

        private List<Song> DoSongsRequest()
        {
            // Best match
            if (_completeName == null)
            {
                TryGetPlaylistFullNameWebRequest(out _completeName);
            }

            List<string> songs;
            if (_completeName != null && TryGetPlaylistSongsWebRequest(out songs))
            {
                return songs.Select(s => new Song(s)).ToList();
            }

            return null;
        }

        #region Web APIs

        private bool TryGetPlaylistFullNameWebRequest(out string fullName)
        {
            // Web request...
            Thread.Sleep(10);

            fullName = PlaylistsDatabase.Playlists.Keys.FirstOrDefault(p => p.ContainsIgnoreCase(_name));

            return fullName != null;
        }

        private bool TryGetPlaylistSongsWebRequest(out List<string> songs)
        {
            // Web request...
            Thread.Sleep(10);

            return PlaylistsDatabase.Playlists.TryGetValue(_completeName, out songs);
        }

        #endregion
    }
}
