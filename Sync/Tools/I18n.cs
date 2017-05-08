using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Tools
{
    public class DefaultI18n : I18nProvider
    {
        public static LanguageElement LANG_Loading = "读取中....";
        public static LanguageElement LANG_Plugins = "已载入 {0:D} 个 插件";
        public static LanguageElement LANG_Sources = "已载入 {0:D} 个 直播源";
        public static LanguageElement LANG_Error = "不能初始化连接器，请确认是否已经安装直播源.";
        public static LanguageElement LANG_Commands = "已载入 {0:D} 个 命令";
        public static LanguageElement LANG_Filters = "已载入 {0:D} 个 过滤器";
        public static LanguageElement LANG_Ready = "准备就绪。";

        public static LanguageElement LANG_Start = "开始工作....";
        public static LanguageElement LANG_Stopping = "停止工作...";
        public static LanguageElement LANG_Restarting = "重新开始工作...";

        public static LanguageElement LANG_LoadingPlugin = "载入 {0:S} 中...";
        public static LanguageElement LANG_LoadPluginErr = "不能载入 {0:S} ({1:S})";
        public static LanguageElement LANG_NotPluginErr = "插件{0:S} 非osuSync插件 ({1:S})";

        public static LanguageElement LANG_NotConfig = "请配置 'config.ini' 后再开始进行同步操作。";
        public static LanguageElement LANG_NoSource = "无法找到任何直播源！请安装一个直播源。";
        public static LanguageElement LANG_MissSource = "找不到默认匹配的直播源，直接使用第一个。";
        public static LanguageElement LANG_SetSource = "设置 {0:S} 为直播弹幕源";
        public static LanguageElement LANG_SupportSend = "提示:当前弹幕源支持游戏内发送到弹幕源的功能，请输入login [用户名] [密码] 来登录!(用户名、密码二者可选输入)";
        public static LanguageElement LANG_CertLength = "Certification长度: {0:D}";
        public static LanguageElement LANG_CertExist = "提示：当前已有登录Certification记录，如需覆盖，请输入login [用户名] [密码]进行覆盖！（用户名密码可选输入）";

        public static LanguageElement LANG_UnknowCommand = "未知命令！ 请输入help查看命令列表。";
        public static LanguageElement LANG_CommandFail = "命令执行失败！ 请输入help查看命令列表。";

        public static LanguageElement LANG_ConfigFile = "配置文件";

        public static LanguageElement LANG_UserCount = "用户总数变更: {0:D}";
        public static LanguageElement LANG_UserCount_Change = "直播间围观人数{0:S}到{1:D}人";

        public static LanguageElement LANG_Source_Disconnecting = "正在断开弹幕源服务器的连接....";
        public static LanguageElement LANG_Source_Disconnected = "服务器连接被断开，3秒后重连！";
        public static LanguageElement LANG_Source_Disconnected_Succ = "源服务器断开连接成功！";
        public static LanguageElement LANG_Source_Connect = "正在连接弹幕源服务器....";
        public static LanguageElement LANG_Source_Connected_Succ = "源服务器连接成功！";

        public static LanguageElement LANG_IRC_Connecting = "[IRC] osu! IRC正在连接中...";
        public static LanguageElement LANG_IRC_Disconnect = "正在断开IRC服务器的连接....";

        public static LanguageElement LANG_IRC_Connect_Timeout = @"osu! IRC连接错误，无法连接到老板小霸王服务器 !!\n请稍后重试或者开一个VPN。";
        public static LanguageElement LANG_IRC_Ready = "[IRC] osu! IRC 已经准备就绪!";

        public static LanguageElement LANG_Current_Online = "当前在线人数: {0:D}";
        public static LanguageElement LANG_Gift_Sent = "我送给你{O:D}份{1:S}!";

        public static LanguageElement LANG_Config = "配置文件: ";
        public static LanguageElement LANG_Config_Status_OK = "OK, 房间ID:";
        public static LanguageElement LANG_Config_Status_Fail = "尚未配置成功";

        public static LanguageElement LANG_Source = "源{0:S}: ";
        public static LanguageElement LANG_IRC = "osu! IRC(聊天):";
        public static LanguageElement LANG_Danmaku = "弹幕发送:";
        public static LanguageElement LANG_Status_Connected = "已连接";
        public static LanguageElement LANG_Status_NotConenct = "未连接";

        public static LanguageElement LANG_Loading_Config = @"正在读取配置文件....\n";
        public static LanguageElement LANG_Config_RoomID = @"房间ID: \t\t";
        public static LanguageElement LANG_Config_osuID = @"主号IRC: \t\t";
        public static LanguageElement LANG_Config_BotID = @"BotIRC: \t\t";
        public static LanguageElement LANG_Config_BotPassLen = @"BotIRC密码长度: \t";

        public static LanguageElement LANG_Welcome = "欢迎使用 osu直播弹幕同步工具 ver {0:S} ";
        public static LanguageElement LANG_Help = @"输入 'help' 获得帮助列表\n\n";
        public static LanguageElement LANG_Command = "命令";
        public static LanguageElement LANG_Command_Description = "描述";

    }


    public interface I18nProvider
    {
    }

    public class LanguageElement
    {
        private string value;

        public LanguageElement(string defaultVal)
        {
            value = defaultVal;
        }

        public static implicit operator LanguageElement(string val)
        {
            return new LanguageElement(val);
        }

        public static implicit operator string(LanguageElement element)
        {
            return element.value;
        }
    }
    /// <summary>
    /// 特定语言的I18n实现
    /// </summary>
    class I18n
    {
        public static string CurrentLang { get => System.Globalization.CultureInfo.CurrentCulture.Name; }
        string Base { get => AppDomain.CurrentDomain.BaseDirectory; }
        string LangFolder { get => Path.Combine(Base, "Language"); }
        string SelectLangFolder {  get => Path.Combine(LangFolder, SelectLang); }
        public string SelectLang;

        /// <summary>
        ///  实例化一个特定区域语言的I18n实例
        /// </summary>
        /// <param name="CultureName">指定区域</param>
        public I18n(string CultureName)
        {
            SelectLang = CultureName;
            if (!Directory.Exists(LangFolder)) Directory.CreateDirectory(LangFolder);
            if (!Directory.Exists(SelectLangFolder)) Directory.CreateDirectory(SelectLangFolder);
        }

        public void ApplyLanguage<T>(T instance) where T : I18nProvider
        {
            string LangFile = Path.Combine(SelectLangFolder, instance.ToString()) + ".lang";
            foreach (FieldInfo item in typeof(T).GetFields())
            {
                if(item.FieldType.Equals(typeof(LanguageElement)))
                {
                    string value = ConfigurationIO.IniReadValue(LangFile, item.Name, SelectLang);
                    if (value == "")
                    {
                        value = item.GetValue(instance) as LanguageElement;
                        ConfigurationIO.Write(LangFile, item.Name, value, SelectLang);
                    }
                    item.SetValue(instance, new LanguageElement(value));
                }
            }
        }


    }
}
