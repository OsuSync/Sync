using Sync.Client;
using Sync.MessageFilter;
using Sync.Source;
using Sync.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Plugins
{
    public class MessageDispatcher
    {
        FilterManager filters;

        internal MessageDispatcher(FilterManager f)
        {
            filters = f;
            MessageManager.LimitLevel = 3;
            MessageManager.Option = MessageManager.PeekOption.Auto;
            MessageManager.Init(filters);
            MessageManager.SetSendMessageAction(new MessageManager.SendMessageAction((target,message) =>{
                    SyncHost.Instance.ClientWrapper.Client.SendMessage(new IRCMessage(target, message));
            }));

        }
        /// <summary>
        /// Send danmaku message to osu!IRC
        /// </summary>
        /// <param name="danmaku">Danmaku</param>
        public void onDanmaku(IBaseDanmakuEvent danmaku)
        {
            IMessageBase msg = new DanmakuMessage(danmaku);
            RaiseMessage<ISourceDanmaku>(msg);
        }

        /// <summary>
        /// Send osu!irc message to damaku
        /// </summary>
        /// <param name="user">Sender</param>
        /// <param name="message">Message</param>
        public void onIRC(StringElement user, StringElement message)
        {
            IMessageBase msg = new IRCMessage(user, message);
            RaiseMessage<ISourceClient>(msg);
        }



        public void RaiseMessage<Source>(IMessageBase msg)
        {
            RaiseMessage(typeof(Source), msg);
        }

        /// <summary>
        /// Raise a message and pass to filter list
        /// </summary>
        private void RaiseMessage(Type msgType, IMessageBase msg)
        {
            IMessageBase newMsg = msg;

            //From danmaku
            if (msgType == typeof(ISourceDanmaku))
            {
                filters.PassFilterDanmaku(ref newMsg);
                if (newMsg.Cancel) return;
                else
                {
                    //Send this danmaku message to osu!irc
                    MessageManager.PostIRCMessage(SyncHost.Instance.ClientWrapper.Client.NickName, newMsg);
                }
                return;
            }

            //From osu!orc
            else if (msgType == typeof(ISourceClient))
            {
                filters.PassFilterOSU(ref newMsg);
                if (newMsg.Cancel) return;
                else
                {
                    //Detect sender is osu!irc self
                    if (newMsg.User.RawText == SyncHost.Instance.ClientWrapper.Client.NickName)
                    {
                        //Send message to danmaku source
                        if (!SyncHost.Instance.SourceWrapper.Sendable)
                        {
                            IO.CurrentIO.WriteColor(DefaultI18n.LANG_SendNotReady, ConsoleColor.Red);
                        }
                        else
                        {
                            SyncHost.Instance.SourceWrapper.SendableSource.Send(newMsg);
                        }
                    }
                    //Send message to irc if Not sender
                    else
                    {
                        MessageManager.PostIRCMessage(SyncHost.Instance.ClientWrapper.Client.NickName, newMsg);
                    }

                }
                return;
            }

            //From danmaku gift(subscribe or other gift)
            else if (msgType == typeof(ISourceGift))
            {
                filters.PassFilterGift(ref newMsg);
                if (newMsg.Cancel) return;
                else
                {
                    SyncHost.Instance.ClientWrapper.Client?.SendMessage(new IRCMessage(SyncHost.Instance.ClientWrapper.Client.NickName, newMsg.Message));
                }
            }

            //The spectator count change
            else if (msgType == typeof(ISourceOnlineChange))
            {
                filters.PassFilterOnlineChange(ref newMsg);
                if (newMsg.Cancel) return;
                else
                {
                    SyncHost.Instance.ClientWrapper.Client?.SendMessage(new IRCMessage(SyncHost.Instance.ClientWrapper.Client.NickName, newMsg.Message));
                }
            }

        }
    }
}
