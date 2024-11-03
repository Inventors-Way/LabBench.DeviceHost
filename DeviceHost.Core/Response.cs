using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DeviceHost.Core
{
    public enum ErrorCode
    {
        NoHandlerFound = 0,
        InvalidCommandFormat,
        InvalidStartOfCommand,
        MissingUseStatement,
        InvalidEndOfCommand,
        NoCommandStatement,
        NoApiKey,
        InvalidNumberOfInstructions,
        InvalidStepInstruction,
        InvalidIncrementInstruction,
        InvalidDecrementInstruction,
        UnknownInstruction,
        InvalidStartCommandContent,
        InvalidParameterSpecification,
        InvalidInteger,
        UnknownCommand,
        OpenFailed,
        NoStatus,
        DeviceClosed,
        CommunicationFailure,
        IncompatibleDevice,
        CloseFailed,
        InvalidCommandContent,
        NoPortStatement,
        NoDeviceStatement,
        HandlerExists,
        UnknownDevice,
        ParketFrammingError
    }

    public class Response
    {
        private readonly StringBuilder builder = new();

        public static string STX => "START;";
        public static string ETX => "END;";

        public Response()
        {
            builder.AppendLine(STX);
        }

        public Response Add(string name)
        {
            builder.AppendLine($"{name};");
            return this;
        }

        public Response Add(string name, string content)
        {
            builder.AppendLine($"{name} {content};");
            return this;
        }

        public Response Add(string name, object content)
        {
            builder.AppendLine($"{name} {content};");
            return this;
        }

        public Response Add(string name, IEnumerable<object> values)
        {
            builder.AppendLine($"{name} {string.Join(",", values)};");
            return this;
        }

        public string Create()
        {
            builder.AppendLine(ETX);
            return builder.ToString();
        }

        public static string Error(ErrorCode code) => new Response()
            .Add("ERR", code)
            .Create();

        public static string OK() => new Response()
            .Add("OK")
            .Create();
    }
}
