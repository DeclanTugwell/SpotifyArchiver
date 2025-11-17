using SpotifyAPI.Web;
using SpotifyArchiver.Application.Abstraction;
using SpotifyArchiver.Application.Implementation.extensions;
using SpotifyArchiver.DataAccess.Abstraction;
using SpotifyArchiver.DataAccess.Abstraction.entities;


namespace SpotifyArchiver.Application.Implementation
{
    public class SpotifyService : ISpotifyService
    {
        private readonly SpotifyAuthService _authService;
        private readonly IPlaylistRepository _playlistRepository;
        private ISpotifyClient? _clientBacking;
        private ISpotifyClient Client
        {
            get
            {
                if (_clientBacking == null)
                    throw new InvalidOperationException("Spotify client has not been initialized.");
                return _clientBacking;
            }
            set => _clientBacking = value;
        }

        public SpotifyService(string clientId, string redirectUri, string configPath, IPlaylistRepository playlistRepository)
        {
            _authService = new SpotifyAuthService(clientId, redirectUri, configPath);
            _playlistRepository = playlistRepository;
        }

        public SpotifyService(ISpotifyClient client, IPlaylistRepository playlistRepository)
        {
            _clientBacking = client;
            _playlistRepository = playlistRepository;
            _authService = new SpotifyAuthService("", "", ""); // This is not ideal, but it's a quick fix for now.
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
                    foreach (var playlist in playlistPage.Items)
                    {
                        if (playlist.Id != null)
                        {
                            listOfPlaylists.Add(new Playlist
                            {
                                SpotifyId = playlist.Id,
                                Name = playlist.Name ?? "N/A",
                                SpotifyUri = playlist.Uri ?? ""
                            });
                        }
                    }
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

        public async Task ArchivePlaylist(string playlistId)
        {
            var spotifyPlaylist = await Client.Playlists.Get(playlistId);
            if (spotifyPlaylist == null)
            {
                throw new InvalidOperationException("Playlist could not be found.");
            }

            var playlist = new Playlist
            {
                SpotifyId = playlistId,
                Name = spotifyPlaylist.Name ?? "N/A",
                SpotifyUri = spotifyPlaylist.Uri ?? "",
                Tracks = await GetAllTracks(spotifyPlaylist.Tracks)
            };

            await _playlistRepository.AddAsync(playlist);
        }

        public async Task DeleteArchivedPlaylistAsync(int playlistId)
        {
            await _playlistRepository.DeletePlaylistAsync(playlistId);
        }

        private async Task<List<Track>> GetAllTracks(Paging<PlaylistTrack<IPlayableItem>>? tracksPage)
        {
            var tracks = new List<Track>();

            var pagesAvailable = tracksPage != null;
            while (pagesAvailable)
            {
                if (tracksPage?.Items != null)
                {
                    tracks.AddRange(tracksPage.Items.Select(spotifyTrack => spotifyTrack.ToTrack()).OfType<Track>());
                }

                if (string.IsNullOrEmpty(tracksPage?.Next) == false)
                {
                    tracksPage = await Client.NextPage(tracksPage);
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
