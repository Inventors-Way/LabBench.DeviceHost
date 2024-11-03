using Inventors.ECP.Profiling;
using Microsoft.VisualBasic;
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
        private enum State
        {
            WaitingForSTX,
            WaitingForETX            
        }


        private StringBuilder current = new();
        private State state = State.WaitingForSTX;

        public static IEnumerable<string> GetLines(string input) =>
            from s in input.Split(";", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.RemoveEmptyEntries)
            select s.Trim();
        
        public IEnumerable<ParseResult> Parse(string input)
        {
            foreach (var line in GetLines(input))
            {
                switch (state)
                {
                    case State.WaitingForSTX:
                        if (line == Response.STX)
                        {
                            current = new();
                            state = State.WaitingForETX;
                        }
                        break;
                    case State.WaitingForETX:
                        if (line == Response.ETX)
                        {
                            var commandContent = current.ToString();

                            if (!Command.Create(commandContent, out Command command, out string error))
                                yield return ParseResult.Fail(error);
                            else
                                yield return ParseResult.Success(command);

                            state = State.WaitingForSTX;

                            break;
                        }

                        if (line == Response.STX)
                        {
                            current = new();
                            yield return ParseResult.Fail(Response.Error(ErrorCode.ParketFrammingError));
                        }

                        current.AppendLine($"{line.Trim()};");
                        break;
                }
            }
        }
    }
}
