using AsyncSongs.Utilities;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AsyncSongs.Spotify
{
    internal class Login
    {
        private readonly static string RedirectUrl = "http://localhost:5000/spotify-callback/";

        private static EmbedIOAuthServer? _server;
        private string? _clientId;

        internal async Task<bool> TryLoginAsync()
        {
            await InitializeAsync();

            Uri requestUri = GetLoginUri();
            try
            {
                BrowserUtil.Open(requestUri);
            }
            catch
            {
                Debug.Fail("Why were we unable to open URL? Please open: {0}", requestUri.OriginalString);
                return false;
            }

            return true;
        }

        internal static Task<string> GetSpotifyClientIdAsync() => AuthUtils.ReadSecretAsync("spotify:clientId");

        private Uri GetLoginUri()
        {
            var loginRequest = new LoginRequest(
                _server!.BaseUri,
                _clientId!,
                LoginRequest.ResponseType.Token)
            {
                Scope = new[] { Scopes.PlaylistReadPrivate }
            };

            return loginRequest.ToUri();
        }

        private async Task OnImplictGrantReceivedAsync(object _, ImplictGrantResponse response)
        {
            // Stop listening to server.
            await _server!.Stop();

            await SpotifyRequests.Instance.SetLoginTokenAsync(response.AccessToken);

            Cleanup();
        }

        /// <summary>
        /// Logic for initializing a login request.
        /// </summary>
        private async Task InitializeAsync()
        {
            _server = new(new Uri(RedirectUrl), 5000);
            await _server.Start();

            _server.ImplictGrantReceived += OnImplictGrantReceivedAsync;
            _clientId = await GetSpotifyClientIdAsync();
        }

        private void Cleanup()
        {
            _server = null;
            _clientId = null;
        }
    }
}
