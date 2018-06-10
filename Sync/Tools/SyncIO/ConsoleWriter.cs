using System;

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
}