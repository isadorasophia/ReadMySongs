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

        private object @lock = new();

        public static async Task Login()
        {
            Uri uri = Utilities.GetLoginUri();
            var _ = await Windows.System.Launcher.LaunchUriAsync(uri);

            return;
        }

        public void TrackLoginRedirect(Uri uri)
        {
            // Extract the user token and save it.
            string token = Utilities.GetToken(uri);
            _user.RegisterToken(token);

            return;
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
