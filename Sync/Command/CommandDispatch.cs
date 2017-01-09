using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Command
{
    public delegate bool CommandDelegate(Arguments arg);

    public class Arguments : List<string>
    {
        public Arguments()
        {

        }

        public Arguments(params string[] args)
        {
            AddRange(args);
        }

        public static implicit operator Arguments(string[] args)
        {
            return new Arguments(args);
        }
    }

    class CommandDispatch
    {
        private Dictionary<string, CommandDelegate> cmdList = new Dictionary<string, CommandDelegate>();

        public bool bind(string name, CommandDelegate func)
        {
            if (cmdList.ContainsKey(name)) return false;
            cmdList.Add(name, func);
            return true;
        }

        public CommandDelegate get(string name)
        {
            if (cmdList.ContainsKey(name)) return cmdList[name];
            else return null;
        }

        public bool invoke(string name, params string[] args)
        {
            if (cmdList.ContainsKey(name)) return cmdList[name](args);
            else return false;
        }
    }
}
