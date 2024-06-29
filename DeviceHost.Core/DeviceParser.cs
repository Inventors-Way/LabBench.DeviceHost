using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DeviceHost.Core
{
    public class DeviceParser
    {
        public DeviceParser(IDeviceServer server) =>
            _server = server;

        public string ApiKey { get; set; } = "1234";

        public ParseResult Parse(string input)
        {
            builder.Append(input);
            var content = builder.ToString();

            if (!_commandPattern.IsMatch(content))
                return new ParseResult();

            var matches = _commandPattern.Matches(content);
            var response = new StringBuilder();

            foreach (var match in matches)
            {
                try
                {
                    if (match is not Match command)
                        break;

                    response.AppendLine(ParseCommand(command.Value));
                }
                catch (Exception ex)
                {
                    response.AppendLine($"ERR:{ex.Message}");
                }
            }

            builder.Clear();
            return new ParseResult(response.ToString());
        }

        public string ParseCommand(string content)
        {
            if (Command.Create(content, ApiKey, out Command command, out string errorMessage))
            {
                if (_server.GetHandler(command) is IDeviceHandler handler)
                {
                    return handler.Execute(command);
                }
                else
                {
                    return "ERR:NO HANDLER FOUND";
                }
            }
            else
            {
                return errorMessage;
            }
        }

        private IDeviceServer _server;
        private readonly StringBuilder builder = new ();
        private readonly Regex _commandPattern = new(@"START\s[\w]+;[\w\s;]*END;", RegexOptions.Compiled);
    }
}
