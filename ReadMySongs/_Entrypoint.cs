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

            Playlist playlist = new Playlist(playlistName);
            Song song = playlist.TryFindSong(text);

            Lyrics lyrics = song.FetchLyrics();

            Console.WriteLine("Good news! There you go:");
            Console.WriteLine("** {0} **", song.Name);
            Console.WriteLine(lyrics.Content);

            // Wait to exit...
            Console.ReadKey();
        }
    }
}
