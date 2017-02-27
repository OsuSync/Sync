using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sync;
using Sync.Tools;
using System.Runtime.InteropServices;
using Sync.Source;
using System.Drawing.Drawing2D;

namespace DefaultGUI
{
    public partial class frmDefault : Form, SyncIO, IDisposable
    {

        [DllImport("user32.dll", SetLastError = true)]
        static extern int FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        static extern bool ShowWindow(int hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        [DllImport("user32.dll")]
        static extern bool ReleaseCapture();

        private static Color BorderColor = Color.FromArgb(0, 122, 204);

        public frmDefault()
        {
            InitializeComponent();
        }

        private void frmDefault_Load(object sender, EventArgs e)
        {
            ShowMe();
        }

        public void ready()
        {
            Invoke(new MethodInvoker(() => {
                string formName = Console.Title;
                int h = FindWindow("ConsoleWindowClass", formName);
                Task.Delay(400);
                ShowWindow(h, 0);
                txtBotIRC.Text = Configuration.BotIRC;
                txtBotIRCPassword.Text = Configuration.BotIRCPassword;
                txtTargetIRC.Text = Configuration.TargetIRC;
                txtLiveID.Text = Configuration.LiveRoomID;
                cbSources.Items.Clear();
                if(DefaultGUI.hoster?.Sources != null)
                foreach (var item in DefaultGUI.hoster?.Sources?.SourceList)
                {
                    cbSources.Items.Add(item);
                }
                cbSources.SelectedItem = DefaultGUI.hoster?.SyncInstance?.Connector?.GetSource();
                IO.SetIO(this);

                var c = new AutoCompleteStringCollection();
                if(DefaultGUI.hoster != null) c.AddRange(DefaultGUI.hoster?.Commands?.Dispatch?.getCommandsHelp().Keys.ToArray());
                txtCmd.AutoCompleteCustomSource = c;
            }));
        }

        delegate void AppendTextDelegate(string text);
        delegate void SetColorDelegate(Color text);
        delegate void SetHeightDelegate(int change);
        delegate void UpdateStautsDelegate(bool danmaku, bool irc);

        public void UpdateStatus(bool danma, bool irc)
        {
            Invoke(new UpdateStautsDelegate((a, b) => {
                lblTipsLiveStatus.ForeColor = a ? Color.Green : Color.Red;
                lblTipsOSUStatus.ForeColor = b ? Color.Green : Color.Red;
            }), new object[] { danma, irc });
        }

        public void UpdateStautsAuto()
        {
            if(DefaultGUI.hoster != null)
            UpdateStatus(DefaultGUI.hoster.SyncInstance.Connector.SourceStatus, DefaultGUI.hoster.SyncInstance.Connector.IRCStatus);
        }

        public void ShowMe()
        {
            Invoke(new MethodInvoker(() => Show()));
            ready();
        }

        public void CloseMe()
        {
            Invoke(new MethodInvoker(() => { Close(); Application.ExitThread(); }));
        }

        public void RefreshDelegate()
        {
            Invoke(new MethodInvoker(() => { Refresh(); }));
        }

        public void AppendText(string text)
        {
            Invoke(new AppendTextDelegate((t) => { txtLog.AppendText(t);txtLog.ScrollToCaret(); }), new object[] { text });
        }

        public void SetHeight(int change)
        {
            Invoke(new SetHeightDelegate((v) => Height += v), new object[] { change });
        }

        public void Clear()
        {
            Invoke(new MethodInvoker(() => txtLog.Clear()));
        }

        public void SetColor(Color c)
        {
            Invoke(new SetColorDelegate((t) => txtLog.SelectionColor = t), new object[] { c });
        }

        public string ReadCommand()
        {
            while(DefaultGUI.InputFlag)
            {

                Thread.Sleep(1);
            }
            DefaultGUI.InputFlag = true;
            var command = txtCmd.Text;
            Invoke(new MethodInvoker(() => txtCmd.Text = ""));
            return command;
        }

        public void Write(string msg, bool newline = true, bool time = true)
        {
            UpdateStautsAuto();
            AppendText((time ? "[" + DateTime.Now.ToLongTimeString() + "] " : "") + msg + (newline ? "\n" : ""));
        }

        public void WriteColor(string text, ConsoleColor color, bool newline = true, bool time = true)
        {
            SetColor(Color.FromName(color.ToString()));
            Write(text, newline, time);
            SetColor(Color.White);
        }

        public void WriteConfig()
        {
            Write("正在读取配置文件....\n");
            Write("房间ID: \t\t" + Configuration.LiveRoomID);
            Write("主号IRC: \t\t" + Configuration.TargetIRC);
            Write("BotIRC: \t\t" + Configuration.BotIRC);
            Write("BotIRC密码长度: \t" + Configuration.BotIRCPassword.Length);
            Write("完成.\n");
        }

        public void WriteHelp()
        {
            WriteHelp("命令", "描述");
            WriteHelp("======", "======");
            foreach (var item in DefaultGUI.hoster.Commands.Dispatch.getCommandsHelp())
            {
                WriteHelp(item.Key, item.Value);
            }
            WriteHelp("======", "======");
            Write("", true, false);
        }

        public void WriteHelp(string cmd, string desc)
        {
            WriteColor(cmd.PadRight(10), ConsoleColor.Cyan, false, false);
            WriteColor(desc, ConsoleColor.White, true, false);
        }

        public void WriteStatus(SyncConnector instance)
        {
            WriteColor("配置文件: ", ConsoleColor.Blue, false);
            if (Configuration.LiveRoomID.Length > 0 && Configuration.TargetIRC.Length > 0 && Configuration.BotIRC.Length > 0 && Configuration.BotIRCPassword.Length > 0)
                WriteColor("OK, 房间ID:" + Configuration.LiveRoomID, ConsoleColor.Green, true, false);
            else
                WriteColor("未配置", ConsoleColor.Red, true, false);

            WriteColor("BiliBili Live连接: ", ConsoleColor.Blue, false);
            if (instance.SourceStatus)
                WriteColor("已连接", ConsoleColor.Green, true, false);
            else
                WriteColor("等待连接", ConsoleColor.Red, true, false);

            WriteColor("osu! IRC(聊天): ", ConsoleColor.Blue, false);
            if (instance.IRCStatus)
                WriteColor("已连接", ConsoleColor.Green, true, false);
            else
                WriteColor("等待连接", ConsoleColor.Red, true, false);

            if (SyncManager.loginable)
            {
                WriteColor("发送弹幕: ", ConsoleColor.Blue, false);
                if (((Sync.Source.ISendable)instance.GetSource()).LoginStauts())
                    WriteColor("已登录", ConsoleColor.Green, true, false);
                else
                    WriteColor("未连接", ConsoleColor.Red, true, false);
            }
        }

        public void WriteWelcome()
        {
            Write("欢迎使用 osu直播弹幕同步工具 ver " +
            System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString());
        }

        private void lblClose_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void txtCmd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                DefaultGUI.InputFlag = false;
            }
        }

