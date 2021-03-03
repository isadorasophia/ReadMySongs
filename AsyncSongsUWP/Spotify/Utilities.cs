using SpotifyAPI.Web;
using System;
using System.Linq;

namespace AsyncSongs.Spotify
{
    class Utilities
    {
        private readonly static string RedirectUrl = "read-my-songs://token/";
        private readonly static string ClientId = "a4bc1dee8ac149809dd8d6dd4e69db64";

        internal static Uri GetLoginUri()
        {
            var loginRequest = new LoginRequest(
                new Uri(RedirectUrl),
                ClientId,
                LoginRequest.ResponseType.Token)
            {
                Scope = new[] { Scopes.PlaylistReadPrivate }
            };

            return loginRequest.ToUri();
        }

        internal static string GetToken(Uri uri)
        {
            if (string.IsNullOrEmpty(uri.Fragment))
            {
                throw new Exception($"Received weird URI: {uri}");
            }

            var arguments = uri.Fragment.Substring(1).Split("&")
                .Select(param => param.Split("="))
                .ToDictionary(param => param[0], param => param[1]);

            if (arguments["access_token"] == null)
            {
                throw new Exception($"No access token found in URI: {uri}");
            }

            return arguments["access_token"];
        }
    }
}
