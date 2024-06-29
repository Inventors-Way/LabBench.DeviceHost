using CPARplusCommLib;
using CPARplusCommLib.Functions;
using DeviceHost.Core.Commands;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            error = string.Empty;

            if (command.Content.Length < 4)
            {
                error = "Invalid content for WAVEFORM command";
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
                            time: ToSeconds(instruction[1])));
                        
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

        public static StopCriterion ParseStopCriterion(int value)
        {
            switch (value)
            {
                case 0: return StopCriterion.STOP_CRITERION_ON_BUTTON_VAS;
                case 1: return StopCriterion.STOP_CRITERION_ON_BUTTON_PRESSED;
                case 2: return StopCriterion.STOP_CRITERION_ON_BUTTON_RELEASED;
                default:
                    Log.Warning("UNKNOWN STOP CRITERION [ {value} ]", value);
                    return StopCriterion.STOP_CRITERION_ON_BUTTON_VAS;
            }
        }

        private static DeviceChannelID ParseChannel(int value)
        {
            switch (value)
            {
                case 0: return DeviceChannelID.NONE;
                case 1: return DeviceChannelID.CH01;
                case 2: return DeviceChannelID.CH02;
                default:
                    Log.Warning("UNKNOWN DEVICE CHANNEL [ {value} ]", value);
                    return DeviceChannelID.NONE;
            }
        }


        public static bool Start(this Command command, out StartStimulation function, out string error)
        {
            function = new StartStimulation();

            if (command.Content.Length != 5)
            {
                error = "Invalid content for START command";
                return false;
            }

            var criterion = new IntegerParameter(command.Content[0], 1, "STOPCRITERION");
            var trigger = new IntegerParameter(command.Content[0], 1, "EXTERNALTRIGGER");
            var overrideRating = new IntegerParameter(command.Content[0], 1, "OVERRIDERATING");
            var outlet01 = new IntegerParameter(command.Content[0], 1, "OUTLET01");
            var outlet02 = new IntegerParameter(command.Content[0], 1, "OUTLET02");

            if (!criterion.Parse(out error)) return false;
            if (!trigger.Parse(out error)) return false;
            if (!overrideRating.Parse(out error)) return false;
            if (!outlet01.Parse(out error)) return false;
            if (!outlet02.Parse(out error)) return false;

            function.Criterion = ParseStopCriterion(criterion[0]);
            function.ExternalTrigger = trigger[0] != 0;
            function.OverrideRating = overrideRating[0] != 0;
            function.Outlet01 = ParseChannel(outlet01[0]);
            function.Outlet02 = ParseChannel(outlet02[0]);

            return true;
        }
    }
}
