using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SpotifyArchiver.Application.Abstraction;
using SpotifyArchiver.Application.Implementation;
using SpotifyArchiver.Presentation;

await Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddSingleton<ISpotifyService>(sp =>
        {
            var clientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID");
            if (string.IsNullOrEmpty(clientId))
            {
                throw new InvalidOperationException("SPOTIFY_CLIENT_ID environment variable not set.");
            }
            
            var redirectUri = Environment.GetEnvironmentVariable("SPOTIFY_REDIRECT_URI") ?? "http://localhost:5000/callback";
            var configPath = Environment.GetEnvironmentVariable("SPOTIFY_CONFIG_PATH") ?? ".spotifyConfig";
            
            return new SpotifyService(clientId, redirectUri, configPath);
        });
        services.AddTransient<ConsoleController>();
        services.AddHostedService<ConsoleHostedService>();
    })
    .RunConsoleAsync();