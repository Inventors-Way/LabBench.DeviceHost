using CPARplusCommLib;
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
            _ => $"ERR: UNKNOWN COMMAND [ {command.Name} ]"
        };

        private string Open()
        {
            if (_device.IsOpen)
                return "OK";

            try
            {
                _device.Open();
                Log.Information("Device on port [ {port} ] opened", _device.Location);
                return "OK";
            }
            catch (Exception ex) 
            {
                return $"ERR:{ex.Message}";
            }
        }

        private string Ping()
        {

            return "OK";
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
