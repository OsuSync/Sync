using Sync.Tools;
using static Sync.Tools.DefaultI18n;
using System;

namespace Sync.Command
{
    /// <summary>
    /// Command manager for plugins commands
    /// </summary>
    public class CommandManager
    {
        CommandDispatch dispatch;

        public CommandManager()
        {
            dispatch = new CommandDispatch();
        }

        /// <summary>
        /// Register command via this dispatch
        /// </summary>
        public CommandDispatch Dispatch
        {
            get { return dispatch; }
        }

        /// <summary>
        /// Invoke command in string
        /// </summary>
        /// <param name="cmd">Command</param>
        public void invokeCmdString(string cmd)
        {
            if (cmd == null || cmd.Length == 0) return;
            string[] args = cmd.Split(" ".ToCharArray(), 2);

            if(args.Length < 1 )
            {
                IO.CurrentIO.Write(LANG_UnknowCommand);
                return;
            }
            string arg = string.Empty;
            if (args.Length > 1) arg = args[1];
            
            if (!dispatch.invoke(args[0], (arg == string.Empty ? new Arguments() : arg.Split(' '))))
            {
                IO.CurrentIO.Write(LANG_CommandFail);
            }
        }
    }
}
