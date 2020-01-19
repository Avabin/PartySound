using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Core.Extesions;

namespace GooglePlayMusic.Internal
{
    public class PythonConnectorServerManager : IDisposable
    {
        private const string PythonScriptFilePath = @"Python\main.py";
        private readonly IGpmConnectorClient _client;

        private Process _pythonServerProcess;

        private ISubject<string> _outputSubject;
        private ISubject<string> _errorSubject;

        private IList<IDisposable> _disposables;

        public PythonServerState ServerState { get; private set; }

        public IObservable<string> StdOutputObservable { get; }
        public IObservable<string> ErrOutputObservable { get; }

        private readonly ISubject<PythonServerState> _serverStateSubject;
        private readonly ISubject<PythonServerState> _serverStateReplaySubject;
        public readonly IObservable<PythonServerState> ServerStateObservable;
        public bool ProcessHasExited { get; private set; }
        public bool IsAuthenticated { get; private set; }
        public bool IsRunning { get; private set; }

        public PythonConnectorServerManager()
        {
            _outputSubject = new Subject<string>();
            _errorSubject = new Subject<string>();
            _client = new RestEase.RestClient("http://localhost:42069").For<IGpmConnectorClient>();
            _disposables = new List<IDisposable>();

            _serverStateSubject = new Subject<PythonServerState>();
            _serverStateReplaySubject = new ReplaySubject<PythonServerState>();
            ServerStateObservable = _serverStateSubject.AsObservable();

            ProcessHasExited = false;
            StdOutputObservable = _outputSubject.AsObservable();
            ErrOutputObservable = _errorSubject.AsObservable();

            _pythonServerProcess = CreatePythonServer();

            var disposable = _serverStateSubject.Subscribe(HandleServerStatusChange);
            var disposableOfSubscriber = _serverStateSubject.Subscribe(_serverStateReplaySubject);

            _disposables.Add(disposable);
            _disposables.Add(disposableOfSubscriber);
        }

        public void Start()
        {

            PublishServerState(PythonServerState.Starting);
            _pythonServerProcess.Start();
            Thread.Sleep(1000);
            _pythonServerProcess.BeginOutputReadLine();
            _pythonServerProcess.BeginErrorReadLine();

            var stdoutDisposable = StdOutputObservable
                .Subscribe(s =>
                {
                    if (string.IsNullOrEmpty(s)) return;
                    if (s.Equals("Google Play Music connector started!")) PublishServerState(PythonServerState.Running);
                    if (s.Contains("Visit the following url:")) PublishServerState(PythonServerState.AwaitingAuthenticationToken);
                });

            _disposables.Add(stdoutDisposable);
        }

        public void Stop()
        {
            PublishServerState(PythonServerState.Closing);
            _pythonServerProcess.Kill(true);
            PublishServerState(PythonServerState.Closed);
            _pythonServerProcess.Dispose();
            _pythonServerProcess = CreatePythonServer();
        }

        public async Task SendAuthToken(string token) => await _pythonServerProcess.StandardInput.WriteLineAsync(token);

        public async Task PerformOAuth()
        {
            PublishServerState(PythonServerState.Authenticating);
            var authAwaitable = _client.PerformOAuth().ContinueWith(task =>
            {
                var s = task.Result;
                if (s.Contains("AUTHENTICATED")) PublishServerState(PythonServerState.Authenticated);
            });

            await authAwaitable;
        }


        public async Task<bool> CheckHealth() => await _client.CheckHealth() == "Healthy";

        public async Task WaitForServerState(PythonServerState state)
        {
            var waitEvent = new ManualResetEventSlim(false);

            _serverStateReplaySubject
                .Where(serverState => serverState == state)
                .Subscribe(serverState => waitEvent.Set());

            await waitEvent.WaitHandle.AsTask();
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable?.Dispose();
            }

            _pythonServerProcess?.Dispose();
        }

        private void PublishServerState(PythonServerState state) => _serverStateSubject?.OnNext(state);

        private Process CreatePythonServer()
        {
            var processStartInfo = new ProcessStartInfo("python.exe")
            {
                Arguments = Path.GetFullPath(PythonScriptFilePath),
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true
            };

            var process = new Process
            {
                StartInfo = processStartInfo,
                EnableRaisingEvents = true
            };

            SetProcessCallbacks(process);

            PublishServerState(PythonServerState.Created);

            return process;
        }

        private void SetProcessCallbacks(Process process)
        {
            process.Exited += (sender, args) => ProcessHasExited = true;

            process.OutputDataReceived += GetDelegateFor(_outputSubject);

            process.ErrorDataReceived += GetDelegateFor(_errorSubject);
        }

        private DataReceivedEventHandler GetDelegateFor(IObserver<string> outBuffer) =>
            (sender, args) => outBuffer.OnNext(args.Data);

        private void HandleServerStatusChange(PythonServerState state)
        {
            ServerState = state;
            switch (state)
            {
                case PythonServerState.Created:
                    break;
                case PythonServerState.Starting:
                    break;
                case PythonServerState.Running:
                    IsRunning = true;
                    break;
                case PythonServerState.Closing:
                    break;
                case PythonServerState.Closed:
                    IsRunning = false;
                    IsAuthenticated = false;
                    break;
                case PythonServerState.Authenticating:
                    break;
                case PythonServerState.AwaitingAuthenticationToken:
                    break;
                case PythonServerState.Authenticated:
                    IsAuthenticated = true;
                    break;
                case PythonServerState.Error:
                    break;
                case PythonServerState.AuthenticationError:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }


}