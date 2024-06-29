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
            var serverMock = TestUtility.CreateDeviceServerMock();
            string script = TestUtility.GetScript("Ports.txt");
            var parser = new DeviceParser(serverMock);
            var result = parser.Parse(script);

            Assert.IsTrue(result.Complete);
            Console.WriteLine(result.Response);

            Assert.IsTrue(serverMock.Server.CommandReceived);
            var cmd = serverMock.Server.Command;

            Assert.IsNotNull(cmd);
            Assert.AreEqual("PORTS", cmd.Name);
        }

        [TestMethod]
        public void T02_Open()
        {
            var serverMock = TestUtility.CreateDeviceServerMock();
            string script = TestUtility.GetScript("Open.txt");
            var parser = new DeviceParser(serverMock);
            var result = parser.Parse(script);

            Assert.IsTrue(result.Complete);
            Console.WriteLine(result.Response);

            Assert.IsTrue(serverMock["COM4"].CommandReceived);
            var cmd = serverMock["COM4"].Command;

            Assert.IsNotNull(cmd);
            Assert.AreEqual("OPEN", cmd.Name);
        }
    }
}
