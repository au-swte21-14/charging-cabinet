#nullable enable
using System.IO;
using Ladeskab.Interfaces;

namespace Ladeskab
{
    public class Logger : ILogger
    {
        public void WriteLine(string format, params object?[] arg)
        {
            using var writer = File.AppendText("log.txt");
            writer.WriteLine(format, arg);
        }
    }
}