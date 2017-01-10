using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Tools
{
    /// <summary>
    /// 控制台帮助类
    /// </summary>
    static class ConsoleWriter
    {
        /// <summary>
        /// 等待用户输入一个命令
        /// </summary>
        /// <returns>输入的字符串</returns>
        public static string ReadCommand()
        {
            WriteColor(">>", ConsoleColor.Green, false, false);
            return Console.ReadLine();
        }
        /// <summary>
        /// 向控制台输出信息
        /// </summary>
        /// <param name="msg">信息</param>
        /// <param name="newline">是否换行</param>
        public static void Write(string msg, bool newline = true, bool time = true)
        {
            Console.Write((time ? "[" + DateTime.Now.ToLongTimeString() + "] " : "") 
                           +  msg 
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
            WriteColor(cmd, ConsoleColor.Green, false, false);
            Write(" : ", false, false);
            WriteColor(desc, ConsoleColor.Blue, true, false);
        }
        /// <summary>
        /// 向屏幕输出某个Sync实例的状态
        /// </summary>
        /// <param name="instance">指定Sync实例</param>
        public static void WriteStatus(Sync instance)
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

            if(Program.loginable)
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
            Write("BiliBili房间ID: " + Configuration.LiveRoomID);
            Write("主号IRC: " + Configuration.TargetIRC);
            Write("机器人IRC: " + Configuration.BotIRC);
            Write("机器人IRC密码长度: " + Configuration.BotIRCPassword.Length);
            Write("完成.\n");
        }

        /// <summary>
        /// 向屏幕输出欢迎信息
        /// </summary>
        public static void WriteWelcome()
        {
            Write("osu直播弹幕同步工具 ver " +
                   System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()
                   + " 载入中..");

            Write("输入 'help' 获得帮助列表\n\n");
        }

        /// <summary>
        /// 输出帮助信息
        /// </summary>
        public static void WriteHelp()
        {
            WriteHelp("exit", "停止弹幕同步，并退出软件");
            WriteHelp("start", "开始工作");
            WriteHelp("stop", "停止工作");
            WriteHelp("chat <message>", "发送消息到osu! （用于检测连接是否出问题）");
            WriteHelp("danmaku <message>", "向直播发送弹幕（仅在支持的时候可用）。");
            WriteHelp("status", "输出当前连接状态。");

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
