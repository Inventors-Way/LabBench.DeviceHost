using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceHost.Core.Commands
{
    public class IntegerParameter :
        Line
    {
        public IntegerParameter(string line, int expectedValues = -1, string expectedName = "") :
            base(line, expectedValues, expectedName)
        {
        }

        public override bool Parse(out string errorMessage)
        {
            if (Parts.Length < 2)
            {
                errorMessage = Response.Error(ErrorCode.InvalidParameterSpecification);
                return false;
            }

            if (!ParseExpectedValues(out errorMessage))
                return false;

            if (!ParseExpectedName(out errorMessage)) 
                return false;

            Name = Parts[0].ToUpper();

            Values = new int[Parts.Length - 1];

            for (int n = 1; n < Parts.Length; n++)
            {
                if (!int.TryParse(Parts[n], out int value))
                {
                    errorMessage = Response.Error(ErrorCode.InvalidInteger);
                    return false;
                }

                Values[n - 1] = value;
            }

            errorMessage = string.Empty;
            return true;
        }

        public string Name { get; private set; } = string.Empty;
        
        public int[] Values { get; private set; } = Array.Empty<int>();

        public int this[int index] => Values[index];

        public int Length => Values.Length;
    }
}
