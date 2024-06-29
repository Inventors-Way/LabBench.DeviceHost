using CPARplusCommLib;
using CPARplusCommLib.Functions;
using DeviceHost.Core.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceHost.Core.Handlers
{
    public static class CPARPlusParser
    {
        public static double ToSeconds(int x) =>
            ((double)x) / 1000.0;

        public static double ToPressure(int x) =>
            ((double)x) / 1000.0;

        public static bool Waveform(this Command command, out SetWaveformProgram function, out string error)
        {
            function = new SetWaveformProgram();

            if (command.Content.Length < 4)
            {
                error = "Invalid content for start command";
                return false;
            }

            var channel = new IntegerParameter(command.Content[0], 1, "CHANNEL");
            var repeat = new IntegerParameter(command.Content[1], 1, "REPEAT");
            var instructions = new IntegerParameter(command.Content[2], 1, "INSTRUCTIONS");

            if (!channel.Parse(out error)) return false;
            if (!repeat.Parse(out error)) return false;
            if (!instructions.Parse(out error)) return false;

            function.Channel = (byte) channel[0];
            function.Repeat = (byte)repeat[0];

            if (instructions[0] != command.Content.Length - 3)
            {
                error = "Invalid content for start command, number of instructions does not match instruction declaration";
                return false;
            }

            for (int n = 3; n < 3 + instructions[0]; ++n)
            {
                var instruction = new IntegerParameter(command.Content[n]);

                if (instruction.Parse(out error)) return false;

                switch (instruction.Name.Trim())
                {
                    case "STEP":
                        if (instruction.Length != 2)
                        {
                            error = "Invalid number of parameters for STEP instruction";
                            return false;
                        }
                        function.Instructions.Add(WaveformInstruction.Step(
                            pressure: ToPressure(instruction[0]),
                            time: ToSeconds(instruction[1]));
                        
                        break;
                    case "INC":
                        if (instruction.Length != 2)
                        {
                            error = "Invalid number of parameters for INC instruction";
                            return false;
                        }
                        function.Instructions.Add(WaveformInstruction.Increment(
                            delta: ToPressure(instruction[0]),
                            time: ToSeconds(instruction[1])));
                        break;
                    case "DEC":
                        if (instruction.Length != 2)
                        {
                            error = "Invalid number of parameters for DEC instruction";
                            return false;
                        }
                        function.Instructions.Add(WaveformInstruction.Decrement(
                            delta: ToPressure(instruction[0]),
                            time: ToSeconds(instruction[1])));
                        break;
                    default:
                        error = $"Invalid instruction [ {instruction.Name} ]";
                        return false;
                }                
            }

            error = "";
            return true;
        }

        public static bool Start(this Command command, out StartStimulation function, out string error)
        {

            throw new NotImplementedException();
        }
    }
}
