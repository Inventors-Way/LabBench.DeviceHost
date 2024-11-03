using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DeviceHost.Core
{
    public class ParseResult
    {
        public static ParseResult Success(Command command) => new(command);

        public static ParseResult Fail(string error) => new(error);

        private ParseResult(Command command) 
        { 
            _command = command;
            _error = string.Empty;
        }

        private ParseResult(string error) =>
            _error = error;

        public Command? Command => _command;

        public string Error => _error;

        public bool IsSuccess => _command is not null;        
       
        private readonly Command? _command;
        private readonly string _error;
    }
}
