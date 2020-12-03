using Microsoft.VisualStudio.TestTools.UnitTesting;
using OctoAwesome.Runtime;
using System.Threading;

namespace OctoAwesome.Network.Tests
{
    [TestClass]
    public class SimulationManagerTests
    {
        private readonly SimulationManager _simulationManager;

        public SimulationManagerTests() => _simulationManager = new SimulationManager(new Settings(), new UpdateHub());

        [TestMethod]
        public void StartStopTest()
        {
            _simulationManager.Start();
            _simulationManager.Stop();
        }

        [TestMethod]
        public void RuntimeTest()
        {
            var reset = new ManualResetEvent(false);
            var timer = new System.Timers.Timer
            {
                Interval = 30000
            };

            timer.Elapsed += (s, e) => reset.Set();


            _simulationManager.Start();
            timer.Start();

            reset.WaitOne();

            _simulationManager.Stop();
            
        }
    }
}
