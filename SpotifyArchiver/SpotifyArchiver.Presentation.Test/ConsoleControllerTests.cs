using Moq;
using SpotifyArchiver.Application.Abstraction;
using SpotifyArchiver.Application.Abstraction.dtos;
using SpotifyArchiver.Presentation;

namespace SpotifyArchiver.Presentation.Test;

[TestFixture]
public class ConsoleControllerTests
{
    private Mock<ISpotifyService> _spotifyServiceMock;
    private ConsoleController _consoleController;
    private StringWriter _stringWriter;
    private StringReader? _stringReader;

    [SetUp]
    public void Setup()
    {
        _spotifyServiceMock = new Mock<ISpotifyService>();
        _consoleController = new ConsoleController(_spotifyServiceMock.Object);
        _stringWriter = new StringWriter();
        Console.SetOut(_stringWriter);
    }

    [TearDown]
    public void TearDown()
    {
        _stringWriter.Dispose();
        _stringReader?.Dispose();
    }

    [Test]
    public async Task RunAsync_When0IsEntered_Exits()
    {
        // Arrange
        _stringReader = new StringReader("0");
        Console.SetIn(_stringReader);

        // Act
        await _consoleController.RunAsync(CancellationToken.None);

        // Assert
        var output = _stringWriter.ToString();
        Assert.That(output, Does.Contain("Welcome to Spotify Archiver!"));
        Assert.That(output, Does.Contain("Choose an option:"));
    }

    [Test]
    public async Task RunAsync_When1IsEntered_CallsGetPlaylistsAsync()
    {
        // Arrange
        _stringReader = new StringReader("1\n0");
        Console.SetIn(_stringReader);
        _spotifyServiceMock.Setup(s => s.GetPlaylistsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Playlist>
            {
                new() { Name = "Playlist1", Owner = "Owner1", Id = "1", Tracks = new List<Track>()}
            });

        // Act
        await _consoleController.RunAsync(CancellationToken.None);

        // Assert
        _spotifyServiceMock.Verify(s => s.GetPlaylistsAsync(It.IsAny<CancellationToken>()), Times.Once);
        var output = _stringWriter.ToString();
        Assert.That(output, Does.Contain("Fetching playlists..."));
        Assert.That(output, Does.Contain("- Playlist1 (by Owner1)"));
    }

    [Test]
    public async Task RunAsync_WhenInvalidChoiceIsEntered_DisplaysErrorMessage()
    {
        // Arrange
        _stringReader = new StringReader("invalid\n0");
        Console.SetIn(_stringReader);

        // Act
        await _consoleController.RunAsync(CancellationToken.None);

        // Assert
        var output = _stringWriter.ToString();
        Assert.That(output, Does.Contain("Invalid choice. Please try again."));
    }
}
