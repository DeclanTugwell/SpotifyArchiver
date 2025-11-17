using Microsoft.Extensions.Hosting;
using SpotifyArchiver.Presentation;

public class ConsoleHostedService : IHostedService
{
    private readonly ConsoleController _consoleController;
    private readonly IHostApplicationLifetime _appLifetime;

    public ConsoleHostedService(ConsoleController consoleController, IHostApplicationLifetime appLifetime)
    {
        _consoleController = consoleController;
        _appLifetime = appLifetime;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _appLifetime.ApplicationStarted.Register(() =>
        {
            Task.Run(async () =>
            {
                try
                {
                    await _consoleController.RunAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
                finally
                {
                    _appLifetime.StopApplication();
                }
            }, cancellationToken);
        });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
