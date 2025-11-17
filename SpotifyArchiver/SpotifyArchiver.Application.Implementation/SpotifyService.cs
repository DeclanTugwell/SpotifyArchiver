using SpotifyAPI.Web;
using SpotifyArchiver.Application.Abstraction;
using SpotifyArchiver.Application.Implementation.extensions;
using SpotifyArchiver.Domain;

namespace SpotifyArchiver.Application.Implementation
{
    public class SpotifyService : ISpotifyService
    {
        private readonly SpotifyAuthService _authService;
        private SpotifyClient? _clientBacking;
        private SpotifyClient Client
        {
            get
            {
                if (_clientBacking == null)
                    throw new InvalidOperationException("Spotify client has not been initialized.");
                return _clientBacking;
            }
        }

        public SpotifyService(string clientId, string redirectUri, string configPath)
        {
            _authService = new SpotifyAuthService(clientId, redirectUri, configPath);
        }

        public async Task<bool> TryAuthenticateAsync(CancellationToken token)
        {
            _clientBacking ??= await _authService.FetchAuthenticatedClient(token);

            return _clientBacking != null;
        }

        public async Task<List<Playlist>> GetPlaylistsAsync()
        {
            var listOfPlaylists = new List<Playlist>();

            var playlistPage = await Client.Playlists.CurrentUsers();
            var pagesAvailable = true;

            while (pagesAvailable)
            {
                if (playlistPage.Items != null)
                {
                    listOfPlaylists.AddPlaylistPage(playlistPage.Items);
                }

                if (string.IsNullOrEmpty(playlistPage.Next) == false)
                {
                    playlistPage = await Client.NextPage(playlistPage);
                }
                else
                {
                    pagesAvailable = false;
                }
            }

            return listOfPlaylists;
        }

        public async Task<List<Track>> GetPlaylistTracksAsync(string playlistId)
        {
            var tracks = new List<Track>();
            var trackPage = await Client.Playlists.GetItems(playlistId);
            var pagesAvailable = true;

            while (pagesAvailable)
            {
                if (trackPage?.Items != null)
                {
                    foreach (var trackItem in trackPage.Items)
                    {
                        if (trackItem.Track is FullTrack track)
                        {
                            tracks.Add(new Track
                            {
                                SpotifyId = track.Id,
                                Name = track.Name,
                                Artist = string.Join(", ", track.Artists.Select(a => a.Name)),
                                Album = track.Album.Name
                            });
                        }
                    }
                }

                if (string.IsNullOrEmpty(trackPage?.Next) == false)
                {
                    trackPage = await Client.NextPage(trackPage);
                }
                else
                {
                    pagesAvailable = false;
                }
            }

            return tracks;
        }
    }
}
