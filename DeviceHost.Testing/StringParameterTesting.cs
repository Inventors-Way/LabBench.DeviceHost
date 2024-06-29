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
            var integer = new StringParameter("NAME Hello", 1);
            Assert.IsTrue(integer.Parse(out _));
            Assert.AreEqual("NAME", integer.Name);
            Assert.AreEqual(1, integer.Length);
            Assert.AreEqual("Hello", integer[0]);
        }

        [TestMethod]
        public void T02_TwoValues()
        {
            var integer = new StringParameter("NAME Hello World", 2);
            Assert.IsTrue(integer.Parse(out _));
            Assert.AreEqual("NAME", integer.Name);
            Assert.AreEqual(2, integer.Length);
            Assert.AreEqual("Hello", integer[0]);
            Assert.AreEqual("World", integer[1]);
        }

        [TestMethod]
        public void T03_InvalidNumberOfValues()
        {
            var integer = new StringParameter("NAME Hello World", 1);
            Assert.IsFalse(integer.Parse(out string msg));
            Console.WriteLine(msg);
        }
    }
}
