using DeviceHost.Core.Commands;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceHost.Core.Handlers
{
    public class ServerHandler :
        IDeviceServer,
        IDeviceHandler
    {
        public IDeviceHandler? GetHandler(Command command)
        {
            if (command.System == SystemID.SERVER)
                return this;

            if (_handlers.ContainsKey(command.Port))
                return _handlers[command.Port];

            return null;
        }

        public void Cleanup()
        {
            foreach (var item in _handlers)
            {
                item.Value.Cleanup();
            }
        }

        public string Execute(Command command)=>
            command.Name switch
            {
                "PORTS" => GetPorts(),
                "CREATE" => Create(command),
                "DELETE" => Delete(command),
                _ => "ERR, NOT IMPLEMENTED",
            };        

        public static string GetPorts()
        {
            return string.Join(";", SerialPort.GetPortNames());
        }

        public string Create(Command command)
        {
            if (command.Content.Length != 2)
            {
                return "ERR:WRONG COMMAND CONTENT";
            }

            var port = new StringParameter(command.Content[0], 1);
            var device = new StringParameter(command.Content[1], 1);

            if (!port.Parse(out string portError))
                return portError;

            if (!device.Parse(out string deviceError))
                return deviceError;

            switch (device[0])
            {
                case "CPARPLUS":
                    return "OK";
                default: return $"ERR: UNKNOWN DEVICE [ {device[0]} ]";
            }
        }

        public string Delete(Command command)
        {
            return "OK";
        }

        private readonly Dictionary<string, IDeviceHandler> _handlers = new();
    }
}
