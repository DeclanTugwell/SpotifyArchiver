using Moq;
using Shouldly;
using SpotifyAPI.Web;
using SpotifyArchiver.Application.Implementation;
using SpotifyArchiver.DataAccess.Abstraction.entities;

namespace SpotifyArchiver.Application.Test
{
    public class SpotifyServiceUnitTests
    {
        [Test]
        public async Task Test_WhenArchivePlaylistCalled_WithValidPlaylistId_ThenPlaylistsArchived()
        {
            var repo = new FakePlaylistRepository();
            var mockPlaylistClient = new Mock<IPlaylistsClient>();
            mockPlaylistClient.Setup(p => p.Get(It.IsAny<string>())).ReturnsAsync(new FullPlaylist { Name = "Test Playlist", Uri = "test:uri" });
            
            var mockClient = new Mock<ISpotifyClient>();
            mockClient.Setup(c => c.Playlists).Returns(mockPlaylistClient.Object);

            var service = new SpotifyService(mockClient.Object, repo);
            
            var playlist = new Playlist { SpotifyId = "testId", Name = "testName", SpotifyUri = "testUri" };
            
            await service.ArchivePlaylist(playlist.SpotifyId);
            
            repo.ArchivedPlaylists.ShouldContain(p => p.SpotifyId == playlist.SpotifyId);
        }

        [Test]
        public async Task Test_WhenDeleteArchivedPlaylistAsyncCalled_ThenPlaylistDeleted()
        {
            var repo = new FakePlaylistRepository();
            var playlist = new Playlist { PlaylistId = 1, SpotifyId = "testId", Name = "testName", SpotifyUri = "testUri" };
            await repo.AddAsync(playlist);
            
            var service = new SpotifyService(new Mock<ISpotifyClient>().Object, repo);
            
            await service.DeleteArchivedPlaylistAsync(playlist.PlaylistId);
            
            repo.ArchivedPlaylists.ShouldNotContain(p => p.PlaylistId == playlist.PlaylistId);
        }
    }
}