using Microsoft.Extensions.Hosting;

namespace SpotifyArchiver.Presentation
{
    public class PresentationService(OperationHandler operationHandler)
        : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (await operationHandler.TryAuthenticate(stoppingToken) == false)
            {
                Console.WriteLine("Authentication Failed.");
            }
            else
            {
                while (stoppingToken.IsCancellationRequested == false)
                {
                    try
                    {
                        operationHandler.ShowAvailableOperations();
                        await operationHandler.AwaitOperation();
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            Console.WriteLine("Closing Application...");
        }
    }
}
