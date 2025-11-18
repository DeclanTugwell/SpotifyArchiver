using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SpotifyArchiver.Application.Abstraction;
using SpotifyArchiver.Application.Implementation;
using SpotifyArchiver.DataAccess.Abstraction.Services;
using SpotifyArchiver.DataAccess.Implementation.Data;
using SpotifyArchiver.DataAccess.Implementation.Services;
using SpotifyArchiver.Presentation;

var clientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID") ?? throw new InvalidOperationException("SPOTIFY_CLIENT_ID not set in Environment Variables");
var redirectUri = Environment.GetEnvironmentVariable("SPOTIFY_REDIRECT_URI") ?? throw new InvalidOperationException("SPOTIFY_REDIRECT_URI not set in Environment Variables");
var configPath = "spotify_tokens.json";

IHostBuilder builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((services) =>
    {
        services.AddDbContext<SpotifyArchiverDbContext>(options =>
            options.UseSqlite("Data Source=music.db"));

        services.AddScoped<IPlaylistRepository, PlaylistRepository>();
        
        services.AddSingleton<ISpotifyService>(sp => 
            new SpotifyService(clientId, redirectUri, configPath, sp.GetRequiredService<IPlaylistRepository>()));

        services.AddSingleton(sp => OperationHandler.Build(sp.GetRequiredService<ISpotifyService>()));
        services.AddHostedService<PresentationService>();
    });

var host = builder.Build();

// Authenticate Spotify service
var spotifyService = host.Services.GetRequiredService<ISpotifyService>();
await spotifyService.TryAuthenticateAsync(CancellationToken.None);

await host.RunAsync();
