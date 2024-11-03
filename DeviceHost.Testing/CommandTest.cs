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

            Assert.IsTrue(command.VerifyCommand(out string _));
            Assert.AreEqual(SystemID.SERVER, command.System);
            Assert.AreEqual(string.Empty, command.Port);
            Assert.AreEqual("PORTS", command.Name);
        }

        [TestMethod]
        public void T03_Create()
        {
            var script = TestUtility.GetScript("Create.txt");
            Assert.IsTrue(Command.Create(script, out Command command, out string _));

            Assert.IsTrue(command.VerifyCommand(out string _));
            Assert.AreEqual(SystemID.SERVER, command.System);
            Assert.AreEqual(string.Empty, command.Port);
            Assert.AreEqual("CREATE", command.Name);
            Assert.AreEqual(2, command.Content.Length);
            Assert.AreEqual("PORT COM8", command.Content[0]);
            Assert.AreEqual("DEVICE CPARPLUS", command.Content[1]);
        }

    }
}
