using System;
using System.Collections.Generic;

namespace Sync.Tools
{
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
}