using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DeviceHost.Core
{
    public class DeviceServer : IDisposable
    {
        public int Port { get; set; } = 8080;

        public IPAddress Address { get; set; } = IPAddress.Any;

        public async Task Run()
        {
            IPEndPoint localEndPoint = new IPEndPoint(Address, Port);

            // Create a TCP socket
            using Socket listener = new(Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                // Bind the socket to the local endpoint and listen for incoming connections
                listener.Bind(localEndPoint);
                listener.Listen(10);

                Console.WriteLine($"Listening on {localEndPoint}...");

                while (true)
                {
                    // Wait for a connection
                    Socket handler = await listener.AcceptAsync();
                    Console.WriteLine("Client connected.");

                    // Process the client connection
                    _ = Task.Run(() => HandleClient(handler));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

        static async Task HandleClient(Socket handler)
        {
            byte[] buffer = new byte[1024];
            try
            {
                while (true)
                {
                    // Receive data from the client
                    int bytesReceived = await handler.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
                    if (bytesReceived == 0)
                    {
                        // Client has closed the connection
                        break;
                    }

                    string data = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
                    Console.WriteLine($"Received: {data}");

                    // Process the data and prepare a response
                    string response = $"Server received: {data}";
                    byte[] responseBytes = Encoding.UTF8.GetBytes(response);

                    // Send the response back to the client
                    await handler.SendAsync(new ArraySegment<byte>(responseBytes), SocketFlags.None);
                    Console.WriteLine("Response sent.");
                }

                // Shutdown and close the connection
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
                Console.WriteLine("Client disconnected.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in HandleClientAsync: {ex.Message}");
            }
            finally
            {
                handler.Dispose();
            }
        }


        #region Dispose Pattern

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~DeviceServer()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
