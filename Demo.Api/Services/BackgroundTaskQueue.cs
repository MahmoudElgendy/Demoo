using System.Threading.Channels;

namespace Demo.Api.Services
{
    public interface IBackgroundTaskQueue
    {
        void QueueBackgroundWorkItem(Func<IServiceProvider, Task> workItem);
        Task<Func<IServiceProvider, Task>> DequeueAsync(CancellationToken token);
    }

    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly Channel<Func<IServiceProvider, Task>> _queue =
            Channel.CreateUnbounded<Func<IServiceProvider, Task>>();

        public void QueueBackgroundWorkItem(Func<IServiceProvider, Task> workItem)
            => _queue.Writer.TryWrite(workItem);

        public async Task<Func<IServiceProvider, Task>> DequeueAsync(CancellationToken token)
            => await _queue.Reader.ReadAsync(token);
    }
}
