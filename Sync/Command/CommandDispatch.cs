using System;
using System.Collections.Generic;

namespace Sync.Command
{
    /// <summary>
    /// Delegate for one Command
    /// </summary>
    /// <param name="arg">Command args</param>
    /// <returns></returns>
    public delegate bool CommandDelegate(Arguments arg);

    /// <summary>
    /// A typedef for arguments list
    /// </summary>
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

    /// <summary>
    /// A Hashmap command dispatcher
    /// </summary>
    public class CommandDispatch
    {
        private Dictionary<string, CommandDelegate> cmdList = new Dictionary<string, CommandDelegate>();
        private Dictionary<string, string> cmdDest = new Dictionary<string, string>();
        public int count
        {
            get { return cmdList.Count; }
        }

        /// <summary>
        /// Bind command to string
        /// </summary>
        /// <param name="name">Invoke name</param>
        /// <param name="func">Functor</param>
        /// <param name="desc">Description</param>
        /// <returns><see cref="true"/> success, <see cref="false"/> fail</returns>
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
    }
}
