using DeviceHost.Core;
using DeviceHost.Core.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceHost.Testing.Mocks
{
    public class DeviceServerMock :
        IDeviceServer,
        IDeviceHandler
    {
        public DeviceServerMock Add(string port, DeviceHandlerMock handler)
        {
            if (_handlers.ContainsKey(port))
            {
                _handlers[port] = handler;
                return this;
            }

            _handlers.Add(port, handler);
            return this;
        }

        public IDeviceHandler? GetHandler(Command command)
        {
            if (command.System == SystemID.SERVER)
                return this;

            if (_handlers.ContainsKey(command.Port)) 
                return _handlers[command.Port];

            return null;
        }

        public void Cleanup() { }

        public string Execute(Command command) => Server.Execute(command);

        public DeviceHandlerMock Server { get; } = new DeviceHandlerMock();

        public DeviceHandlerMock this[string port]
        {
            get
            {
                if (!_handlers.ContainsKey(port))
                    throw new ArgumentException($"No handler found for port [ {port} ]", nameof(port));

                return _handlers[port];
            }
        }

        private readonly Dictionary<string, DeviceHandlerMock> _handlers = new();
    }
}
