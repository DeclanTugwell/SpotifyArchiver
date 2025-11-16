using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SpotifyArchiver.Application.Abstraction;
using SpotifyArchiver.Application.Implementation;
using SpotifyArchiver.DataAccess.Abstraction;
using SpotifyArchiver.DataAccess.Implementation;
using SpotifyArchiver.Presentation;
using SQLitePCL;

var clientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID") ?? throw new InvalidOperationException("SPOTIFY_CLIENT_ID not set in Environment Variables");
var redirectUri = Environment.GetEnvironmentVariable("SPOTIFY_REDIRECT_URI") ?? throw new InvalidOperationException("SPOTIFY_REDIRECT_URI not set in Environment Variables");
var configPath = "spotify_tokens.json";

Batteries.Init();

IHostBuilder builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((services) =>
    {
        services.AddDbContext<MusicDbContext>(options =>
            options.UseSqlite("Data Source=music.db"));

        services.AddScoped<IPlaylistRepository, PlaylistRepository>();
        services.AddScoped<ISpotifyService, SpotifyService>(provider =>
        {
            var repo = provider.GetRequiredService<IPlaylistRepository>();
            return new SpotifyService(clientId, redirectUri, configPath, repo);
        });

        services.AddHostedService<PresentationService>();
        services.AddSingleton<OperationHandler>(provider =>
        {
            var spotify = provider.GetRequiredService<ISpotifyService>();
            var repo = provider.GetRequiredService<IPlaylistRepository>();
            return OperationHandler.Build(spotify, repo);
        });
    });

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MusicDbContext>();
    db.Database.Migrate();
}
await host.RunAsync();
