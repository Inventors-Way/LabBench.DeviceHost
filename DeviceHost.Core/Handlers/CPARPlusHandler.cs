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

                if (msg.StopPressed)
                    latchedButton = true;
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
            "RATING" => Rating(),
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

        private static int PressureToInteger(double pressure) =>
            (int)(pressure * 10);

        private static int RatingToInteger(double vas) =>
            (int)(vas * 10);

        public string Rating()
        {
            lock (lockObject)
            {
                if (status is null)
                    return Response.Error(ErrorCode.NoStatus);

                var response = new Response();
                response.Add("Score", RatingToInteger(status.VasScore));
                response.Add("FinalScore", RatingToInteger(status.FinalVasScore));
                response.Add("Button", status.StopPressed);
                response.Add("LatchedButton", latchedButton);
                latchedButton = false;

                return response.Create();  
            }
        }

        private string Signals()
        {
            lock (lockObject)
            {
                var response = new Response();

                while (statusQueue.Count > 0)
                {
                    var item = statusQueue.Dequeue();
                    response.Add("DATA", new string[]
                    {
                        $"{PressureToInteger(item.ActualPressure01)}",
                        $"{PressureToInteger(item.ActualPressure02)}",
                        $"{RatingToInteger(item.VasScore)}"
                    });
                }

                return response.Create();
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

                var response = new Response()
                    .Add("STATE", status.SystemState)
                    .Add("RESPONSE_CONNECTED", status.VasConnected)
                    .Add("RESPONSE_OK", status.VasConnected && status.VasIsLow)
                    .Add("POWER", status.PowerOn)
                    .Add("START_POSSIBLE", status.StartPossible)
                    .Add("CONDITION", status.Condition)
                    .Add("FINAL_PRESSURE01", PressureToInteger(status.FinalPressure01))
                    .Add("FINAL_PRESSURE02", PressureToInteger(status.FinalPressure02))
                    .Add("SUPPLY_PRESSURE_OK", !status.SupplyPressureLow)
                    .Add("SUPPLY_PRESSURE", PressureToInteger(status.SupplyPressure));

                return response.Create();
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
        private bool latchedButton;
    }
}
