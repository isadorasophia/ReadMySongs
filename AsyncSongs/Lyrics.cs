﻿namespace AsyncSongs
{
    class Lyrics
    {
        public readonly Song Song;
        public string Content { get; private set; }

        public Lyrics(Song song, string content)
        {
            Song = song;
            Content = content;
        }

        public bool HasText(string text)
        {
            return Content.Contains(text, System.StringComparison.OrdinalIgnoreCase);
        }
    }
}