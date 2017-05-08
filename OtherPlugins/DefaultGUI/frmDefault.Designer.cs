using static DefaultGUI.Language;

namespace DefaultGUI
{
    partial class frmDefault
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDefault));
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.lblTipsTargetIRC = new System.Windows.Forms.Label();
            this.txtTargetIRC = new System.Windows.Forms.TextBox();
            this.lblTipsBotIRC = new System.Windows.Forms.Label();
            this.txtBotIRC = new System.Windows.Forms.TextBox();
            this.txtBotIRCPassword = new System.Windows.Forms.TextBox();
            this.lblTipsBotIRCPass = new System.Windows.Forms.Label();
            this.lblTipsStauts = new System.Windows.Forms.Label();
            this.lblTipsLiveStatus = new System.Windows.Forms.Label();
            this.lblTipsOSUStatus = new System.Windows.Forms.Label();
            this.cmdStart = new System.Windows.Forms.Button();
            this.cmdStop = new System.Windows.Forms.Button();
            this.cmdLogin = new System.Windows.Forms.Button();
            this.cmdConsole = new System.Windows.Forms.Button();
            this.cmdExit = new System.Windows.Forms.Button();
            this.txtLiveID = new System.Windows.Forms.TextBox();
            this.lblTipsLiveID = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.cbSources = new System.Windows.Forms.ComboBox();
            this.txtCmd = new System.Windows.Forms.TextBox();
            this.lblClose = new System.Windows.Forms.Button();
            this.lblMin = new System.Windows.Forms.Button();
            this.lblCollapse = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.txtLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtLog.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.txtLog.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtLog.ForeColor = System.Drawing.Color.White;
            this.txtLog.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.txtLog.Location = new System.Drawing.Point(12, 60);
            this.txtLog.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.txtLog.ShortcutsEnabled = false;
            this.txtLog.Size = new System.Drawing.Size(559, 243);
            this.txtLog.TabIndex = 0;
            this.txtLog.Text = "";
            this.txtLog.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtLog_PreviewKeyDown);
            // 
            // lblTipsTargetIRC
            // 
            this.lblTipsTargetIRC.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblTipsTargetIRC.ForeColor = System.Drawing.Color.White;
            this.lblTipsTargetIRC.Location = new System.Drawing.Point(300, 13);
            this.lblTipsTargetIRC.Name = "lblTipsTargetIRC";
            this.lblTipsTargetIRC.Size = new System.Drawing.Size(61, 24);
            this.lblTipsTargetIRC.TabIndex = 1;
            this.lblTipsTargetIRC.Text = UI_TIPS_TARGET_IRC;
            this.lblTipsTargetIRC.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtTargetIRC
            // 
            this.txtTargetIRC.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(55)))));
            this.txtTargetIRC.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTargetIRC.Font = new System.Drawing.Font("Microsoft YaHei Light", 12F);
            this.txtTargetIRC.ForeColor = System.Drawing.SystemColors.Info;
            this.txtTargetIRC.Location = new System.Drawing.Point(366, 13);
            this.txtTargetIRC.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.txtTargetIRC.Name = "txtTargetIRC";
            this.txtTargetIRC.Size = new System.Drawing.Size(180, 29);
            this.txtTargetIRC.TabIndex = 2;
            this.txtTargetIRC.TextChanged += new System.EventHandler(this.txtTargetIRC_TextChanged);
            // 
            // lblTipsBotIRC
            // 
            this.lblTipsBotIRC.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblTipsBotIRC.ForeColor = System.Drawing.Color.White;
            this.lblTipsBotIRC.Location = new System.Drawing.Point(12, 14);
            this.lblTipsBotIRC.Name = "lblTipsBotIRC";
            this.lblTipsBotIRC.Size = new System.Drawing.Size(98, 28);
            this.lblTipsBotIRC.TabIndex = 3;
            this.lblTipsBotIRC.Text = UI_TIPS_BOTIRC;
            this.lblTipsBotIRC.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtBotIRC
            // 
            this.txtBotIRC.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(55)))));
            this.txtBotIRC.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBotIRC.Font = new System.Drawing.Font("Microsoft YaHei Light", 12F);
            this.txtBotIRC.ForeColor = System.Drawing.SystemColors.Info;
            this.txtBotIRC.Location = new System.Drawing.Point(112, 13);
            this.txtBotIRC.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtBotIRC.Name = "txtBotIRC";
            this.txtBotIRC.Size = new System.Drawing.Size(180, 29);
            this.txtBotIRC.TabIndex = 4;
            this.txtBotIRC.TextChanged += new System.EventHandler(this.txtBotIRC_TextChanged);
            // 
            // txtBotIRCPassword
            // 
            this.txtBotIRCPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(55)))));
            this.txtBotIRCPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBotIRCPassword.Font = new System.Drawing.Font("Microsoft YaHei Light", 12F);
            this.txtBotIRCPassword.ForeColor = System.Drawing.SystemColors.Info;
            this.txtBotIRCPassword.Location = new System.Drawing.Point(112, 43);
            this.txtBotIRCPassword.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtBotIRCPassword.Name = "txtBotIRCPassword";
            this.txtBotIRCPassword.Size = new System.Drawing.Size(180, 29);
            this.txtBotIRCPassword.TabIndex = 6;
            this.txtBotIRCPassword.UseSystemPasswordChar = true;
            this.txtBotIRCPassword.TextChanged += new System.EventHandler(this.txtBotIRCPassword_TextChanged);
            // 
            // lblTipsBotIRCPass
            // 
            this.lblTipsBotIRCPass.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblTipsBotIRCPass.ForeColor = System.Drawing.Color.White;
            this.lblTipsBotIRCPass.Location = new System.Drawing.Point(9, 43);
            this.lblTipsBotIRCPass.Name = "lblTipsBotIRCPass";
            this.lblTipsBotIRCPass.Size = new System.Drawing.Size(103, 24);
            this.lblTipsBotIRCPass.TabIndex = 5;
            this.lblTipsBotIRCPass.Text = UI_TIPS_BOTIRC_PASS;
            this.lblTipsBotIRCPass.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTipsStauts
            // 
            this.lblTipsStauts.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblTipsStauts.ForeColor = System.Drawing.Color.White;
            this.lblTipsStauts.Location = new System.Drawing.Point(38, 79);
            this.lblTipsStauts.Name = "lblTipsStauts";
            this.lblTipsStauts.Size = new System.Drawing.Size(78, 24);
            this.lblTipsStauts.TabIndex = 7;
            this.lblTipsStauts.Text = UI_TIPS_STATUS;
            this.lblTipsStauts.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTipsLiveStatus
            // 
            this.lblTipsLiveStatus.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblTipsLiveStatus.ForeColor = System.Drawing.Color.Red;
            this.lblTipsLiveStatus.Location = new System.Drawing.Point(42, 79);
            this.lblTipsLiveStatus.Name = "lblTipsLiveStatus";
            this.lblTipsLiveStatus.Size = new System.Drawing.Size(139, 24);
            this.lblTipsLiveStatus.TabIndex = 8;
            this.lblTipsLiveStatus.Text = UI_TIPS_DANMAKU;
            this.lblTipsLiveStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTipsOSUStatus
            // 
            this.lblTipsOSUStatus.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblTipsOSUStatus.ForeColor = System.Drawing.Color.Red;
            this.lblTipsOSUStatus.Location = new System.Drawing.Point(103, 79);
            this.lblTipsOSUStatus.Name = "lblTipsOSUStatus";
            this.lblTipsOSUStatus.Size = new System.Drawing.Size(145, 24);
            this.lblTipsOSUStatus.TabIndex = 9;
            this.lblTipsOSUStatus.Text = UI_TIPS_OSU_IRC;
            this.lblTipsOSUStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmdStart
            // 
            this.cmdStart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.cmdStart.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.cmdStart.FlatAppearance.BorderSize = 0;
            this.cmdStart.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.cmdStart.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.cmdStart.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.cmdStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdStart.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmdStart.ForeColor = System.Drawing.Color.White;
            this.cmdStart.Location = new System.Drawing.Point(35, 108);
            this.cmdStart.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmdStart.Name = "cmdStart";
            this.cmdStart.Size = new System.Drawing.Size(94, 55);
            this.cmdStart.TabIndex = 12;
            this.cmdStart.Text = UI_BOTTON_START;
            this.cmdStart.UseVisualStyleBackColor = false;
            this.cmdStart.Click += new System.EventHandler(this.cmdStart_Click);
            // 
            // cmdStop
            // 
            this.cmdStop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.cmdStop.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.cmdStop.FlatAppearance.BorderSize = 0;
            this.cmdStop.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.cmdStop.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.cmdStop.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.cmdStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdStop.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmdStop.ForeColor = System.Drawing.Color.White;
            this.cmdStop.Location = new System.Drawing.Point(137, 108);
            this.cmdStop.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmdStop.Name = "cmdStop";
            this.cmdStop.Size = new System.Drawing.Size(94, 55);
            this.cmdStop.TabIndex = 13;
            this.cmdStop.Text = UI_BOTTON_STOP;
            this.cmdStop.UseVisualStyleBackColor = false;
            this.cmdStop.Click += new System.EventHandler(this.cmdStop_Click);
            // 
            // cmdLogin
            // 
            this.cmdLogin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.cmdLogin.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.cmdLogin.FlatAppearance.BorderSize = 0;
            this.cmdLogin.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.cmdLogin.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.cmdLogin.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.cmdLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdLogin.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmdLogin.ForeColor = System.Drawing.Color.White;
            this.cmdLogin.Location = new System.Drawing.Point(238, 108);
            this.cmdLogin.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmdLogin.Name = "cmdLogin";
            this.cmdLogin.Size = new System.Drawing.Size(94, 55);
            this.cmdLogin.TabIndex = 14;
            this.cmdLogin.Text = UI_BOTTON_LOGIN_DANMAKU;
            this.cmdLogin.UseVisualStyleBackColor = false;
            this.cmdLogin.Click += new System.EventHandler(this.cmdLogin_Click);
            // 
            // cmdConsole
            // 
            this.cmdConsole.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.cmdConsole.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.cmdConsole.FlatAppearance.BorderSize = 0;
            this.cmdConsole.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.cmdConsole.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.cmdConsole.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.cmdConsole.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdConsole.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmdConsole.ForeColor = System.Drawing.Color.White;
            this.cmdConsole.Location = new System.Drawing.Point(340, 108);
            this.cmdConsole.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmdConsole.Name = "cmdConsole";
            this.cmdConsole.Size = new System.Drawing.Size(94, 55);
            this.cmdConsole.TabIndex = 15;
            this.cmdConsole.Text = UI_BOTTON_SWITCH_CON;
            this.cmdConsole.UseVisualStyleBackColor = false;
            this.cmdConsole.Click += new System.EventHandler(this.cmdConsole_Click);
            // 
            // cmdExit
            // 
            this.cmdExit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.cmdExit.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.cmdExit.FlatAppearance.BorderSize = 0;
            this.cmdExit.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.cmdExit.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.cmdExit.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.cmdExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdExit.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmdExit.ForeColor = System.Drawing.Color.White;
            this.cmdExit.Location = new System.Drawing.Point(441, 108);
            this.cmdExit.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmdExit.Name = "cmdExit";
            this.cmdExit.Size = new System.Drawing.Size(94, 55);
            this.cmdExit.TabIndex = 16;
            this.cmdExit.Text = UI_BOTTON_EXIT;
            this.cmdExit.UseVisualStyleBackColor = false;
            this.cmdExit.Click += new System.EventHandler(this.cmdExit_Click);
            // 
            // txtLiveID
            // 
            this.txtLiveID.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(55)))));
            this.txtLiveID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLiveID.Font = new System.Drawing.Font("Microsoft YaHei Light", 12F);
            this.txtLiveID.ForeColor = System.Drawing.SystemColors.Info;
            this.txtLiveID.Location = new System.Drawing.Point(366, 43);
            this.txtLiveID.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtLiveID.Name = "txtLiveID";
            this.txtLiveID.Size = new System.Drawing.Size(180, 29);
            this.txtLiveID.TabIndex = 18;
            this.txtLiveID.TextChanged += new System.EventHandler(this.txtLiveID_TextChanged);
            // 
            // lblTipsLiveID
            // 
            this.lblTipsLiveID.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblTipsLiveID.ForeColor = System.Drawing.Color.White;
            this.lblTipsLiveID.Location = new System.Drawing.Point(300, 44);
            this.lblTipsLiveID.Name = "lblTipsLiveID";
            this.lblTipsLiveID.Size = new System.Drawing.Size(61, 23);
            this.lblTipsLiveID.TabIndex = 17;
            this.lblTipsLiveID.Text = UI_TIPS_LIVE_ID;
            this.lblTipsLiveID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTitle
            // 
            this.lblTitle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblTitle.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lblTitle.Location = new System.Drawing.Point(12, 4);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(567, 52);
            this.lblTitle.TabIndex = 19;
            this.lblTitle.Text = "osu! Sync GUI";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblTitle_MouseDown);
            this.lblTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblTitle_MouseUp);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(42)))), ((int)(((byte)(44)))));
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.cbSources);
            this.panel1.Controls.Add(this.lblTipsStauts);
            this.panel1.Controls.Add(this.txtLiveID);
            this.panel1.Controls.Add(this.lblTipsLiveID);
            this.panel1.Controls.Add(this.cmdExit);
            this.panel1.Controls.Add(this.cmdConsole);
            this.panel1.Controls.Add(this.cmdLogin);
            this.panel1.Controls.Add(this.cmdStop);
            this.panel1.Controls.Add(this.cmdStart);
            this.panel1.Controls.Add(this.lblTipsLiveStatus);
            this.panel1.Controls.Add(this.txtBotIRCPassword);
            this.panel1.Controls.Add(this.lblTipsBotIRCPass);
            this.panel1.Controls.Add(this.txtBotIRC);
            this.panel1.Controls.Add(this.lblTipsBotIRC);
            this.panel1.Controls.Add(this.txtTargetIRC);
            this.panel1.Controls.Add(this.lblTipsTargetIRC);
            this.panel1.Controls.Add(this.lblTipsOSUStatus);
            this.panel1.Location = new System.Drawing.Point(2, 347);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(580, 175);
            this.panel1.TabIndex = 23;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(299, 73);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 23);
            this.label1.TabIndex = 20;
            this.label1.Text = UI_TIPS_LIVE_SOURCE;
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbSources
            // 
            this.cbSources.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(55)))));
            this.cbSources.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSources.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbSources.ForeColor = System.Drawing.SystemColors.Info;
            this.cbSources.Location = new System.Drawing.Point(366, 71);
            this.cbSources.Name = "cbSources";
            this.cbSources.Size = new System.Drawing.Size(180, 25);
            this.cbSources.TabIndex = 19;
            this.cbSources.SelectedIndexChanged += new System.EventHandler(this.cbSources_SelectedIndexChanged);
            // 
            // txtCmd
            // 
            this.txtCmd.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtCmd.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.txtCmd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(55)))));
            this.txtCmd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCmd.Font = new System.Drawing.Font("Microsoft YaHei", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtCmd.ForeColor = System.Drawing.Color.White;
            this.txtCmd.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.txtCmd.Location = new System.Drawing.Point(12, 309);
            this.txtCmd.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtCmd.Name = "txtCmd";
            this.txtCmd.Size = new System.Drawing.Size(559, 30);
            this.txtCmd.TabIndex = 24;
            this.txtCmd.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCmd_KeyDown);
            // 
            // lblClose
            // 
            this.lblClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.lblClose.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.lblClose.FlatAppearance.BorderSize = 0;
            this.lblClose.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.lblClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.lblClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblClose.Font = new System.Drawing.Font("Microsoft YaHei", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblClose.ForeColor = System.Drawing.Color.White;
            this.lblClose.Location = new System.Drawing.Point(537, 1);
            this.lblClose.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lblClose.Name = "lblClose";
            this.lblClose.Size = new System.Drawing.Size(44, 33);
            this.lblClose.TabIndex = 25;
            this.lblClose.Text = "×";
            this.lblClose.UseVisualStyleBackColor = false;
            this.lblClose.Click += new System.EventHandler(this.lblClose_Click);
            // 
            // lblMin
            // 
            this.lblMin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.lblMin.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.lblMin.FlatAppearance.BorderSize = 0;
            this.lblMin.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.lblMin.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.lblMin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblMin.Font = new System.Drawing.Font("Microsoft YaHei", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblMin.ForeColor = System.Drawing.Color.White;
            this.lblMin.Location = new System.Drawing.Point(493, 1);
            this.lblMin.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lblMin.Name = "lblMin";
            this.lblMin.Size = new System.Drawing.Size(44, 33);
            this.lblMin.TabIndex = 26;
            this.lblMin.Text = "-";
            this.lblMin.UseVisualStyleBackColor = false;
            this.lblMin.Click += new System.EventHandler(this.lblMin_Click);
            // 
            // lblCollapse
            // 
            this.lblCollapse.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.lblCollapse.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.lblCollapse.FlatAppearance.BorderSize = 0;
            this.lblCollapse.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.lblCollapse.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.lblCollapse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblCollapse.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblCollapse.ForeColor = System.Drawing.Color.White;
            this.lblCollapse.Location = new System.Drawing.Point(449, 1);
            this.lblCollapse.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lblCollapse.Name = "lblCollapse";
            this.lblCollapse.Size = new System.Drawing.Size(44, 33);
            this.lblCollapse.TabIndex = 27;
            this.lblCollapse.Text = "♂";
            this.lblCollapse.UseVisualStyleBackColor = false;
            this.lblCollapse.Click += new System.EventHandler(this.lblCollapse_Click);
            // 
            // frmDefault
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.ClientSize = new System.Drawing.Size(583, 525);
            this.Controls.Add(this.lblCollapse);
            this.Controls.Add(this.lblMin);
            this.Controls.Add(this.lblClose);
            this.Controls.Add(this.txtCmd);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblTitle);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft YaHei Light", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "frmDefault";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "osu! Sync GUI";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmDefault_FormClosing);
            this.Load += new System.EventHandler(this.frmDefault_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ControlsPaint);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox txtLog;
        private System.Windows.Forms.Label lblTipsTargetIRC;
        private System.Windows.Forms.TextBox txtTargetIRC;
        private System.Windows.Forms.Label lblTipsBotIRC;
        private System.Windows.Forms.TextBox txtBotIRC;
        private System.Windows.Forms.TextBox txtBotIRCPassword;
        private System.Windows.Forms.Label lblTipsBotIRCPass;
        private System.Windows.Forms.Label lblTipsStauts;
        private System.Windows.Forms.Label lblTipsLiveStatus;
        private System.Windows.Forms.Label lblTipsOSUStatus;
        private System.Windows.Forms.Button cmdStart;
        private System.Windows.Forms.Button cmdStop;
        private System.Windows.Forms.Button cmdLogin;
        private System.Windows.Forms.Button cmdConsole;
        private System.Windows.Forms.Button cmdExit;
        private System.Windows.Forms.TextBox txtLiveID;
        private System.Windows.Forms.Label lblTipsLiveID;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtCmd;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button lblClose;
        private System.Windows.Forms.Button lblMin;
        private System.Windows.Forms.Button lblCollapse;
        private System.Windows.Forms.ComboBox cbSources;
    }
}