using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kan1.NamedPipe;
using Newtonsoft.Json;

namespace NamedPipeServer.Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await StartStringLengthServer();

            await Task.Delay(-1);
        }

        static async Task StartStringLengthServer()
        {
            var server = new NamedPipeServer<string, int>("string_length", (txt) => txt.Length);
            server.OnRequestReceived += (o, e) => Console.WriteLine($"request: {e}");
            server.OnResponseSended += (o, e) => Console.WriteLine($"response: {e}");

            var source = new CancellationTokenSource();
            await server.StartAsync(source.Token);
        }
    }
}
