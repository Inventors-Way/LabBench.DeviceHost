using CPARplusCommLib;
using Inventors.ECP.Functions;
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
                Location = port
            };
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

        private string Ping()
        {
            if (!_device.IsOpen)
                return "ERR;Device is closed";

            try
            {
                var function = new DeviceIdentification();
                _device.Execute(function);

                if (_device.IsCompatible(function))
                {
                    return $"OK;{function.Device}, Rev. {function.Version}";
                }
                else
                {
                    return $"ERR:INCOMPATIBLE DEVICE [ {function.Device}, Rev. {function.Version}]";
                }
            }
            catch (Exception ex)
            {
                return $"ERR;{ex.Message}";
            }
        }

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

        public void Cleanup()
        {
            _device.Dispose();
        }

        private readonly CPARplusCentral _device;
    }
}
