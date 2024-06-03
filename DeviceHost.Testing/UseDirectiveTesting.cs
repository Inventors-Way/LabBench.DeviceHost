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
        public void UseServer()
        {
            UseDirective directive = new("USE SERVER");

            Assert.AreEqual(directive.System, SystemID.SERVER);
            Assert.AreEqual(directive.Port, "none");
            Assert.AreEqual(directive.Device, DeviceID.None);
        }

        [TestMethod]
        public void UsePort()
        {
            UseDirective directive = new("USE PORT COM4 CPARPLUS");

            Assert.AreEqual(directive.System, SystemID.PORT);
            Assert.AreEqual(directive.Port, "COM4");
            Assert.AreEqual(directive.Device, DeviceID.CPARPlus);
        }
    }
}
