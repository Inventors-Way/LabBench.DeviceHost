using CPARplusCommLib;
using CPARplusCommLib.Functions;
using CPARplusCommLib.Messages;
using Inventors.ECP;
using Inventors.ECP.Functions;
using LIOLite.Functions;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceHost.Core.Handlers
{
    public class CPARPlusHandler :
        IDeviceHandler
    {
        public CPARPlusHandler(string port) 
        {
            _device = new CPARplusCentral()
            {
                Location = port,
                PingEnabled = true
            };
            _device.StatusReceived += StatusReceived;
            _device.EventReceived += EventReceived;
        }

        private void EventReceived(object? sender, EventMessage msg)
        {
            lock (lockObject)
            {
                switch (msg.Event)
                {
                    case EventID.EVT_START_STIMULATION:
                        stimulating = true;
                        statusQueue.Clear();
                        break;
                    case EventID.EVT_STOP_STIMULATION:
                        stimulating = false;
                        break;
                }
            }
        }

        private void StatusReceived(object? sender, StatusMessage msg)
        {   
            lock (lockObject) 
            {
                if (stimulating)
                    statusQueue.Enqueue(msg);

                status = msg;
            }
        }

        public string Execute(Command command) => command.Name switch
        {
            "OPEN" => Open(),
            "CLOSE" => Close(),
            "STATE" => State(),
            "PING" => Ping(),
            "CLEAR" => Clear(),
            "START" => Start(command),
            "STOP" => Stop(),
            "WAVEFORM" => Waveform(command),
            _ => $"ERR;UNKNOWN COMMAND [ {command.Name} ]"
        };

        private string Start(Command command)
        {
            if (!command.Start(out StartStimulation function, out string error))
                return error;

            return Run(function, (function) => "OK;");
        }

        private string Stop() =>
            Run(new StopStimulation(), (function) => "OK;");

        private string Waveform(Command command)
        {
            if (!command.Waveform(out SetWaveformProgram function, out string error))
                return error;

            return Run(function, (function) => "OK;");
        }

        private string Open()
        {
            if (_device.IsOpen)
                return "OK;";

            try
            {
                _device.Open();
                Log.Information("Device on port [ {port} ] opened", _device.Location);
                return "OK;";
            }
            catch (Exception ex) 
            {
                return $"ERR;{ex.Message}";
            }
        }

        public string State()
        {
            lock (lockObject)
            {
                if (status is null)
                    return "ERR;No status";

                StringBuilder sb = new StringBuilder();

                sb.AppendLine($"State {status.SystemState};");
                sb.AppendLine($"Stimulating {stimulating};");
                sb.AppendLine($"VasConnected {status.VasConnected};");
                sb.AppendLine($"VasIsLow {status.VasIsLow};");
                sb.AppendLine($"PowerOn {status.PowerOn};");
                sb.AppendLine($"StartPossible {status.StartPossible};");
                sb.AppendLine($"SupplyPressureLow {status.SupplyPressureLow};");
                sb.AppendLine($"Condition {status.Condition};");
                sb.AppendLine($"VasScore {((int) (status.VasScore*10))};");
                sb.AppendLine($"FinalVasScore {((int) (status.FinalVasScore*10))};");
                sb.AppendLine($"SupplyPressure {((int) status.SupplyPressure)};");
                sb.Append($"StopPressed {status.StopPressed};");

                return sb.ToString();
            }
        }

        private string Run<T>(T function, Func<T, string> onSuccess)
            where T : DeviceFunction
        {
            if (!_device.IsOpen)
                return "ERR;Device is closed;";

            try
            {
                _device.Execute(function);
                return onSuccess(function); 
            }
            catch (Exception ex)
            {
                return $"ERR;{ex.Message}";
            }
        }

        private string Ping() =>
            Run(new DeviceIdentification(), (function) =>
            {
                var device = $"{function.Device}, Rev. {function.Version}";

                if (_device.IsCompatible(function))
                {
                    Log.Information("PING: {device}", device);
                    return $"OK;{device};";
                }
                else
                {
                    Log.Error("PING: INCOMPATIBLE DEVICE: {device}", device);
                    return $"ERR:Incompatible device [ {device} ];";
                }
            });

        private string Clear() =>
            Run(new ClearWaveformPrograms(), (function) => "OK;");

        private string Close()
        {
            if (!_device.IsOpen)
                return "OK;";

            try
            {
                _device.Close();
                Log.Information("Device on port [ {port} ] closed", _device.Location);
                return "OK;";
            }
            catch (Exception ex)
            {
                return $"ERR;{ex.Message};";
            }
        }

        public void Cleanup() =>
            _device.Dispose();

        private readonly CPARplusCentral _device;
        private readonly object lockObject = new object();
        private readonly Queue<StatusMessage> statusQueue = new();
        private StatusMessage? status;
        private bool stimulating = false;
    }
}
