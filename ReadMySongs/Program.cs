using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace ReadMySongs
{
    class Lyrics
    {
        private static CultureInfo Culture = CultureInfo.InvariantCulture;

        public string Content { get; private set; }

        public Lyrics(string content)
        {
            Content = content;
        }

        public bool HasText(string text)
        {
            return Culture.CompareInfo.IndexOf(Content, text, CompareOptions.IgnoreCase) >= 0;
        }
    }

    class Song
    {
        public string Name { get; private set; }

        private Lyrics _cachedLyrics = null;

        public Song(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Fetch lyrics for the song. If none was found, return null.
        /// </summary>
        public Lyrics FetchLyrics()
        {
            if (_cachedLyrics == null)
            {
                string lyrics = DoLyricsRequest();
                if (lyrics != null)
                {
                    _cachedLyrics = new Lyrics(lyrics);
                }
            }

            return _cachedLyrics;
        }

        private string DoLyricsRequest()
        {
            // Web request...
            Thread.Sleep(10);

            string lyrics;
            if (LyricsDatabase.Songs.TryGetValue(Name, out lyrics))
            {
                return lyrics;
            }

            return null;
        }
    }

    class Playlist
    {
        List<Song> _songs;

        public Playlist(List<Song> songs)
        {
            _songs = songs;
        }

        public Playlist(List<string> songNames)
            : this(songNames.Select(s => new Song(s)).ToList()) { }

        /// <summary>
        /// Find the first song that contains <paramref name="text"/> in its lyrics.
        /// </summary>
        /// <returns>
        /// The song that its lyrics has <paramref name="text"/>. Otherwise, return null.
        /// </returns>
        public Song TryFindSong(string text)
        {
            foreach (Song song in _songs)
            {
                Lyrics lyrics = song.FetchLyrics();

                if (lyrics != null && lyrics.HasText(text))
                {
                    return song;
                }
            }

            return null;
        }
    }

    class Program
    {
        static void Main()
        {
            Console.WriteLine("Please type what is in your mind:");
            string text = Console.ReadLine();

            Playlist playlist = new Playlist(new List<string>{ "IslandSong" });
            Song song = playlist.TryFindSong(text);

            Lyrics lyrics = song.FetchLyrics();

            Console.WriteLine("Found song!!");
            Console.WriteLine("** {0} **", song.Name);
            Console.WriteLine(lyrics.Content);

            // Wait to exit...
            Console.ReadKey();
        }
    }
}
