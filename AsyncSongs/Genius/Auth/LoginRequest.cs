using System;
using System.Text;
using System.Web;

namespace AsyncSongs.Genius
{
    public class LoginRequest
    {
        private const string AuthorizeUrl = @"https://api.genius.com/oauth/authorize";

        /// <summary>
        /// Create a login request.
        /// The scope will always be set to "me" and the responseType is "code".
        /// See https://docs.genius.com/#/authentication-h1.
        /// </summary>
        public LoginRequest(Uri redirectUri, string clientId)
        {
            RedirectUri = redirectUri;
            ClientId = clientId;
        }

        public Uri RedirectUri { get; }
        public string ClientId { get; }
        public string? State { get; set; }

        public Uri ToUri()
        {
            var builder = new StringBuilder(AuthorizeUrl);
            builder.Append($"?client_id={ClientId}");
            builder.Append($"&redirect_uri={HttpUtility.UrlEncode(RedirectUri.ToString())}");
            builder.Append($"&scope=me");
            builder.Append($"&state=available");
            builder.Append($"&response_type=code");

            return new Uri(builder.ToString());
        }
    }
}
