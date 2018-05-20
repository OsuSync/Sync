using Sync.Command;
using Sync.MessageFilter;
using Sync.Source;
using Sync.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using static Sync.Tools.DefaultI18n;

namespace Sync.Plugins.BuildInPlugin
{
    public class InternalPlugin : Plugin
    {
        #region Updater Decleare

        [DataContract]
        public class UpdateData
        {
            [DataMember(Order = 0)]
            public int id { get; set; }

            [DataMember(Order = 1)]
            public string name { get; set; }

            [DataMember(Order = 2)]
            public string author { get; set; }

            [DataMember(Order = 3)]
            public string latestHash { get; set; }

            [DataMember(Order = 4)]
            public string downloadUrl { get; set; }

            [DataMember(Order = 5)]
            public string description { get; set; }

            [DataMember(Order = 6)]
            public string guid { get; set; }

            [DataMember(Order = 7)]
            public string fileName { get; set; }
        }

        [DataContract]
        public class SyncUpdate
        {
            [DataMember(Order = 0)]
            public string versionHash { get; set; }

            [DataMember(Order = 1)]
            public string downloadURL { get; set; }

            [DataMember(Order = 2)]
            public string versionId { get; set; }
        }

        #endregion Updater Decleare

        private PluginConfigurationManager config;

        public InternalPlugin() : base("InternalPlugin", "OsuSync")
        {
        }

        public override void OnEnable()
        {
            config = new PluginConfigurationManager(this);

            this.EventBus.BindEvent<PluginEvents.InitCommandEvent>(p =>
            {
                Func<string, CommandDelegate, string, bool> addCmd = p.Commands.Dispatch.bind;
                addCmd("plugins", Plugins, "Install & Update Plugins online, type 'plugins' to get help.");
                BindCommondCommand(p.Commands.Dispatch);
            });

            Updater.update = this;
        }

        #region Command Execute

        #region Commond Command

        private void BindCommondCommand(CommandDispatch dispatch)
        {
            dispatch.bind("exit", exit, LANG_COMMANDS_EXIT);
            dispatch.bind("restart", restart, LANG_COMMANDS_RESTART);
            dispatch.bind("stop", stop, LANG_COMMANDS_STOP);
            dispatch.bind("start", start, LANG_COMMANDS_START);
            dispatch.bind("status", status, LANG_COMMANDS_STATUS);
            dispatch.bind("sourcemsg", sourcemsg, LANG_COMMANDS_SOURCEMSG);
            dispatch.bind("clientmsg", clientmsg, LANG_COMMANDS_CLIENTMSG);

            dispatch.bind("clientusermsg", chatuser, LANG_COMMANDS_CLIENTUSERMSG);
            dispatch.bind("disable", disable, LANG_COMMANDS_DISABLE);
            dispatch.bind("client", switchclient, LANG_COMMANDS_SWITCH_CLIENT);
            dispatch.bind("sourcelogin", sourcelogin, LANG_COMMANDS_SOURCELOGIN);

            dispatch.bind("clear", clear, LANG_COMMANDS_CLEAR);

            dispatch.bind("help", help, LANG_COMMANDS_HELP);
            dispatch.bind("listlang", languages, LANG_COMMANDS_LISTLANG);

            dispatch.bind("lang", language, LANG_COMMANDS_LANG);
            dispatch.bind("msgmgr", msgmgr, LANG_COMMANDS_MSGMGR);
            dispatch.bind("sources", listsource, LANG_COMMANDS_SOURCES);
            dispatch.bind("filters", filters, LANG_COMMANDS_FILTERS);
        }

        public bool listsource(Arguments arg)
        {
            foreach (SourceBase src in SyncHost.Instance.Sources.SourceList)
            {
                IO.CurrentIO.WriteColor("", ConsoleColor.Gray, false);
                IO.CurrentIO.WriteColor(LANG_COMMANDS_SOURCES_NAME, ConsoleColor.Cyan, false, false);
                IO.CurrentIO.WriteColor(src.Name.PadRight(18), ConsoleColor.White, false, false);
                IO.CurrentIO.WriteColor(LANG_COMMANDS_SOURCES_AUTHOR, ConsoleColor.DarkCyan, false, false);
                IO.CurrentIO.WriteColor(src.Author, ConsoleColor.White, true, false);
            }
            return true;
        }

