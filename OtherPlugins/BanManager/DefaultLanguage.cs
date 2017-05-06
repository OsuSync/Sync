using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sync.Tools;

namespace BanManagerPlugin
{
    public class DefaultLanguage : I18nProvider
    {
        public static LanguageElement LANG_HELP_BAN = "禁止某user/id/regex发送信息到irc";
        public static LanguageElement LANG_HELP_UNBAN = "解除禁止某user/id/regex";
        public static LanguageElement LANG_HELP_WHITELIST = "添加某user/id/regex到白名单，白名单的人将一直有权限发送信息到irc";
        public static LanguageElement LANG_HELP_REMOVE_WHITELIST = "将某user/id/regex从白名单移除";
        public static LanguageElement LANG_HELP_ACCESS = "设置发送消息到irc权限";
        public static LanguageElement LANG_HELP_LIST = "获取白名单或者禁止名单的用户和规则";

        public static LanguageElement LANG_ERR_COMMAND = "错误的指令";
    }
}
