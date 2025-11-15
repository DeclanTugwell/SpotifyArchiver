using Microsoft.Extensions.Hosting;

namespace SpotifyArchiver.Presentation
{
    public class PresentationService(OperationHandler operationHandler)
        : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
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
                    Console.WriteLine(ex.ToString());
                }
            }

            Console.WriteLine("Closing Application...");
        }
    }
}
