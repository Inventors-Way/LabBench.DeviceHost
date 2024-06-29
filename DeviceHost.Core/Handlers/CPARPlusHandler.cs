using CPARplusCommLib;
using CPARplusCommLib.Messages;
using Inventors.ECP;
using Inventors.ECP.Functions;
using LIOLite.Functions;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceHost.Core.Handlers
{
    public class CPARPlusHandler :
        IDeviceHandler
    {
        public CPARPlusHandler(string port) 
        {
            _device = new CPARplusCentral()
            {
                Location = port,
                PingEnabled = true
            };
            _device.StatusReceived += StatusReceived;
            _device.EventReceived += EventReceived;
        }

        private void EventReceived(object? sender, EventMessage e)
        {            
        }

        private void StatusReceived(object? sender, StatusMessage e)
        {            
        }

        public string Execute(Command command) => command.Name switch
        {
            "OPEN" => Open(),
            "CLOSE" => Close(),
            "PING" => Ping(),
            _ => $"ERR;UNKNOWN COMMAND [ {command.Name} ]"
        };

        private string Open()
        {
            if (_device.IsOpen)
                return "OK;";

            try
            {
                _device.Open();
                Log.Information("Device on port [ {port} ] opened", _device.Location);
                return "OK;";
            }
            catch (Exception ex) 
            {
                return $"ERR;{ex.Message}";
            }
        }

        private string Run<T>(T function, Func<T, string> onSuccess)
            where T : DeviceFunction
        {
            if (!_device.IsOpen)
                return "ERR;Device is closed";

            try
            {
                _device.Execute(function);
                return onSuccess(function); 
            }
            catch (Exception ex)
            {
                return $"ERR;{ex.Message}";
            }
        }

        private string Ping() =>
            Run(new DeviceIdentification(), (function) =>
            {
                var device = $"{function.Device}, Rev. {function.Version}";

                if (_device.IsCompatible(function))
                {
                    Log.Information("PING: {device}", device);
                    return $"OK;{device}";
                }
                else
                {
                    Log.Error("PING: INCOMPATIBLE DEVICE: {device}", device);
                    return $"ERR:INCOMPATIBLE DEVICE [ {device} ]";
                }
            });

        private string Close()
        {
            if (!_device.IsOpen)
                return "OK";

            try
            {
                _device.Close();
                Log.Information("Device on port [ {port} ] closed", _device.Location);
                return "OK";
            }
            catch (Exception ex)
            {
                return $"ERR:{ex.Message}";
            }
        }

        public void Cleanup() =>
            _device.Dispose();

        private readonly CPARplusCentral _device;
    }
}
