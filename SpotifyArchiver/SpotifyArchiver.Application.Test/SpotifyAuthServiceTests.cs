using SpotifyArchiver.Application.Implementation;
using System.Text.Json;
using SpotifyAPI.Web;

namespace SpotifyArchiver.Application.Test
{
    [TestFixture]
    public class SpotifyAuthServiceTests
    {
        private const string TokenPath = "spotify_tokens.json";

        [SetUp]
        public void SetUp()
        {
            // Clean up any old token files before each test
            if (File.Exists(TokenPath))
            {
                File.Delete(TokenPath);
            }
            // Environment variables SPOTIFY_CLIENT_ID and SPOTIFY_REDIRECT_URI are assumed to be set externally.
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up token files after each test
            if (File.Exists(TokenPath))
            {
                File.Delete(TokenPath);
            }
        }

        [Test]
        public async Task GetAuthenticatedClient_WhenValidTokenFileExists_ReturnsAuthenticatedClient()
        {
            // Arrange
            var token = new PKCETokenResponse
            {
                AccessToken = "dummy_access_token",
                RefreshToken = "dummy_refresh_token",
                ExpiresIn = 3600,
                Scope = "user-read-email",
                TokenType = "Bearer",
                CreatedAt = DateTime.UtcNow
            };
            await File.WriteAllTextAsync(TokenPath, JsonSerializer.Serialize(token));
            
            var authService = new SpotifyAuthService();

            // Act
            var client = await authService.GetAuthenticatedClient();

            // Assert
            Assert.That(client, Is.Not.Null);
        }

        [Test, Explicit("This test is explicit because it requires manual user interaction in a browser.")]
        public async Task GetAuthenticatedClient_WhenNoTokenExists_PerformsFullAuthFlow()
        {
            // Arrange
            var authService = new SpotifyAuthService();
            Console.WriteLine("This test will open a browser window for Spotify authentication.");
            Console.WriteLine("Please log in and grant permissions to continue the test.");

            // Act
            var client = await authService.GetAuthenticatedClient();
            
            // Assert
            Assert.That(client, Is.Not.Null);
            Assert.That(File.Exists(TokenPath), Is.True, "The token file should have been created.");

            var json = await File.ReadAllTextAsync(TokenPath);
            var token = JsonSerializer.Deserialize<PKCETokenResponse>(json);

            Assert.That(token, Is.Not.Null);
            Assert.That(token.AccessToken, Is.Not.Null.And.Not.Empty);
            Assert.That(token.RefreshToken, Is.Not.Null.And.Not.Empty);

            // Verify the client is usable by making a simple, non-intrusive API call
            var profile = await client.UserProfile.Current();
            Assert.That(profile, Is.Not.Null);
            Assert.That(profile.Id, Is.Not.Null.And.Not.Empty);
            Console.WriteLine($"Successfully authenticated as {profile.DisplayName} ({profile.Id})");
        }
    }
}
