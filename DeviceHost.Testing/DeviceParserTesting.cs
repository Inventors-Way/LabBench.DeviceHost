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
            string script = TestUtility.GetPacket("Ports.txt", "COM8");
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
            string script = TestUtility.GetPacket("Open.txt", "COM8");
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
            string script = TestUtility.GetPackets(new string[]
                {
                "Ports.txt",
                "Open.txt"
                }, "COM3");

            var parser = new CommandParser();
            var results = parser.Parse(script).ToList();

            Assert.AreEqual(2, results.Count);
            var r = results[0];

            Assert.IsNotNull(r);
            Assert.IsTrue(r.IsSuccess);
            var cmd = r.Command;
            Assert.IsNotNull(cmd);
            Assert.AreEqual("PORTS", cmd.Name);
            Assert.AreEqual(0, cmd.Content.Length);

            r = results[1];

            Assert.IsNotNull(r);
            Assert.IsTrue(r.IsSuccess);
            cmd = r.Command;
            Assert.IsNotNull(cmd);
            Assert.AreEqual("OPEN", cmd.Name);
            Assert.AreEqual(0, cmd.Content.Length);
        }
    }
}
