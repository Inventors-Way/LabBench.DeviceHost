using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace DeviceHost.Testing
{
    public class TestUtility
    {
        public static string? GetScript(string name)
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
    }
}
