using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceHost.Core.Commands
{
    public class StringParameter :
        Line
    {
        public StringParameter(string line) :
            base(line)
        {
        }

        public StringParameter(string line, int expectedValues) :
            base(line, expectedValues)
        {
        }

        public override bool Parse(out string errorMessage)
        {
            if (Parts.Length < 2)
            {
                errorMessage = $"INVALID PARAMETER SPECIFICATION";
                return false;
            }

            if ((ExpectedValues >= 0) && (Parts.Length - 1 != ExpectedValues))
            {
                errorMessage = $"INVALID NUMBER OF VALUES, FOUND [ {Parts.Length - 1} ] EXPECTED [ {ExpectedValues} ]";
                return false;
            }

            Name = Parts[0].ToUpper();

            Values = new string[Parts.Length - 1];

            for (int n = 1; n < Parts.Length; n++)
                Values[n - 1] = Parts[n];

            errorMessage = "";
            return true;
        }

        public string Name { get; private set; } = string.Empty;

        public string[] Values { get; private set; } = Array.Empty<string>();

        public string this[int index] => Values[index];

        public int Length => Values.Length;
    }
}
