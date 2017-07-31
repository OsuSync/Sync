using Sync.MessageFilter;
using Sync.Source;
using System.Collections.Generic;
using System;
using System.Text;
using static BanManagerPlugin.DefaultLanguage;

namespace BanManagerPlugin.Ban
{
    class BanServerFilter : IFilter, ISourceOsu
    {

        BanManager bindManager = null;
        public void SetBanManager(BanManager manager)
        {
            this.bindManager = manager;
        }

        public delegate void CommandExecutor(string[] args);

        protected BanServerFilter() {}
        public BanServerFilter(BanManager refManager)
        {
            SetBanManager(refManager);

            AddCommand("?ban",LANG_HELP_BAN,banCommand);
            AddCommand("?unban", LANG_HELP_UNBAN, unbanCommand);
            AddCommand("?whitelist",LANG_HELP_WHITELIST, whitelistCommand);
            AddCommand("?remove_whitelist",LANG_HELP_REMOVE_WHITELIST, remove_whitelistCommand);
            AddCommand("?access",LANG_HELP_ACCESS, accessCommand);
            AddCommand("?list"  , LANG_HELP_LIST, listCommand);

        }

        static char[] split = { ' ' };

        public void onMsg(ref IMessageBase msg)
        {
            string message = msg.Message.RawText;
            string[] args;
            if (message[0] != '?')
                return;
            for (int i = 0; i < basecommandArray.Count; i++)
            {
                msg.Cancel = true;

                if (!IsBaseCommand(basecommandArray[i], message))
                    continue;

                

                args = message.Substring(basecommandArray[i].Length).Split(split, StringSplitOptions.RemoveEmptyEntries);
                for (int t = 0; t < args.Length; t++)
                    args[t] = args[t].Trim();

                if (args.Length == 0) // like ?ban ,?whitelist for help
                {
                    BaseDanmakuEvent danmaku = new BaseDanmakuEvent();
                    danmaku.Danmuku = basecommandHelpArray[i];
                    bindManager.GetMessageDispatcher().RaiseMessage<ISourceDanmaku>(new DanmakuMessage(danmaku));
                }
                else {
                    try
                    {
                        baseCommandExecuteArray[i](args);
                    }
                    catch (Exception e)
                    {
                        BaseDanmakuEvent danmaku = new BaseDanmakuEvent();
                        danmaku.Danmuku = e.Message;
                        bindManager.GetMessageDispatcher().RaiseMessage < ISourceDanmaku >( new DanmakuMessage(danmaku));
                    }
                }
                break;
            }
            return;
        }

        public void AddCommand(string command,string helpString,CommandExecutor executor) {
            basecommandArray.Add(command);
            basecommandHelpArray.Add(helpString);
            baseCommandExecuteArray.Add(executor);
        }

        private List<string> basecommandArray = new List<string>();

        private List<string> basecommandHelpArray = new List<string>();

        private List<CommandExecutor> baseCommandExecuteArray=new List<CommandExecutor>();

        /// <summary>
        /// 判断是否是指令消息
        /// </summary>
        /// <param name="command"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private bool IsBaseCommand(string command, string message)
        {
            return (message.StartsWith(command));
        }

        private void ThrowErrorMessage(string message= null)
        {
            if (message == null)
                message = LANG_ERR_COMMAND;
            throw new Exception(message);
        }

        //base command executor
        public void banCommand(string[] message)
        {
            switch (message[0])
            {
                case "-id":
                    if (message.Length < 2)
                        ThrowErrorMessage();
                    int id = 0;
                    if (!Int32.TryParse(message[1], out id))
                        ThrowErrorMessage();
                    else
                        bindManager.GetFliterInfo().AddBanId(id);
                    break;

                case "-regex":
                    if(message.Length < 2)
                        ThrowErrorMessage();
                    bindManager.GetFliterInfo().AddBanRuleRegex(message[1]);
                    break;

                default:
                    bindManager.GetFliterInfo().AddBanUserName(message[0]);
                    break;
            }
        }

