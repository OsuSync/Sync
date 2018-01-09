using Sync.Command;
using Sync.Plugins;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.IO.Packaging;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Tools
{
    public class InternalUpdate : Plugin
    {
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

        public InternalUpdate() : base("Internal Updater", "Deliay")
        {
        }

        public override void OnEnable()
        {
            this.EventBus.BindEvent<PluginEvents.InitCommandEvent>(p => {
                Func<string, CommandDelegate, string, bool> addCmd = p.Commands.Dispatch.bind;
                addCmd("plugins", Plugins, "Install & Update Plugins online, to get help type 'plugins help'");
            });
            Updater.update = this;
        }

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
                default:
                    return Help();
            }

        }

        private bool Help()
        {
            IO.CurrentIO.Write("Help for 'plugins' command:");
            IO.CurrentIO.WriteHelp("install", "install [guid/name] Install plugin");
            IO.CurrentIO.WriteHelp("remove", "remove [name] Remove plugin by name");
            IO.CurrentIO.WriteHelp("search", "search [keyword] Search plugins");
            IO.CurrentIO.WriteHelp("update", "Check update for Sync and Plugins");
            IO.CurrentIO.WriteHelp("list", "List current installed plugins");
            return true;
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

        private bool Remove(string name)
        {
            var type = getHoster().EnumPluings().FirstOrDefault(p => p.Name.Contains(name));
            if(type == null)
            {
                IO.CurrentIO.WriteColor($"Plugin {name} not exist", ConsoleColor.Red);
                return false;
            }
            else
            {
                var result = Serializer<UpdateData[]>($"http://sync.mcbaka.com/api/Update/search/{name}")?[0];
                var target = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins", Path.GetFileName(result.fileName));
                if (File.Exists(target)) File.Delete(target);

                RequireRestart("Remove done. Restart to apply effect");
                return true;
            }
        }

        private void RequireRestart(string msg)
        {
            IO.CurrentIO.WriteColor($"{msg}? (Y/N):", ConsoleColor.Green, false);
            var result = IO.CurrentIO.ReadCommand();
            if (result.ToLower().StartsWith("y")) SyncHost.Instance.RestartSync();
        }

        private bool Install(string guid)
        {
            if(CheckUpdate(guid))
            {
                RequireRestart("Install done. Restart to load plugin");
                return true;
            }
            else
            {
                if (Serializer<UpdateData[]>($"http://sync.mcbaka.com/api/Update/search/{guid}") is UpdateData[] datas)
                {
                    if (CheckUpdate(datas[0].guid))
                    {
                        RequireRestart("Install done. Restart to load plugin");
                        return true;
                    }
                    else return false;
                }
                return false;
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
                    if(MD5HashFile(target).ToLower() != result.latestHash)
                    {
                        IO.CurrentIO.Write($"Download: {result.downloadUrl}...");
                        if(!DownloadSingleFile(result.downloadUrl, target, result.fileName))
                        {
                            IO.CurrentIO.WriteColor("Download Failed!", ConsoleColor.Red);
                        }
                    }
                    else
                    {
                        IO.CurrentIO.Write($"{result.name} Up-to-date");
                    }
                }
                catch (Exception e)
                {
                    IO.CurrentIO.Write($"Can't find plugin [{item.Name}] update :  {e.TargetSite.Name} : {e.Message}");
                    continue;
                }

            }

            RequireRestart("Update done. Restart to reload plugin");
            return true;
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
                    return DownloadSingleFile(result.downloadUrl, target, result.fileName); ;
                }
                else
                {
                    IO.CurrentIO.Write($"{result.name} Up-to-date");
                    return false;
                }
            }
            catch (Exception e)
            {
                IO.CurrentIO.Write($"Can't find plugin [{guid}] update :  {e.TargetSite.Name} : {e.Message}");
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

                if(dlUrl.EndsWith("zip"))
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
                            } catch { }
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


    }

    static class Updater
    {
        private const string SourceEXEName = "Sync.exe";
        private const string UpdateEXEName = "Sync_update.exe";
        private const string UpdateArg = "--update";
        private static readonly string CurrentEXEName = Path.GetFileName(Process.GetCurrentProcess().Modules[0].FileName);
        private static readonly string CurrentPath = AppDomain.CurrentDomain.BaseDirectory;
        internal static InternalUpdate update;

        public static bool ApplyUpdate(string[] args)
        {
            if (CurrentEXEName == SourceEXEName)
            {
                if (args.Length > 0 && args[0] == UpdateArg)
                {
                    Process.GetProcesses().FirstOrDefault(p => p.MainModule.FileName.EndsWith(UpdateEXEName))?.Kill();
                    File.Delete(Path.Combine(CurrentPath, UpdateEXEName));
                }
                else if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, UpdateEXEName)))
                {
                    Process.Start(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, UpdateEXEName));
                    return true;
                }
            }
            else if (CurrentEXEName == UpdateEXEName)
            {
                var dest = Path.Combine(CurrentPath, SourceEXEName);
                Process.GetProcesses().FirstOrDefault(p => p.MainModule.FileName.EndsWith(SourceEXEName))?.Kill();
                File.Delete(dest);
                File.Copy(Path.Combine(CurrentPath, CurrentEXEName), dest);
                Process.Start(dest, UpdateArg);
                return true;
            }
            return false;
        }
    }
}
