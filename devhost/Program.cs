using DeviceHost.Core;

namespace devhost
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            using var server = new DeviceServer();
            Console.WriteLine("Starting server");
            await server.Run();
        }
    }
}
