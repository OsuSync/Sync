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
        public BanManagerPlugin() : base("Ban Manager", "Dark Projector")
        {
            banManager = new BanManager();
            base.onInitFilter += manager => manager.AddFilters(banManager.GetClientFliter(), banManager.GetServerFliter());
            base.onInitPlugin += () => Sync.Tools.ConsoleWriter.WriteColor(Name + " By " + Author, ConsoleColor.DarkCyan);
        }
    }
}
