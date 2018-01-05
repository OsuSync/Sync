using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Sync.Tools.DefaultI18n;

namespace Sync.Tools
{
    [Obsolete]
    public interface ISyncIO : ISyncOutput, ISyncInput
    {
    }

    public interface ISyncConsoleWriter : ISyncOutput, ISyncInput
    {

    }

    public interface ISyncOutput 
    {
        void Write(string msg, bool newline = true, bool time = true);
        void WriteColor(string text, ConsoleColor color, bool newline = true, bool time = true);
        void WriteHelp(string cmd, string desc);
        void WriteHelp();
        void WriteStatus();
        void WriteWelcome();
        void Clear();
    }

    public interface ISyncInput
    {
        string ReadCommand();
    }

    public sealed class IOWrapper : ISyncConsoleWriter
    {
        private ISyncInput currI;
        private List<ISyncOutput> currOs = new List<ISyncOutput>();

        public void Clear() => currOs.ForEach(p => p.Clear());

        public string ReadCommand() => currI.ReadCommand();

        public void Write(string msg, bool newline = true, bool time = true) => currOs.ForEach(p => p.Write(msg, newline, time));

        public void WriteColor(string text, ConsoleColor color, bool newline = true, bool time = true) => currOs.ForEach(p => p.WriteColor(text, color, newline, time));

        public void WriteHelp(string cmd, string desc) => currOs.ForEach(p => p.WriteHelp(cmd, desc));

        public void WriteHelp() => currOs.ForEach(p => p.WriteHelp());

        public void WriteStatus() => currOs.ForEach(p => p.WriteStatus());

        public void WriteWelcome() => currOs.ForEach(p => p.WriteWelcome());

        internal void SetInput(ISyncInput input)
        {
            this.currI = input;
        }

        internal void AddOutput(ISyncOutput output)
        {
            if (currOs.Contains(output)) return;
            currOs.Add(output);
        }
    }

    public static class IO
    {
        public static readonly NConsoleWriter DefaultIO = new NConsoleWriter();
        public static readonly FileLoggerWriter FileLogger;
        public static readonly IOWrapper CurrentIO = new IOWrapper();

        [Obsolete("Obsoleted, instead with AddOutput and SetInput", true)]
        public static void SetIO(ISyncIO specIO)
        {
            CurrentIO.SetInput(specIO);
            CurrentIO.AddOutput(specIO);
        }
        
        public static void SetIO(ISyncConsoleWriter specIO)
        {
            CurrentIO.SetInput(specIO);
            CurrentIO.AddOutput(specIO);
        }

        static IO()
        {
            SetIO(DefaultIO);
            try
            {
                FileLogger = new FileLoggerWriter();
                AddOutput(FileLogger);
            }
            catch
            {
                DefaultIO.Write("Initial File Logger failed!!");
            }
        }

        public static void AddOutput(ISyncOutput output) => CurrentIO.AddOutput(output);

        public static void SetInput(ISyncInput input) => CurrentIO.SetInput(input);
    }

    public class NConsoleWriter : ISyncConsoleWriter, ISyncOutput, ISyncInput
    {
        private bool wait = false;

        /// <summary>
        /// Read Line
        /// </summary>
        /// <returns>Input chars</returns>
        public string ReadCommand()
        {
            WriteColor("", ConsoleColor.Green, false, false);
            wait = true;
            return Console.ReadLine();
        }
        /// <summary>
        /// Write a message to console
        /// </summary>
        /// <param name="msg">Message</param>
        /// <param name="newline">Display in new line</param>
        public void Write(string msg, bool newline = true, bool time = true)
        {
            if (wait)
            {
                wait = false;
                Console.SetCursorPosition(0, Console.CursorTop);
            }

            string ms = System.Text.RegularExpressions.Regex.Replace(msg, @"\\t|\\n", m =>
            {
                switch (m.ToString())
                {
                    case @"\t": return "\t";
                    case @"\n": return "\n";
                }
                return m.ToString();
            });

            Console.Write((time ? "[" + DateTime.Now.ToLongTimeString() + "] " : "")
               + ms
               + (newline ? "\n" : ""));
        }
        /// <summary>
        /// Write a message with color
        /// </summary>
        /// <param name="text">Message</param>
        /// <param name="color">Color</param>
        /// <param name="newline">Display in new line</param>
        public void WriteColor(string text, ConsoleColor color, bool newline = true, bool time = true)
        {
            Console.ForegroundColor = color;
            Write(text, newline, time);
            Console.ResetColor();
        }
        /// <summary>
        /// Write a formated help message
        /// </summary>
        /// <param name="cmd">命令</param>
        /// <param name="desc">命令描述</param>
        public void WriteHelp(string cmd, string desc)
        {
            WriteColor(cmd.PadRight(10), ConsoleColor.Cyan, false, false);
            WriteColor(desc, ConsoleColor.White, true, false);
        }
        /// <summary>
        /// Write current work status
        /// </summary>
        public void WriteStatus()
        {
            WriteColor(SyncHost.Instance.SourceWrapper.Source?.Status.ToString(), ConsoleColor.Magenta);
            WriteColor(SyncHost.Instance.ClientWrapper.Client?.CurrentStatus.ToString(), ConsoleColor.Magenta);
        }

        /// <summary>
        /// Write welcolme message
        /// </summary>
        public void WriteWelcome()
        {
            Write(string.Format(LANG_Welcome,
                   System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()));

            Write(LANG_Help);
        }
        /// <summary>
        /// Write all commands
        /// </summary>
        public void WriteHelp()
        {
            WriteHelp(LANG_Command, LANG_Command_Description);
            WriteHelp("======", "======");
            foreach (var item in SyncHost.Instance.Commands.Dispatch.getCommandsHelp())
            {
                WriteHelp(item.Key, item.Value);
            }
            WriteHelp("======", "======");
            Write("", true, false);

        }
        /// <summary>
        /// Clear screen
        /// </summary>
        public void Clear()
        {
            Console.Clear();
        }
    }

    public class FileLoggerWriter : ISyncOutput
    {
        StreamWriter logger;
        internal FileLoggerWriter()
        {
            if(Configuration.LoggerFile == "")
            {
                Configuration.LoggerFile = "Log.txt";
            }
            logger = new StreamWriter(File.Open(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Configuration.LoggerFile), FileMode.OpenOrCreate, FileAccess.Write))
            { AutoFlush = true };


        }

        public void Clear()
        {
        }

        public void Write(string msg, bool newline = true, bool time = true)
        {
            string ms = System.Text.RegularExpressions.Regex.Replace(msg, @"\\t|\\n", m =>
            {
                switch (m.ToString())
                {
                    case @"\t": return "\t";
                    case @"\n": return "\n";
                }
                return m.ToString();
            });
            logger.Write((time ? "[" + DateTime.Now.ToLongTimeString() + "] " : "")
               + ms
               + (newline ? "\n" : ""));
        }

        public void WriteColor(string text, ConsoleColor color, bool newline = true, bool time = true)
        {
            Write(text, newline, time);
        }

        public void WriteHelp(string cmd, string desc)
        {
        }

        public void WriteHelp()
        {
        }

        public void WriteStatus()
        {
        }

        public void WriteWelcome()
        {
            Write(string.Format(LANG_Welcome,
                   System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()));

            Write(LANG_Help);
        }
    }
}
