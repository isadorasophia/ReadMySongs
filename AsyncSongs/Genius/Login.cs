using AsyncSongs.Utilities;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AsyncSongs.Genius
{
    internal class Login
    {
        public readonly static string RedirectUrl = "http://localhost:5000/genius-callback/";

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

        private Uri GetLoginUri()
        {
            var loginRequest = new LoginRequest(
                _server!.BaseUri,
                _clientId!);

            return loginRequest.ToUri();
        }

        private async Task OnAuthorizationCodeReceivedAsync(object _, AuthorizationCodeResponse response)
        {
            // Stop listening to server.
            await _server!.StopAsync();

            // TODO: Implement post operation requesting the access token with response.Code.
            // await GeniusRequests.Instance.SetLoginTokenAsync(response.AccessToken).ConfigureAwait(false);

            Cleanup();
        }

        /// <summary>
        /// Logic for initializing a login request.
        /// </summary>
        private async Task InitializeAsync()
        {
            _server = new(new Uri(RedirectUrl), 5000);
            await _server.StartAsync();

            _server.AuthorizationCodeReceived += OnAuthorizationCodeReceivedAsync;
            _clientId = await GetGeniusClientIdAsync();
        }

        private void Cleanup()
        {
            _server = null;
            _clientId = null;
        }

        internal static Task<string> GetGeniusClientIdAsync() => AuthUtils.ReadSecretAsync("genius:clientId");
    }
}
