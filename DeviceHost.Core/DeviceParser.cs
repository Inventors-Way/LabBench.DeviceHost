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

            }

            builder.Clear();
            return new ParseResult(response.ToString());
        }

        private readonly StringBuilder builder = new ();
        private readonly Regex _commandPattern = new(@"START\s[\w]+;[\w\s;]*END;", RegexOptions.Compiled);
    }
}
