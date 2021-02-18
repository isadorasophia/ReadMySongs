using ReadMySongs.Utilities;

namespace ReadMySongs
{
    public class Lyrics
    {
        public string Content { get; private set; }

        public Lyrics(string content)
        {
            Content = content;
        }

        public bool HasText(string text)
        {
            return Content.ContainsIgnoreCase(text);
        }
    }
}