        public void unbanCommand(string[] message)
        {
            int id = 0;

            switch (message[0])
            {
                case "-id":
                    if (message.Length < 2)
                        ThrowErrorMessage();                 
                    if (!Int32.TryParse(message[1], out id))
                        ThrowErrorMessage();
                    else
                        bindManager.GetFliterInfo().RemoveBanId(id);
                    break;

                case "-regex":
                    if (message.Length < 2)
                        ThrowErrorMessage();
                    if (!Int32.TryParse(message[1], out id))
                        ThrowErrorMessage();
                    else
                        bindManager.GetFliterInfo().RemovBanListRuleRegex(id);
                    break;
                default:
                    bindManager.GetFliterInfo().RemoveBanUserName(message[0]);
                    break;
            }
        }

        public void whitelistCommand(string[] message)
        {
            switch (message[0])
            {
                case "-id":
                    if (message.Length < 2)
                        ThrowErrorMessage();
                    int id = 0;
                    if (!Int32.TryParse(message[1], out id))
                        ThrowErrorMessage();
                    else
                        bindManager.GetFliterInfo().AddWhiteListId(id);
                    break;

                case "-regex":
                    if (message.Length < 2)
                        ThrowErrorMessage(); ;
                    bindManager.GetFliterInfo().AddWhiteListRuleRegex(message[1]);
                    break;

                default:
                    bindManager.GetFliterInfo().AddWhiteListUserName(message[0]);
                    break;
            }
        }

        public void remove_whitelistCommand(string[] message)
        {
            int id = 0;

            switch (message[0])
            {
                case "-id":
                    if (message.Length < 2)
                        ThrowErrorMessage();
                    if (!Int32.TryParse(message[1], out id))
                        ThrowErrorMessage();
                    else
                        bindManager.GetFliterInfo().RemoveWhiteListId(id);
                    break;

                case "-regex":
                    if (message.Length < 2)
                        ThrowErrorMessage();
                    if (!Int32.TryParse(message[1], out id))
                        ThrowErrorMessage();
                    else
                        bindManager.GetFliterInfo().RemoveWhiteListRuleRegex(id);
                    break;
                default:
                    bindManager.GetFliterInfo().RemoveWhiteListUserName(message[0]);
                    break;
            }
        }

        public void accessCommand(string[] message)
        {
            //咕咕咕咕咕
        }

        public void listCommand(string[] message)
        {
            StringBuilder sb = new StringBuilder(200);
            BaseDanmakuEvent danmaku = new BaseDanmakuEvent();
            switch (message[0])
            {
                case "-ban":
                    foreach (var userName in bindManager.GetFliterInfo().GetBanUserList())
                        sb.AppendFormat("{0} || ",userName);
                    foreach (var rule in bindManager.GetFliterInfo().GetBanRuleRegexList())
                        sb.AppendFormat("{0}:\"{1}\" || ", rule.id,rule.expression);
                    
                    danmaku.Danmuku = sb.ToString();
                    bindManager.GetMessageDispatcher().RaiseMessage<ISourceDanmaku>(new DanmakuMessage(danmaku));
                    break;

                case "-whitelist":
                    foreach (var userName in bindManager.GetFliterInfo().GetWhiteListUserList())
                        sb.AppendFormat("{0} || ", userName);
                    foreach (var rule in bindManager.GetFliterInfo().GetWhiteListRuleRegexList())
                        sb.AppendFormat("{0}:\"{1}\" || ", rule.id, rule.expression);
                    
                    danmaku.Danmuku = sb.ToString();
                    bindManager.GetMessageDispatcher().RaiseMessage<ISourceDanmaku>(new DanmakuMessage(danmaku));
                    break;
            }
        }

        public void Dispose()
        {
            //nothing to do
        }
    }
}
