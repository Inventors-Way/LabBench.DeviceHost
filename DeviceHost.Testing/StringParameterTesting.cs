using DeviceHost.Core.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceHost.Testing
{
    [TestClass]
    public class StringParameterTesting
    {
        [TestMethod]
        public void T01_OneValue()
        {
            var integer = new StringParameter("NAME Hello");
            Assert.IsTrue(integer.Parse(out _));
            Assert.AreEqual("NAME", integer.Name);
            Assert.AreEqual(1, integer.Length);
            Assert.AreEqual("Hello", integer[0]);
        }

        [TestMethod]
        public void T02_TwoValues()
        {
            var integer = new StringParameter("NAME Hello World");
            Assert.IsTrue(integer.Parse(out _));
            Assert.AreEqual("NAME", integer.Name);
            Assert.AreEqual(2, integer.Length);
            Assert.AreEqual("Hello", integer[0]);
            Assert.AreEqual("World", integer[1]);
        }
    }
}
