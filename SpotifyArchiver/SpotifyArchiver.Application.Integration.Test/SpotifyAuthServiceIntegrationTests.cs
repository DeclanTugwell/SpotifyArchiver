using NUnit.Framework;
using SpotifyAPI.Web;
using SpotifyArchiver.Application.Implementation;
using System;
using System.Threading.Tasks;

namespace SpotifyArchiver.Application.Integration.Test;

[TestFixture]
[Explicit("These tests require manual user interaction and environment variables.")]
public class SpotifyAuthServiceIntegrationTests
{
    private SpotifyAuthService _authService = null!;

    [SetUp]
    public void Setup()
    {
        var clientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID");
        var redirectUri = Environment.GetEnvironmentVariable("SPOTIFY_REDIRECT_URI");

        if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(redirectUri))
        {
            Assert.Inconclusive("SPOTIFY_CLIENT_ID and SPOTIFY_REDIRECT_URI environment variables must be set.");
        }

        _authService = new SpotifyAuthService();
    }

    [Test]
    public async Task GetAuthenticatedClient_WhenUserLogsIn_ReturnsValidClient()
    {
        // Act
        Console.WriteLine("Starting authentication flow in test. Please follow browser instructions.");
        var client = await _authService.GetAuthenticatedClient();
        Console.WriteLine("Authentication flow complete. Verifying client...");

        // Assert
        Assert.That(client, Is.Not.Null);

        // Verify the client is authenticated by making a simple API call
        var profile = await client.UserProfile.Current();
        Assert.That(profile, Is.Not.Null);
        Assert.That(string.IsNullOrEmpty(profile.Id), Is.False, "User profile should have an ID.");

        Console.WriteLine($"Successfully authenticated as {profile.DisplayName} ({profile.Id})");
    }
}