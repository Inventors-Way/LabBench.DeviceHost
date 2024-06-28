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
            _line = line;
            Parts = (from part in line.Split(' ')
                         where !string.IsNullOrEmpty(part)
                         select part.Trim()).ToArray();
        }

        protected string[] Parts { get; }

        public int Length => _line.Length;


        private readonly string _line;
    }
}
