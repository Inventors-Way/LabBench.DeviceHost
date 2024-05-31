using System.IO;
using System.Net.Sockets;
using System.Text;

namespace DeviceHost.Testing
{
    [TestClass]
    public class BasicTesting
    {
        [TestMethod]
        public void Ping()
        {
            string serverIp = "127.0.0.1"; // IP address of the server
            int serverPort = 9797;        // Port of the server

            try
            {
                using TcpClient client = new(serverIp, serverPort);
                using NetworkStream stream = client.GetStream();

                // Send a message to the server
                string messageToSend = "Hello, Server!";
                byte[] dataToSend = Encoding.UTF8.GetBytes(messageToSend);
                stream.Write(dataToSend, 0, dataToSend.Length);
                Console.WriteLine("Sent: {0}", messageToSend);

                // Receive the response from the server
                byte[] dataToReceive = new byte[256];
                int bytesRead = stream.Read(dataToReceive, 0, dataToReceive.Length);
                string response = Encoding.UTF8.GetString(dataToReceive, 0, bytesRead);
                Console.WriteLine("Received: {0}", response);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex.Message);
            }
        }
    }
}