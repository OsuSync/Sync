using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Tools
{
    public enum LogType
    {
        Infomation,
        Warning,
        Error,
    }

    public class Logger
    {
        static ConsoleColor[] LOGTYPE_COLORS = new[]
        {
            ConsoleColor.Green,
            ConsoleColor.Yellow,
            ConsoleColor.Red
        };

        private readonly string prefix;
        private readonly ISyncOutput output;

        public Logger(string prefix, ISyncOutput output = null)
        {
            this.prefix = prefix;
            this.output = output ?? IO.CurrentIO;
        }

        public void Log(string message, LogType type) => output?.WriteColor($"[{prefix}]{message}", LOGTYPE_COLORS[(int)type]);
        public void LogInfomation(string message) => Log(message, LogType.Infomation);
        public void LogWarning(string message) => Log(message, LogType.Warning);
        public void LogError(string message) => Log(message, LogType.Error);
    }

    public class Logger<T> : Logger
    {
        public Logger(ISyncOutput output=null) : base(typeof(T).Name, output)
        {

        }
    }
}
