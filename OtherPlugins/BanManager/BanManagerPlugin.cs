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
            base.onInitFilter += manager => {
                banManager = new BanManager(manager,null);
                manager.AddFilters(banManager.GetClientFliter(), banManager.GetServerFliter());
            };

            base.onLoadComplete += host=>banManager.SetMessageDispatcher(host.Messages);

            base.onInitPlugin += () => Sync.Tools.IO.CurrentIO.WriteColor(Name + " By " + Author, ConsoleColor.DarkCyan);
        }

        public override void Dispose()
        {
            //nothing to do
        }
    }
}
