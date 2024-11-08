﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Net.Sockets;
using DeviceHost.Core;
using DeviceHost.Testing.Mocks;

namespace DeviceHost.Testing
{
    public class TestUtility
    {
        private readonly static string serverIp = "127.0.0.1"; // IP address of the server
        private readonly static int serverPort = 9797;        // Port of the server

        public static DeviceServerMock CreateDeviceServerMock() => 
            new DeviceServerMock()
                .Add("COM8", new DeviceHandlerMock());
        

        public static string GetScript(string name)
        {
            if (Assembly.GetExecutingAssembly().Location is not string assemblyFileName)
                throw new InvalidOperationException("Invalid path to assembly");

            if (Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) is not string assemblyPath)
                throw new InvalidOperationException("Invalid path to assembly");

            DirectoryInfo dirInfo = new DirectoryInfo(assemblyPath);
            
            if (dirInfo.Parent?.Parent?.Parent?.ToString() is not string basePath)
                throw new InvalidOperationException("Invalid path to assembly");

            var scriptPath = Path.Combine(basePath, "Scripts");
            var scriptFilePath = Path.Combine(scriptPath, name);

            return File.ReadAllText(scriptFilePath);
        }

        public static string GetPacket(string name, string port)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"{Response.STX};");
            builder.Append(String.Format(GetScript(name), port));
            builder.AppendLine($"{Response.ETX};");
            return builder.ToString();
        }

        public static string GetPackets(IEnumerable<string> names, string port)
        {
            var builder = new StringBuilder();

            foreach (var name in names)
            {
                builder.AppendLine($"{Response.STX};");
                builder.Append(String.Format(GetScript(name), port));
                builder.AppendLine($"{Response.ETX};");
            }

            return builder.ToString();
        }

        public static string Send(string command)
        {
            using TcpClient client = new(serverIp, serverPort);
            using NetworkStream stream = client.GetStream();

            // Send a message to the server
            byte[] dataToSend = Encoding.UTF8.GetBytes(command);
            stream.Write(dataToSend, 0, dataToSend.Length);

            // Receive the response from the server
            byte[] dataToReceive = new byte[1024];
            int bytesRead = stream.Read(dataToReceive, 0, dataToReceive.Length);
            string response = Encoding.UTF8.GetString(dataToReceive, 0, bytesRead);

            return response;
        }
    }
}
