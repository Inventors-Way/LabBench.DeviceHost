using DeviceHost.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceHost.Testing.Mocks
{
    public class DeviceHandlerMock :
        IDeviceHandler
    {
        public string Execute(Command command)
        {
            CommandReceived = true;
            Command = command;
            return "OK";
        }

        public void Reset()
        {
            CommandReceived = false;
            Command = null;
        }

        public bool CommandReceived { get; private set; } = false;

        public Command? Command { get; private set; }
    }
}
