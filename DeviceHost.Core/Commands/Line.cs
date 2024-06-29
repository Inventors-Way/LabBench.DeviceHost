using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceHost.Core.Commands
{
    public abstract class Line
    {
        public Line(string line)
        {
            Content = line;
            Parts = (from part in line.Split(' ')
                     where !string.IsNullOrEmpty(part)
                     select part.Trim()).ToArray();
        }

        public abstract bool Parse(out string errorMessage);

        public string Content { get; }

        public string[] Parts { get; }
    }
}
