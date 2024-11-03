using Inventors.ECP.Profiling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DeviceHost.Core
{
    public class CommandParser
    {
        public CommandParser(IDeviceServer server) =>
            _server = server;

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
                    response.AppendLine(ex.Message);
                }
            }

            builder.Clear();
            return new ParseResult(response.ToString());
        }

        public string ParseCommand(string content)
        {
            if (!Command.Create(content, out Command command, out string errorMessage))
                return errorMessage;

            if (_server.GetHandler(command) is IDeviceHandler handler)
            {
                return handler.Execute(command);
            }
            else
            {
                return Response.Error(ErrorCode.NoHandlerFound);
            }
        }

        private readonly IDeviceServer _server;
        private readonly StringBuilder builder = new ();
        private readonly Regex _commandPattern = new(@"START;[\w\s;]*END;", RegexOptions.Compiled);
    }
}
