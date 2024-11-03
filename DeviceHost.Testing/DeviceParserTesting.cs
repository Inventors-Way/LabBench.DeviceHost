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
        public void Ports()
        {
            string script = TestUtility.GetPacket("Ports.txt");
            var parser = new CommandParser();
            var results = parser.Parse(script).ToList();

            Assert.AreEqual(1, results.Count);
            var r = results[0];

            Assert.IsNotNull(r);
            Assert.IsTrue(r.IsSuccess);
            var cmd = r.Command;
            Assert.IsNotNull(cmd);
            Assert.AreEqual("PORTS", cmd.Name);
            Assert.AreEqual(0, cmd.Content.Length);
        }

        [TestMethod]
        public void Open()
        {
            string script = TestUtility.GetPacket("Open.txt");
            var parser = new CommandParser();
            var results = parser.Parse(script).ToList();

            Assert.AreEqual(1, results.Count);
            var r = results[0];

            Assert.IsNotNull(r);
            Assert.IsTrue(r.IsSuccess);
            var cmd = r.Command;
            Assert.IsNotNull(cmd);
            Assert.AreEqual("OPEN", cmd.Name);
            Assert.AreEqual(0, cmd.Content.Length);
        }

        [TestMethod]
        public void CombinedCommands()
        {
            string script = String.Format(TestUtility.GetPackets(new string[]
                {
                "Open.txt",
                "Ports.txt"
                }), "COM3");
            

        }
    }
}
