namespace SharpBroadcast.MediaServer
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.olvClients = new BrightIdeasSoftware.FastObjectListView();
            this.colChannelName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colAddressInfo = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colMediaInfo = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colServerInfo = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colClientCount = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.tcMain = new System.Windows.Forms.TabControl();
            this.pageLog = new System.Windows.Forms.TabPage();
            this.mmLogger = new System.Windows.Forms.RichTextBox();
            this.pageWhite = new System.Windows.Forms.TabPage();
            this.listWhitelist = new System.Windows.Forms.ListBox();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.btnDel = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.timerRefreshInfo = new System.Windows.Forms.Timer(this.components);
            this.notifyIconMain = new System.Windows.Forms.NotifyIcon(this.components);
            this.menuNotify = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.olvClients)).BeginInit();
            this.tcMain.SuspendLayout();
            this.pageLog.SuspendLayout();
            this.pageWhite.SuspendLayout();
            this.pnlTop.SuspendLayout();
            this.menuNotify.SuspendLayout();
            this.SuspendLayout();
            // 
            // olvClients
            // 
            this.olvClients.AllColumns.Add(this.colChannelName);
            this.olvClients.AllColumns.Add(this.colAddressInfo);
            this.olvClients.AllColumns.Add(this.colMediaInfo);
            this.olvClients.AllColumns.Add(this.colServerInfo);
            this.olvClients.AllColumns.Add(this.colClientCount);
            this.olvClients.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colChannelName,
            this.colAddressInfo,
            this.colMediaInfo,
            this.colServerInfo,
            this.colClientCount});
            this.olvClients.FullRowSelect = true;
            this.olvClients.HasCollapsibleGroups = false;
            this.olvClients.Location = new System.Drawing.Point(12, 12);
            this.olvClients.Name = "olvClients";
            this.olvClients.ShowGroups = false;
            this.olvClients.Size = new System.Drawing.Size(760, 225);
            this.olvClients.TabIndex = 3;
            this.olvClients.UseCompatibleStateImageBehavior = false;
            this.olvClients.View = System.Windows.Forms.View.Details;
            this.olvClients.VirtualMode = true;
            // 
            // colChannelName
            // 
            this.colChannelName.AspectName = "ChannelName";
            this.colChannelName.Text = "Channel Name";
            this.colChannelName.Width = 90;
            // 
            // colAddressInfo
            // 
            this.colAddressInfo.AspectName = "AddressInfo";
            this.colAddressInfo.Text = "Media Source Address";
            this.colAddressInfo.Width = 150;
            // 
            // colMediaInfo
            // 
            this.colMediaInfo.AspectName = "MediaInfo";
            this.colMediaInfo.Text = "Media Info";
            this.colMediaInfo.Width = 200;
            // 
            // colServerInfo
            // 
            this.colServerInfo.AspectName = "ServerInfo";
            this.colServerInfo.Text = "Server Info";
            this.colServerInfo.Width = 200;
            // 
            // colClientCount
            // 
            this.colClientCount.AspectName = "ClientCount";
            this.colClientCount.Text = "Client Count";
            this.colClientCount.Width = 80;
            // 
            // tcMain
            // 
            this.tcMain.Controls.Add(this.pageLog);
            this.tcMain.Controls.Add(this.pageWhite);
            this.tcMain.Location = new System.Drawing.Point(12, 243);
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Size = new System.Drawing.Size(760, 190);
            this.tcMain.TabIndex = 5;
            // 
            // pageLog
            // 
            this.pageLog.Controls.Add(this.mmLogger);
            this.pageLog.Location = new System.Drawing.Point(4, 22);
            this.pageLog.Name = "pageLog";
            this.pageLog.Padding = new System.Windows.Forms.Padding(3);
            this.pageLog.Size = new System.Drawing.Size(752, 164);
            this.pageLog.TabIndex = 0;
            this.pageLog.Text = "Log";
            this.pageLog.UseVisualStyleBackColor = true;
            // 
            // mmLogger
            // 
            this.mmLogger.Location = new System.Drawing.Point(6, 6);
            this.mmLogger.Name = "mmLogger";
            this.mmLogger.Size = new System.Drawing.Size(740, 149);
            this.mmLogger.TabIndex = 5;
            this.mmLogger.Text = "";
            // 
            // pageWhite
            // 
            this.pageWhite.Controls.Add(this.listWhitelist);
            this.pageWhite.Controls.Add(this.pnlTop);
            this.pageWhite.Location = new System.Drawing.Point(4, 22);
            this.pageWhite.Name = "pageWhite";
            this.pageWhite.Padding = new System.Windows.Forms.Padding(3);
            this.pageWhite.Size = new System.Drawing.Size(752, 164);
            this.pageWhite.TabIndex = 1;
            this.pageWhite.Text = "Whitelist";
            this.pageWhite.UseVisualStyleBackColor = true;
            // 
            // listWhitelist
            // 
            this.listWhitelist.FormattingEnabled = true;
            this.listWhitelist.ItemHeight = 12;
            this.listWhitelist.Location = new System.Drawing.Point(6, 42);
            this.listWhitelist.Name = "listWhitelist";
            this.listWhitelist.Size = new System.Drawing.Size(740, 112);
            this.listWhitelist.TabIndex = 2;
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.btnDel);
            this.pnlTop.Controls.Add(this.btnAdd);
            this.pnlTop.Location = new System.Drawing.Point(6, 6);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(740, 30);
            this.pnlTop.TabIndex = 1;
            // 
            // btnDel
            // 
            this.btnDel.Location = new System.Drawing.Point(84, 3);
            this.btnDel.Name = "btnDel";
            this.btnDel.Size = new System.Drawing.Size(75, 23);
            this.btnDel.TabIndex = 1;
            this.btnDel.Text = "Delete";
            this.btnDel.UseVisualStyleBackColor = true;
            this.btnDel.Click += new System.EventHandler(this.btnDel_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(3, 3);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // timerRefreshInfo
            // 
            this.timerRefreshInfo.Interval = 3000;
            this.timerRefreshInfo.Tick += new System.EventHandler(this.timerRefreshInfo_Tick);
            // 
            // notifyIconMain
            // 
            this.notifyIconMain.ContextMenuStrip = this.menuNotify;
            this.notifyIconMain.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIconMain.Icon")));
            this.notifyIconMain.Text = "Media Server";
            this.notifyIconMain.Visible = true;
            this.notifyIconMain.DoubleClick += new System.EventHandler(this.notifyIconMain_DoubleClick);
            // 
            // menuNotify
            // 
            this.menuNotify.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2});
            this.menuNotify.Name = "menuNotify";
            this.menuNotify.Size = new System.Drawing.Size(106, 48);
            this.menuNotify.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuNotify_ItemClicked);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(105, 22);
            this.toolStripMenuItem1.Tag = "1";
            this.toolStripMenuItem1.Text = "Show";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(105, 22);
            this.toolStripMenuItem2.Tag = "2";
            this.toolStripMenuItem2.Text = "Exit";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 441);
            this.Controls.Add(this.tcMain);
            this.Controls.Add(this.olvClients);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Media Server";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.olvClients)).EndInit();
            this.tcMain.ResumeLayout(false);
            this.pageLog.ResumeLayout(false);
            this.pageWhite.ResumeLayout(false);
            this.pnlTop.ResumeLayout(false);
            this.menuNotify.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private BrightIdeasSoftware.FastObjectListView olvClients;
        private BrightIdeasSoftware.OLVColumn colChannelName;
        private BrightIdeasSoftware.OLVColumn colAddressInfo;
        private BrightIdeasSoftware.OLVColumn colMediaInfo;
        private BrightIdeasSoftware.OLVColumn colServerInfo;
        private BrightIdeasSoftware.OLVColumn colClientCount;
        private System.Windows.Forms.TabControl tcMain;
        private System.Windows.Forms.TabPage pageLog;
        private System.Windows.Forms.RichTextBox mmLogger;
        private System.Windows.Forms.TabPage pageWhite;
        private System.Windows.Forms.ListBox listWhitelist;
        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Button btnDel;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Timer timerRefreshInfo;
        private System.Windows.Forms.NotifyIcon notifyIconMain;
        private System.Windows.Forms.ContextMenuStrip menuNotify;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
    }
}

