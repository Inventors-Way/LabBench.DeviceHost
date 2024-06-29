using DeviceHost.Core.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceHost.Core
{
    public interface IDeviceServer
    {
        IDeviceHandler? GetHandler(UseDirective directive);
    }
}
