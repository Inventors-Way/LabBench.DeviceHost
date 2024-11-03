using DeviceHost.Core;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace DeviceHost.Testing
{
    [TestClass]
    public class IntegrationTesting
    {
        private bool Enabled { get; } = true;

        private void TestScript(string packet)
        {
            if (!Enabled)
            {
                Console.WriteLine("INTEGRATION TESTS IS DISABLED");
                return;
            }

            try
            {
                Console.WriteLine("Sent:");
                Console.WriteLine(packet);

                string response = TestUtility.Send(packet);
                Console.WriteLine("Received:");
                Console.WriteLine(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex.Message);
            }
        }

        [TestMethod]
        public async Task T01_BasicTest()
        {
            var server = new DeviceServer();
            var cts = server.Start();

            TestScript(TestUtility.GetPacket("Ports.txt", "COM8"));
            TestScript(TestUtility.GetPacket("Create.txt", "COM8"));
            TestScript(TestUtility.GetPacket("Open.txt", "COM8"));
            TestScript(TestUtility.GetPacket("Waveform.txt", "COM8"));
            TestScript(TestUtility.GetPacket("State.txt", "COM8"));
            TestScript(TestUtility.GetPacket("Signals.txt", "COM8"));
            TestScript(TestUtility.GetPacket("Start.txt", "COM8"));
            await Task.Delay(50);
            TestScript(TestUtility.GetPacket("State.txt", "COM8"));

            await Task.Delay(1000);
            TestScript(TestUtility.GetPacket("Signals.txt", "COM8"));
            TestScript(TestUtility.GetPacket("State.txt", "COM8"));

            TestScript(TestUtility.GetPacket("Close.txt", "COM8"));

            cts.Cancel();
            await server.Join();
        }

        [TestMethod]
        public async Task T02_CombinedTest()
        {
            var server = new DeviceServer();
            var cts = server.Start();

            TestScript(TestUtility.GetPackets(new string[]
                {
                    "Ports.txt",
                    "Create.txt",
                    "Open.txt",
                    "Close.txt"
                }, "COM8"));

            cts.Cancel();
            await server.Join();
        }
    }
}