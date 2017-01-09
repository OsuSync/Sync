using Sync.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Command
{
    class CommandManager
    {
        CommandDispatch dispatch;

        public CommandManager()
        {
            dispatch = new CommandDispatch();

            new BaseCommand(this);

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
            if(!dispatch.invoke(args[0], args.Skip(1).ToArray()))
            {
                ConsoleWriter.Write("命令执行失败！ 请输入help查看命令列表。");
            }
        }
    }
}
