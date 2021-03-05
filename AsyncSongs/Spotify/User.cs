using AsyncSongs.Utilities;
using SpotifyAPI.Web;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AsyncSongs.Spotify
{
    internal class User
    {
        private const string TokenFile = @"spotify-token.txt";
        private static string TokenPath => Path.Combine(AuthUtils.BasePath, TokenFile);

        public SpotifyClient Client { get; private set; }

        public bool IsLogged => Client is not null;

        /// <summary>
        /// Event for notifying that a user logged in.
        /// </summary>
        public event Func<Task> UserLoggedIn;

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
            Client = new SpotifyClient(token);

            if (save)
            {
                await File.WriteAllTextAsync(TokenPath, token);
            }

            if (UserLoggedIn is not null)
            {
                // Fire and forget.
                _ = UserLoggedIn();
            }
        }
    }
}
