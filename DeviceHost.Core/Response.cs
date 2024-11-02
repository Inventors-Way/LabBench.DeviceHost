using System;
using System.Collections.Generic;
using System.Linq;
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
        UnknownDevice
    }

    public class Response
    {
        public static string Error(ErrorCode code)
        {
            var builder = new StringBuilder();
            builder.AppendLine("START;");
            builder.AppendLine($"ERR {code};");
            builder.AppendLine("END;");
            return builder.ToString();
        }

        public static string OK()
        {
            var builder = new StringBuilder();
            builder.AppendLine("START;");
            builder.AppendLine("OK;");
            builder.AppendLine("END;");
            return builder.ToString();
        }

        internal static string Error(object deviceClosed)
        {
            throw new NotImplementedException();
        }
    }
}
