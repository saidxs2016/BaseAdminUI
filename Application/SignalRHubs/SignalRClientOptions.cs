namespace Application.SignalRHubs;

public class SignalRClientOptions
{
    public string Name { get; set; }
    public string Type { get; set; }
    public string ConnectionString { get; set; }
    public int MaxReconnectCount { get; set; }
    public int ReconnectInterval { get; set; }

    public string? AuthUri { get; set; }
    public string? AuthToken { get; set; }

    public int[] ReconnectIntervalsAfterConnect { get; set; }
    public TimeSpan[] GetReconnectIntervalsAfterConnect => ReconnectIntervalsAfterConnect.Select(i => TimeSpan.FromSeconds(i)).ToArray();

}
