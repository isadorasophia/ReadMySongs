using Genius;

namespace AsyncSongsServer.Genius
{
    public class User
    {
        public string Token { get; private set; }
        public GeniusClient Client { get; private set; }

        internal void Register()
        {
            Client = new GeniusClient(Utilities.ClientId);
        }
    }
}
