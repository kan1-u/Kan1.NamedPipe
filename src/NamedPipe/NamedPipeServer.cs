using System;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Kan1.NamedPipe.Formatters;

namespace Kan1.NamedPipe
{
    public class NamedPipeServer<R, S>
    {
        public string PipeName { get; private set; }
        public Func<R, S> Func { get; private set; }
        public IStreamFormatter Formatter { get; private set; }

        public event EventHandler<R> OnRequestReceived;
        public event EventHandler<S> OnResponseSended;

        public NamedPipeServer(string pipeName, Func<R, S> func) : this(pipeName, func, new JsonStreamFormatter(Encoding.UTF8)) { }
        public NamedPipeServer(string pipeName, Func<R, S> func, IStreamFormatter formatter)
        {
            this.PipeName = pipeName;
            this.Func = func;
            this.Formatter = formatter;
        }

        public async Task StartAsync(CancellationToken cancellationToken, int numberOfServerInstances = 1)
        {
            Func<Task> task = () => Task.Run(async () =>
            {
                while (true)
                {
                    using (var server = new NamedPipeServerStream(PipeName, PipeDirection.InOut, numberOfServerInstances))
                    {
                        await server.WaitForConnectionAsync(cancellationToken);
                        if (cancellationToken.IsCancellationRequested) break;
                        var request = Formatter.Deserialize<R>(server);
                        OnRequestReceived?.Invoke(this, request);
                        var response = Func(request);
                        Formatter.Serialize(server, response);
                        OnResponseSended?.Invoke(this, response);
                    }
                }
            });

            await Task.Run(() =>
            {
                for (int i = 0; i < numberOfServerInstances; i++)
                {
                    task();
                }
            });
        }
    }
}
