using Genius;
using System.Threading.Tasks;

namespace AsyncSongs.Genius
{
    internal class User
    {
        public string? Token { get; private set; }
        public GeniusClient? Client { get; private set; }

        public bool IsLogged => Client is not null;

        internal async Task InitializeAsync()
        {
            Client = new GeniusClient(await Login.GetGeniusClientIdAsync());
        }
    }
}
