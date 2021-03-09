using AsyncSongs.Utilities;
using Genius;
using System.IO;
using System.Threading.Tasks;

namespace AsyncSongs.Genius
{
    internal class User
    {
        private const string TokenFile = @"genius-token.txt";
        private static string TokenPath => Path.Combine(AuthUtils.BasePath!, TokenFile);

        public GeniusClient? Client { get; private set; }

        public bool IsLogged => Client is not null;

        internal async Task InitializeAsync()
        {
            if (IsLogged || !File.Exists(TokenPath))
            {
                // Nothing to do, report immediately.
                return;
            }

            string token = await AuthUtils.ReadFileAsync(TokenPath);
            await RegisterTokenAsync(token, save: false);
        }

        internal async Task RegisterTokenAsync(string token, bool save = false)
        {
            Client = new GeniusClient(token);

            if (save)
            {
                await File.WriteAllTextAsync(TokenPath, token);
            }
        }
    }
}
