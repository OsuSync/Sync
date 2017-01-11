using Sync.Tools;

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
                ConsoleWriter.Write("未知命令！ 请输入help查看命令列表。");
                return;
            }
            string arg = string.Empty;
            if (args.Length > 1) arg = args[1];
            
            if (!dispatch.invoke(args[0], (arg == string.Empty ? new Arguments() : arg.Split(' '))))
            {
                ConsoleWriter.Write("命令执行失败！ 请输入help查看命令列表。");
            }
        }
    }
}
