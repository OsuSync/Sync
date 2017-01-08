using Microsoft.Win32;
using Sync.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sync.Source.BiliBili
{
    /// <summary>
    /// 实现发送弹幕功能
    /// </summary>
    class BiliBiliSender
    {
        private Thread formThread;
        private HTMLViewForm form;
        //窗体与循环线程

        public string user = "", password = "";
        //自动完成用户名密码

        public bool loginStauts = false;
        //登录状态

        /// <summary>
        /// 实例化发送弹幕类
        /// </summary>
        /// <param name="user">用户名(这里用作自动填写，可以为null)</param>
        /// <param name="password">密码(这里用作自动填写，可以为null)</param>
        public BiliBiliSender(string user, string password)
        {
            formThread = new Thread(show);
            formThread.SetApartmentState(ApartmentState.STA);
            formThread.Name = "ActiveXThread";
            this.user = user;
            this.password = password;
            if(Configuration.LoginCookie.Length > 0)
            {
                loginStauts = true;
            }
        }

        /// <summary>
        /// 显示窗体的单元线程
        /// </summary>
        [STAThread]
        public void show()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            form = new HTMLViewForm(this);
            
            Application.Run(form);
        }

        /// <summary>
        /// 设置IE兼容模式并开始模拟登录。
        /// </summary>
        public void login()
        {
            var appName = System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName;
            Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Internet Explorer\MAIN\FeatureControl\FEATURE_BROWSER_EMULATION", appName, 11000, RegistryValueKind.DWord);
            formThread.Start();
            loginStauts = true;
        }
        
        /// <summary>
        /// 接收回调返回的cookies信息
        /// </summary>
        /// <param name="cookies">Cookies信息</param>
        public void setCookies(string cookies)
        {
            Configuration.LoginCookie = cookies;
            ConsoleWriter.WriteColor("登陆信息保存成功！", ConsoleColor.DarkYellow);
        }

        /// <summary>
        /// 发送弹幕
        /// </summary>
        /// <param name="msg">弹幕</param>
        public void send(string msg)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://live.bilibili.com/msg/send");
            request.Method = "POST";
            long unix = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            string postData = "color=16777215&fontsize=25&mode=1&msg=" + msg + "&rnd=" + unix + "&roomid=" + Configuration.LiveRoomID + "";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentType = "application/x-www-form-urlencoded";
            request.CookieContainer = new CookieContainer();

            string[] cookies = Configuration.LoginCookie.Split("; ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            Uri live = new Uri("http://live.bilibili.com/");

            foreach (var i in cookies)
            {
                string[] cookie = i.Split("=".ToCharArray(), 2);
                request.CookieContainer.Add(live, new Cookie(cookie[0], cookie[1]));
            }

            request.ContentLength = byteArray.Length;

            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Flush();
            dataStream.Close();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            ConsoleWriter.Write("发送完成!");
        }

    }
}
