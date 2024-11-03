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

        private void TestScript(string script, string port)
        {
            if (!Enabled)
            {
                Console.WriteLine("INTEGRATION TESTS IS DISABLED");
                return;
            }

            try
            {
                string packet = String.Format(TestUtility.GetPacket(script), port);
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

            TestScript("Ports.txt", "COM8");
            TestScript("Create.txt", "COM8");
            TestScript("Open.txt", "COM8");
            TestScript("Waveform.txt", "COM8");
            TestScript("State.txt", "COM8");
            TestScript("Start.txt", "COM8");
            await Task.Delay(50);
            TestScript("State.txt", "COM8");

            await Task.Delay(500);
            TestScript("Signals.txt", "COM8");
            TestScript("State.txt", "COM8");

            TestScript("Close.txt", "COM8");

            cts.Cancel();
            await server.Join();
        }
    }
}