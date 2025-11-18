using SpotifyAPI.Web;
using SpotifyArchiver.Application.Abstraction;
using SpotifyArchiver.Application.Implementation.extensions;
using SpotifyArchiver.DataAccess.Abstraction.Entities;
using SpotifyArchiver.DataAccess.Abstraction.Services;

namespace SpotifyArchiver.Application.Implementation
{
    public class SpotifyService : ISpotifyService
    {
        private readonly SpotifyAuthService _authService;
        private readonly IPlaylistRepository _playlistRepository;
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

        public SpotifyService(string clientId, string redirectUri, string configPath, IPlaylistRepository playlistRepository)
        {
            _authService = new SpotifyAuthService(clientId, redirectUri, configPath);
            _playlistRepository = playlistRepository;
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

        public async Task<Playlist> GetPlaylistAsync(string playlistId)
        {
            var playlist = await Client.Playlists.Get(playlistId);
            return new Playlist(playlist.Id ?? "", playlist.Name ?? "", playlist.Tracks?.Total ?? 0);
        }

        public async Task ArchivePlaylistAsync(string playlistId)
        {
            var playlist = await Client.Playlists.Get(playlistId);
            
            var playlistEntity = new PlaylistEntity
            {
                SpotifyId = playlist.Id ?? "",
                Name = playlist.Name ?? "",
                Description = playlist.Description,
                Owner = playlist.Owner?.DisplayName ?? "",
                SnapshotId = playlist.SnapshotId,
                Uri = playlist.Uri
            };

            await _playlistRepository.SavePlaylistAsync(playlistEntity);
        }
    }
}
