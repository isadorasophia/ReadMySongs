using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AsyncSongsServer.Spotify
{
    public class SpotifyServices
    {
        public static SpotifyServices Instance = new();
        private SpotifyServices()
        {
        }

        private User _user = new();
        private Cache _cache = new();

        private object @lock = new();

        public void Login()
        {
            // TODO: can this be async?

            Uri uri = Utilities.GetLoginUri();
            Process.Start(new ProcessStartInfo(uri.AbsoluteUri) { UseShellExecute = true }); // Works ok on windows
        }

        public void RegisterTokenFrom(string accessToken)
        {
            // Extract the user token and save it.
            _user.RegisterToken(accessToken);
        }

        public async Task<string> GetUsername()
        {
            IUserProfileClient profile = _user.Client?.UserProfile;
            if (profile is null)
            {
                return "Unknown";
            }

            PrivateUser user = await profile.Current();
            return user.Id;
        }

        public async Task<string> GetPlaylistId(string name)
        {
            ISearchClient search = _user.Client?.Search;
            if (search is null)
            {
                return null;
            }

            SearchRequest request = new(SearchRequest.Types.Playlist, name);
            SearchResponse response = await search.Item(request);

            SimplePlaylist playlist = response.Playlists.Items.FirstOrDefault();
            return playlist.Id;
        }

        internal async Task<List<Song>> GetTracks(string playlistId)
        {
            FullPlaylist playlist = null;
            lock (@lock)
            {
                _cache.Playlists.TryGetValue(playlistId, out playlist);
            }

            if (playlist is null)
            {
                playlist = await _user.Client?.Playlists.Get(playlistId);

                lock (@lock)
                {
                    _cache.Playlists.Add(playlistId, playlist);
                }
            }

            List<Song> songs = new();

            IEnumerable<FullTrack> tracks =
                (await _user.Client?.PaginateAll(playlist.Tracks))
                    .Select(i => i.Track as FullTrack)
                    .Where(t => t is not null);

            foreach (FullTrack track in tracks)
            {
                string name = track.Name;
                string artist = track.Artists.FirstOrDefault()?.Name;

                songs.Add(new(name, artist));
            }

            return songs;
        }
    }
}
