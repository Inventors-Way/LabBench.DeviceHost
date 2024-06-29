using DeviceHost.Core.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceHost.Testing
{
    [TestClass]
    public class IntegerParameterTesting
    {
        [TestMethod]
        public void OneInteger()
        {
            var integer = new IntegerParameter("NAME 2");
            Assert.IsTrue(integer.Parse(out _));
            Assert.AreEqual("NAME", integer.Name);
            Assert.AreEqual(1, integer.Length);
            Assert.AreEqual(2, integer[0]);
        }

        [TestMethod]
        public void TwoIntegers()
        {
            var integer = new IntegerParameter("NAME 2 3");
            Assert.IsTrue(integer.Parse(out _));
            Assert.AreEqual("NAME", integer.Name);
            Assert.AreEqual(2, integer.Length);
            Assert.AreEqual(2, integer[0]);
            Assert.AreEqual(3, integer[1]);
        }

        [TestMethod]
        public void NegativeInteger()
        {
            var integer = new IntegerParameter("NAME 2 -43");
            Assert.IsTrue(integer.Parse(out _));
            Assert.AreEqual("NAME", integer.Name);
            Assert.AreEqual(2, integer.Length);
            Assert.AreEqual(2, integer[0]);
            Assert.AreEqual(-43, integer[1]);
        }
    }
}
