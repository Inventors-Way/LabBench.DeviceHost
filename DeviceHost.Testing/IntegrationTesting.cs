using System.IO;
using System.Net.Sockets;
using System.Text;

namespace DeviceHost.Testing
{
    [TestClass]
    public class IntegrationTesting
    {
        private bool Enabled { get; } = true;

        private void TestScript(string script)
        {
            if (!Enabled)
            {
                Console.WriteLine("INTEGRATION TESTS IS DISABLED");
                return;
            }

            try
            {
                string messageToSend = TestUtility.GetScript(script);
                Console.WriteLine("Sent:");
                Console.WriteLine(messageToSend);
                Console.WriteLine();

                string response = TestUtility.Send(messageToSend);
                Console.WriteLine("Received:");
                Console.WriteLine(response);
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex.Message);
            }
        }

        [TestMethod]
        public void T01_GetPorts() => TestScript("Ports.txt");

        [TestMethod]
        public void T02_Open() => TestScript("Open.txt");

        [TestMethod]
        public void T03_Waveform() => TestScript("Waveform.txt");

        [TestMethod]
        public void T04_Start() 
        { 
            TestScript("Start.txt");
            //await Task.Delay(5000);

        }

        [TestMethod]
        public void T05_Close() => TestScript("Close.txt");

    }
}