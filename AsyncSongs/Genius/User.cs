using Genius;
using System.Threading.Tasks;

namespace AsyncSongs.Genius
{
    internal class User
    {
        public string Token { get; private set; }
        public GeniusClient Client { get; private set; }

        internal async Task RegisterAsync()
        {
            Client = new GeniusClient(await Login.GetGeniusClientIdAsync());
        }
    }
}
