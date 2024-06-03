using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceHost.Core.Commands
{
    public class UseDirective
    {
        public UseDirective(string line)
        {
            var parts = (from part in line.Split(' ')
                        where !string.IsNullOrEmpty(part)
                        select part.Trim()).ToArray();

            if (parts.Length == 0)
                throw new ArgumentException("INVALID USE DIRECTIVE, WRONG LENGTH");

            if (parts[0] != "USE")
                throw new ArgumentException("INVALID USE DIRECTIVE, DO NOT START WITH USE");

            if (parts.Length == 2)
            {
                if (parts[1] != "SERVER")
                    throw new ArgumentException("INVALID USE DIRECTIVE, INVALID TYPE");

                System = SystemID.SERVER;
                Port = "none";
                Device = DeviceID.None;
                return;
            }

            if (parts.Length == 4)
            {
                System = parts[1] switch
                {
                    "PORT" => SystemID.PORT,
                    _ => throw new ArgumentException("INVALID USE DIRECTIVE, INVALID TYPE"),
                };

                Port = parts[2];

                Device = parts[3] switch
                {
                    "CPARPLUS" => DeviceID.CPARPlus,
                    _ => throw new ArgumentException("INVALID USE DIRECTIVE, INVALID DEVICE CLASS"),
                };

                return;
            }
        }

        public SystemID System { get; }

        public string Port { get; } = string.Empty;

        public DeviceID Device { get; }

    }
}