        private bool switchclient(Arguments arg)
        {
            if (arg.Count == 0)
            {
                foreach (var item in SyncHost.Instance.Clients.Clients)
                {
                    IO.CurrentIO.WriteColor("", ConsoleColor.Gray, false);
                    IO.CurrentIO.WriteColor(LANG_COMMANDS_CLIENT_NAME, ConsoleColor.Cyan, false, false);
                    IO.CurrentIO.WriteColor(item.ClientName.PadRight(18), ConsoleColor.White, false, false);
                    IO.CurrentIO.WriteColor(LANG_COMMANDS_CLIENT_AUTHOR, ConsoleColor.DarkCyan, false, false);
                    IO.CurrentIO.WriteColor(item.Author, ConsoleColor.White, true, false);
                }

                IO.CurrentIO.WriteColor(string.Format(LANG_COMMANDS_CURRENT, SyncHost.Instance.ClientWrapper.Client?.ClientName ?? "还没指定发送源"), ConsoleColor.Green);
            }
            else
            {
                if (SyncHost.Instance.Clients.Clients.FirstOrDefault(p => p.ClientName == arg[0]) == null) return false;
                Configuration.Instance.Client = arg[0];
                SyncHost.Instance.ClientWrapper.ResetClient();
            }
            return true;
        }

        private bool disable(Arguments arg)
        {
            if (arg.Count == 0)
                IO.CurrentIO.WriteColor(LANG_NO_PLUGIN_SELECT, ConsoleColor.Red);
            else
                foreach (var item in Sync.SyncHost.Instance.EnumPluings())
                {
                    if (item.Name == arg[0])
                    {
                        item.OnDisable();
                        IO.CurrentIO.WriteColor(LANG_PLUGIN_DISABLED + arg[0], ConsoleColor.Red);
                    }
                }
            return true;
        }

        private bool sourcelogin(Arguments arg)
        {
            if (SyncHost.Instance.SourceWrapper.Sendable)
            {
                if (arg.Count == 0) SyncHost.Instance.SourceWrapper.SendableSource.Login("", "");
                else if (arg.Count == 1) SyncHost.Instance.SourceWrapper.SendableSource.Login(arg[0], "");
                else if (arg.Count == 2) SyncHost.Instance.SourceWrapper.SendableSource.Login(arg[0], arg[1]);
            }
            else
                IO.CurrentIO.WriteColor(string.Format(LANG_SOURCE_NOT_SUPPORT_SEND, SyncHost.Instance.SourceWrapper.Source?.GetType().Name), ConsoleColor.Red);

            return true;
        }

        public bool clientmsg(Arguments arg)
        {
            if (arg.Count == 0 || (SyncHost.Instance.SourceWrapper.SendableSource != null && SyncHost.Instance.SourceWrapper.SendableSource.SendStatus == false))
            {
                IO.CurrentIO.Write(LANG_COMMANDS_CHAT_IRC_NOTCONNECT);
                return true;
            }

            SyncHost.Instance.Messages.RaiseMessage<ISourceClient>(new IRCMessage("Console", string.Join(" ", arg)));
            return true;
        }

        public bool chatuser(Arguments arg)
        {
            if (arg.Count < 1 || (SyncHost.Instance.SourceWrapper.SendableSource != null && SyncHost.Instance.SourceWrapper.SendableSource.SendStatus == false))
            {
                IO.CurrentIO.Write(LANG_COMMANDS_CHAT_IRC_NOTCONNECT);
            }
            string message = "";
            for (int i = 1; i < arg.Count; i++)
                message += arg[i] + " ";
            SyncHost.Instance.Messages.RaiseMessage<ISourceClient>(new IRCMessage(arg[0].Trim(), message));
            return true;
        }

