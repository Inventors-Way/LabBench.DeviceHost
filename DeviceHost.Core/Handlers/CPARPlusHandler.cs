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
using System.Threading; 

namespace DeviceHost.Core.Handlers
{
    public class CPARPlusHandler :
        IDeviceHandler
    {
        public CPARPlusHandler(string port) 
        {
            _device = new CPARplusCentral()
            {
                Location = port
            };
            _device.StatusReceived += StatusReceived;
            _device.EventReceived += EventReceived;
            timer = new Timer(DeviceTick, null, 0, 1000);
        }

        private void DeviceTick(object? state)
        {
            try
            {
                if (!_device.IsOpen)
                    return;

                _device.Execute(new Ping());
            }
            catch (Exception ex) 
            {
                Log.Warning("Ping failed: {exception}", ex);
            }
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
            "SIGNALS" => Signals(),
            _ => Response.Error(ErrorCode.UnknownCommand)
        };

        private string Start(Command command)
        {
            if (!command.Start(out StartStimulation function, out string error))
                return error;

            return Run(function, (function) => Response.OK());
        }

        private string Stop() =>
            Run(new StopStimulation(), (function) => Response.OK());

        private string Waveform(Command command)
        {
            if (!command.Waveform(out SetWaveformProgram function, out string error))
                return error;

            return Run(function, (function) => Response.OK());
        }

        private int PressureToInteger(double pressure) =>
            (int)(pressure * 10);

        private int RatingToInteger(double vas) =>
            (int)(vas * 10);

        private string Signals()
        {
            lock (lockObject)
            {
                var builder = new StringBuilder();

                builder.AppendLine("Pressure01;Pressure02;Rating;");

                while (statusQueue.Count > 0)
                {
                    var item = statusQueue.Dequeue();

                    builder.Append($"{PressureToInteger(item.ActualPressure01)};");
                    builder.Append($"{PressureToInteger(item.ActualPressure02)};");
                    builder.AppendLine($"{RatingToInteger(item.VasScore)};");
                }

                return builder.ToString();
            }
        }

        private string Open()
        {
            if (_device.IsOpen)
                return Response.OK();

            try
            {
                _device.Open();
                Log.Information("Device on port [ {port} ] opened", _device.Location);
                return Response.OK();
            }
            catch (Exception ex) 
            {
                Log.Error(ex.Message);
                return Response.Error(ErrorCode.OpenFailed);
            }
        }

        public string State()
        {
            lock (lockObject)
            {
                if (status is null)
                    return Response.Error(ErrorCode.NoStatus);

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
                sb.AppendLine($"FinalPressure01 {((int)(status.FinalPressure01 * 10))};");
                sb.AppendLine($"FinalPressure02 {((int)(status.FinalPressure02 * 10))};");
                sb.AppendLine($"SupplyPressure {((int) status.SupplyPressure)};");
                sb.Append($"StopPressed {status.StopPressed};");

                return sb.ToString();
            }
        }

        private string Run<T>(T function, Func<T, string> onSuccess)
            where T : DeviceFunction
        {
            if (!_device.IsOpen)
                return Response.Error(ErrorCode.DeviceClosed);

            try
            {
                _device.Execute(function);
                return onSuccess(function); 
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return Response.Error(ErrorCode.CommunicationFailure);
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
                    return Response.Error(ErrorCode.IncompatibleDevice);
                }
            });

        private string Clear() =>
            Run(new ClearWaveformPrograms(), (function) => Response.OK());

        private string Close()
        {
            if (!_device.IsOpen)
                return Response.OK();

            try
            {
                _device.Close();
                Log.Information("Device on port [ {port} ] closed", _device.Location);
                return Response.OK();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return Response.Error(ErrorCode.CloseFailed);
            }
        }

        public void Cleanup()
        {
            timer.Dispose();
            _device.Dispose();
        }

        private readonly CPARplusCentral _device;
        private readonly object lockObject = new object();
        private readonly Queue<StatusMessage> statusQueue = new();
        private StatusMessage? status;
        private bool stimulating = false;
        private readonly Timer timer;
    }
}
