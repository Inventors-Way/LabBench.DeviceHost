using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DeviceHost.Core
{
    public class Command
    {
        public Command(string content) 
        {
            _lines = (from line in content.Split(';')
                      where !string.IsNullOrEmpty(line)
                      select line.Trim()).ToArray();
        }

        public bool Validate(out string errorMessage)
        {
            if (_lines.Length < 4) 
            {
                errorMessage = "ERR:INVALID COMMAND FORMAT";
                return false;
            }

            if (!startRegex.IsMatch(_lines[0])) 
            {
                errorMessage = "ERR:INVALID START OF COMMAND";
                return false;
            }

            if (!_lines[1].StartsWith("USE"))
            {
                errorMessage = "ERR:MISSING USE INSTRUCTION";
                return false;
            }

            if (!_lines[^1].Contains("END"))
            {
                errorMessage = "ERR:INVALID END OF COMMAND";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        public bool VerifyKey(string apiKey)
        {
            if (!startRegex.IsMatch(_lines[0]))
                throw new InvalidOperationException("INVALID START OF COMMAND");

            var parts = _lines[0].Split(' ');

            if (parts.Length != 2)
                throw new InvalidOperationException("NO API KEY FOUND IN START OF COMMAND");

            return parts[1] == apiKey;
        }

        public string Execute()
        {

            return "";
        }

        private readonly string[] _lines;
        private readonly Regex startRegex = new(@"START\s[\w]+", RegexOptions.Compiled);
    }
}
