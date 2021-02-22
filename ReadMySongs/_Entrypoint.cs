using System;

namespace ReadMySongs
{
    class _Entrypoint
    {
        static void Main()
        {
            Console.WriteLine("What is your playlist name? (You can guess!)");
            string playlistName = Console.ReadLine();

            Console.WriteLine("Please type what is in your mind:");
            string text = Console.ReadLine();

            Playlist playlist = new(playlistName);

            Song song = playlist.TryFindSong(text).Result;

            if (song != null)
            {
                Lyrics lyrics = song.FetchLyrics().Result;

                Console.WriteLine("Good news! There you go:");
                Console.WriteLine("** {0} **", song.Name);
                Console.WriteLine(lyrics.Content);
            }
            else
            {
                Console.WriteLine("Song not found, sorry :(");
            }
        }
    }
}
