using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace Sync.Tools
{
    public static class ProgramUpdater
    {
        static string updateCheckLink = "http://d.mcbaka.com/";

        /// <summary>
        /// 检查更新
        /// </summary>
        public static async void CheckUpdate()
        {
            await Task.Run(() =>
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.CreateHttp(updateCheckLink);
                var response = (HttpWebResponse)request.GetResponse();
                var responseHtml = new StreamReader(response.GetResponseStream()).ReadToEnd();

                //文件格式osuSync_v{version}.zip
                MatchCollection results = Regex.Matches(responseHtml, @"<a href=""osuSync_v([\w\.]+).zip"">osuSync_v(\1).zip</a>");

                string responseVersion = "";

                foreach (Match file in results)
                {
                    if (responseVersion.Length == 0)
                        responseVersion = file.Groups[1].Value;
                    else
                    {
                        int cmp = compareVersion(responseVersion, file.Groups[1].Value);

                        if (cmp < 0)
                            responseVersion = file.Groups[1].Value;
                    }
                }

                if (responseVersion.Length == 0)
                    return;

                string[] currentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString().Split('.');
                string[] ftpFileVersion = responseVersion.Split('.');

                int result = compareVersion(currentVersion, ftpFileVersion);

                if (result < 0)
                {
                    MessageBoxResult btnResult = MessageBox.Show($"osuSync有新版本啦\n最新版本:{responseVersion}\n当前版本:{Assembly.GetExecutingAssembly().GetName().Version.ToString()}\n是否下载更新压缩包?", "更新检查", MessageBoxButton.YesNo);
                    if (btnResult==(MessageBoxResult.Yes))
                    {
                        try
                        {
                            using (WebClient client = new WebClient())
                            {
                                //获取最新版本文件并保存
                                var data = client.DownloadData(updateCheckLink + $"osuSync_v{ responseVersion}.zip");
                                //保存到根目录的update目录
                                Directory.CreateDirectory($"{AppDomain.CurrentDomain.BaseDirectory}\\update");
                                System.IO.File.WriteAllBytes($"{AppDomain.CurrentDomain.BaseDirectory}update\\osuSync_v{responseVersion}.zip", data);
                            }
                            MessageBox.Show("下载完成!", "更新检查", MessageBoxButton.OK);
                            Process.Start(AppDomain.CurrentDomain.BaseDirectory + "update");
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show($"更新失败，原因{e.Message}", "更新检查", MessageBoxButton.OK);
                        }
                    }
                }
            });
        }


        /// <summary>
        /// 版本对比，格式是x.xx.x.x
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        static int compareVersion(string a, string b) => compareVersion(a.Split('.'), b.Split('.'));
        
        /// <summary>
        /// 版本对比
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        static int compareVersion(string[] a,string[] b)
        {
            int index = 0;
            while (true)
            {
                if (index >= a.Length && index >= b.Length)
                    return 0;

                int va = index >= a.Length ? 0 : int.Parse(a[index]);
                int vb = index >= b.Length ? 0 : int.Parse(b[index]);

                index++;

                if (va == vb)
                    continue;
                return va - vb;
            }
        }
    }
}
