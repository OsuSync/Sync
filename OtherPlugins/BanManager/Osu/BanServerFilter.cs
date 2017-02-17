    using Sync.MessageFilter;
using Sync.Source;
using System.Collections.Generic;
using System;
using System.Text;

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

            AddCommand("?ban","禁止某user/id/regex发送信息到irc",banCommand);
            AddCommand("?unban", "解除禁止某user/id/regex", unbanCommand);
            AddCommand("?whitelist","添加某user/id/regex到白名单，白名单的人将一直有权限发送信息到irc", whitelistCommand);
            AddCommand("?remove_whitelist","将某user/id/regex从白名单移除", remove_whitelistCommand);
            AddCommand("?access","设置发送消息到irc权限", accessCommand);
            AddCommand("?list", "获取白名单或者禁止名单的用户和规则", listCommand);

        }

        static char[] split = { ' ' };

        public void onMsg(ref MessageBase msg)
        {
            string message = msg.message.RawText;
            string[] args;
            if (message[0] != '?')
                return;
            for (int i = 0; i < basecommandArray.Count; i++)
            {
                msg.cancel = true;

                if (!IsBaseCommand(basecommandArray[i], message))
                    continue;

                

                args = message.Substring(basecommandArray[i].Length).Split(split, StringSplitOptions.RemoveEmptyEntries);
                for (int t = 0; t < args.Length; t++)
                    args[t] = args[t].Trim();

                if (args.Length == 0) // like ?ban ,?whitelist for help
                {
                    CBaseDanmuku danmaku = new CBaseDanmuku();
                    danmaku.danmuku = basecommandHelpArray[i];
                    bindManager.GetMessageDispatcher().RaiseMessage<ISourceDanmaku>(new DanmakuMessage(danmaku));
                }
                else {
                    try
                    {
                        baseCommandExecuteArray[i](args);
                    }
                    catch (Exception e)
                    {
                        CBaseDanmuku danmaku = new CBaseDanmuku();
                        danmaku.danmuku = e.Message;
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

        private void ThrowErrorMessage(string message= "错误的指令")
        {
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
            CBaseDanmuku danmaku = new CBaseDanmuku();
            switch (message[0])
            {
                case "-ban":
                    foreach (var userName in bindManager.GetFliterInfo().GetBanUserList())
                        sb.AppendFormat("{0} || ",userName);
                    foreach (var rule in bindManager.GetFliterInfo().GetBanRuleRegexList())
                        sb.AppendFormat("{0}:\"{1}\" || ", rule.id,rule.expression);
                    
                    danmaku.danmuku = sb.ToString();
                    bindManager.GetMessageDispatcher().RaiseMessage<ISourceDanmaku>(new DanmakuMessage(danmaku));
                    break;

                case "-whitelist":
                    foreach (var userName in bindManager.GetFliterInfo().GetWhiteListUserList())
                        sb.AppendFormat("{0} || ", userName);
                    foreach (var rule in bindManager.GetFliterInfo().GetWhiteListRuleRegexList())
                        sb.AppendFormat("{0}:\"{1}\" || ", rule.id, rule.expression);
                    
                    danmaku.danmuku = sb.ToString();
                    bindManager.GetMessageDispatcher().RaiseMessage<ISourceDanmaku>(new DanmakuMessage(danmaku));
                    break;
            }
        }
    }
}
