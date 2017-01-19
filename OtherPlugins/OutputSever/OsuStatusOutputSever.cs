using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sync.Plugins;
using Sync.Command;
using Sync;
using System.Threading;
using Sync.MessageFilter;
using System.Threading.Tasks;
using System.Net.Sockets;
using MemoryReader;
using MemoryReader.Listen.InterFace;
using MemoryReader.BeatmapInfo;
using MemoryReader.Mods;

namespace OsuStatusOutputSever
{
    public class OsuStatusOutputSever : Plugin, IOSUListener, IFilter, ISourceOsu
    {
        static int PORT = 7582;
        TcpListener listenerServer = null;
        Thread socketThread = null;

        List<TcpClient> clientList = new List<TcpClient>();

        object lockObj = new object();
        Mutex mutexLock = new Mutex();

        bool isRunning = false;
        public bool IsRun { private set { } get { return isRunning; } }

        public void Stop()
        {
            if (!isRunning)
                return;

            mutexLock.WaitOne();//hold up socketThreadRun()

            TcpClient client = null;
            for (int i = 0; i < clientList.Count; i++)
            {
                client = clientList[i];
                client.Close();
            }

            clientList.Clear();

            isRunning = false;
        }

        public void Start()
        {
            if (isRunning)
                return;
            mutexLock.ReleaseMutex();
            isRunning = true;
        }

        public OsuStatusOutputSever() : base("Ban Manager", "Dark Projector")
        {
            base.onInitPlugin += () => Sync.Tools.ConsoleWriter.WriteColor(Name + " By " + Author, ConsoleColor.DarkCyan);

            base.onInitCommand += commandManager =>
            {
                commandManager.Dispatch.bind("syncserver", commandProcess, "将本程序部分数据通过TCP分发到其他程序供使用");
            };

            listenerServer = new TcpListener(PORT);
            mutexLock.WaitOne();
            socketThread = new Thread(socketThreadRun);


        }

        #region finish
        private bool commandProcess(Arguments args)
        {
            if (args.Count == 0)
                Sync.Tools.ConsoleWriter.WriteColor("syncserver <参数>\n-start 开始将内容传发到监听指定端口的客户端上\n-stop 终止传送", ConsoleColor.Yellow);
            else
            {
                switch (args[0].Trim())
                {
                    case "-start":
                        Start();
                        Sync.Tools.ConsoleWriter.WriteColor("syncserver开始运行", ConsoleColor.Yellow);
                        break;
                    case "-stop":
                        Stop();
                        Sync.Tools.ConsoleWriter.WriteColor("syncserver运行终止", ConsoleColor.Yellow);
                        break;
                    default:
                        Sync.Tools.ConsoleWriter.WriteColor("syncserver未知参数", ConsoleColor.Red);
                        break;
                }
            }
            return true;
        }

        const string commandSyncServer = "?syncserver";

        public void onMsg(ref MessageBase msg)
        {
            if (!msg.message.RawText.Trim().StartsWith(commandSyncServer))
                return;
            msg.cancel = true;
            char[] splitChars = { ' ' };
            string message = msg.message.RawText.Substring(commandSyncServer.Length).Trim();
            string[] args = message.Split(splitChars);
            commandProcess(new Arguments(args));
        }

        private void socketThreadRun(object state)
        {
            if (!isRunning)
                return;
            while (true)
            {
                mutexLock.WaitOne();

                clientList.Add(listenerServer.AcceptTcpClient());

                mutexLock.ReleaseMutex();
            }
        }
        #endregion

        public void OnCurrentBeatmapSetChange(BeatmapSet beatmap)
        {
            throw new NotImplementedException();
        }

        public void OnCurrentBeatmapChange(Beatmap beatmap)
        {
            throw new NotImplementedException();
        }

        public void OnCurrentModsChange(ModsInfo mod)
        {
            //throw new NotImplementedException();
        }

        public void OnHPChange(double hp)
        {
            throw new NotImplementedException();
        }

        public void OnAccuracyChange(double acc)
        {
            throw new NotImplementedException();
        }
    }
}
