using Sync.Tools;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace DefaultPlugin.Sources
{
    /// <summary>
    /// 用于模仿bilibili登录实现，抓取cookies的窗体
    /// </summary>
    public partial class HTMLViewForm : Form
    {
        BiliBiliSender parent;
        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool InternetGetCookieEx(string pchURL, string pchCookieName, StringBuilder pchCookieData, ref System.UInt32 pcchCookieData, int dwFlags, IntPtr lpReserved);

        /// <summary>
        /// 初始化窗口
        /// </summary>
        /// <param name="p">传入发送父类BiliBiliSender</param>
        public HTMLViewForm(object p)
        {
            InitializeComponent();
            parent = (BiliBiliSender)p;
        }

        /// <summary>
        /// 文档准备完成时回调
        /// </summary>
        private void viewer_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            this.Text = e.Url.AbsolutePath;

            //如果在主页就跳转到登录页面
            if(e.Url.AbsolutePath.EndsWith("/"))
                viewer.Navigate("https://account.bilibili.com/ajax/miniLogin/minilogin");

            //如果在指定房间号，说明登录成功，抓取cookies并关闭窗体
            if (viewer.Document.Url.AbsolutePath.EndsWith(BiliBili.BiliBili.RoomID.ToString()))
            {
                parent.setCookies(GetCookies("http://live.bilibili.com/"));
                this.Close();
            }

            //如果URI结尾是redirect，说明登录成功，接下来跳转到指定房间
            if (e.Url.AbsolutePath.EndsWith("redirect"))
                viewer.Navigate("http://live.bilibili.com/" + BiliBili.BiliBili.RoomID);
            
            //实现简易自动完成
            if (e.Url.AbsolutePath.EndsWith("minilogin") && parent.user != null)
                viewer.Document.GetElementById("login-username").SetAttribute("value", parent.user);
            if (e.Url.AbsolutePath.EndsWith("minilogin") && parent.password != null)
                viewer.Document.GetElementById("login-passwd").SetAttribute("value", parent.password);
        }

        /// <summary>
        /// 通过系统API调用获取Cookies
        /// </summary>
        /// <param name="url">指定域</param>
        /// <returns></returns>
        private string GetCookies(string url)
        {
            uint datasize = 1024;
            StringBuilder cookieData = new StringBuilder((int)datasize);
            if (!InternetGetCookieEx(url, null, cookieData, ref datasize, 0x2000, IntPtr.Zero))
            {
                if (datasize < 0)
                    return null;

                cookieData = new StringBuilder((int)datasize);
                if (!InternetGetCookieEx(url, null, cookieData, ref datasize, 0x00002000, IntPtr.Zero))
                    return null;
            }
            return cookieData.ToString();
        }

    }
}
