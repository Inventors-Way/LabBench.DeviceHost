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
        public IDeviceHandler? GetHandler(UseDirective directive)
        {
            if (directive.System == SystemID.SERVER)
                return this;

            if (_handlers.ContainsKey(directive.Port)) 
                return _handlers[directive.Port];

            return null;
        }

        public string Execute(Command command) => Server.Execute(command);

        public DeviceHandlerMock Server { get; } = new DeviceHandlerMock();

        private readonly Dictionary<string, IDeviceHandler> _handlers = new();       
    }
}
