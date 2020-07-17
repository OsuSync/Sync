using Sync.Command;
using Sync.Plugins;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Sync.Tools.Builtin
{
    internal sealed class PluginCommand
    {
        #region SingleInstance
        private static PluginCommand instance = null;
        internal static PluginCommand Instance => instance ?? (instance = new PluginCommand());
        #endregion

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

        public bool Plugins(Arguments arg)
        {
            if (arg.Count == 0) return Help();
            switch (arg[0])
            {
                case "search":
                    return Search(arg[1]);

                case "update":
                    if (arg.Count > 1)
                    {
                        var part_plugin_name = arg[1];
                        return Update(part_plugin_name, arg.Any(a => a == "--no_ask"));
                    }
                    else
                        return Update(arg.Any(a => a == "--no_ask"));

                case "install":
                    return Install(arg[1]);

                case "list":
                    return List();

                case "remove":
                    return Remove(arg[1]);

                case "latest":
                    return SyncUpdateCheck(true);

                default:
                    return Help();
            }
        }

        private bool AskAgreeUpdate(UpdateData data)
        {
            IO.CurrentIO.WriteColor($"----------------\nplugin {data.name} have a new version!\nFile hash:{data.latestHash}\nDownload? (Y/N):", ConsoleColor.Green, false);
            var result = IO.CurrentIO.ReadCommand();
            return result.ToLower() == "y";
        }

        public bool ShouldDownloadUpdate(UpdateData update_data, string current_file_path, bool no_ask)
        {
            //todo : version compare
            if (!File.Exists(current_file_path)) { return true; }

            var current_file_hash = MD5HashFile(current_file_path).ToLower();

            return (current_file_hash != update_data.latestHash) && AskAgreeUpdate(update_data);
        }

        internal bool InternalUpdate(string plugin_guid, bool no_ask)
        {
            try
            {
                var result = Serializer<UpdateData>($"http://sync.mcbaka.com/api/Update/plugin/{plugin_guid}");
                IO.CurrentIO.Write($"Fetched update: {result.name} by {result.author} [{plugin_guid}]");
                var target = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins", result.fileName);

                if (ShouldDownloadUpdate(result, target, no_ask))
                {
                    IO.CurrentIO.Write($"Download: {result.downloadUrl}...");

                    if (!DownloadSingleFile(result.downloadUrl, target, result.fileName))
                    {
                        throw new Exception("Download update files failed!");
                    }
                }
                else
                {
                    IO.CurrentIO.Write(string.Format(LANG_VERSION_LATEST_OR_CANEL, result.name));
                }

                return true;
            }
            catch (Exception e)
            {
                IO.CurrentIO.WriteColor(string.Format(LANG_UPDATE_ERROR, e.TargetSite.Name, e.Message), ConsoleColor.Yellow);
            }

            return false;
        }

        internal void InternalUpdate(IEnumerable<Plugin> update_plugins, bool no_ask)
        {
            foreach (var item in update_plugins)
            {
                InternalUpdate(item.getGuid(), no_ask);
            }

            if (update_plugins.Count() != 0)
                RequireRestart(LANG_UPDATE_DONE);
        }

        private bool Update(bool no_ask = false)
        {
            IEnumerable<Plugin> plugins = SyncHost.Instance.EnumPluings();

            InternalUpdate(plugins, no_ask);

            return true;
        }

        private bool Update(string part_plugin_name, bool no_ask = false)
        {
            if (string.IsNullOrWhiteSpace(part_plugin_name))
                return Update(no_ask);

            IEnumerable<Plugin> plugins = from plugin in SyncHost.Instance.EnumPluings()
                                          where plugin.Name.ToLower().Contains(part_plugin_name.ToLower())
                                          select plugin;

            InternalUpdate(plugins, no_ask);

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

        internal bool InstallByKeyword(string keyword, bool requireRestart = true)
        {
            if (Serializer<UpdateData[]>($"http://sync.mcbaka.com/api/Update/search/{keyword}") is UpdateData[] datas)
            {
                if (datas.Length == 0 || InternalUpdate(datas[0].guid, true))
                {
                    if (requireRestart) RequireRestart(LANG_INSTALL_DONE);
                    return true;
                }
                else return false;
            }
            return false;
        }

        private bool Install(string guid)
        {
            if (InternalUpdate(guid, true))
            {
                RequireRestart(LANG_INSTALL_DONE);
                return true;
            }
            else
            {
                return InstallByKeyword(guid);
            }
        }

        private bool Remove(string name)
        {
            var type = SyncHost.Instance.EnumPluings().FirstOrDefault(p => p.Name.ToLower().Contains(name.ToLower()));
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
            var list = SyncHost.Instance.EnumPluings();
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

        internal bool SyncUpdateCheck(bool download = false)
        {
            try
            {
                IO.CurrentIO.WriteColor("Fetch Sync update..", ConsoleColor.Cyan);
                var result = Serializer<SyncUpdate>($"http://sync.mcbaka.com/api/Update/latest");

                if (!File.Exists(Updater.CurrentFullSourceEXEPath) || MD5HashFile(Updater.CurrentFullSourceEXEPath) != result.versionHash)
                {
                    if (download)
                    {
                        IO.CurrentIO.Write($"Download: {result.downloadURL}...");
                        DownloadSingleFile(result.downloadURL, Updater.CurrentFullUpdateEXEPath, "Sync");
                        RequireRestart("Update downloaded. Restart to apply effect");
                    }
                    else
                        IO.CurrentIO.WriteColor($"There is a new version Sync! please visit https://github.com/OsuSync/Sync/releases/latest " +
                            $", you can type \"plugins latest\" in Sync for automatically updating self.", ConsoleColor.Cyan);
                }
                else
                    IO.CurrentIO.WriteColor("Sync update check done.Enjoy~", ConsoleColor.Cyan);
            }
            catch (Exception e)
            {
                IO.CurrentIO.WriteColor("Fetch Sync update info failed,please check your network if it can able to connect http://sync.mcbaka.com/", ConsoleColor.Red);
                return false;
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

        private void RequireRestart(string msg)
        {
            IO.CurrentIO.WriteColor($"{msg}? (Y/N):", ConsoleColor.Green, false);
            var result = IO.CurrentIO.ReadCommand();
            if (result.ToLower().StartsWith("y")) SyncHost.Instance.RestartSync();
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
                string convertdTotal = (totalBytes / 1024).ToString("0.00");
                IO.CurrentIO.WriteHelp(name, $"Total: {convertdTotal} KB");
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
                        IO.CurrentIO.WriteHelp($"{(downloadspeed / 1024).ToString("0.0")}KB/s", $"{(totalDownloadedByte / 1024).ToString("0.0")}/{convertdTotal} KB");//UI更新
                    }
                }

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
                                string fileFullName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, entry.FullName);
                                string directoryName = Path.GetDirectoryName(fileFullName);
                                if (!Directory.Exists(directoryName))
                                    Directory.CreateDirectory(directoryName);
                                entry.ExtractToFile(fileFullName, true);
                            }
                            catch { }
                        }
                    }

                    File.Delete(zip);
                }

                IO.CurrentIO.WriteColor($"[{name}] Done.", ConsoleColor.Green);
                return true;
            }
            catch (Exception e)
            {
                IO.CurrentIO.WriteColor($"Error while {e.TargetSite.Name} : {e.Message}", ConsoleColor.Red);
                return false;
            }
        }
    }
}