        public bool sourcemsg(Arguments arg)
        {
            if (SyncHost.Instance.SourceWrapper.Sendable)
            {
                if (SyncHost.Instance.SourceWrapper.SendableSource.SendStatus)
                {
                    SyncHost.Instance.SourceWrapper.SendableSource.Send(new IRCMessage(string.Empty, string.Join("", arg)));
                    return true;
                }
                else
                {
                    IO.CurrentIO.Write(LANG_COMMANDS_DANMAKU_REQUIRE_LOGIN);
                }
            }
            else
            {
                IO.CurrentIO.Write(LANG_COMMANDS_DANMAKU_NOT_SUPPORT);
            }
            return true;
        }

        public bool start(Arguments arg)
        {
            if (SyncHost.Instance.SourceWrapper.Source == null)
            {
                IO.CurrentIO.WriteColor(LANG_COMMANDS_START_NO_SOURCE, ConsoleColor.Red);
                return true;
            }

            if (SyncHost.Instance.ClientWrapper.Client == null)
            {
                IO.CurrentIO.WriteColor(LANG_COMMANDS_START_NO_CLIENT, ConsoleColor.Red);
                return true;
            }

            if (SyncHost.Instance.SourceWrapper.Source.Status == SourceStatus.CONNECTED_WORKING)
            {
                IO.CurrentIO.WriteColor(LANG_COMMANDS_START_ALREADY_RUN, ConsoleColor.Red);
                return true;
            }

            SyncHost.Instance.ClientWrapper?.Client?.StartWork();
            SyncHost.Instance.SourceWrapper?.Source?.Connect();
            return true;
        }

        public bool stop(Arguments arg)
        {
            SyncHost.Instance.ClientWrapper.Client?.StopWork();
            SyncHost.Instance.SourceWrapper.Source?.Disconnect();

            return true;
        }

        public bool status(Arguments arg)
        {
            IO.CurrentIO.WriteStatus();
            return true;
        }

        public bool language(Arguments arg)
        {
            if (arg.Count == 0)
            {
                CultureInfo info = CultureInfo.GetCultureInfo(I18n.Instance.CurrentLanguage);
                IO.CurrentIO.WriteColor(string.Format(LANG_COMMANDS_CURRENT_LANG, info.Name, info.NativeName), ConsoleColor.Yellow);
                return true;
            }
            else if (arg.Count == 1)
            {
                try
                {
                    CultureInfo info = CultureInfo.GetCultureInfo(arg[0]);
                    Configuration.Instance.Language = arg[0];
                    IO.CurrentIO.WriteColor(string.Format(LANG_COMMANDS_LANG_SWITCHED, arg[0], info.NativeName), ConsoleColor.Green);
                    return true;
                }
                catch (CultureNotFoundException)
                {
                    IO.CurrentIO.WriteColor(LANG_COMMANDS_LANG_NOT_FOUND, ConsoleColor.Red);
                    return false;
                }
            }
            return false;
        }

        public bool clear(Arguments arg)
        {
            IO.CurrentIO.Clear();
            IO.CurrentIO.WriteWelcome();
            return true;
        }

        public bool help(Arguments arg)
        {
            IO.CurrentIO.WriteHelp();
            return true;
        }

