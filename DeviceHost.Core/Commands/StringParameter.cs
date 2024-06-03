using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceHost.Core.Commands
{
    public class StringParameter
    {
        public StringParameter(string line)
        {
            var parts = (from part in line.Split(' ')
                         where !string.IsNullOrEmpty(part)
                         select part.Trim()).ToArray();

            if (parts.Length < 2)
                throw new ArgumentException("INVALID PARAMETER SPECIFICATION");

            Name = parts[0].ToUpper();

            values = new string[parts.Length - 1];

            for (int n = 1; n < parts.Length; n++)
                values[n-1] = parts[n]; 
        }

        public string Name { get; }

        public string this[int index] => values[index];

        public int Length => values.Length;

        private readonly string[] values;
    }
}
