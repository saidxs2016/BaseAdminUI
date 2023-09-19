using System.Threading.Channels;

namespace Application.LocalQueues;


public interface ISignalRLogQueue
{
    ValueTask EnqueueAsync(dynamic item, CancellationToken token = default);

    ValueTask<dynamic> DequeueAsync(CancellationToken cancellationToken);
}

public class SignalRLogQueue : ISignalRLogQueue
{
    private readonly Channel<dynamic> _queue;
    private readonly int _capacity = 100;

    public SignalRLogQueue()
    {
        var options = new BoundedChannelOptions(_capacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
        };
        _queue = Channel.CreateBounded<dynamic>(options);
    }

    public async ValueTask EnqueueAsync(dynamic item, CancellationToken token = default)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        await _queue.Writer.WriteAsync(item, cancellationToken: token);
    }

    public async ValueTask<dynamic> DequeueAsync(CancellationToken cancellationToken = default) => await _queue.Reader.ReadAsync(cancellationToken);
}
