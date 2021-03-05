using System;
using System.Threading.Tasks;

namespace AsyncSongs.Genius
{
    public interface IAuthServer : IDisposable
    {
        event Func<object, AuthorizationCodeResponse, Task> AuthorizationCodeReceived;

        Task StartAsync();
        Task StopAsync();

        Uri BaseUri { get; }
    }
}
