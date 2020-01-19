using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using GooglePlayMusic.Internal;
using NUnit.Framework;

namespace GpmConnectorTests
{
    public class PythonServerTests
    {
        private PythonConnectorServerManager _pythonConnectorServerManager;

        [OneTimeSetUp]
        public void Setup()
        {
            _pythonConnectorServerManager = new PythonConnectorServerManager();
        }

        [Test]
        public async Task TestStart()
        {
            // Arrange
            StartAndWaitUntilConnectorStarted();

            // Act
            var health = await _pythonConnectorServerManager.CheckHealth();

            // Assert
            Assert.True(health);
        }


        [Test]
        public async Task TestAuth()
        {
            /* Cannot automate */
            // // Arrange
            // StartAndWaitUntilConnectorStarted();
            //
            // // Act
            // var awaitableAuth = _pythonConnectorServerManager.PerformOAuth();
            //
            // await _pythonConnectorServerManager.WaitForServerState(PythonServerState.AwaitingAuthenticationToken);
            //
            // await _pythonConnectorServerManager.SendAuthToken("4/vgFrVN598m_tALYFnh1yngwCjaXkUXB6_2jDgZob60DCuZc3swOhhI8");
            //
            // await awaitableAuth;
            //
            // // Assert
            //
            // _pythonConnectorServerManager.IsAuthenticated.Should().BeTrue();

        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _pythonConnectorServerManager.Stop();
            _pythonConnectorServerManager.Dispose();
        }


        private void StartAndWaitUntilConnectorStarted()
        {
            var waitEvent = new ManualResetEventSlim();
            waitEvent.Reset();

            _pythonConnectorServerManager.ServerStateObservable.Subscribe((state) =>
            {
                if (state == PythonServerState.Running) waitEvent.Set();
            });

            _pythonConnectorServerManager.Start();

            waitEvent.Wait();
        }
    }
}