using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SpotifyArchiver.Application.Abstraction;
using SpotifyArchiver.Application.Abstraction.Dtos;
using SpotifyArchiver.Presentation;

namespace SpotifyArchiver.Presentation.Test
{
    [TestFixture]
    public class OperationsHandlerTests
    {
        private Mock<ISpotifyService> _spotifyServiceMock;
        private OperationsHandler _operationsHandler;
        private StringWriter _stringWriter;
        private TextWriter _originalOut;

        [SetUp]
        public void Setup()
        {
            _spotifyServiceMock = new Mock<ISpotifyService>();
            _operationsHandler = new OperationsHandler(_spotifyServiceMock.Object);
            _stringWriter = new StringWriter();
            _originalOut = Console.Out;
            Console.SetOut(_stringWriter);
        }

        [TearDown]
        public void TearDown()
        {
            Console.SetOut(_originalOut);
            _stringWriter.Dispose();
        }

        [Test]
        public async Task ExecuteOperationAsync_GetPlaylists_ShouldCallServiceAndPrintPlaylists()
        {
            // Arrange
            var playlists = new List<PlaylistDTO>
            {
                new PlaylistDTO { Id = "1", Name = "Playlist 1" },
                new PlaylistDTO { Id = "2", Name = "Playlist 2" }
            };
            _spotifyServiceMock.Setup(s => s.GetPlaylistsAsync()).ReturnsAsync(playlists);

            // Act
            await _operationsHandler.ExecuteOperationAsync("get-playlists");

            // Assert
            _spotifyServiceMock.Verify(s => s.GetPlaylistsAsync(), Times.Once);
            var output = _stringWriter.ToString();
            Assert.That(output, Does.Contain("- Playlist 1 (1)"));
            Assert.That(output, Does.Contain("- Playlist 2 (2)"));
        }

        [Test]
        public async Task ExecuteOperationAsync_UnknownOperation_ShouldPrintUnknownOperationMessage()
        {
            // Arrange
            var operation = "unknown-operation";

            // Act
            await _operationsHandler.ExecuteOperationAsync(operation);

            // Assert
            var output = _stringWriter.ToString();
            Assert.That(output, Does.Contain($"Unknown operation: {operation}"));
        }

        [Test]
        public async Task ExecuteOperationAsync_GetPlaylists_EmptyList_ShouldNotPrintPlaylists()
        {
            // Arrange
            var playlists = new List<PlaylistDTO>();
            _spotifyServiceMock.Setup(s => s.GetPlaylistsAsync()).ReturnsAsync(playlists);

            // Act
            await _operationsHandler.ExecuteOperationAsync("get-playlists");

            // Assert
            _spotifyServiceMock.Verify(s => s.GetPlaylistsAsync(), Times.Once);
            var output = _stringWriter.ToString();
            Assert.That(output.Trim(), Is.EqualTo(""));
        }
    }
}