using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Kan1.NamedPipe;

namespace NamedPipeClient.Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new NamedPipeClient<string, int>(".", "string_length");
            client.OnResponseReceived += (o, e) => Console.WriteLine(e);

            while (true)
            {
                try
                {
                    var txt = Console.ReadLine();
                    var source = new CancellationTokenSource(1000);
                    var response = await client.RequestAsync(txt, source.Token);
                }
                catch (OperationCanceledException e) { Console.WriteLine(e.Message); }
                catch (IOException e) { Console.WriteLine(e.Message); }
            }
        }
    }
}
