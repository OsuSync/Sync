using System;
using static Sync.Tools.DefaultI18n;

namespace Sync.Tools
{
    public class NConsoleWriter : ISyncConsoleWriter, ISyncOutput, ISyncInput
    {
        private bool wait = false;

        /// <summary>
        /// Read Line
        /// </summary>
        /// <returns>Input chars</returns>
        public string ReadCommand()
        {
            WriteColor(">", ConsoleColor.Green, false, false);
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
            WriteColor("Source:" + SyncHost.Instance.SourceWrapper.Source?.Status.ToString(), ConsoleColor.Magenta);
            WriteColor("Client:" + SyncHost.Instance.ClientWrapper.Client?.CurrentStatus.ToString(), ConsoleColor.Magenta);
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
}