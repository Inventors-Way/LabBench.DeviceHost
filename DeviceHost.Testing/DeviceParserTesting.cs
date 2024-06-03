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
        public void T01_GetPorts()
        {
            string script = TestUtility.GetScript("GetPorts.txt");
            var parser = new DeviceParser();
            var result = parser.Parse(script);

            Assert.IsTrue(result.Complete);
            Console.WriteLine(result.Response);
        }
    }
}
