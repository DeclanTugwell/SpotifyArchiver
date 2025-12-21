using Microsoft.Extensions.Hosting;
using SysConsole = System.Console;

namespace SpotifyArchiver.Presentation.Console
{
    public class PresentationService(OperationHandler operationHandler)
        : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (await operationHandler.TryAuthenticate(stoppingToken) == false)
            {
                SysConsole.WriteLine("Authentication Failed.");
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
                        SysConsole.WriteLine(ex.Message);
                    }
                }
            }

            SysConsole.WriteLine("Closing Application...");
        }
    }
}
