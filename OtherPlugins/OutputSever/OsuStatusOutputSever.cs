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
using System.Net;
using System.IO;
using MemoryReader;
using MemoryReader.Listen.InterFace;
using MemoryReader.BeatmapInfo;
using MemoryReader.Mods;

namespace OsuStatusOutputSever
{
    public class OsuStatusOutputSever : Plugin, IOSUListener, IFilter, ISourceOsu
    {
        int beatmapSetId = -1,beatmapId=-1,combo=0,mods=0;
        double currentHP = -1,currentACC=-1;

        Timer senderTimer = null;

        static int PORT = 7582;
        TcpListener listenerServer = null;
        Thread socketThread = null;

        TcpClient currentClient = null;

        volatile bool isRunning = false;
        public bool IsRun { private set { } get { return isRunning; } }

        public void Stop()
        {
            if (!isRunning)
                return;

            currentClient.Close();
            listenerServer.Stop();

            currentClient = null;
            isRunning = false;
        }

        public void Start()
        {
            if (isRunning)
                return;
            isRunning = true;
        }

        public OsuStatusOutputSever() : base("OsuStatusOutputSever", "Dark Projector")
        {
            base.onInitPlugin += () => Sync.Tools.IO.CurrentIO.WriteColor(Name + " By " + Author, ConsoleColor.DarkCyan);

            base.onInitCommand += commandManager =>
            {
                commandManager.Dispatch.bind("syncserver", commandProcess, "将本程序部分数据通过TCP分发到其他程序供使用");
            };

            base.onLoadComplete += host => {
                foreach(var itor in host.EnumPluings())
                {
                    if (itor is MemoryReader.MemoryReader)
                    {
                        ((MemoryReader.MemoryReader)itor).RegisterOSUListener(this);
                        Sync.Tools.IO.CurrentIO.WriteColor("注册osuStatus侦听器成功", ConsoleColor.Yellow);
                        break;
                    }
                }
            };

            listenerServer = new TcpListener(IPAddress.Parse("127.0.0.1"),PORT);
            socketThread = new Thread(socketThreadRun);
            senderTimer=new Timer(sendStatusTimerRun, null, 0, 250);
            listenerServer.Start();
            socketThread.Start();
        }

        #region finish
        private bool commandProcess(Arguments args)
        {
            if (args.Count == 0)
                Sync.Tools.IO.CurrentIO.WriteColor("syncserver <参数>\n-start 开始将内容传发到监听指定端口的客户端上\n-stop 终止传送", ConsoleColor.Yellow);
            else
            {
                switch (args[0].Trim())
                {
                    case "-start":
                        Start();
                        Sync.Tools.IO.CurrentIO.WriteColor("syncserver开始运行", ConsoleColor.Yellow);
                        break;
                    case "-stop":
                        Stop();
                        Sync.Tools.IO.CurrentIO.WriteColor("syncserver运行终止", ConsoleColor.Yellow);
                        break;
                    case "-status":
                        Sync.Tools.IO.CurrentIO.WriteColor(string.Format("s{0} b{1} h{2} a{3} c{4} m{5}", beatmapSetId, beatmapId, currentHP, currentACC, combo, mods), ConsoleColor.Yellow);
                        break;
                    default:
                        Sync.Tools.IO.CurrentIO.WriteColor("syncserver未知参数", ConsoleColor.Red);
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
            while (true)
            {
                while (
                    (!isRunning)|| currentClient != null&&currentClient.Connected) {
                    Thread.Sleep(100);
                    if(currentClient != null && (!currentClient.Connected) )
                    {
                        Sync.Tools.IO.CurrentIO.WriteColor("client lost.", ConsoleColor.Blue);
                        currentClient = null;
                    }
                }
                 
                Sync.Tools.IO.CurrentIO.WriteColor("listenning........", ConsoleColor.Blue);
                currentClient = listenerServer.AcceptTcpClient();
                Sync.Tools.IO.CurrentIO.WriteColor("got client.", ConsoleColor.Blue);
            }
        }

        const int bufferSize = sizeof(int)*2+sizeof(double)*2;

        private void sendStatusTimerRun(object state)
        {
            byte[] message = Encoding.Default.GetBytes(string.Format("s{0} b{1} h{2} a{3} c{4} m{5}", beatmapSetId, beatmapId, currentHP, currentACC,combo,mods));

            try
            {
                currentClient.GetStream().Write(message, 0, message.Length);
                currentClient.GetStream().Flush();
                /*
                BinaryWriter writer = new BinaryWriter(currentClient.GetStream());
                writer.Write(beatmapSetId);
                writer.Flush();*/
            }
            catch { }//skip
        }
        #endregion

        public void OnCurrentBeatmapSetChange(BeatmapSet beatmap)
        {
            beatmapSetId = beatmap.BeatmapSetID;
        }

        public void OnCurrentBeatmapChange(Beatmap beatmap)
        {
            beatmapId = beatmap.BeatmapID;
        }

        public void OnCurrentModsChange(ModsInfo mod)
        {
            mods = (int)mod.Mod;
        }

        public void OnComboChange(int combo)
        {
            this.combo = combo;
        }

        public void OnHPChange(double hp)
        {
            currentHP = hp;
        }

        public void OnAccuracyChange(double acc)
        {
            currentACC = acc;
        }
    }
}
