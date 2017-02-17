using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Tools
{
    public interface SyncIO
    {
        string ReadCommand();
        void Write(string msg, bool newline = true, bool time = true);
        void WriteColor(string text, ConsoleColor color, bool newline = true, bool time = true);
        void WriteHelp(string cmd, string desc);
        void WriteHelp();
        void WriteConfig();
        void WriteStatus(SyncConnector instance);
        void WriteWelcome();
        void Clear();
    }

    public class IO
    {
        public static SyncIO CurrentIO { get { return currIO; } private set { currIO = value; } }
        private static SyncIO currIO = new NConsoleWriter();
        public static void SetIO(SyncIO specIO)
        {
            CurrentIO = specIO;
        }
    }

    public class NConsoleWriter : SyncIO
    {
        private static bool wait = false;

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
        public void WriteStatus(SyncConnector instance)
        {
            WriteColor("配置文件: ", ConsoleColor.Blue, false);
            if (Configuration.LiveRoomID.Length > 0 && Configuration.TargetIRC.Length > 0 && Configuration.BotIRC.Length > 0 && Configuration.BotIRCPassword.Length > 0)
                WriteColor("OK, 房间ID:" + Configuration.LiveRoomID, ConsoleColor.Green, true, false);
            else
                WriteColor("未配置", ConsoleColor.Red, true, false);

            WriteColor("BiliBili Live连接: ", ConsoleColor.Blue, false);
            if (instance.SourceStatus)
                WriteColor("已连接", ConsoleColor.Green, true, false);
            else
                WriteColor("等待连接", ConsoleColor.Red, true, false);

            WriteColor("osu! IRC(聊天): ", ConsoleColor.Blue, false);
            if (instance.IRCStatus)
                WriteColor("已连接", ConsoleColor.Green, true, false);
            else
                WriteColor("等待连接", ConsoleColor.Red, true, false);

            if (SyncManager.loginable)
            {
                WriteColor("发送弹幕: ", ConsoleColor.Blue, false);
                if (((Source.ISendable)instance.GetSource()).LoginStauts())
                    WriteColor("已登录", ConsoleColor.Green, true, false);
                else
                    WriteColor("未连接", ConsoleColor.Red, true, false);
            }
        }
        /// <summary>
        /// 向屏幕输出配置文件状态
        /// </summary>
        public void WriteConfig()
        {
            Write("正在读取配置文件....\n");
            Write("房间ID: \t\t" + Configuration.LiveRoomID);
            Write("主号IRC: \t\t" + Configuration.TargetIRC);
            Write("BotIRC: \t\t" + Configuration.BotIRC);
            Write("BotIRC密码长度: \t" + Configuration.BotIRCPassword.Length);
            Write("完成.\n");
        }
        /// <summary>
        /// 向屏幕输出欢迎信息
        /// </summary>
        public void WriteWelcome()
        {
            Write("欢迎使用 osu直播弹幕同步工具 ver " +
                   System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());

            Write("输入 'help' 获得帮助列表\n\n");
        }
        /// <summary>
        /// 输出帮助信息
        /// </summary>
        public void WriteHelp()
        {
            WriteHelp("命令", "描述");
            WriteHelp("======", "======");
            foreach (var item in Program.host.Commands.Dispatch.getCommandsHelp())
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

    /// <summary>
    /// 控制台帮助类(旧)
    /// </summary>
    [Obsolete("[已弃用]请使用IO类替代")]
    public class ConsoleWriter
    {
        private static bool wait = false;

        /// <summary>
        /// 等待用户输入一个命令
        /// </summary>
        /// <returns>输入的字符串</returns>
        public static string ReadCommand()
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
        public static void Write(string msg, bool newline = true, bool time = true)
        {
            if(wait)
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
        public static void WriteColor(string text, ConsoleColor color, bool newline = true, bool time = true)
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
        public static void WriteHelp(string cmd, string desc)
        {
            WriteColor(cmd.PadRight(10), ConsoleColor.Cyan, false, false);
            WriteColor(desc, ConsoleColor.White, true, false);
        }
        /// <summary>
        /// 向屏幕输出某个Sync实例的状态
        /// </summary>
        /// <param name="instance">指定Sync实例</param>
        public static void WriteStatus(SyncConnector instance)
        {
            WriteColor("配置文件: ", ConsoleColor.Blue, false);
            if (Configuration.LiveRoomID.Length > 0 && Configuration.TargetIRC.Length > 0 && Configuration.BotIRC.Length > 0 && Configuration.BotIRCPassword.Length > 0)
                WriteColor("OK, 房间ID:" + Configuration.LiveRoomID, ConsoleColor.Green, true, false);
            else
                WriteColor("未配置", ConsoleColor.Red, true, false);

            WriteColor("BiliBili Live连接: ", ConsoleColor.Blue, false);
            if (instance.SourceStatus)
                WriteColor("已连接", ConsoleColor.Green, true, false);
            else
                WriteColor("等待连接", ConsoleColor.Red, true, false);

            WriteColor("osu! IRC(聊天): ", ConsoleColor.Blue, false);
            if (instance.IRCStatus)
                WriteColor("已连接", ConsoleColor.Green, true, false);
            else
                WriteColor("等待连接", ConsoleColor.Red, true, false);

            if(SyncManager.loginable)
            {
                WriteColor("发送弹幕: ", ConsoleColor.Blue, false);
                if (((Source.ISendable)instance.GetSource()).LoginStauts())
                    WriteColor("已登录", ConsoleColor.Green, true, false);
                else
                    WriteColor("未连接", ConsoleColor.Red, true, false);
            }
        }
        /// <summary>
        /// 向屏幕输出配置文件状态
        /// </summary>
        public static void WriteConfig()
        {
            Write("正在读取配置文件....\n");
            Write("房间ID: \t\t" + Configuration.LiveRoomID);
            Write("主号IRC: \t\t" + Configuration.TargetIRC);
            Write("BotIRC: \t\t" + Configuration.BotIRC);
            Write("BotIRC密码长度: \t" + Configuration.BotIRCPassword.Length);
            Write("完成.\n");
        }
        /// <summary>
        /// 向屏幕输出欢迎信息
        /// </summary>
        public static void WriteWelcome()
        {
            Write("欢迎使用 osu直播弹幕同步工具 ver " +
                   System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());

            Write("输入 'help' 获得帮助列表\n\n");
        }
        /// <summary>
        /// 输出帮助信息
        /// </summary>
        public static void WriteHelp()
        {
            WriteHelp("命令", "描述");
            WriteHelp("======", "======");
            foreach (var item in Program.host.Commands.Dispatch.getCommandsHelp())
            {
                WriteHelp(item.Key, item.Value);
            }
            WriteHelp("======", "======");
            Write("", true, false);

        }
        /// <summary>
        /// 清空屏幕
        /// </summary>
        public static void Clear()
        {
            Console.Clear();
        }
    }
}
