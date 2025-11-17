using SpotifyArchiver.Application.Abstraction;
using SpotifyArchiver.DataAccess.Abstraction;
using SpotifyArchiver.Domain;

namespace SpotifyArchiver.Application.Implementation.Features.Commands
{
    public class ArchivePlaylistCommandHandler
    {
        private readonly ISpotifyService _spotifyService;
        private readonly IPlaylistRepository _playlistRepository;

        public ArchivePlaylistCommandHandler(ISpotifyService spotifyService, IPlaylistRepository playlistRepository)
        {
            _spotifyService = spotifyService;
            _playlistRepository = playlistRepository;
        }

        public async Task HandleAsync(ArchivePlaylistCommand command)
        {
            var playlists = await _spotifyService.GetPlaylistsAsync();
            var playlistToArchive = playlists.FirstOrDefault(p => p.SpotifyId == command.PlaylistId);

            if (playlistToArchive != null)
            {
                var tracks = await _spotifyService.GetPlaylistTracksAsync(command.PlaylistId);
                playlistToArchive.Tracks = tracks;
                await _playlistRepository.AddPlaylistAsync(playlistToArchive);
            }
        }
    }
}
