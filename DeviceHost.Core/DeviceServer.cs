using System.Net;
using System.Net.Sockets;
using System.Text;
using DeviceHost.Core.Commands;
using Serilog;

namespace DeviceHost.Core
{
    public class DeviceServer : 
        IDeviceServer,
        IDeviceHandler,
        IDisposable
    {
        public int Port { get; set; } = 9797;

        public IPAddress Address { get; set; } = IPAddress.Parse("127.0.0.1");

        public string ApiKey { get; set; } = "1234";

        public async Task Run()
        {
            IPEndPoint localEndPoint = new(Address, Port);

            // Create a TCP socket
            using Socket listener = new(Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                // Bind the socket to the local endpoint and listen for incoming connections
                listener.Bind(localEndPoint);
                listener.Listen(10);

                Log.Information("Device server listening on {localEndPoint}", localEndPoint);

                while (true)
                {
                    // Wait for a connection
                    Socket handler = await listener.AcceptAsync();
                    Log.Information("Client connected (remote endpoint: {endpoint}).", handler.RemoteEndPoint);

                    // Process the client connection
                    _ = Task.Run(() => HandleClient(handler));
                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception: {exception}", ex);
            }
        }

        async Task HandleClient(Socket handler)
        {
            byte[] buffer = new byte[65535];
            var parser = new DeviceParser()
            {
                ApiKey = ApiKey
            };


            try
            {
                while (true)
                {
                    int bytesReceived = await handler.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);

                    if (bytesReceived == 0)
                        break;

                    string data = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
                    
                    var response = parser.Parse(data);

                    if (response.Complete)
                    {
                        byte[] responseBytes = Encoding.UTF8.GetBytes(response.Response);
                        await handler.SendAsync(new ArraySegment<byte>(responseBytes), SocketFlags.None);
                    }
                }

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

                Log.Information("Client disconnected.");
            }
            catch (Exception ex)
            {
                Log.Error("Exception in HandleClientAsync: {exception}", ex);
            }
            finally
            {
                handler.Dispose();
            }
        }

        #region IDeviceServer

        public IDeviceHandler GetHandler(UseDirective directive)
        {
            throw new NotImplementedException();
        }

        #endregion
        #region IDeviceHandler

        public string Execute(Command command)
        {
            return "ERR, NOT IMPLEMENTED";
        }

        #endregion
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
