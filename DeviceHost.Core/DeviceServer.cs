using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Text;
using DeviceHost.Core.Commands;
using DeviceHost.Core.Handlers;
using Serilog;

namespace DeviceHost.Core
{
    public class DeviceServer : 
        IDeviceServer,
        IDisposable
    {
        public int Port { get; set; } = 9797;

        public IPAddress Address { get; set; } = IPAddress.Parse("127.0.0.1");

        public bool Running { get; private set; } = false;

        public CancellationTokenSource Start()
        {
            CancellationTokenSource cts = new();
            CancellationToken cancellationToken = cts.Token;

            Task.Run(async () =>
            {
                Running = true;
                IPEndPoint localEndPoint = new(Address, Port);

                // Create a TCP socket
                using Socket listener = new(Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    // Bind the socket to the local endpoint and listen for incoming connections
                    listener.Bind(localEndPoint);
                    listener.Listen(10);
                    listener.ReceiveTimeout = 100;

                    Log.Information("Device server listening on {localEndPoint}", localEndPoint);

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        Socket handler = await listener.AcceptAsync(cancellationToken);
                        Log.Information("Client connected (remote endpoint: {endpoint}).", handler.RemoteEndPoint);
                        await HandleClient(handler, cancellationToken);
                    }
                }
                catch (OperationCanceledException)
                {

                }
                catch (Exception ex)
                {
                    Log.Error("Exception: {exception}", ex);
                }

                Log.Information("Device server closed");
                Running = false;
            }, CancellationToken.None);

            return cts;
        }

        async Task HandleClient(Socket handler, CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[65535];
            var parser = new DeviceParser(this);

            try
            {
                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        int bytesReceived = await handler.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None, cancellationToken);

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
                }
                catch (OperationCanceledException)
                {

                }
                finally
                {
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                    Log.Information("Client disconnected.");
                }
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

        public async Task Join()
        {
            await Task.Run(async () =>
            {
                while (Running)
                    await Task.Delay(10);
            });
        }

        #region IDeviceServer

        public IDeviceHandler? GetHandler(Command command) => _handler.GetHandler(command);

        public void Cleanup() => _handler.Cleanup();

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
                    _handler.Cleanup();
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

        private readonly ServerHandler _handler = new();
    }
}
