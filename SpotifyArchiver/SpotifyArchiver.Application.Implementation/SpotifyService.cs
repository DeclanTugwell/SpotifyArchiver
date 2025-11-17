using SpotifyAPI.Web;
using SpotifyArchiver.Application.Abstraction;
using SpotifyArchiver.Application.Abstraction.dtos;


namespace SpotifyArchiver.Application.Implementation
{
    public class SpotifyService : ISpotifyService
    {
        private readonly SpotifyAuthService _authService;
        private SpotifyClient? _client;

        public SpotifyService(string clientId, string redirectUri, string configPath)
        {
            _authService = new SpotifyAuthService(clientId, redirectUri, configPath);
        }

        public async Task<bool> EnsureAuthenticatedAsync(CancellationToken token)
        {
            _client??= await _authService.FetchAuthenticatedClient(token);

            return _client != null;
        }

        public async Task<IEnumerable<Playlist>> GetPlaylistsAsync(CancellationToken cancellationToken)
        {
            if (_client == null)
            {
                await EnsureAuthenticatedAsync(cancellationToken);
            }
            
            var playlists = new List<Playlist>();
            var page = await _client!.Playlists.CurrentUsers(cancellationToken);
            
            await foreach (var item in _client.Paginate(page))
            {
                if (item == null) continue;
                
                var playlistTracks = new List<Track>();
                if (item.Tracks?.Href != null && item.Id != null)
                {
                    var tracksPage = await _client.Playlists.GetItems(item.Id, cancellationToken);
                    if (tracksPage != null)
                    {
                        await foreach (var trackItem in _client.Paginate(tracksPage))
                        {
                            if (trackItem.Track is FullTrack fullTrack)
                            {
                                playlistTracks.Add(new Track
                                {
                                    Id = fullTrack.Id,
                                    Name = fullTrack.Name,
                                    Artists = fullTrack.Artists.Select(a => a.Name)
                                });
                            }
                        }
                    }
                }

                playlists.Add(new Playlist
                {
                    Id = item.Id ?? string.Empty,
                    Name = item.Name,
                    Owner = item.Owner?.DisplayName ?? string.Empty,
                    Tracks = playlistTracks
                });
            }

            return playlists;
        }
    }
}
