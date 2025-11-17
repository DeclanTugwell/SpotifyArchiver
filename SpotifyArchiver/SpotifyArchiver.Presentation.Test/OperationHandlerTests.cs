using Shouldly;
using SpotifyArchiver.Application.Test;
using SpotifyArchiver.DataAccess.Abstraction.entities;

namespace SpotifyArchiver.Presentation.Test
{
    [TestFixture]
    public class OperationHandlerTests
    {
        private FakeSpotifyService _spotifyService;
        private FakePlaylistRepository _playlistRepository;

        [SetUp]
        public void Setup()
        {
            _spotifyService = new FakeSpotifyService();
            _playlistRepository = new FakePlaylistRepository();
        }

        [Test]
        public void Build_Should_AddExpectedOperations()
        {
            var handler = OperationHandler.Build(_spotifyService, _playlistRepository);

            handler.ShouldNotBeNull();

            var operationsField = typeof(OperationHandler)
                .GetField("_operations", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            var operations = operationsField!.GetValue(handler) as List<Operation>;
            operations.ShouldNotBeNull();
            operations.Count.ShouldBe(5);
            operations.Select(o => o.Name).ShouldBe(["Help", "List Playlists", "Archive Playlist", "List Archived Playlists", "Get Archived Songs"]);
        }

        [Test]
        public void ShowAvailableOperations_Should_WriteOperationsToConsole()
        {
            var handler = OperationHandler.Build(_spotifyService, _playlistRepository);
            var opField = typeof(OperationHandler).GetField("_operations", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            var operations = opField!.GetValue(handler) as List<Operation>;
            operations.ShouldNotBeNull();

            using var sw = new StringWriter();
            Console.SetOut(sw);

            handler.ShowAvailableOperations();

            var output = sw.ToString();
            output.ShouldContain("Available Operations:");
            output.ShouldContain("Help");
            output.ShouldContain("List Playlists");
        }

        [Test]
        public async Task AwaitOperation_Should_ExecuteValidOperation()
        {
            var handler = OperationHandler.Build(_spotifyService, _playlistRepository);

            // Simulate user selecting second operation - list playlists
            using var sr = new StringReader("1");
            await using var sw = new StringWriter();
            Console.SetIn(sr);
            Console.SetOut(sw);

            await handler.AwaitOperation();

            sw.ToString().ShouldContain("Your Playlists:");
            sw.ToString().ShouldContain("Test Playlist");
            _spotifyService.GetPlaylistsCalled.ShouldBeTrue();
        }

        [Test]
        public async Task AwaitOperation_Should_RetryOnInvalidOperation()
        {
            var handler = OperationHandler.Build(_spotifyService, _playlistRepository);

            using var sr = new StringReader("-1\n0");
            await using var sw = new StringWriter();
            Console.SetIn(sr);
            Console.SetOut(sw);

            await handler.AwaitOperation();

            sw.ToString().ShouldContain("Invalid operation selected");
            sw.ToString().ShouldContain("Operation Descriptions:");
        }

        [Test]
        public async Task HelpOperation_Should_ListOperationDescriptions()
        {
            var handler = OperationHandler.Build(_spotifyService, _playlistRepository);
            var opField = typeof(OperationHandler).GetField("_operations", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var operations = (List<Operation>)opField!.GetValue(handler)!;

            await using var sw = new StringWriter();
            Console.SetOut(sw);

            await operations[0].Execute();

            var output = sw.ToString();
            output.ShouldContain("Operation Descriptions:");
            output.ShouldContain("Help");
            output.ShouldContain("List Playlists");
        }

        [Test]
        public async Task QueryArchivedPlaylists_Should_ListPlaylists()
        {
            var playlist = new Playlist()
            {
                PlaylistId = 1,
                Name = "Archived Playlist 1",
                SpotifyId = "archived1",
                SpotifyUri = "spotify:playlist:archived1"
            };
            _playlistRepository.ArchivedPlaylists.Add(playlist);

            var handler = OperationHandler.Build(_spotifyService, _playlistRepository);
            var opField = typeof(OperationHandler).GetField("_operations", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var operations = (List<Operation>)opField!.GetValue(handler)!;

            await using var sw = new StringWriter();
            Console.SetOut(sw);

            await operations[3].Execute();

            var output = sw.ToString();
            output.ShouldContain("Your Archived Playlists:");
            output.ShouldContain(playlist.SpotifyId);
            output.ShouldContain(playlist.Name);
        }

        [Test]
        public async Task QueryArchivedSongs_Should_ListSongs()
        {
            var playlistDbId = 1;
            var playlist = new Playlist()
            {
                PlaylistId = playlistDbId,
                Name = "Archived Playlist 1",
                SpotifyId = "archived1",
                SpotifyUri = "spotify:playlist:archived1",
                Tracks = new List<Track>
                {
                    new() { Name = "Test Song 1", ArtistName = "Test Artist 1", SpotifyId = "spotify:track:1", SpotifyUri = "uri1" },
                    new() { Name = "Test Song 2", ArtistName = "Test Artist 2", SpotifyId = "spotify:track:2", SpotifyUri = "uri2" },
                }
            };
            _playlistRepository.ArchivedPlaylists.Add(playlist);

            var handler = OperationHandler.Build(_spotifyService, _playlistRepository);
            var opField = typeof(OperationHandler).GetField("_operations", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var operations = (List<Operation>)opField!.GetValue(handler)!;

            using var sr = new StringReader(playlistDbId.ToString());
            await using var sw = new StringWriter();
            Console.SetIn(sr);
            Console.SetOut(sw);

            await operations[4].Execute();

            var output = sw.ToString();
            output.ShouldContain("Archived Songs:");
            output.ShouldContain("Test Song 1");
            output.ShouldContain("Test Artist 1");
            output.ShouldContain("Test Song 2");
            output.ShouldContain("Test Artist 2");
        }
    }
}
