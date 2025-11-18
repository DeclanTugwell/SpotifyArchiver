using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace SpotifyArchiver.Presentation
{
    public class Orchestrator : BackgroundService
    {
        private readonly OperationsHandler _operationsHandler;

        public Orchestrator(OperationsHandler operationsHandler)
        {
            _operationsHandler = operationsHandler;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.Write("Enter command: ");
                var input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Please enter a valid command.");
                    continue;
                }

                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Exiting...");
                    break;
                }

                try
                {
                    await _operationsHandler.ExecuteOperationAsync(input);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }
    }
}
