using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using GooglePlayMusic.Internal;

namespace Playground
{
    class Program
    {
        static async Task Main(string[] args)
        {

            var pythonConnector = new PythonConnectorServerManager();

            pythonConnector.ServerStateObservable
                .Subscribe(state => Console.WriteLine($"NEW SERVER STATE {state}"));

            pythonConnector.StdOutputObservable.Subscribe(s => Console.WriteLine($"STDOUD>{s}"));
            pythonConnector.ErrOutputObservable.Subscribe(s => Console.WriteLine($"STDERR!!!>{s}"));

            pythonConnector.Start();

            await pythonConnector.WaitForServerState(PythonServerState.Running);

            var awaitableAuth = pythonConnector.PerformOAuth();

            await pythonConnector.WaitForServerState(PythonServerState.AwaitingAuthenticationToken);

            Console.WriteLine("COPY TOKEN HERE >");
            var token = Console.ReadLine();

            await pythonConnector.SendAuthToken(token);

            await awaitableAuth;

            Console.ReadKey();
        }
    }
}