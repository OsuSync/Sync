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
            if (DefaultConfiguration.Instance.LogDirectory == "")
            {
                DefaultConfiguration.Instance.LogDirectory = @"Logs\\";
            }

            if(DefaultConfiguration.Instance.LogFilename == "")
            {
                DefaultConfiguration.Instance.LogFilename = "Log-{Date}.txt";
            }

            string date = $"{DateTime.Now.ToString().Replace(" ","@").Replace(":", ".")}";
            string log = Path.Combine(DefaultConfiguration.Instance.LogDirectory, DefaultConfiguration.Instance.LogFilename).Replace("{Date}", date);
            if (!Path.IsPathRooted(log))
                log = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,log);
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