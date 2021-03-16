using AsyncSongs.ReadSongs;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AsyncSongs.Spotify
{
    public class SpotifyRequests
    {
        public static SpotifyRequests Instance = new();
        private SpotifyRequests() { }

        private User _user = new();
        private Cache _cache = new();
        private Login _login = new();

        private object @lock = new();

        public async Task InitializeAsync()
        {
            await _user.InitializeAsync();
        }

        public void OnLogin(Func<Task> a) => _user.UserLoggedIn += a;

        /// <summary>
        /// Try to login to spotify account.
        /// </summary>
        /// <returns>Whether it succeeded sending the request.</returns>
        public async Task<bool> LoginAsync()
        {
            await InitializeAsync();

            if (!_user.IsLogged)
            {
                return await _login.TryLoginAsync();
            }

            return true;
        }

        /// <summary>
        /// Get the username.
        /// </summary>
        /// <returns>Username. Returns null if user has an invalid credential.</returns>
        public async Task<string?> GetUsernameAsync()
        {
            IUserProfileClient? profile = _user.Client?.UserProfile;
            if (profile is null)
            {
                return "Unknown";
            }

            try
            {
                PrivateUser user = await profile.Current();
                return user.DisplayName;
            }
            catch (APIUnauthorizedException)
            {
                // TODO: Capture this elsewhere? For now, this is good enough.
                _user.Logout();
                return null;
            }
        }

        public async Task<string?> GetPlaylistIdAsync(string name)
        {
            if (!_user.IsLogged)
            {
                Debug.Fail("Please login first.");
                return null;
            }

            ISearchClient? search = _user.Client!.Search;
            if (search is null)
            {
                return null;
            }

            SearchRequest request = new(SearchRequest.Types.Playlist, name);
            SearchResponse response = await search.Item(request);

            SimplePlaylist? playlist = response.Playlists.Items?.FirstOrDefault();
            return playlist?.Id;
        }

        internal async Task<List<Song>> GetTracksAsync(string playlistId)
        {
            if (!_user.IsLogged)
            {
                Debug.Fail("Please login first.");
                return new();
            }

            FullPlaylist? playlist = null;
            lock (@lock)
            {
                _cache.Playlists.TryGetValue(playlistId, out playlist);
            }

            if (playlist is null)
            {
                playlist = await _user.Client!.Playlists.Get(playlistId);

                lock (@lock)
                {
                    _cache.Playlists.Add(playlistId, playlist);
                }
            }

            List<Song> songs = new();
            if (playlist is null || playlist.Tracks is null)
            {
                Debug.Fail("Unable to find a suitable playlist?");
                return songs;
            }

            IEnumerable<FullTrack> tracks = 
                (await _user.Client!.PaginateAll(playlist.Tracks))
                    .Where(t => t is not null && t.Track is FullTrack)
                    .Select(t => (t.Track as FullTrack)!);

            foreach (FullTrack track in tracks)
            {
                string name = track.Name;
                string artist = track.Artists.FirstOrDefault()?.Name!;

                songs.Add(new(name, artist));
            }

            return songs;
        }

        internal Task SetLoginTokenAsync(string token) => _user.RegisterTokenAsync(token, save: true);
    }
}
