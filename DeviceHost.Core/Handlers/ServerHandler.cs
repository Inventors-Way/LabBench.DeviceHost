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

        public string Execute(Command command) =>
            command.Name switch
            {
                "PORTS" => GetPorts(),
                "CREATE" => Create(command),
                "DELETE" => Delete(command),
                _ => Response.Error(ErrorCode.UnknownCommand)
            };        

        public static string GetPorts()
        {
            return string.Join(";", SerialPort.GetPortNames());
        }

        public string Create(Command command)
        {
            if (command.Content.Length != 2)
            {
                return Response.Error(ErrorCode.InvalidCommandContent);
            }

            var port = new StringParameter(command.Content[0], 1);
            var device = new StringParameter(command.Content[1], 1);

            if (!port.Parse(out string portError))
                return portError;

            if (port.Name != "PORT")
                return Response.Error(ErrorCode.NoPortStatement);

            if (!device.Parse(out string deviceError))
                return deviceError;

            if (device.Name != "DEVICE")
                return Response.Error(ErrorCode.NoDeviceStatement);

            switch (device[0])
            {
                case "CPARPLUS":
                    if (_handlers.ContainsKey(port[0]))
                    {
                        Log.Error("Error: attempting to create handler on an allready bound port");
                        return Response.Error(ErrorCode.HandlerExists);
                    }

                    _handlers.Add(port[0], new CPARPlusHandler(port[0]));
                    Log.Information("Creating handler [ {device} ] on port [ {port} ]", device[0], port[0]);

                    return Response.OK(); 

                default: return Response.Error(ErrorCode.UnknownDevice);
            }
        }

        public string Delete(Command command)
        {
            if (command.Content.Length != 1)
            {
                return Response.Error(ErrorCode.InvalidCommandContent);
            }

            var port = new StringParameter(command.Content[0], 1);

            if (!port.Parse(out string portError))
                return portError;

            if (port.Name != "PORT")
                return Response.Error(ErrorCode.NoPortStatement);

            if (!_handlers.ContainsKey(port[0]))
                return Response.OK();

            _handlers[port[0]].Cleanup();
            _handlers.Remove(port[0]);

            Log.Information("Handler deleted for port [ {port} ]", port[0]);

            return Response.OK();
        }

        private readonly Dictionary<string, IDeviceHandler> _handlers = new();
    }
}
