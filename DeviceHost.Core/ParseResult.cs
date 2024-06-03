using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceHost.Core
{
    public class ParseResult
    {
        public ParseResult()
        {
            Complete = false;
        }

        public ParseResult(string response) 
        {
            Complete = true;
            Response = response;
        }

        public bool Complete { get; }

        public string Response { get; } = "";
    }
}