        private void lblMin_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void txtLog_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            txtCmd.Focus();
            txtCmd_KeyDown(sender, new KeyEventArgs(e.KeyCode));
        }

        private async void lblCollapse_Click(object sender, EventArgs e)
        {
            await Task.Run(new Action(() => {
                if (Height == 345)
                {
                    while (Height != 525) { SetHeight(5); Task.Delay(2); };
                }
                else
                {
                    while (Height != 345) { SetHeight(-5); Task.Delay(2); };
                }
                RefreshDelegate();
            }));
        }

        private void cmdExit_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void cmdConsole_Click(object sender, EventArgs e)
        {
            string formName = Console.Title;
            int h = FindWindow("ConsoleWindowClass", formName);
            ShowWindow(h, 1);
            IO.SetIO(new NConsoleWriter());
            DefaultGUI.InputFlag = false;
            Visible = false;
        }

        private void txtBotIRC_TextChanged(object sender, EventArgs e)
        {
            Configuration.BotIRC = txtBotIRC.Text;
        }

        private void txtBotIRCPassword_TextChanged(object sender, EventArgs e)
        {
            Configuration.BotIRCPassword = txtBotIRCPassword.Text;
        }

        private void txtTargetIRC_TextChanged(object sender, EventArgs e)
        {
            Configuration.TargetIRC = txtTargetIRC.Text;
        }

        private void txtLiveID_TextChanged(object sender, EventArgs e)
        {
            Configuration.LiveRoomID = txtLiveID.Text;
        }

        private void cbSources_SelectedIndexChanged(object sender, EventArgs e)
        {
            Configuration.Provider = (cbSources.SelectedItem as ISourceBase).getSourceName();
        }

        private void lblTitle_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, 161, 2, 0);
            }
            base.OnMouseDown(e);
        }

        private void lblTitle_MouseUp(object sender, MouseEventArgs e)
        {
            base.OnMouseUp(e);
        }

        private void cmdStart_Click(object sender, EventArgs e)
        {
            DefaultGUI.hoster.SyncInstance.Connector.Connect();
            cmdStart.Enabled = false;
            cmdStop.Enabled = true;
        }

        private void cmdStop_Click(object sender, EventArgs e)
        {
            DefaultGUI.hoster.SyncInstance.Connector.Disconnect();
            cmdStart.Enabled = true;
            cmdStop.Enabled = false;
        }

        private void cmdLogin_Click(object sender, EventArgs e)
        {
            if(SyncManager.loginable)
            {
                ISendable s = DefaultGUI.hoster.SyncInstance.Connector.GetSource() as ISendable;
                s.Login(null, null);
            }
        }

        private void DrawRoundRect(Graphics graphics, Control targer, Color color)
        {
            float X = float.Parse(targer.Width.ToString()) - 1;
            float Y = float.Parse(targer.Height.ToString()) - 1;
            PointF[] points =
            {
                new PointF(0,0),
                new PointF(X,0),
                new PointF(X,Y),
                new PointF(0,Y),
                new PointF(0,0)
            };
            GraphicsPath path = new GraphicsPath();
            path.AddLines(points);
            Pen pen = new Pen(color, 1);
            pen.DashStyle = DashStyle.Solid;
            graphics.DrawPath(pen, path);
        }

        private void ControlsPaint(object sender, PaintEventArgs e)
        {
            DrawRoundRect(e.Graphics, (Control)sender, BorderColor);
        }

        private void frmDefault_FormClosing(object sender, FormClosingEventArgs e)
        {
            cbSources.Items.Clear();
        }
    }
}
