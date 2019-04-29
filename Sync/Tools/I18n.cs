using Sync.Tools.ConfigurationAttribute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Sync.Tools
{
    public class DefaultI18n : I18nProvider
    {
        public static LanguageElement LANG_Loading = "读取中....";
        public static LanguageElement LANG_Plugins = "已载入 {0:D} 个 插件";
        public static LanguageElement LANG_Sources = "已载入 {0:D} 个 直播源";
        public static LanguageElement LANG_Client = "已载入 {0:D} 个 Client";
        public static LanguageElement LANG_Error = "不能初始化连接器，请确认是否已经安装直播源.";
        public static LanguageElement LANG_Commands = "已载入 {0:D} 个 命令";
        public static LanguageElement LANG_Filters = "已载入 {0:D} 个 过滤器";
        public static LanguageElement LANG_Ready = "准备就绪。";

        public static LanguageElement LANG_RqueireLogin = "请登录到RnW账号";
        public static LanguageElement LANG_AccountName = "用户名:";
        public static LanguageElement LANG_AccountPw = "密码:";
        public static LanguageElement LANG_AccountSave = "账户保存成功! 将开始连接到服务器";

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
        public static LanguageElement LANG_SendNotReady = "当前Client未标志弹幕发送可用，请尝试使用Login登录";

        public static LanguageElement LANG_UnknowCommand = "未知命令！ 请输入help查看命令列表。";
        public static LanguageElement LANG_CommandFail = "命令执行失败！ 请输入help查看命令列表。";

        public static LanguageElement LANG_ConfigFile = "配置文件";

        public static LanguageElement LANG_UserCount = "用户总数变更: {0:D}";
        public static LanguageElement LANG_UserCount_Change = "直播间围观人数{0:S}到{1:D}人";
        public static LanguageElement LANG_UserCount_Change_Increase = "增加";
        public static LanguageElement LANG_UserCount_Change_Decrease = "减少";

        public static LanguageElement LANG_Source_Disconnecting = "正在断开弹幕源服务器的连接....";
        public static LanguageElement LANG_Source_Disconnected = "服务器连接被断开，3秒后重连！";
        public static LanguageElement LANG_Source_Disconnected_Succ = "源服务器断开连接成功！";
        public static LanguageElement LANG_Source_Connect = "正在连接弹幕源服务器....";
        public static LanguageElement LANG_Source_Connected_Succ = "源服务器连接成功！";

        public static LanguageElement LANG_Current_Online = "当前在线人数: {0:D}";
        public static LanguageElement LANG_Gift_Sent = "我送给你{O:D}份{1:S}!";

        public static LanguageElement LANG_Config = "配置文件: ";
        public static LanguageElement LANG_Config_Status_OK = "OK, 房间ID:{0}";
        public static LanguageElement LANG_Config_Status_Fail = "尚未配置成功";

        public static LanguageElement LANG_Source = "源{0:S}: ";
        public static LanguageElement LANG_IRC = "Client:";
        public static LanguageElement LANG_Danmaku = "弹幕发送:";
        public static LanguageElement LANG_Status_Connected = "已连接";
        public static LanguageElement LANG_Status_NotConenct = "未连接";

        public static LanguageElement LANG_Loading_Config = @"正在读取配置文件....\n";

        public static LanguageElement LANG_Welcome = "欢迎使用 osu直播弹幕同步工具 ver {0:S} ";
        public static LanguageElement LANG_Help = @"输入 'help' 获得帮助列表\n\n";
        public static LanguageElement LANG_Command = "命令";
        public static LanguageElement LANG_Command_Description = "描述";

        public static LanguageElement LANG_MsgMgr_Limit = "当前消息管理器 开始 管制，只有?send命令的内容才会发送到irc频道";
        public static LanguageElement LANG_MsgMgr_Free = "当前消息管理器 解除 管制,内容可以直接发送到irc频道";
        public static LanguageElement LANG_Plugin_Cycle_Reference = "发现插件之间的循环引用关系，插件 {0:S} 将不会按照开发者指定的依赖关系进行加载";


        //from default plugin
        public static LanguageElement LANG_COMMANDS_LOGIN = "login <user> [pass] 登录到目标弹幕网站，启动弹幕发送功能";
        public static LanguageElement LANG_COMMANDS_EXIT = "退出软件";
        public static LanguageElement LANG_COMMANDS_CLEAR = "清空屏幕";
        public static LanguageElement LANG_COMMANDS_STATUS = "获得当前连接状态属性";
        public static LanguageElement LANG_COMMANDS_STOP = "停止当前连接";
        public static LanguageElement LANG_COMMANDS_START = "开始同步";
        public static LanguageElement LANG_COMMANDS_HELP = "打印帮助信息";
        public static LanguageElement LANG_COMMANDS_SOURCEMSG = "danmaku <message> 发送弹幕测试";
        public static LanguageElement LANG_COMMANDS_CLIENTMSG = "chat <message> 发送IRC信息测试";
        public static LanguageElement LANG_COMMANDS_CLIENTUSERMSG = "chatuser <username> <message> 按照username名字发送IRC信息测试";
        public static LanguageElement LANG_COMMANDS_EXIT_DONE = "退出操作已完成，如果窗口还未关闭，您可以强制关闭。";
        public static LanguageElement LANG_COMMANDS_SOURCES = "获得当前所有弹幕源列表";
        public static LanguageElement LANG_COMMANDS_MSGMGR = "查看或者设置消息控制器相关内容,添加--help参数获取帮助";
        public static LanguageElement LANG_COMMANDS_FILTERS = "列表所有当前可用消息过滤器";
        public static LanguageElement LANG_COMMANDS_DISABLE = "向插件发送禁用消息 disable (插件名称)";
        public static LanguageElement LANG_COMMANDS_SWITCH_CLIENT = "切换到指定Client实例，不带名称则为获取Client列表";
        public static LanguageElement LANG_COMMANDS_SOURCELOGIN = "登录到弹幕源 sourcelogin [用户名] [密码]";
        public static LanguageElement LANG_COMMANDS_RESTART = "重新启动应用程序";
        public static LanguageElement LANG_COMMANDS_LANG = "lang [cultureName] Get/Set language";
        public static LanguageElement LANG_COMMANDS_LISTLANG = "listlang [--all] List (supported/all) languages";
        public static LanguageElement LANG_COMMANDS_FILTERS_ITEM = "过滤项";
        public static LanguageElement LANG_COMMANDS_FILTERS_OBJ = "过滤器";
        public static LanguageElement LANG_COMMANDS_CLIENT_NAME = "Client";
        public static LanguageElement LANG_COMMANDS_CLIENT_AUTHOR = "作者";
        public static LanguageElement LANG_COMMANDS_SOURCES_NAME = "弹幕源";
        public static LanguageElement LANG_COMMANDS_SOURCES_AUTHOR = "作者";
        public static LanguageElement LANG_COMMANDS_CURRENT = "当前设置为 {0:S}";
        public static LanguageElement LANG_COMMANDS_DANMAKU_NOT_SUPPORT = @"提示：当前弹幕源不支持发送弹幕，请更换弹幕源！\n";
        public static LanguageElement LANG_COMMANDS_CHAT_IRC_NOTCONNECT = "osu! irc 尚未连接，您还不能发送消息。";
        public static LanguageElement LANG_COMMANDS_DANMAKU_REQUIRE_LOGIN = "你必须登录才能发送弹幕!";
        public static LanguageElement LANG_COMMANDS_START_ALREADY_RUN = "同步实例已经在运行。";
        public static LanguageElement LANG_COMMANDS_ARGUMENT_WRONG = "参数不正确";
        public static LanguageElement LANG_COMMANDS_MSGMGR_HELP = @"\n--status :查看当前消息管理器的信息\n--limit <数值> :是设置限制发送信息的等级，越低就越容易触发管控\n--option <名称> :是设置管控的方式，其中Auto是自动管控，ForceAll强行全都发送,ForceLimit是仅发送使用?send命令的消息,DisableAll是拦截任何管道内的信息";
        public static LanguageElement LANG_COMMANDS_MSGMGR_LIMIT = "限制中...";
        public static LanguageElement LANG_COMMANDS_MSGMGR_FREE = "无限制";
        public static LanguageElement LANG_COMMANDS_MSGMGR_STATUS = "MessageManager mode:{4:S},status:{0:D},queueCount/limitCount/recoverTime:{1}/{2}/{3}";
        public static LanguageElement LANG_COMMANDS_MSGMGR_LIMIT_SPEED_SET = "设置限制发送速度等级为{0}";
        public static LanguageElement LANG_COMMANDS_MSGMGR_LIMIT_STYPE_SET = "设置消息管理器的管制方式为{0}";

        public static LanguageElement LANG_COMMANDS_START_NO_SOURCE = "还未钦定任何一个接收源";
        public static LanguageElement LANG_COMMANDS_START_NO_CLIENT = "还未钦定任何一个发送源";
        public static LanguageElement LANG_COMMANDS_CURRENT_LANG = "当前语言: {0:S}\t{1:S}";
        public static LanguageElement LANG_COMMANDS_LANG_SWITCHED = "成功切换语言至 {1:S}({0:S})";
        public static LanguageElement LANG_COMMANDS_LANG_NOT_FOUND = "切换语言失败,请检查语言代码参数是否正确";

        public static LanguageElement LANG_UPDATE_DONE = "更新完成,是否重启软件";
        public static LanguageElement LANG_INSTALL_DONE = "下载完成,是否重启软件";
        public static LanguageElement LANG_PLUGIN_NOT_FOUND = "插件 {0} 不存在";
        public static LanguageElement LANG_REMOVE_DONE = "删除成功,是否重启软件";
        public static LanguageElement LANG_VERSION_LATEST = "{0} 已是最新";
        public static LanguageElement LANG_UPDATE_CHECK_ERROR = "无法根据 [{0}] 检查更新 :  {1} : {2}";
        public static LanguageElement LANG_UPDATE_ERROR = "无法更新 :  {0} : {1}";

        public static LanguageElement LANG_SOURCE_NOT_SUPPORT_SEND = "接收源 {0} 并不支持发送功能";
        public static LanguageElement LANG_NO_PLUGIN_SELECT = "还未钦定插件名称";
        public static LanguageElement LANG_PLUGIN_DISABLED = "已禁用 ";

        public static LanguageElement LANG_NO_ANY_SOURCE = "没有任何弹幕接收源,请检查Plugins目录或使用\"plugins install DefaultPlugin\"来安装默认插件";
        public static LanguageElement LANG_Instance_Exist = "只能存在一个Sync进程，等待上一个Sync结束";
    }

    public interface I18nProvider
    {
    }

    public struct LanguageElement
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

        public override string ToString()
        {
            return value;
        }
    }

    /// <summary>
    /// I18n Manager
    /// </summary>
    public class I18n
    {
        public static string CurrentSystemLang { get => System.Globalization.CultureInfo.CurrentCulture.Name; }
        private string Base { get => AppDomain.CurrentDomain.BaseDirectory; }
        public string LangFolder { get => Path.Combine(Base, "Language"); }
        public string SelectLangFolder { get => Path.Combine(LangFolder, CurrentLanguage); }
        public string CurrentLanguage;

        private static I18n instance;

        private static List<I18nProvider> ApplyedProvider = new List<I18nProvider>();

        public static I18n Instance
        {
            get
            {
                if (instance == null)
                {
                    if (DefaultConfiguration.Instance.Language == DefaultConfiguration.DEFAULT_LANGUAGE || DefaultConfiguration.Instance.Language.ToString().Length == 0)
                    {
                        instance = new I18n(CurrentSystemLang);
                        DefaultConfiguration.Instance.Language = CurrentSystemLang;
                    }
                    else
                    {
                        instance = new I18n(DefaultConfiguration.Instance.Language);
                    }
                }
                return instance;
            }
            private set
            {
                instance = value;
            }
        }

        public static void SwitchToCulture(string CultureName)
        {
            Instance = new I18n(CultureName);
            foreach (var item in ApplyedProvider)
            {
                Instance.ApplyLanguage(item);
            }
        }

        private I18n()
        {
        }

        /// <summary>
        ///  Constructor for initial one language
        /// </summary>
        /// <param name="CultureName">Cultura name</param>
        private I18n(string CultureName)
        {
            CurrentLanguage = CultureName;
            if (!Directory.Exists(LangFolder)) Directory.CreateDirectory(LangFolder);
            if (!Directory.Exists(SelectLangFolder)) Directory.CreateDirectory(SelectLangFolder);
        }

        public void ApplyLanguage(I18nProvider instance)
        {
            if (!ApplyedProvider.Exists(p => p == instance)) ApplyedProvider.Add(instance);
            string LangFile = Path.Combine(SelectLangFolder, instance.GetType().FullName) + ".lang";
            foreach (FieldInfo item in instance.GetType().GetFields())
            {
                if (item.FieldType.Equals(typeof(LanguageElement)))
                {
                    string value = ConfigurationIO.IniReadValue(LangFile, item.Name, CurrentLanguage);
                    if (value == "")
                    {
                        value = (LanguageElement)item.GetValue(instance);
                        ConfigurationIO.IniWriteValue(LangFile, item.Name, value, CurrentLanguage);
                    }
                    item.SetValue(instance, new LanguageElement(value));
                }
                else if (item.FieldType.Equals(typeof(GuiLanguageElement)))
                {
                    string value = ConfigurationIO.IniReadValue(LangFile, item.Name, CurrentLanguage);
                    if (value == "")
                    {
                        value = (GuiLanguageElement)item.GetValue(instance);
                        ConfigurationIO.IniWriteValue(LangFile, item.Name, value, CurrentLanguage);
                    }
                    item.SetValue(instance, new GuiLanguageElement(value));
                }
            }
        }

        public override string ToString() => $"CurrentLanguage={CurrentLanguage}";
    }
}