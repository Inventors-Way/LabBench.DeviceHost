using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceHost.Core
{
    public interface IDeviceHandler
    {
        string Execute(Command command);

        void Cleanup();
    }
}
