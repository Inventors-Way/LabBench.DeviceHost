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
        public static bool Create(string content, string apiKey, out Command command, out string errorMessage)
        {
            command = new Command(content);
            return command.Parse(apiKey, out errorMessage);
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

        private bool Parse(string apiKey, out string errorMessage)
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

            if (!VerifyKey(apiKey, out errorMessage))
                return false;

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

        public bool VerifyKey(string apiKey, out string errorMessage)
        {
            if (!startRegex.IsMatch(_lines[0]))
            {
                errorMessage = "INVALID START OF COMMAND";
                return false;
            }

            var parts = _lines[0].Split(' ');

            if (parts.Length != 2)
            {
                errorMessage = "NO API KEY FOUND IN START OF COMMAND";
                return false;
            }

            if (!(parts[1].Trim() == apiKey))
            {
                errorMessage = "INVALID API KEY";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        private readonly string[] _lines;
        private readonly Regex startRegex = new(@"START\s[\w]+", RegexOptions.Compiled);
    }
}
