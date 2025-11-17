using Microsoft.Extensions.DependencyInjection;
using SpotifyArchiver.Application.Abstraction;
using SpotifyArchiver.Application.Implementation;
using System;
using System.Threading.Tasks;

namespace SpotifyArchiver
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);

            var serviceProvider = services.BuildServiceProvider();

            Console.WriteLine("Spotify Archiver");
            Console.WriteLine("Attempting to authenticate with Spotify...");

            try
            {
                var spotifyService = serviceProvider.GetRequiredService<ISpotifyService>();
                var playlists = await spotifyService.GetCurrentUserPlaylists();
                
                // The following code will not be reached until the method is implemented
                Console.WriteLine("Successfully fetched playlists:");
                foreach (var playlist in playlists)
                {
                    Console.WriteLine($"- {playlist.Name}");
                }
            }
            catch (NotImplementedException)
            {
                Console.WriteLine("Authentication successful, but fetching playlists is not yet implemented.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ISpotifyAuthService, SpotifyAuthService>();
            services.AddSingleton<ISpotifyService, SpotifyService>();
        }
    }
}
