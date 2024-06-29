using DeviceHost.Core;
using DeviceHost.Core.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceHost.Testing
{
    [TestClass]
    public class UseDirectiveTesting
    {
        [TestMethod]
        public void T01_UseServer()
        {
            UseDirective directive = new("USE SERVER");
            Assert.IsTrue(directive.Parse(out _));
            Assert.AreEqual(SystemID.SERVER, directive.System);
            Assert.AreEqual(string.Empty, directive.Port);
            Assert.AreEqual(DeviceID.None, directive.Device);
        }

        [TestMethod]
        public void T02_UsePort()
        {
            UseDirective directive = new("USE PORT COM4 CPARPLUS");
            Assert.IsTrue(directive.Parse(out _));
            Assert.AreEqual(SystemID.PORT, directive.System);
            Assert.AreEqual("COM4", directive.Port);
            Assert.AreEqual(DeviceID.CPARPlus, directive.Device);
        }
    }
}
