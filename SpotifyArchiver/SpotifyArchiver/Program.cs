using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SpotifyArchiver.Application.Abstraction;
using SpotifyArchiver.Application.Implementation;
using SpotifyArchiver.Presentation;

public class Program
{
    public static async Task Main(string[] args)
    {
        await Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                var clientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID");
                var redirectUri = Environment.GetEnvironmentVariable("SPOTIFY_REDIRECT_URI");
                var configPath = "config.json"; // This can remain hardcoded or also come from env/config

                if (string.IsNullOrEmpty(clientId))
                {
                    throw new InvalidOperationException("SPOTIFY_CLIENT_ID environment variable is not set.");
                }
                if (string.IsNullOrEmpty(redirectUri))
                {
                    throw new InvalidOperationException("SPOTIFY_REDIRECT_URI environment variable is not set.");
                }

                services.AddSingleton<ISpotifyService>(sp => 
                    new SpotifyService(clientId, redirectUri, configPath));
                services.AddSingleton<OperationsHandler>();
                services.AddHostedService<Orchestrator>();
            })
            .RunConsoleAsync();
    }
}
