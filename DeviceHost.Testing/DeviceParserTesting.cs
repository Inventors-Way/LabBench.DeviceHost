using DeviceHost.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceHost.Testing
{
    [TestClass]
    public class DeviceParserTesting
    {

        [TestMethod]
        public void T01_Ports()
        {
            string script = TestUtility.GetPacket("Ports.txt");
            var parser = new CommandParser();
            var result = parser.Parse(script);

        }

        [TestMethod]
        public void T02_Open()
        {
            string script = TestUtility.GetPacket("Open.txt");
            var parser = new CommandParser();
            var result = parser.Parse(script);
        }
    }
}
