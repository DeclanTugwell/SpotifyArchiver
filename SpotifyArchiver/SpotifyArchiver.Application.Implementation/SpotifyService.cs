using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SpotifyAPI.Web;
using SpotifyArchiver.Application.Abstraction;
using SpotifyArchiver.Application.Abstraction.Dtos;


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

        public async Task<List<PlaylistDTO>> GetPlaylistsAsync()
        {
            await EnsureAuthenticatedAsync(CancellationToken.None);

            if (_client == null)
            {
                return new List<PlaylistDTO>();
            }

            var playlists = new List<PlaylistDTO>();
            var page = await _client.Playlists.CurrentUsers();

            await foreach (var playlist in _client.Paginate(page))
            {
                playlists.Add(new PlaylistDTO
                {
                    Id = playlist.Id,
                    Name = playlist.Name,
                    Description = playlist.Description,
                    Public = playlist.Public ?? false,
                    Collaborative = playlist.Collaborative ?? false,
                    Owner = new UserDTO
                    {
                        Id = playlist.Owner.Id,
                        DisplayName = playlist.Owner.DisplayName
                    },
                    Images = playlist.Images.Select(i => new ImageDTO
                    {
                        Url = i.Url,
                        Height = i.Height,
                        Width = i.Width
                    }).ToList(),
                    SnapshotId = playlist.SnapshotId,
                    Uri = playlist.Uri
                });
            }

            return playlists;
        }
    }
}
