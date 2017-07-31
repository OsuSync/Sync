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
using Sync.Tools;
using static OsuStatusOutputSever.DefaultLanguage;

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
        public bool IsRun { get { return isRunning; } }

		/*
        public override void Dispose()
        {
            ForceStop();

            socketThread.Abort();

            beatmapSetId = -1;
            beatmapId = -1;
            combo = 0;
            mods = 0;
            currentHP = -1;
            currentACC = -1;
        }
		*/

        void ForceStop()
        {
            isRunning = true;
            Stop();
        }

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
            I18n.Instance.ApplyLanguage(new DefaultLanguage());

            base.onInitPlugin += () => Sync.Tools.IO.CurrentIO.WriteColor(Name + " By " + Author, ConsoleColor.DarkCyan);

            base.onInitCommand += commandManager =>
            {
                commandManager.Dispatch.bind("syncserver", commandProcess,LANG_COMMAND_DP);
            };

            base.onLoadComplete += host => {
                foreach(var itor in host.EnumPluings())
                {
                    if (itor is MemoryReader.MemoryReader)
                    {
                        ((MemoryReader.MemoryReader)itor).RegisterOSUListener(this);
                        Sync.Tools.IO.CurrentIO.WriteColor(LANG_REGISTER_SUCCESS, ConsoleColor.Yellow);
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
                Sync.Tools.IO.CurrentIO.WriteColor(LANG_HELP, ConsoleColor.Yellow);
            else
            {
                switch (args[0].Trim())
                {
                    case "-start":
                        Start();
                        Sync.Tools.IO.CurrentIO.WriteColor(LANG_MSG_START, ConsoleColor.Yellow);
                        break;
                    case "-stop":
                        Stop();
                        Sync.Tools.IO.CurrentIO.WriteColor(LANG_MSG_STOP, ConsoleColor.Yellow);
                        break;
                    case "-status":
                        Sync.Tools.IO.CurrentIO.WriteColor(string.Format(LANG_MSG_STATUS, beatmapSetId, beatmapId, currentHP, currentACC, combo, mods), ConsoleColor.Yellow);
                        break;
                    default:
                        Sync.Tools.IO.CurrentIO.WriteColor(LANG_MSG_UNKNOWN_COMMAND, ConsoleColor.Red);
                        break;
                }
            }
            return true;
        }

        const string commandSyncServer = "?syncserver";

        public void onMsg(ref IMessageBase msg)
        {
            if (!msg.Message.RawText.Trim().StartsWith(commandSyncServer))
                return;
            msg.Cancel = true;
            char[] splitChars = { ' ' };
            string message = msg.Message.RawText.Substring(commandSyncServer.Length).Trim();
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
                        Sync.Tools.IO.CurrentIO.WriteColor(LANG_MSG_LOST_CONNECTION, ConsoleColor.Blue);
                        currentClient = null;
                    }
                }
                 
                Sync.Tools.IO.CurrentIO.WriteColor(LANG_MSG_START_LISTENNING, ConsoleColor.Blue);
                currentClient = listenerServer.AcceptTcpClient();
                Sync.Tools.IO.CurrentIO.WriteColor(LANG_MSG_GET_CONNECTION , ConsoleColor.Blue);
            }
        }

        const int bufferSize = sizeof(int)*2+sizeof(double)*2;

        private void sendStatusTimerRun(object state)
        {
            byte[] message = Encoding.Default.GetBytes(string.Format("s{0} b{1} h{2} a{3} c{4} m{5}", beatmapSetId, beatmapId, currentHP, currentACC,combo,mods));

            try
            {
                currentClient?.GetStream().Write(message, 0, message.Length);
                currentClient?.GetStream().Flush();
            }
            catch { return; }//skip
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
