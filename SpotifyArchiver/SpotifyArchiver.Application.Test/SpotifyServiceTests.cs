using Shouldly;
using SpotifyArchiver.Application.Implementation;
using NUnit.Framework;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Collections.Generic;
using SpotifyArchiver.Application.Abstraction.Dtos;

namespace SpotifyArchiver.Application.Test
{
    public class SpotifyServiceTests
    {
        private readonly string _clientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID") ?? throw new InvalidOperationException("SPOTIFY_CLIENT_ID not set");
        private readonly string _redirectUri = Environment.GetEnvironmentVariable("SPOTIFY_REDIRECT_URI") ?? throw new InvalidOperationException("SPOTIFY_REDIRECT_URI not set");
        private readonly string _configPath = "spotify_tokens_test.json";

        [Test]
        [Explicit("Requires SPOTIFY_CLIENT_ID and SPOTIFY_REDIRECT_URI environment variables to be set based on setup within Spotify Developer Portal")]
        [Category("Spotify Integration")]
        public async Task TestAuthenticationFlow()
        {
            var service = new SpotifyService(_clientId, _redirectUri, _configPath);
            var authenticated = await service.EnsureAuthenticatedAsync(CancellationToken.None);
            authenticated.ShouldBeTrue();
            File.Exists(_configPath).ShouldBeTrue();
        }

        [Test]
        [Explicit("Requires SPOTIFY_CLIENT_ID and SPOTIFY_REDIRECT_URI environment variables to be set and a user with at least one playlist")]
        [Category("Spotify Integration")]
        public async Task GetPlaylists_ShouldReturnAtLeastOnePlaylist()
        {
            // Arrange
            var service = new SpotifyService(_clientId, _redirectUri, _configPath);
            
            // Act
            var playlists = await service.GetPlaylistsAsync();

            // Assert
            playlists.ShouldNotBeNull();
            playlists.ShouldNotBeEmpty();
        }
    }
}
