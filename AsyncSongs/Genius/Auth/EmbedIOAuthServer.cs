using EmbedIO;
using EmbedIO.Actions;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncSongs.Genius
{
    /// <summary>
    /// Wrapper for Genius OAuth2 authentication requests.
    /// This code was taken mostly from: https://github.com/JohnnyCrazy/SpotifyAPI-NET/blob/8e5a9bffc4f63f586ad59b647e9d99dc94a7c1b2/SpotifyAPI.Web.Auth/EmbedIOAuthServer.cs
    /// </summary>
    internal class EmbedIOAuthServer : IAuthServer
    {
        public Uri BaseUri { get; }
        public int Port { get; }

        public event Func<object, AuthorizationCodeResponse, Task>? AuthorizationCodeReceived;

        private const string AssetsResourcePath = "Genius.Resources.auth_assets";
        private const string DefaultResourcePath = "Genius.Resources.default_site";

        private CancellationTokenSource? _cancelTokenSource;
        private readonly WebServer _webServer;

        public EmbedIOAuthServer(Uri baseUri, int port)
            : this(baseUri, port, Assembly.GetExecutingAssembly(), DefaultResourcePath) { }

        public EmbedIOAuthServer(Uri baseUri, int port, Assembly resourceAssembly, string resourcePath)
        {
            BaseUri = baseUri;
            Port = port;

            _webServer = new WebServer(port)
                .WithModule(new ActionModule("/", HttpVerbs.Get, (ctx) =>
                {
                    var query = ctx.Request.QueryString;

                    AuthorizationCodeReceived?.Invoke(this, new AuthorizationCodeResponse(query["code"]!)
                    {
                        State = query["state"]
                    });

                  return ctx.SendStringAsync("OK", "text/plain", Encoding.UTF8);
              }))
              .WithEmbeddedResources("/auth_assets", Assembly.GetExecutingAssembly(), AssetsResourcePath)
              .WithEmbeddedResources(baseUri.AbsolutePath, resourceAssembly, resourcePath);
        }

        public Task StartAsync()
        {
            _cancelTokenSource = new CancellationTokenSource();
            _webServer.Start(_cancelTokenSource.Token);
            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            _cancelTokenSource?.Cancel();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _webServer?.Dispose();
            }
        }
    }
}
