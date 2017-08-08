using Sync.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sync;
using Sync.Command;
using BanManagerPlugin.Ban;

namespace BanManagerPlugin
{
    public class BanManagerPlugin : Plugin
    {
        BanManager banManager = null;

        public BanManagerPlugin() : base("Ban Manager", "Dark Projector")
        {
            Sync.Tools.I18n.Instance.ApplyLanguage(new DefaultLanguage());

            base.EventBus.BindEvent<PluginEvents.InitFilterEvent>(manager => {
                banManager = new BanManager(manager.Filters,null);
                manager.Filters.AddFilters(banManager.GetClientFliter(), banManager.GetServerFliter());
            });

            base.EventBus.BindEvent<PluginEvents.LoadCompleteEvent>(host=>banManager.SetMessageDispatcher(host.Host.Messages));

            base.EventBus.BindEvent<PluginEvents.InitPluginEvent>((e) => Sync.Tools.IO.CurrentIO.WriteColor(Name + " By " + Author, ConsoleColor.DarkCyan));
        }

    }
}
