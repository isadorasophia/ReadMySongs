using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using AsyncSongs.WebRequests;

namespace AsyncSongs
{
    class Playlist
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

        private object _locker = new();

        /// <summary>
        /// Find the first song that contains <paramref name="text"/> in its lyrics.
        /// </summary>
        /// <returns>
        /// The song that its lyrics has <paramref name="text"/>. Otherwise, return null.
        /// </returns>
        public Task<Song> TryFindSong(string text)
        {
            Task<List<Song>> fetchSongsTask = Task.Run(FetchSongs);

            List<Song> candidateSongs = new();

            Task<Task> matchSongsTask = fetchSongsTask.ContinueWith(t =>
            {
                List<Task> matchLyricsTasks = new();
                foreach (Song song in t.Result)
                {
                    Task<Lyrics> tt = Task.Run(song.FetchLyrics);

                    Task matchLyricsTask = tt.ContinueWith(ttt =>
                    {
                        Lyrics lyrics = ttt.Result;
                        if (lyrics != null && lyrics.HasText(text))
                            lock (_locker) { candidateSongs.Add(lyrics.Song); }
                    }, TaskScheduler.Default);

                    matchLyricsTasks.Add(matchLyricsTask);
                }

                return Task.WhenAll(matchLyricsTasks);
            }, TaskScheduler.Default);

            return matchSongsTask.ContinueWith(_ => candidateSongs.FirstOrDefault(), TaskScheduler.Default);
        }

        public List<Song> FetchSongs()
        {
            if (_cachedSongs == null)
            {
                _cachedSongs = DoSongsRequest();
                Task.Run(NotifyUser);
            }

            return _cachedSongs;
        }

        private List<Song> DoSongsRequest()
        {
            // Best match
            if (_completeName == null)
            {
                _completeName = TryGetPlaylistFullNameWebRequest();
            }

            if (TryGetPlaylistSongsWebRequest() is List<string> songs)
            {
                return songs.Select(s => new Song(s)).ToList();
            }

            return null;
        }

        private Task NotifyUser()
        {
            return new Task(() =>
            {
                Thread.Sleep(10);
            });
        }

        #region Web APIs

        private string TryGetPlaylistFullNameWebRequest()
        {
            // Web request...
            Thread.Sleep(10);

            if (MockPlaylistsDatabase.Playlists.Keys.FirstOrDefault(p => p.Contains(_name, System.StringComparison.OrdinalIgnoreCase)) is string fullName)
            {
                return fullName;
            }

            return null;
        }

        private List<string> TryGetPlaylistSongsWebRequest()
        {
            if (_completeName == null)
            {
                Debug.Fail("Why we do not have the complete name at this point?");
                return null;
            }

            // Web request...
            Thread.Sleep(10);

            if (MockPlaylistsDatabase.Playlists.TryGetValue(_completeName, out List<string> songs))
            {
                return songs;
            }

            return null;
        }

        #endregion
    }
}
