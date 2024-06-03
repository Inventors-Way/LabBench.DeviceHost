using Inventors.ECP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceHost.Core
{
    public enum DeviceID
    {
        None,
        CPARPlus
    }

    public static class DeviceExtensions
    {
        public static DeviceID ToDevice(this string self) =>
            self.ToUpper() switch
            {
                "NONE" => DeviceID.None,
                "CPARPLUS" => DeviceID.CPARPlus,
                _ => throw new ArgumentException("INVALID USE DIRECTIVE, INVALID DEVICE CLASS"),
            };

    }
}
