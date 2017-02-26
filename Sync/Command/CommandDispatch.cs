using System;
using System.Collections.Generic;

namespace Sync.Command
{
    public delegate bool CommandDelegate(Arguments arg);

    public class Arguments : List<string>, IDisposable
    {
        public Arguments()
        {

        }

        public Arguments(params string[] args)
        {
            AddRange(args);
        }

        public void Dispose()
        {
            Clear();
        }

        public static implicit operator Arguments(string[] args)
        {
            return new Arguments(args);
        }
    }

    public class CommandDispatch : IDisposable
    {
        private Dictionary<string, CommandDelegate> cmdList = new Dictionary<string, CommandDelegate>();
        private Dictionary<string, string> cmdDest = new Dictionary<string, string>();
        public int count
        {
            get { return cmdList.Count; }
        }

        public bool bind(string name, CommandDelegate func, string desc)
        {
            if (cmdList.ContainsKey(name)) return false;
            cmdList.Add(name, func);
            cmdDest.Add(name, desc);
            return true;
        }

        public CommandDelegate get(string name)
        {
            if (cmdList.ContainsKey(name)) return cmdList[name];
            else return null;
        }

        public IDictionary<string, string> getCommandsHelp()
        {
            return cmdDest;
        }

        public bool invoke(string name, Arguments args)
        {
            try
            {
                if (cmdList.ContainsKey(name)) return cmdList[name](args);
                else return false;
            }
            catch(Exception e)
            {
                Tools.IO.CurrentIO.Write(e.Message);
                Tools.IO.CurrentIO.Write(e.StackTrace);
                Tools.IO.CurrentIO.Write(e.Source);
                return false;
            }
        }

        public void Dispose()
        {
            cmdList.Clear();
            cmdDest.Clear();
            cmdList = null;
            cmdDest = null;
        }
    }
}
