using Application.LocalQueues;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.SignalRHubs.Clients;


public sealed class NotificationClient1
{
    // *********************************** Altaki kod kapalı kod sormadan müdahale etmeyiniz ***********************************

    private readonly ISignalRLogQueue _queue;
    private readonly SignalRClientOptions _signalRSetting;
    private static int _try_connect_counter = 0;
    private readonly string _targetClient;

    public NotificationClient1(ISignalRLogQueue queue, IConfiguration configuration, IOptions<List<SignalRClientOptions>> options)
    {
        _targetClient = GetType().Name;
        _signalRSetting = options.Value.FirstOrDefault(i => i.Name == _targetClient);
        _signalRSetting.ConnectionString = configuration[$"SignalRClientOptions:Hubs:{_signalRSetting.ConnectionString}"];
        _queue = queue;

    }

    // ================ Get Connection ================  
    private static HubConnection? _connection;
    public HubConnection? Connection => _connection;
    // ================ Start Connection ================
    public Task<bool> StartHubConnectionAsync(CancellationToken token = default)
    {
        try
        {
            token.ThrowIfCancellationRequested();

            _connection = new HubConnectionBuilder()
                .WithUrl(_signalRSetting.ConnectionString, c =>
                {
                    c.AccessTokenProvider = async () =>
                    {
                        if (!string.IsNullOrEmpty(_signalRSetting.AuthToken))
                            return _signalRSetting.AuthToken;

                        else if (!string.IsNullOrEmpty(_signalRSetting.AuthUri))
                        {
                            using HttpClient client = new();
                            try
                            {
                                var res = await client.GetStringAsync(_signalRSetting.AuthUri);
                                if (!string.IsNullOrEmpty(res))
                                    return res;
                            }
                            catch (Exception) { }

                        }

                        return null;
                    };
                    // c.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling;
                })
                .ConfigureLogging(c =>
                {
                    // c.ClearProviders();
                    c.SetMinimumLevel(LogLevel.Warning);
                })
                .WithAutomaticReconnect(_signalRSetting.GetReconnectIntervalsAfterConnect)
                //.AddMessagePackProtocol()
                .Build();

            _connection.StartAsync(token).Wait(token);
            _try_connect_counter = 0;
            _connection.Reconnecting += async (ex) =>
            {
                var log = new
                {
                    Name = _targetClient,
                    Date = DateTime.Now,
                    LogLevel = "Warning",
                    Message = $"\nSignalR OnReconnecting method is tirgger.\nError:{ex.Message}"
                };
                await _queue.EnqueueAsync(log, token);
            };
            _connection.Reconnected += async (id) =>
            {
                var log = new
                {
                    Name = _targetClient,
                    Date = DateTime.Now,
                    LogLevel = "Info",
                    Message = $"\nSignalR OnReconnected method is tirgger, ConnectionID:{id}"

                };
                await _queue.EnqueueAsync(log);
            };
            _connection.Closed += async (ex) =>
            {
                var log = new
                {
                    Name = _targetClient,
                    Date = DateTime.Now,
                    LogLevel = "Error",
                    Message = $"\nSignalR OnClosed method is tirgger.\nError:{ex.Message}"

                };
                await _queue.EnqueueAsync(log);
                _ = StartHubConnectionAsync(token);
            };

            var log = new
            {
                Name = _targetClient,
                Date = DateTime.Now,
                LogLevel = "Info",
                Message = $"SignalR is Connected, ConnectionID:{Connection.ConnectionId}"

            };
            _ = _queue.EnqueueAsync(log, token);
        }
        catch (Exception ex)
        {

            if (_try_connect_counter % 25 == 0)
            {
                var log = new
                {
                    Name = _targetClient,
                    LogLevel = "Error",
                    Date = DateTime.Now,
                    Message = $"SignalR Hub is not connected, after {_try_connect_counter + 1} try connect number.\n Error: {ex.Message}"
                };

                _ = _queue.EnqueueAsync(log, token);
            }

            Task.Delay(_signalRSetting.ReconnectInterval, token).Wait(token);
            _try_connect_counter++;

            if (_signalRSetting.MaxReconnectCount > 0 && _try_connect_counter >= _signalRSetting.MaxReconnectCount)
            {
                var log = new
                {
                    Name = _targetClient,
                    Date = DateTime.Now,
                    LogLevel = "Error",
                    Message = $"SignalR Hub is not connected, after {_try_connect_counter} try connect number.\n Error: {ex.Message}"

                };
                _ = _queue.EnqueueAsync(log, token);
            }
            else
            {
                _ = StartHubConnectionAsync(token);
            }
        }

        return Task.FromResult(true);
    }
}
