using AsyncSongs.Utilities;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AsyncSongs.Genius
{
    internal class Login
    {
        public readonly static string RedirectUrl = "http://localhost:5500/genius-callback/";

        private static EmbedIOAuthServer? _server;
        private string? _clientId;
        private string? _clientSecret;

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

            OAuthClient client = new(response.Code, _clientId!, _clientSecret!, new Uri(RedirectUrl));
            string token = await client.GetAccessTokenAsync();

            await GeniusRequests.Instance.SetLoginTokenAsync(token);

            Cleanup();
        }

        /// <summary>
        /// Logic for initializing a login request.
        /// </summary>
        private async Task InitializeAsync()
        {
            _server = new(new Uri(RedirectUrl), 5500);
            await _server.StartAsync();

            _server.AuthorizationCodeReceived += OnAuthorizationCodeReceivedAsync;
            _clientId = await AuthUtils.ReadSecretAsync("genius:clientId");
            _clientSecret = await AuthUtils.ReadSecretAsync("genius:clientSecret");
        }

        private void Cleanup()
        {
            _server = null;
            _clientId = null;
        }
    }
}
