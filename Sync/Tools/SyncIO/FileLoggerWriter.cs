using System;
using System.IO;
using static Sync.Tools.DefaultI18n;

namespace Sync.Tools
{
    public class FileLoggerWriter : ISyncOutput
    {
        private StreamWriter logger;

        internal FileLoggerWriter()
        {
            if (Configuration.Instance.LoggerFile == "")
            {
                Configuration.Instance.LoggerFile = @"Logs\\Log.{Date}.txt";
            }
            string date = $"{DateTime.Now.ToShortDateString()}@{DateTime.Now.ToShortTimeString().Replace(":", "-")}";
            string log = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ((string)Configuration.Instance.LoggerFile).Replace("{Date}", date));
            Directory.CreateDirectory(Path.GetDirectoryName(log));

            logger = new StreamWriter(File.Open(log, FileMode.OpenOrCreate, FileAccess.Write))
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