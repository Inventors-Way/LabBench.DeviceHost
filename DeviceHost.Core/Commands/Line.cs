using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceHost.Core.Commands
{
    public abstract class Line
    {
        public Line(string line, int expectedValues = -1, string expectedName = "")
        {
            Content = line;
            Parts = (from part in line.Split(' ')
                     where !string.IsNullOrEmpty(part)
                     select part.Trim()).ToArray();
            ExpectedValues = expectedValues;
            ExpectedName = expectedName;
        }

        public abstract bool Parse(out string errorMessage);

        protected bool ParseExpectedValues(out string errorMessage)
        {
            if ((ExpectedValues >= 0) && (Parts.Length - 1 != ExpectedValues))
            {
                errorMessage = $"INVALID NUMBER OF VALUES, FOUND [ {Parts.Length - 1} ] EXPECTED [ {ExpectedValues} ]";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        protected bool ParseExpectedName(out string errorMessage)
        {
            if (!string.IsNullOrEmpty(ExpectedName) && (Parts[0] != ExpectedName))
            {
                errorMessage = $"INVALID NAME, FOUND [ {Parts[0]} ] EXPECTED [ {ExpectedName} ]";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        public string Content { get; }

        public string[] Parts { get; }

        protected int ExpectedValues { get; } = -1;

        private string ExpectedName { get; } = string.Empty;

    }
}
