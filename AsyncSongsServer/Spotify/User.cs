using SpotifyAPI.Web;

namespace AsyncSongsServer.Spotify
{
    public class User
    {
        public string Token { get; private set; }
        public SpotifyClient Client { get; private set; }

        internal void RegisterToken(string token)
        {
            Token = token;
            Client = new SpotifyClient(token);
        }
    }
}
