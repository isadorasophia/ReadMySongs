using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSongs.Genius
{
    /// <summary>
    /// Wrapper for Genius OAuth2 authentication requests.
    /// This code was taken mostly from: https://github.com/JohnnyCrazy/SpotifyAPI-NET/blob/8e5a9bffc4f63f586ad59b647e9d99dc94a7c1b2/SpotifyAPI.Web.Auth/EmbedIOAuthServer.cs
    /// </summary>
    internal class OAuthClient
    {
        private const string TokenUrl = @"https://api.genius.com/oauth/token";

        public string Code { get; }
        public string ClientId { get; }
        public string ClientSecret { get; }

        public Uri RedirectUri { get; }

        public OAuthClient(string code, string clientId, string clientSecret, Uri redirectUri)
        {
            Code = code;
            ClientId = clientId;
            ClientSecret = clientSecret;
            RedirectUri = redirectUri;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            using (var wb = new WebClient())
            {
                NameValueCollection data = new();
                data["code"] = Code;
                data["client_id"] = ClientId;
                data["client_secret"] = ClientSecret;
                data["redirect_uri"] = RedirectUri.ToString();
                data["response_type"] = "code";
                data["grant_type"] = "authorization_code";

                byte[] response = await wb.UploadValuesTaskAsync(new Uri(TokenUrl), "POST", data);
                var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(Encoding.UTF8.GetString(response));

                return json["access_token"];
            }
        }
    }
}
