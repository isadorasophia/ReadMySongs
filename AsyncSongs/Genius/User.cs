using AsyncSongs.Utilities;
using Genius;
using System.IO;
using System.Threading.Tasks;

namespace AsyncSongs.Genius
{
    internal class User
    {
        private bool useTestToken = true;

        private const string TokenFile = @"genius-token.txt";
        private static string TokenPath => Path.Combine(AuthUtils.BasePath!, TokenFile);

        public GeniusClient? Client { get; private set; }

        public bool IsLogged => Client is not null;

        internal async Task InitializeAsync()
        {
            string token;

            if (useTestToken)
            {
                token = "ThisIsNotARealToken";
            }
            else
            {
                if (IsLogged || !File.Exists(TokenPath))
                {
                    // Nothing to do, report immediately.
                    return;
                }
                
                token = await AuthUtils.ReadFileAsync(TokenPath);
            }

            await RegisterTokenAsync(token, save: false);
        }

        internal async Task RegisterTokenAsync(string token, bool save = false)
        {
            string token2 = "dddde";
            Client = new GeniusClient(token2);

            if (save)
            {
                await File.WriteAllTextAsync(TokenPath, token);
            }
        }
    }
}
