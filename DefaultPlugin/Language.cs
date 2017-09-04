using Sync.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefaultPlugin
{
    class Language : I18nProvider
    {
        public static LanguageElement LANG_COMMANDS_LOGIN = "login <user> [pass] 登录到目标弹幕网站，启动弹幕发送功能";
        public static LanguageElement LANG_COMMANDS_EXIT = "退出软件";
        public static LanguageElement LANG_COMMANDS_CLEAR = "清空屏幕";
        public static LanguageElement LANG_COMMANDS_STATUS = "获得当前连接状态属性";
        public static LanguageElement LANG_COMMANDS_STOP = "停止当前连接";
        public static LanguageElement LANG_COMMANDS_START = "开始同步";
        public static LanguageElement LANG_COMMANDS_HELP = "打印帮助信息";
        public static LanguageElement LANG_COMMANDS_SOURCEMSG = "danmaku <message> 发送弹幕测试";
        public static LanguageElement LANG_COMMANDS_CLIENTMSG = "chat <message> 发送IRC信息测试";
        public static LanguageElement LANG_COMMANDS_SOURCES = "获得当前所有弹幕源列表";
        public static LanguageElement LANG_COMMANDS_MSGMGR = "查看或者设置消息控制器相关内容,添加--help参数获取帮助";
        public static LanguageElement LANG_COMMANDS_FILTERS = "列表所有当前可用消息过滤器";
        public static LanguageElement LANG_COMMANDS_SOURCELOGIN = "登录到弹幕源 sourcelogin [用户名] [密码]";
        public static LanguageElement LANG_COMMANDS_RESTART = "重新启动应用程序";
        public static LanguageElement LANG_COMMANDS_LANG = "lang [cultureName] Get/Set language";
        public static LanguageElement LANG_COMMANDS_LISTLANG = "listlang [--all] List (supported/all) languages";
        public static LanguageElement LANG_COMMANDS_BILIBILI = "setbili roomID 设置Bilibili直播源的房间号";
        public static LanguageElement LANG_COMMANDS_FILTERS_ITEM = "过滤项";
        public static LanguageElement LANG_COMMANDS_FILTERS_OBJ = "过滤器";
        public static LanguageElement LANG_COMMANDS_BOTIRC_CURRENT = "当前BotIRC: {0:S}";
        public static LanguageElement LANG_COMMANDS_BOTIRC_SET = "当前BotIRC设置为 {0:S}";
        public static LanguageElement LANG_COMMANDS_IRC_CURRENT = "当前目标IRC: {0:S}";
        public static LanguageElement LANG_COMMANDS_IRC_SET = "当前目标IRC设置为 ";
        public static LanguageElement LANG_COMMANDS_TARGET_CURRENT = "当前直播ID: {0:S}";
        public static LanguageElement LANG_COMMANDS_TARGET_SET = "当前直播ID设置为 ";
        public static LanguageElement LANG_COMMANDS_SOURCES_NAME = "弹幕源";
        public static LanguageElement LANG_COMMANDS_SOURCES_AUTHOR = "作者";
        public static LanguageElement LANG_COMMANDS_DANMAKU_NOT_SUPPORT = @"提示：当前弹幕源不支持发送弹幕，请更换弹幕源！\n";
        public static LanguageElement LANG_COMMANDS_EXIT_DONE = "退出操作已完成，如果窗口还未关闭，您可以强制关闭。";
        public static LanguageElement LANG_COMMANDS_CHAT_IRC_NOTCONNECT = "osu! irc 尚未连接，您还不能发送消息。";
        public static LanguageElement LANG_COMMANDS_DANMAKU_REQUIRE_LOGIN = "你必须登录才能发送弹幕!";
        public static LanguageElement LANG_COMMANDS_START_ALREADY_RUN = "同步实例已经在运行。";
        public static LanguageElement LANG_COMMANDS_ARGUMENT_WRONG = "参数不正确";
        public static LanguageElement LANG_COMMANDS_MSGMGR_HELP = @"\n--status :查看当前消息管理器的信息\n--limit <数值> :是设置限制发送信息的等级，越低就越容易触发管控\n--option <名称> :是设置管控的方式，其中auto是自动管控，force_all强行全都发送,force_limit是仅发送使用?send命令的消息";
        public static LanguageElement LANG_COMMANDS_MSGMGR_LIMIT = "限制中...";
        public static LanguageElement LANG_COMMANDS_MSGMGR_FREE = "无限制";
        public static LanguageElement LANG_COMMANDS_MSGMGR_STATUS = "MessageManager mode:{4:S},status:{0:D},queueCount/limitCount/recoverTime:{1}/{2}/{3}";
        public static LanguageElement LANG_COMMANDS_MSGMGR_LIMIT_SPEED_SET = "设置限制发送速度等级为{0}";
        public static LanguageElement LANG_COMMANDS_MSGMGR_LIMIT_STYPE_SET = "设置消息管理器的管制方式为{0}";
        public static LanguageElement LANG_SEND_COOKIE_SAVED = "登陆信息保存成功！";
        public static LanguageElement LANG_SEND_DONE = "发送完成";
        public static LanguageElement LANG_DOUYU_FAIL = "连接状态检测失败! 期待{0}, 结果{1}";
        public static LanguageElement LANG_DOUYU_AUTH_SUCC = "斗鱼服务器连接认证成功！";
        public static LanguageElement LANG_DOUYU_DANMAKU = "收到弹幕: {0}:{1}";
        public static LanguageElement LANG_DOUYU_GIFT = "酬勤";
        public static LanguageElement LANG_BILIBILI_ONLINECHANGE = "直播间人数变化 {0}";
    }
}
