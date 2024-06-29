using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceHost.Core.Commands
{
    public class UseDirective :
        Line
    {
        public UseDirective(string line) :
            base(line)
        {
        }

        public override bool Parse(out string errorMessage)
        {
            if (Parts.Length == 0)
            {
                errorMessage = "INVALID USE DIRECTIVE, WRONG LENGTH";
                return false;
            }

            if (Parts[0].ToUpper() != "USE")
            {
                errorMessage = "INVALID USE DIRECTIVE, DO NOT START WITH USE";
                return false;
            }

            if (Parts.Length == 2)
            {
                if (Parts[1].ToUpper() != "SERVER")
                {
                    errorMessage = "INVALID USE DIRECTIVE, INVALID TYPE";
                    return false;
                }

                System = SystemID.SERVER;
                Port = "none";
                Device = DeviceID.None;

                errorMessage = string.Empty;
                return true;
            }
            else if (Parts.Length == 4)
            {
                switch (Parts[1].ToUpper())
                {
                    case "PORT": 
                        System = SystemID.PORT; 
                        break;
                    default:
                        errorMessage = "NO PORT SPECIFICATION";
                        return false;
                };

                Port = Parts[2];

                switch (Parts[3].ToUpper())
                {
                    case "CPARPLUS":
                        Device = DeviceID.CPARPlus;
                        break;
                    default:
                        errorMessage = "INVALID DEVICE";
                        return false;
                }

                errorMessage = string.Empty;
                return true;
            }
            else
            {
                errorMessage = "INVALID USE DIRECTIVE FORMAT";
                return false;
            }
        }

        public SystemID System { get; private set; }


        public string Port { get; private set; } = string.Empty;

        public DeviceID Device { get; private set; } = DeviceID.None;
    }
}
