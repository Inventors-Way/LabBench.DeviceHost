using DeviceHost.Core.Commands;
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
        public static bool Create(string content, out Command command, out string errorMessage)
        {
            command = new Command(content);
            return command.Parse(out errorMessage);
        }

        public SystemID System { get; private set; } = SystemID.SERVER;

        public DeviceID Device { get; private set; } = DeviceID.None;

        public string Port { get; private set; } = string.Empty;

        public string Name { get; private set; } = string.Empty;

        public string[] Content { get; private set; } = Array.Empty<string>();

        private Command(string content)
        {
            _lines = (from line in content.Split(';')
                      where !string.IsNullOrEmpty(line)
                      select line.Trim()).ToArray();
        }

        private bool Parse(out string errorMessage)
        {
            if (_lines.Length < 4) 
            {
                errorMessage = $"ERR:INVALID COMMAND FORMAT, CONTENT TOO SHORT (lines < 4, lines = {_lines.Length})";
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

            var useDirective = new UseDirective(_lines[1]);

            if (!useDirective.Parse(out errorMessage))
                return false;

            System = useDirective.System;
            Device = useDirective.Device;
            Port = useDirective.Port;

            var cmdDirective = new StringParameter(_lines[2]);

            if (!cmdDirective.Parse(out errorMessage))
                return false;

            if (cmdDirective.Name != "CMD")
            {
                errorMessage = "ERR:NO COMMAND DIRECTIVE";
                return false;
            }

            Name = cmdDirective[0];

            if (_lines.Length > 4)
                Content = _lines[3..^1];

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

        private readonly string[] _lines;
        private readonly Regex startRegex = new(@"START\s[\w]+", RegexOptions.Compiled);
    }
}
