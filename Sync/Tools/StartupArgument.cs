using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Tools
{
    internal static class StartupArgument
    {
        private static Action<StartupHelper> ForceStartArg(string arg) => (_) => StartupHelper.ForceStart = true;
        private static Action<StartupHelper> NeedUpdateSyncArg(string arg) => (_) => StartupHelper.NeedUpdateSync = true;

        internal static Dictionary<string, Func<string, Action<StartupHelper>>> Arguments = new Dictionary<string, Func<string, Action<StartupHelper>>>()
        {
            { "f", ForceStartArg },
            { "--force-start", ForceStartArg },
            { "u", NeedUpdateSyncArg },
            { "--update", NeedUpdateSyncArg }
        };

        internal static Dictionary<string, Func<string, Action<StartupHelper>>> Actions = new Dictionary<string, Func<string, Action<StartupHelper>>>();
    }
}
