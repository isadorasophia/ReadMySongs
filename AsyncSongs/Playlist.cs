﻿using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using System.Threading;

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

        /// <summary>
        /// Find the first song that contains <paramref name="text"/> in its lyrics.
        /// </summary>
        /// <returns>
        /// The song that its lyrics has <paramref name="text"/>. Otherwise, return null.
        /// </returns>
        public async Task<Song> TryFindSong(string text)
        {
            foreach (Song song in await FetchSongs())
            {
                Lyrics lyrics = await song.FetchLyrics();

                if (lyrics != null && lyrics.HasText(text))
                {
                    return song;
                }
            }

            return null;
        }

        public async Task<List<Song>> FetchSongs()
        {
            if (_cachedSongs == null)
            {
                Task t = NotifyUser();

                _cachedSongs = await DoSongsRequest();
                t.Start();
            }

            return _cachedSongs;
        }

        private async Task<List<Song>> DoSongsRequest()
        {
            // Best match
            if (_completeName == null)
            {
                _completeName = await TryGetPlaylistFullNameWebRequest();
            }

            if (await TryGetPlaylistSongsWebRequest() is List<string> songs)
            {
                return songs.Select(s => new Song(s)).ToList();
            }

            return null;
        }

        #region Web APIs

        private async Task<string> TryGetPlaylistFullNameWebRequest()
        {
            // Web request...
            await Task.Delay(10);

            if (PlaylistsDatabase.Playlists.Keys.FirstOrDefault(p => p.Contains(_name, System.StringComparison.OrdinalIgnoreCase)) is string fullName)
            {
                return fullName;
            }

            return null;
        }

        private async Task<List<string>> TryGetPlaylistSongsWebRequest()
        {
            if (_completeName == null)
            {
                Debug.Fail("Why we do not have the complete name at this point?");
                return null;
            }

            // Web request...
            await Task.Delay(10);

            if (PlaylistsDatabase.Playlists.TryGetValue(_completeName, out List<string> songs))
            {
                return songs;
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

        #endregion
    }
}