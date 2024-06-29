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
            Assert.IsTrue(Command.Create(script, out Command command, out string _));

            Assert.IsTrue(command.VerifyKey("1234"));
            Assert.AreEqual(SystemID.SERVER, command.System);
            Assert.AreEqual(string.Empty, command.Port);
            Assert.AreEqual("PORTS", command.Name);

        }
    }
}