        public bool msgmgr(Arguments arg)
        {
            int value = 0;

            if (arg.Count == 0)
                IO.CurrentIO.WriteColor(LANG_COMMANDS_ARGUMENT_WRONG, ConsoleColor.Red);
            else
            {
                switch (arg[0].Trim())
                {
                    case "--help":
                        IO.CurrentIO.WriteColor(LANG_COMMANDS_MSGMGR_HELP, ConsoleColor.Yellow);
                        break;

                    case "--status":
                        IO.CurrentIO.WriteColor(String.Format(LANG_COMMANDS_MSGMGR_STATUS, (string)(MessageManager.IsLimit ? LANG_COMMANDS_MSGMGR_LIMIT : LANG_COMMANDS_MSGMGR_FREE), MessageManager.CurrentQueueCount, MessageManager.LimitLevel, MessageManager.RecoverTime, MessageManager.Option.ToString()), ConsoleColor.Yellow);
                        break;

                    case "--limit":
                        if (arg.Count == 2 && Int32.TryParse(arg[1].Trim(), out value))
                        {
                            MessageManager.LimitLevel = value;
                            IO.CurrentIO.WriteColor(string.Format(LANG_COMMANDS_MSGMGR_LIMIT_SPEED_SET, MessageManager.LimitLevel), ConsoleColor.Yellow);
                        }
                        else
                            IO.CurrentIO.WriteColor(LANG_COMMANDS_ARGUMENT_WRONG, ConsoleColor.Red);
                        break;

                    case "--option":
                        if (arg.Count == 2)
                        {
                            switch (arg[1].Trim().ToLower())
                            {
                                case "auto":
                                    MessageManager.Option = MessageManager.PeekOption.Auto;
                                    break;

                                case "force_all":
                                    MessageManager.Option = MessageManager.PeekOption.Force_All;
                                    break;

                                case "force_limit":
                                    MessageManager.Option = MessageManager.PeekOption.Only_Send_Command;
                                    break;
                            }
                            IO.CurrentIO.WriteColor(string.Format(LANG_COMMANDS_MSGMGR_LIMIT_STYPE_SET, MessageManager.Option.ToString()), ConsoleColor.Yellow);
                        }
                        else
                            IO.CurrentIO.WriteColor(LANG_COMMANDS_ARGUMENT_WRONG, ConsoleColor.Red);
                        break;
                }
            }
            return true;
        }

        public bool languages(Arguments arg)
        {
            if (arg.Count > 0 && arg[0] == "--all")
            {
                foreach (var item in CultureInfo.GetCultures(CultureTypes.NeutralCultures))
                {
                    IO.CurrentIO.WriteColor(string.Format("CultureName: {0:S}\t{1:S}", item.Name, item.NativeName), ConsoleColor.Yellow);
                }
            }
            else
            {
                foreach (var item in System.IO.Directory.EnumerateDirectories(I18n.Instance.LangFolder))
                {
                    string name = item.Substring(item.LastIndexOf('\\') + 1);
                    IO.CurrentIO.WriteColor(string.Format("CultureName: {0:S}\t{1:S}", name, CultureInfo.GetCultureInfo(name).NativeName), ConsoleColor.Yellow);
                }
            }
            return true;
        }

        public bool exit(Arguments arg)
        {
            stop(arg);
            SyncHost.Instance.ExitSync();
            IO.CurrentIO.Write(LANG_COMMANDS_EXIT_DONE);
            return true;
        }

        private bool restart(Arguments arg)
        {
            Process.Start(Assembly.GetEntryAssembly().Location);
            Environment.Exit(0);
            return true;
        }

        private bool filters(Arguments arg)
        {
            foreach (var item in SyncHost.Instance.Filters.GetFiltersEnum())
            {
                IO.CurrentIO.WriteColor("", ConsoleColor.Gray, false);
                IO.CurrentIO.WriteColor(LANG_COMMANDS_FILTERS_ITEM, ConsoleColor.Cyan, false, false);
                IO.CurrentIO.WriteColor(item.Key.Name.PadRight(22), ConsoleColor.White, false, false);
                IO.CurrentIO.WriteColor(LANG_COMMANDS_FILTERS_OBJ, ConsoleColor.DarkCyan, false, false);
                IO.CurrentIO.WriteColor(item.Value.GetType().Name, ConsoleColor.White, true, false);
            }
            return true;
        }

        #endregion Commond Command

        #region Update Command

