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
            if (!line.StartsWith("USE"))
                throw new ArgumentException("INVALID USE DIRECTIVE");


        }


    }
}
