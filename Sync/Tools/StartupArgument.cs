using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Tools
{
    internal static class StartupArgument
    {
        internal static Dictionary<char, Func<string, Action<StartupHelper>>> Arguments = new Dictionary<char, Func<string, Action<StartupHelper>>>()
        {
            { 'f', (string arg) => (_) => StartupHelper.ForceStart = true },
            { 'u', (string arg) => (_) => StartupHelper.NeedUpdateSync = true },

        };

        internal static Dictionary<char, Func<string, Action<StartupHelper>>> Actions = new Dictionary<char, Func<string, Action<StartupHelper>>>();
    }
}
