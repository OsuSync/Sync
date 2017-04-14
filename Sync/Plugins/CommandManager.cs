using Sync.Tools;
using static Sync.Tools.DefaultI18n;
using System;

namespace Sync.Command
{
    public class CommandManager
    {
        CommandDispatch dispatch;

        public CommandManager()
        {
            dispatch = new CommandDispatch();
        }

        public CommandDispatch Dispatch
        {
            get { return dispatch; }
        }

        public void invokeCmdString(string cmd)
        {
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
