using DeviceHost.Core.Commands;
using Serilog;
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

            if (port.Name != "PORT")
                return "ERR:NO PORT STATEMENT";

            if (!device.Parse(out string deviceError))
                return deviceError;

            if (device.Name != "DEVICE")
                return "ERR:NO DEVICE STATEMENT";

            switch (device[0])
            {
                case "CPARPLUS":
                    if (_handlers.ContainsKey(port[0]))
                    {
                        Log.Error("Error: attempting to create handler on an allready bound port");
                        return $"ERR: HANDLER ALLREADY CREATED FOR PORT [ {port[0]} ]"; 
                    }

                    _handlers.Add(port[0], new CPARPlusHandler());
                    Log.Information("Creating handler [ {device} ] on port [ {port} ]", device[0], port[0]);

                    return "OK";

                default: return $"ERR: UNKNOWN DEVICE [ {device[0]} ]";
            }
        }

        public string Delete(Command command)
        {
            if (command.Content.Length != 1)
            {
                return "ERR:WRONG COMMAND CONTENT";
            }

            var port = new StringParameter(command.Content[0], 1);

            if (!port.Parse(out string portError))
                return portError;

            if (port.Name != "PORT")
                return "ERR:NO PORT STATEMENT";

            if (!_handlers.ContainsKey(port[0]))
                return $"ERR:NO HANDLER FOUND FOR PORT [ {port[0]} ]";

            _handlers[port[0]].Cleanup();
            _handlers.Remove(port[0]);

            Log.Information("Handler deleted for port [ {port} ]", port[0]);

            return "OK";
        }

        private readonly Dictionary<string, IDeviceHandler> _handlers = new();
    }
}
