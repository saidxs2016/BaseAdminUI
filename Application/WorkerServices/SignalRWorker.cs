using Application.LocalQueues;
using Application.SignalRHubs.Clients;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using Serilog.Core;
using Serilog.Core.Enrichers;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Application.WorkerServices;

public class SignalRWorker : BackgroundService
{
    private readonly ISignalRLogQueue _queue;
    private readonly ILogger<SignalRWorker> _logger;
    private readonly NotificationClient1 _notificationClient1;

    public SignalRWorker(ISignalRLogQueue queue, ILogger<SignalRWorker> logger, NotificationClient1 notificationClient1)
    {
        _queue = queue;
        _logger = logger;
        _notificationClient1 = notificationClient1;
    }



    protected override Task ExecuteAsync(CancellationToken token)
    {
        try
        {
            token.ThrowIfCancellationRequested();
            Task.Run(async () => await DoWork(token), token);
        }
        catch (Exception) { }

        return Task.CompletedTask;

    }


    private async Task DoWork(CancellationToken token)
    {

        //   ======================== Init SignalR Clients Asyncron ========================
        try
        {

            _ = _notificationClient1.StartHubConnectionAsync(token);

        }
        catch (Exception) { }


        //   ======================== SignalR Log Handler ========================
        while (!token.IsCancellationRequested)
        {
            var log = await _queue.DequeueAsync(token);
            try
            {
                JsonObject logAsObj = JsonNode.Parse(JsonSerializer.Serialize(log)).AsObject();
                string message = Convert.ToString(logAsObj["Message"]);
                string level = Convert.ToString(logAsObj["LogLevel"]);

                var enrichers = new List<ILogEventEnricher>
                {
                    new PropertyEnricher("Date", Convert.ToDateTime(logAsObj["Date"]?.ToString())),
                    new PropertyEnricher("Name", Convert.ToString(logAsObj["Name"])),
                };

                using (LogContext.Push(enrichers.ToArray()))
                {
                    if (level == "Warning")
                        _logger.LogWarning(message);
                    if (level == "Error")
                        _logger.LogError(new Exception(message), message);
                    if (level == "Info")
                    {
                        Console.WriteLine(message);
                        _logger.LogInformation(message);
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }

}
