using System;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Kan1.NamedPipe.Formatters;

namespace Kan1.NamedPipe
{
    public class NamedPipeClient<S, R>
    {
        public string ServerName { get; private set; }
        public string PipeName { get; private set; }
        public IStreamFormatter Formatter { get; private set; }

        public event EventHandler<R> OnResponseReceived;
        public event EventHandler<S> OnRequestSended;

        public NamedPipeClient(string serverName, string pipeName) : this(serverName, pipeName, new JsonStreamFormatter(Encoding.UTF8)) { }
        public NamedPipeClient(string serverName, string pipeName, IStreamFormatter formatter)
        {
            this.ServerName = serverName;
            this.PipeName = pipeName;
            this.Formatter = formatter;
        }

        public R Request(S obj)
        {
            using (var client = new NamedPipeClientStream(ServerName, PipeName))
            {
                client.Connect();
                Formatter.Serialize(client, obj);
                OnRequestSended?.Invoke(this, obj);
                var response = Formatter.Deserialize<R>(client);
                OnResponseReceived?.Invoke(this, response);
                return response;
            }
        }

        public async Task<R> RequestAsync(S obj, CancellationToken cancellationToken)
        {
            using (var client = new NamedPipeClientStream(ServerName, PipeName))
            {
                await client.ConnectAsync(cancellationToken);
                Formatter.Serialize(client, obj);
                OnRequestSended?.Invoke(this, obj);
                var response = Formatter.Deserialize<R>(client);
                OnResponseReceived?.Invoke(this, response);
                return response;
            }
        }
    }
}
