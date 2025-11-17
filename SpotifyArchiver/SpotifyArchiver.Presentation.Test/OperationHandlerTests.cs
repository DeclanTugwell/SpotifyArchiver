using Shouldly;

namespace SpotifyArchiver.Presentation.Test
{
    [TestFixture]
    public class OperationHandlerTests
    {
        private FakeSpotifyService _spotifyService;

        [SetUp]
        public void Setup()
        {
            _spotifyService = new FakeSpotifyService();
        }

        [Test]
                public void Build_Should_AddExpectedOperations()
                {
                    var handler = OperationHandler.Build(_spotifyService);
        
                    handler.ShouldNotBeNull();
        
                    var operationsField = typeof(OperationHandler)
                        .GetField("_operations", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
                    var operations = operationsField!.GetValue(handler) as List<Operation>;
                    operations.ShouldNotBeNull();
                    operations.Count.ShouldBe(4);
                    operations.Select(o => o.Name).ShouldBe(["Help", "List Playlists", "Archive Playlist", "List Archived Playlists"]);
                }
        
                [Test]
                public void ShowAvailableOperations_Should_WriteOperationsToConsole()
                {
                    var handler = OperationHandler.Build(_spotifyService);
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
                    var handler = OperationHandler.Build(_spotifyService);
        
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
                public async Task AwaitOperation_Should_ExecuteListArchivedPlaylistsOperation()
                {
                    var handler = OperationHandler.Build(_spotifyService);
        
                    // Simulate user selecting fourth operation - list archived playlists
                    using var sr = new StringReader("3");
                    await using var sw = new StringWriter();
                    Console.SetIn(sr);
                    Console.SetOut(sw);
        
                    await handler.AwaitOperation();
        
                    sw.ToString().ShouldContain("Archived Playlists:");
                    sw.ToString().ShouldContain("Archived Playlist");
                    _spotifyService.GetArchivedPlaylistsCalled.ShouldBeTrue();
                }
        
                [Test]
                public async Task AwaitOperation_Should_RetryOnInvalidOperation()
                {
                    var handler = OperationHandler.Build(_spotifyService);
        
                    // Fake input: invalid first, then valid second
                    using var sr = new StringReader("-1\n0");
                    await using var sw = new StringWriter();
                    Console.SetIn(sr);
                    Console.SetOut(sw);
        
                    // Act
                    await handler.AwaitOperation();
        
                    // Assert
                    sw.ToString().ShouldContain("Invalid operation selected");
                    sw.ToString().ShouldContain("Operation Descriptions:");
                }
        
                [Test]
                public async Task HelpOperation_Should_ListOperationDescriptions()
                {
                    var handler = OperationHandler.Build(_spotifyService);
                    var opField = typeof(OperationHandler).GetField("_operations", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    var operations = (List<Operation>)opField!.GetValue(handler)!;
        
                    await using var sw = new StringWriter();
                    Console.SetOut(sw);
        
                    await operations![0].Execute();
        
                    var output = sw.ToString();
                    output.ShouldContain("Operation Descriptions:");
                    output.ShouldContain("Help");
                    output.ShouldContain("List Playlists");
                }    }
}
