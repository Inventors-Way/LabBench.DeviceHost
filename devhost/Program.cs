using CommandLine;
using DeviceHost.Core;
using Serilog;
using System.Net;

namespace devhost
{
    internal class Program
    {
        public class Options
        {
            [Option('a', "address", Required = false, HelpText = "The address on which to host the device host (default: 127.0.0.1")]
            public string Address { get; set; } = "127.0.0.1";

            [Option('p', "port", Required = false, HelpText = "The port on which to bind the device hort (default: 9797)")]
            public int Port { get; set; } = 9797;

            [Option('l', "log-file", Required = false, HelpText = "Path to log file. If empty logging to a file is disabled (default: empty)")]
            public string LogFile { get; set; } = string.Empty;
        }

        static void ConfigureLogging(Options options) 
        {
            if (string.IsNullOrEmpty(options.LogFile))
            {
                Log.Logger = new LoggerConfiguration()           
                    .MinimumLevel.Debug()
                    .WriteTo.Console()
                    .CreateLogger();
                
                Log.Information("Configuration logging: Console");
            }
            else
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console()
                    .WriteTo.File(options.LogFile, rollingInterval: RollingInterval.Day)
                    .CreateLogger();
                Log.Information("Configuration logging: Console, File ({LogFile})", options.LogFile);
            }
        }

        static async Task Main(string[] args)
        {
            try
            {
                var result = Parser.Default.ParseArguments<Options>(args);

                if (result.Value is Options options)
                {
                    ConfigureLogging(options);
                    Log.Information("Close program by pressing Ctrl + E");
                    Log.Information("Starting server (address: {adress}, port: {port})", options.Address, options.Port);

                    using var server = new DeviceServer()
                    {
                        Address = IPAddress.Parse(options.Address),
                        Port = options.Port
                    };
                    var tokenSource = server.Start();

                    while (true)
                    {
                        if (Console.KeyAvailable)
                        {
                            var key = Console.ReadKey(false);

                            if ((key.Key == ConsoleKey.E) && (key.Modifiers == ConsoleModifiers.Control))
                            {
                                tokenSource.Cancel();
                                break;
                            }
                        }
                    }

                    await server.Join();
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error: {exception}", ex);
            }
        }
    }
}
