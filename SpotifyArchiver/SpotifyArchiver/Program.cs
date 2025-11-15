using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SpotifyArchiver.Application.Implementation;
using SpotifyArchiver.Presentation;

var clientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID") ?? throw new InvalidOperationException("SPOTIFY_CLIENT_ID not set in Environment Variables");
var redirectUri = Environment.GetEnvironmentVariable("SPOTIFY_REDIRECT_URI") ?? throw new InvalidOperationException("SPOTIFY_REDIRECT_URI not set in Environment Variables");
var configPath = "spotify_tokens.json";
var spotifyService = new SpotifyService(clientId, redirectUri, configPath);
await spotifyService.TryAuthenticateAsync(CancellationToken.None);

IHostBuilder builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((services) =>
    {
        services.AddSingleton(OperationHandler.Build(spotifyService));
        services.AddHostedService<PresentationService>();
    });

var host = builder.Build();
await host.RunAsync();
