using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Sync.Tools.DefaultI18n;

namespace Sync.Tools
{
    public interface SyncIO
    {
        string ReadCommand();
        void Write(string msg, bool newline = true, bool time = true);
        void WriteColor(string text, ConsoleColor color, bool newline = true, bool time = true);
        void WriteHelp(string cmd, string desc);
        void WriteHelp();
        void WriteStatus();
        void WriteWelcome();
        void Clear();
    }

    public class IO
    {
        public static readonly NConsoleWriter DefaultIO = new NConsoleWriter();
        public static SyncIO CurrentIO { get { return currIO; } private set { currIO = value; } }
        private static SyncIO currIO = DefaultIO;
        public static void SetIO(SyncIO specIO)
        {
            CurrentIO = specIO;
        }
    }

    public class NConsoleWriter : SyncIO
    {
        private bool wait = false;

        /// <summary>
        /// 等待用户输入一个命令
        /// </summary>
        /// <returns>输入的字符串</returns>
        public string ReadCommand()
        {
            WriteColor("", ConsoleColor.Green, false, false);
            wait = true;
            return Console.ReadLine();
        }
        /// <summary>
        /// 向控制台输出信息
        /// </summary>
        /// <param name="msg">信息</param>
        /// <param name="newline">是否换行</param>
        public void Write(string msg, bool newline = true, bool time = true)
        {
            if (wait)
            {
                wait = false;
                Console.SetCursorPosition(0, Console.CursorTop);
            }
            Console.Write((time ? "[" + DateTime.Now.ToLongTimeString() + "] " : "")
               + msg
               + (newline ? "\n" : ""));
        }
        /// <summary>
        /// 向控制台输出带颜色的信息
        /// </summary>
        /// <param name="text">信息文本</param>
        /// <param name="color">颜色</param>
        /// <param name="newline">是否换行</param>
        public void WriteColor(string text, ConsoleColor color, bool newline = true, bool time = true)
        {
            Console.ForegroundColor = color;
            Write(text, newline, time);
            Console.ResetColor();
        }
        /// <summary>
        /// 格式化帮助信息
        /// </summary>
        /// <param name="cmd">命令</param>
        /// <param name="desc">命令描述</param>
        public void WriteHelp(string cmd, string desc)
        {
            WriteColor(cmd.PadRight(10), ConsoleColor.Cyan, false, false);
            WriteColor(desc, ConsoleColor.White, true, false);
        }
        /// <summary>
        /// 向屏幕输出某个Sync实例的状态
        /// </summary>
        /// <param name="instance">指定Sync实例</param>
        public void WriteStatus()
        {
            WriteColor(SyncHost.Instance.SourceWrapper.Source?.Status.ToString(), ConsoleColor.Magenta);
            WriteColor(SyncHost.Instance.ClientWrapper.Client?.CurrentStatus.ToString(), ConsoleColor.Magenta);
        }

        /// <summary>
        /// 向屏幕输出欢迎信息
        /// </summary>
        public void WriteWelcome()
        {
            Write(string.Format(LANG_Welcome,
                   System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()));

            Write(LANG_Help);
        }
        /// <summary>
        /// 输出帮助信息
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
        /// 清空屏幕
        /// </summary>
        public void Clear()
        {
            Console.Clear();
        }
    }
}
