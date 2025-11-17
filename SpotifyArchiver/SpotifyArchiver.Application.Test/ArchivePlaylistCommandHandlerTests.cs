using Moq;
using Shouldly;
using SpotifyArchiver.Application.Abstraction;
using SpotifyArchiver.Application.Implementation.Features.Commands;
using SpotifyArchiver.DataAccess.Abstraction;
using SpotifyArchiver.Domain;

namespace SpotifyArchiver.Application.Test
{
    public class ArchivePlaylistCommandHandlerTests
    {
        private Mock<ISpotifyService> _spotifyServiceMock;
        private Mock<IPlaylistRepository> _playlistRepositoryMock;
        private ArchivePlaylistCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _spotifyServiceMock = new Mock<ISpotifyService>();
            _playlistRepositoryMock = new Mock<IPlaylistRepository>();
            _handler = new ArchivePlaylistCommandHandler(_spotifyServiceMock.Object, _playlistRepositoryMock.Object);
        }

        [Test]
        public async Task WhenHandleAsyncCalled_WithValidPlaylistId_ThenPlaylistIsArchived()
        {
            // Arrange
            var playlistId = "playlist1";
            var playlists = new List<Playlist>
            {
                new Playlist { SpotifyId = playlistId, Name = "Test Playlist" }
            };
            var tracks = new List<Track>
            {
                new Track { Name = "Track 1" },
                new Track { Name = "Track 2" }
            };

            _spotifyServiceMock.Setup(s => s.GetPlaylistsAsync()).ReturnsAsync(playlists);
            _spotifyServiceMock.Setup(s => s.GetPlaylistTracksAsync(playlistId)).ReturnsAsync(tracks);

            var command = new ArchivePlaylistCommand(playlistId);

            // Act
            await _handler.HandleAsync(command);

            // Assert
            _spotifyServiceMock.Verify(s => s.GetPlaylistsAsync(), Times.Once);
            _spotifyServiceMock.Verify(s => s.GetPlaylistTracksAsync(playlistId), Times.Once);
            _playlistRepositoryMock.Verify(r => r.AddPlaylistAsync(It.Is<Playlist>(p => p.SpotifyId == playlistId && p.Tracks.Count == 2)), Times.Once);
        }

        [Test]
        public async Task WhenHandleAsyncCalled_WithInvalidPlaylistId_ThenNothingIsArchived()
        {
            // Arrange
            var playlistId = "playlist1";
            var playlists = new List<Playlist>
            {
                new Playlist { SpotifyId = "otherId", Name = "Test Playlist" }
            };

            _spotifyServiceMock.Setup(s => s.GetPlaylistsAsync()).ReturnsAsync(playlists);

            var command = new ArchivePlaylistCommand(playlistId);

            // Act
            await _handler.HandleAsync(command);

            // Assert
            _spotifyServiceMock.Verify(s => s.GetPlaylistsAsync(), Times.Once);
            _spotifyServiceMock.Verify(s => s.GetPlaylistTracksAsync(It.IsAny<string>()), Times.Never);
            _playlistRepositoryMock.Verify(r => r.AddPlaylistAsync(It.IsAny<Playlist>()), Times.Never);
        }
    }
}