        private bool Plugins(Arguments arg)
        {
            if (arg.Count == 0) return Help();
            switch (arg[0])
            {
                case "search":
                    return Search(arg[1]);

                case "update":
                    return Update();

                case "install":
                    return Install(arg[1]);

                case "list":
                    return List();

                case "remove":
                    return Remove(arg[1]);

                case "latest":
                    return Latest();

                default:
                    return Help();
            }
        }

        private bool Update()
        {
            IEnumerable<Plugin> plugins = this.getHoster().EnumPluings();
            foreach (var item in plugins)
            {
                try
                {
                    IO.CurrentIO.Write($"Fetch update: {item.Name} by {item.Author} [{item.getGuid()}]");
                    var result = Serializer<UpdateData>($"http://sync.mcbaka.com/api/Update/plugin/{item.getGuid()}");
                    var target = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins", result.fileName);
                    if (MD5HashFile(target).ToLower() != result.latestHash)
                    {
                        IO.CurrentIO.Write($"Download: {result.downloadUrl}...");
                        if (!DownloadSingleFile(result.downloadUrl, target, result.fileName))
                        {
                            IO.CurrentIO.WriteColor("Download Failed!", ConsoleColor.Red);
                        }
                    }
                    else
                    {
                        IO.CurrentIO.Write(string.Format(LANG_VERSION_LATEST, result.name));
                    }
                }
                catch (Exception e)
                {
                    IO.CurrentIO.Write(string.Format(LANG_UPDATE_ERROR, e.TargetSite.Name, e.Message));
                    continue;
                }
            }

            RequireRestart(LANG_UPDATE_DONE);
            return true;
        }

        private bool Search(string keyword)
        {
            try
            {
                var result = Serializer<UpdateData[]>($"http://sync.mcbaka.com/api/Update/search/{keyword}");

                foreach (var item in result)
                {
                    IO.CurrentIO.WriteColor("Name", ConsoleColor.Cyan, false, false);
                    IO.CurrentIO.WriteColor(item.name.PadRight(15), ConsoleColor.White, false, false);
                    IO.CurrentIO.WriteColor("Author", ConsoleColor.DarkCyan, false, false);
                    IO.CurrentIO.WriteColor(item.author.PadRight(15), ConsoleColor.White, false, false);
                    IO.CurrentIO.WriteColor("Description", ConsoleColor.DarkCyan, false, false);
                    IO.CurrentIO.WriteColor(item.description, ConsoleColor.White, true, false);
                    IO.CurrentIO.WriteColor("GUID ", ConsoleColor.DarkCyan, false, false);
                    IO.CurrentIO.WriteColor(item.guid.PadRight(32), ConsoleColor.White, true, false);
                    IO.CurrentIO.WriteColor("===============", ConsoleColor.White, true, false);
                }
                return true;
            }
            catch (Exception e)
            {
                IO.CurrentIO.Write($"Error while {e.TargetSite.Name} : {e.Message}");
            }
            return false;
        }

        private bool Install(string guid)
        {
            if (CheckUpdate(guid))
            {
                RequireRestart(LANG_INSTALL_DONE);
                return true;
            }
            else
            {
                if (Serializer<UpdateData[]>($"http://sync.mcbaka.com/api/Update/search/{guid}") is UpdateData[] datas)
                {
                    if (datas.Length == 0 || CheckUpdate(datas[0].guid))
                    {
                        RequireRestart(LANG_INSTALL_DONE);
                        return true;
                    }
                    else return false;
                }
                return false;
            }
        }

        private bool Remove(string name)
        {
            var type = getHoster().EnumPluings().FirstOrDefault(p => p.Name.ToLower().Contains(name.ToLower()));
            if (type == null)
            {
                IO.CurrentIO.WriteColor(string.Format(LANG_PLUGIN_NOT_FOUND, name), ConsoleColor.Red);
                return false;
            }
            else
            {
                var result = Serializer<UpdateData[]>($"http://sync.mcbaka.com/api/Update/search/{name}")?[0];
                var target = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins", Path.GetFileName(result.fileName));
                if (File.Exists(target)) File.Delete(target);

                RequireRestart(LANG_REMOVE_DONE);
                return true;
            }
        }

