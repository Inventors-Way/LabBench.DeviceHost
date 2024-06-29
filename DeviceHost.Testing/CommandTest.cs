using DeviceHost.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceHost.Testing
{
    [TestClass]
    public class CommandTest
    {
        [TestMethod]
        public void T01_GetPorts()
        {
            var script = TestUtility.GetScript("Ports.txt");
            Assert.IsTrue(Command.Create(script, "1234", out Command command, out string _));

            Assert.IsTrue(command.VerifyKey("1234", out string _));
            Assert.AreEqual(SystemID.SERVER, command.System);
            Assert.AreEqual(string.Empty, command.Port);
            Assert.AreEqual("PORTS", command.Name);
        }

        [TestMethod]
        public void T02_GetPortsInvalidKey()
        {
            var script = TestUtility.GetScript("Ports.txt");
            Assert.IsFalse(Command.Create(script, "2234", out Command command, out string msg));
            Console.WriteLine(msg);
        }

    }
}
