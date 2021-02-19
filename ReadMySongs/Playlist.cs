using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        private object _locker = new object();

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
        public Task<Song> TryFindSong(string text)
        {
            Task<List<Song>> fetchSongsTask = new Task<List<Song>>(FetchSongs);
            fetchSongsTask.Start();

            List<Song> candidateSongs = new List<Song>();

            Task<List<Task>> fetchLyricsTask = fetchSongsTask.ContinueWith(t =>
            {
                NotifyUser();

                List<Task> matchLyricsTasks = new List<Task>();
                foreach (Song song in t.Result)
                {
                    Task<Lyrics> tt = new Task<Lyrics>(song.FetchLyrics);
                    tt.Start();

                    Task matchLyricsTask = tt.ContinueWith(ttt =>
                    {
                        Lyrics lyrics = ttt.Result;
                        if (lyrics != null && lyrics.HasText(text))
                        {
                            lock (_locker)
                            {
                                candidateSongs.Add(lyrics.Song);
                            }
                        }
                    }, TaskScheduler.Default);

                    matchLyricsTasks.Add(matchLyricsTask);
                }

                return matchLyricsTasks;
            }, TaskScheduler.Default);

            Task<Song> findTargetSongTask = fetchLyricsTask.ContinueWith(t =>
            {
                Task[] matchSongTasks = fetchLyricsTask.Result.ToArray();
                Task.WaitAll(matchSongTasks);

                return candidateSongs.FirstOrDefault();
            });

            return findTargetSongTask;
        }

        public List<Song> FetchSongs()
        {
            if (_cachedSongs == null)
            {
                Task t = NotifyUser();

                _cachedSongs = DoSongsRequest();
                t.Start();
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

        private Task NotifyUser()
        {
            return new Task(() =>
            {
                Thread.Sleep(10);
            });
        }

        #endregion
    }
}