        private bool List()
        {
            var list = getHoster().EnumPluings();
            foreach (var item in list)
            {
                IO.CurrentIO.WriteColor("Name", ConsoleColor.Cyan, false, false);
                IO.CurrentIO.WriteColor(item.Name.PadRight(25), ConsoleColor.White, false, false);
                IO.CurrentIO.WriteColor("Author", ConsoleColor.DarkCyan, false, false);
                IO.CurrentIO.WriteColor(item.Author.PadRight(20), ConsoleColor.White, false, false);
                var info = item.GetType().GetCustomAttribute<SyncPluginID>();
                IO.CurrentIO.WriteColor("Support Update:", ConsoleColor.DarkCyan, false, false);
                if (info != null)
                {
                    IO.CurrentIO.WriteColor("Yes".PadRight(15), ConsoleColor.White, false, false);
                    IO.CurrentIO.WriteColor("Ver:", ConsoleColor.DarkCyan, false, false);
                    IO.CurrentIO.WriteColor(info.Version.PadRight(15), ConsoleColor.White, true, false);
                }
                else
                {
                    IO.CurrentIO.WriteColor("No", ConsoleColor.White, true, false);
                }
            }
            return true;
        }

        internal bool Latest()
        {
            IO.CurrentIO.WriteColor("Fetch Sync update..", ConsoleColor.Cyan);
            var result = Serializer<SyncUpdate>($"http://sync.mcbaka.com/api/Update/latest");
            if (!File.Exists(Updater.CurrentFullSourceEXEPath) || MD5HashFile(Updater.CurrentFullSourceEXEPath) != result.versionHash)
            {
                IO.CurrentIO.Write($"Download: {result.downloadURL}...");
                DownloadSingleFile(result.downloadURL, Updater.CurrentFullUpdateEXEPath, "Sync");
                RequireRestart("Update downloaded. Restart to apply effect");
            }
            return true;
        }

        private bool Help()
        {
            IO.CurrentIO.Write("Help for 'plugins' command:");
            IO.CurrentIO.WriteHelp("install", "install [guid/name] Install plugin");
            IO.CurrentIO.WriteHelp("remove", "remove [name] Remove plugin by name");
            IO.CurrentIO.WriteHelp("search", "search [keyword] Search plugins");
            IO.CurrentIO.WriteHelp("update", "Check update for Sync and Plugins");
            IO.CurrentIO.WriteHelp("list", "List current installed plugins");
            IO.CurrentIO.WriteHelp("latest", "Check latest Sync update");
            return true;
        }

        #endregion Update Command

        #endregion Command Execute

        #region Update Tool

        private void RequireRestart(string msg)
        {
            IO.CurrentIO.WriteColor($"{msg}? (Y/N):", ConsoleColor.Green, false);
            var result = IO.CurrentIO.ReadCommand();
            if (result.ToLower().StartsWith("y")) SyncHost.Instance.RestartSync();
        }

        internal bool CheckUpdate(string guid)
        {
            try
            {
                IO.CurrentIO.Write($"Fetch update: {guid}");
                var result = Serializer<UpdateData>($"http://sync.mcbaka.com/api/Update/plugin/{guid}");
                var target = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins", result.fileName);
                if (!File.Exists(target) || MD5HashFile(target) != result.latestHash)
                {
                    IO.CurrentIO.Write($"Download: {result.downloadUrl}...");
                    return DownloadSingleFile(result.downloadUrl, target, result.fileName);
                }
                else
                {
                    IO.CurrentIO.Write(string.Format(LANG_VERSION_LATEST, result.name));
                    return false;
                }
            }
            catch (Exception e)
            {
                IO.CurrentIO.Write(string.Format(LANG_UPDATE_CHECK_ERROR, guid, e.TargetSite.Name, e.Message));
                return false;
            }
        }

