using Sync.MessageFilter;

namespace BanManagerPlugin.Ban
{
    class BanServerFilter : IBanMessageFilters, ISourceOsu
    {
        protected BanServerFilter() {}
        public BanServerFilter(BanManager refManager)
        {
            SetBanManager(refManager);
        }

        public new void onMsg(ref MessageBase msg)
        {
            string message = msg.message.RawText;
            string param;
            if (message[0] != '?')
                return;
            if (IsCommand(banCommand, message))
            {
                param = message.Substring(banCommand.Length).Trim();
                bindManager.GetFliterInfo().AddBanUserName(param);
                msg.cancel = true;
                return;
            }

            if (IsCommand(unbanCommand, message))
            {
                param = message.Substring(unbanCommand.Length).Trim();
                bindManager.GetFliterInfo().RemoveBanUserName(param);
                msg.cancel = true;
                return;
            }

            if (IsCommand(whitelistCommand, message))
            {
                param = message.Substring(whitelistCommand.Length).Trim();
                bindManager.GetFliterInfo().AddWhiteListUserName(param);
                msg.cancel = true;
                return;
            }

            if (IsCommand(removewhitelistCommand, message))
            {
                param = message.Substring(removewhitelistCommand.Length).Trim();
                bindManager.GetFliterInfo().RemoveWhiteListUserName(param);
                msg.cancel = true;
                return;
            }

            if (IsCommand(changeaccesstypeCommand, message))
            {
                //todo 鸽一会
                msg.cancel = true;
                return;
            }
            return;
        }

        /// <summary>
        /// 可支持触发的指令
        /// </summary>
        private const string banCommand = "?ban", unbanCommand = "?unban", whitelistCommand = "?whitelist", removewhitelistCommand = "?remove_whitelist", changeaccesstypeCommand = "?access";

        /// <summary>
        /// 判断是否是指令消息
        /// </summary>
        /// <param name="command"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private bool IsCommand(string command, string message)
        {
            return (message.StartsWith(command));
        }
    }
}
