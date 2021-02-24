using System.Threading.Tasks;

namespace AsyncSongs
{
    class Song
    {
        public string Name { get; private set; }

        public Lyrics Lyrics = null;

        public Song(string name, string lyrics)
        {
            Name = name;
            Lyrics = new Lyrics(lyrics);
        }
    }
}
