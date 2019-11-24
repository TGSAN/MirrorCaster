using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MirrorCaster
{
    internal class StreamPipe
    {
        private const int BufferSize = 4096;

        public Stream Source { get; protected set; }
        public Stream Destination { get; protected set; }

        private CancellationTokenSource _cancellationToken;
        private Task _worker;

        public StreamPipe(Stream source, Stream destination)
        {
            Source = source;
            Destination = destination;
        }

        public StreamPipe Connect()
        {
            _cancellationToken = new CancellationTokenSource();
            _worker = Task.Run(async () =>
            {
                byte[] buffer = new byte[BufferSize];
                while (true)
                {
                    _cancellationToken.Token.ThrowIfCancellationRequested();
                    int count = await Source.ReadAsync(buffer, 0, BufferSize, _cancellationToken.Token);
                    if (count <= 0)
                    {
                        break;
                    }

                    await Destination.WriteAsync(buffer, 0, count, _cancellationToken.Token);
                    await Destination.FlushAsync(_cancellationToken.Token);
                }
            }, _cancellationToken.Token);
            return this;
        }

        public void Disconnect()
        {
            _cancellationToken.Cancel();
        }
    }
}
