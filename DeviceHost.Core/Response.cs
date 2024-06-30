using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceHost.Core
{
    public enum ErrorCode
    {
        NoHandlerFound,
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
        public static string Error(ErrorCode code) => $"ERR;{code};";

        public static string OK() => "OK;";

        internal static string Error(object deviceClosed)
        {
            throw new NotImplementedException();
        }
    }
}
