using Sync.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sync;
using Sync.Command;
using Sync.MessageFilter;
using Sync.Tools;
using static Sync.Plugins.PluginEvents;
using System.Threading.Tasks;

namespace PPQuery
{
    [SyncRequirePlugin(typeof(DefaultPlugin.DefaultPlugin))]
    public class PPQuery : Plugin, IFilter, ISourceClient
    {
        public PPQuery() : base("PP Query", "Deliay")
        {

        }

        public override void OnEnable()
        {
            Instance.BindEvent<InitFilterEvent>((evt) => evt.Filters.AddFilter(this));
            IO.CurrentIO.WriteColor("PP Query Plugin By Deliay >w<", ConsoleColor.DarkCyan);
        }

        public void onMsg(ref IMessageBase msg)
        {
            if (msg.User.RawText == SyncHost.Instance.ClientWrapper.Client.NickName)
            {
                if (msg.Message.RawText.StartsWith(DefaultPlugin.Clients.DirectOSUIRCBot.CONST_ACTION_FLAG) && msg.Message.RawText.Contains("osu.ppy.sh/b/"))
                {
                    msg.Cancel = true;
                    SyncHost.Instance.ClientWrapper.Client.SendMessage(new IRCMessage("tillerino", msg.Message.RawText));
                }
            }

            if (msg.User.Result.ToLower().Equals("tillerino"))
            {
                msg.Cancel = true;
                SyncHost.Instance.ClientWrapper.Client.SendMessage(new IRCMessage(SyncHost.Instance.ClientWrapper.Client.NickName, msg.Message.RawText));
            }
        }
    }
}
