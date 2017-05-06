using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sync.Tools;

namespace OsuStatusOutputSever
{
    public class DefaultLanguage : I18nProvider
    {
        public static LanguageElement LANG_COMMAND_DP = "将本程序部分数据通过TCP分发到其他程序供使用";
        public static LanguageElement LANG_REGISTER_SUCCESS = "注册osuStatus侦听器成功";
        public static LanguageElement LANG_HELP = @"syncserver <参数>\n-start 开始将内容传发到监听指定端口的客户端上\n-stop 终止传送";

        public static LanguageElement LANG_MSG_START = "syncserver开始运行";
        public static LanguageElement LANG_MSG_STOP = "syncserver运行终止";
        public static LanguageElement LANG_MSG_STATUS = "s{0} b{1} h{2} a{3} c{4} m{5}";
        public static LanguageElement LANG_MSG_UNKNOWN_COMMAND = "syncserver未知参数";

        public static LanguageElement LANG_MSG_LOST_CONNECTION = "客户端无法连接";
        public static LanguageElement LANG_MSG_START_LISTENNING = "等待客户端连接...";
        public static LanguageElement LANG_MSG_GET_CONNECTION = "连接成功!";
    }
}