        private T Serializer<T>(string url)
        {
            WebClient web;
            web = new WebClient();
            Random rd = new Random();
            Stream data = web.OpenRead(url + "?rd=" + rd.Next());
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            return (T)serializer.ReadObject(data);
        }

        private string MD5HashFile(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            md5.ComputeHash(fs);
            byte[] b = md5.Hash;

            fs.Dispose();
            md5.Clear();

            StringBuilder sb = new StringBuilder(32);
            for (int i = 0; i < b.Length; i++)
                sb.Append(b[i].ToString("X2"));
            return sb.ToString();
        }

        private bool DownloadSingleFile(string dlUrl, string path, string name)
        {
            try
            {
                //打开HTTP连接，获得文件长度
                IO.CurrentIO.WriteColor($"Download {name} from {dlUrl}", ConsoleColor.Magenta);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
                HttpWebRequest Myrq = (HttpWebRequest)WebRequest.Create(dlUrl);
                HttpWebResponse myrp = (HttpWebResponse)Myrq.GetResponse();
                long totalBytes = myrp.ContentLength;
                IO.CurrentIO.WriteHelp(name, totalBytes.ToString());
                //判断是否存在已经下载的文件，如果存在，且长度相等，则直接解压

                if (File.Exists(path + "_")) File.Delete(path + "_");       //如果存在目标缓存文件，就删掉它

                Stream st = myrp.GetResponseStream();                       //获得http流
                Stream so = new FileStream(path + "_", FileMode.Create);    //创建文件流

                long totalDownloadedByte = 0;                               //已经下载的字节数量
                long updateDownloadByte = 0;                                //已经计算下载速度的字节位置
                long downloadspeed = 0;                                     //当前下载速度 byte
                Stopwatch time = new Stopwatch();                           //下载速度计时器
                time.Reset();
                time.Start();
                byte[] buffer = new byte[1024];                             //缓冲区
                int osize = st.Read(buffer, 0, buffer.Length);              //向缓冲区填入数据

                while (osize > 0)                                           //判断还剩没剩数据
                {
                    so.Write(buffer, 0, osize);                             //写入文件流

                    totalDownloadedByte += osize;      //更新当前下载进度
                    osize = st.Read(buffer, 0, buffer.Length);              //获得下一个缓冲区

                    if (time.ElapsedMilliseconds > 1000)                     //数据统计大于1000ms，则更新下载速度
                    {
                        downloadspeed = totalDownloadedByte - updateDownloadByte;                   //这段时间已经下载的数据量
                        time.Restart();                                     //计时器重新开始计时
                        updateDownloadByte = totalDownloadedByte;           //更新已经计算速度的数据位置
                        IO.CurrentIO.WriteHelp($"{downloadspeed.ToString()}Byte/s", $"{totalDownloadedByte.ToString()}Byte OK");//UI更新
                    }
                }

                IO.CurrentIO.WriteHelp($"{downloadspeed.ToString()}Byte/s", $"{totalDownloadedByte.ToString()}Byte OK");//UI更新

                so.Close();
                st.Close();

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                File.Copy(path + "_", path);
                File.Delete(path + "_");

                if (dlUrl.EndsWith("zip"))
                {
                    var zip = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path) + ".zip");
                    if (File.Exists(zip)) File.Delete(zip);
                    File.Move(path, zip);
                    using (var archive = ZipFile.Open(zip, ZipArchiveMode.Update))
                    {
                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            if (entry.Length == 0) continue;
                            IO.CurrentIO.WriteHelp(entry.FullName, entry.Length.ToString());
                            try
                            {
                                entry.ExtractToFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, entry.FullName), true);
                            }
                            catch { }
                        }
                    }

                    File.Delete(zip);
                }

                IO.CurrentIO.Write($"[{name}] Done.");
                return true;
            }
            catch (Exception e)
            {
                IO.CurrentIO.Write($"Error while {e.TargetSite.Name} : {e.Message}");
                return false;
            }
        }

        #endregion Update Tool
    }
}