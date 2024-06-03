using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceHost.Core.Commands
{
    public class IntegerParameter
    {
        public IntegerParameter(string line)
        {
            var parts = (from part in line.Split(' ')
                         where !string.IsNullOrEmpty(part)
                         select part.Trim()).ToArray();

            if (parts.Length < 2)
                throw new ArgumentException("INVALID PARAMETER SPECIFICATION");

            Name = parts[0].ToUpper();

            values = new int[parts.Length - 1];

            for (int n = 1; n < parts.Length; n++)
            {
                values[n - 1] = int.Parse(parts[n]);
            }
        }

        public string Name { get; }

        public int this[int index] => values[index];

        public int Length => values.Length;

        private readonly int[] values;

    }
}
