using AsyncSongsServer.Spotify;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsyncSongsServer.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ReadSongsController : ControllerBase
    {
        [HttpGet]
        public void Login()
        {
            SpotifyServices.Instance.Login();
        }

        [HttpGet]
        public void TrackLoginRedirect()
        {
            int a = 4;
            //SpotifyServices.Instance.RegisterTokenFrom(access_token);
        }

        [HttpGet]
        public async Task<string> GetUsername()
        {
            return await SpotifyServices.Instance.GetUsername();
        }

        [HttpGet]
        public async Task<Song> SearchSong(string text, string playlistName = null)
        {
            List<Playlist> playlists = new();
            if (playlistName is null)
            {
                // TODO? Search through all playlists instead.
                // playlists = await FetchPlaylists();
            }
            else
            {
                playlists.Add(new Playlist(playlistName));
            }

            List<Task<Song>> tasks = new List<Task<Song>>();
            foreach (Playlist playlist in playlists)
            {
                tasks.Add(playlist.TryFindSong(text));
            }

            Song[] songs = await Task.WhenAll(tasks);
            return songs.FirstOrDefault(s => s != null);
        }
    }
}
