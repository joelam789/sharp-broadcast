namespace SharpBroadcast.StreamRecorder
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
            this.gbClientList = new System.Windows.Forms.GroupBox();
            this.olvClients = new BrightIdeasSoftware.FastObjectListView();
            this.colChannelName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colChannelURL = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colStatus = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colMediaInfo = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colLastDataTime = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.gbActions = new System.Windows.Forms.GroupBox();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.richLogBox = new System.Windows.Forms.RichTextBox();
            this.timerRefreshInfo = new System.Windows.Forms.Timer(this.components);
            this.menuNotify = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.notifyIconMain = new System.Windows.Forms.NotifyIcon(this.components);
            this.gbClientList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvClients)).BeginInit();
            this.gbActions.SuspendLayout();
            this.menuNotify.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbClientList
            // 
            this.gbClientList.Controls.Add(this.olvClients);
            this.gbClientList.Location = new System.Drawing.Point(12, 76);
            this.gbClientList.Name = "gbClientList";
            this.gbClientList.Size = new System.Drawing.Size(760, 230);
            this.gbClientList.TabIndex = 5;
            this.gbClientList.TabStop = false;
            this.gbClientList.Text = "Clients";
            // 
            // olvClients
            // 
            this.olvClients.AllColumns.Add(this.colChannelName);
            this.olvClients.AllColumns.Add(this.colChannelURL);
            this.olvClients.AllColumns.Add(this.colStatus);
            this.olvClients.AllColumns.Add(this.colMediaInfo);
            this.olvClients.AllColumns.Add(this.colLastDataTime);
            this.olvClients.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colChannelName,
            this.colChannelURL,
            this.colStatus,
            this.colMediaInfo,
            this.colLastDataTime});
            this.olvClients.FullRowSelect = true;
            this.olvClients.GridLines = true;
            this.olvClients.HasCollapsibleGroups = false;
            this.olvClients.HideSelection = false;
            this.olvClients.Location = new System.Drawing.Point(6, 21);
            this.olvClients.Name = "olvClients";
            this.olvClients.ShowGroups = false;
            this.olvClients.Size = new System.Drawing.Size(748, 200);
            this.olvClients.TabIndex = 3;
            this.olvClients.UseCompatibleStateImageBehavior = false;
            this.olvClients.View = System.Windows.Forms.View.Details;
            this.olvClients.VirtualMode = true;
            // 
            // colChannelName
            // 
            this.colChannelName.AspectName = "ChannelName";
            this.colChannelName.Text = "Channel Name";
            this.colChannelName.Width = 100;
            // 
            // colChannelURL
            // 
            this.colChannelURL.AspectName = "ChannelURL";
            this.colChannelURL.Text = "Channel URL";
            this.colChannelURL.Width = 220;
            // 
            // colStatus
            // 
            this.colStatus.AspectName = "Status";
            this.colStatus.Text = "Status";
            this.colStatus.Width = 80;
            // 
            // colMediaInfo
            // 
            this.colMediaInfo.AspectName = "MediaInfo";
            this.colMediaInfo.Text = "Media Info";
            this.colMediaInfo.Width = 200;
            // 
            // colLastDataTime
            // 
            this.colLastDataTime.AspectName = "LastDataTime";
            this.colLastDataTime.Text = "Last Data Time";
            this.colLastDataTime.Width = 120;
            // 
            // gbActions
            // 
            this.gbActions.Controls.Add(this.btnStop);
            this.gbActions.Controls.Add(this.btnStart);
            this.gbActions.Location = new System.Drawing.Point(12, 12);
            this.gbActions.Name = "gbActions";
            this.gbActions.Size = new System.Drawing.Size(760, 58);
            this.gbActions.TabIndex = 4;
            this.gbActions.TabStop = false;
            this.gbActions.Text = "Actions";
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(126, 21);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(100, 23);
            this.btnStop.TabIndex = 7;
            this.btnStop.Text = "Stop recording";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(20, 21);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(100, 23);
            this.btnStart.TabIndex = 6;
            this.btnStart.Text = "Start to record";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // richLogBox
            // 
            this.richLogBox.Location = new System.Drawing.Point(12, 312);
            this.richLogBox.Name = "richLogBox";
            this.richLogBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.richLogBox.Size = new System.Drawing.Size(760, 120);
            this.richLogBox.TabIndex = 6;
            this.richLogBox.Text = "";
            // 
            // timerRefreshInfo
            // 
            this.timerRefreshInfo.Interval = 2000;
            this.timerRefreshInfo.Tick += new System.EventHandler(this.timerRefreshInfo_Tick);
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
            this.toolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItem1.Tag = "1";
            this.toolStripMenuItem1.Text = "Show";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItem2.Tag = "2";
            this.toolStripMenuItem2.Text = "Exit";
            // 
            // notifyIconMain
            // 
            this.notifyIconMain.ContextMenuStrip = this.menuNotify;
            this.notifyIconMain.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIconMain.Icon")));
            this.notifyIconMain.Text = "Stream Recorder";
            this.notifyIconMain.Visible = true;
            this.notifyIconMain.DoubleClick += new System.EventHandler(this.notifyIconMain_DoubleClick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 441);
            this.Controls.Add(this.richLogBox);
            this.Controls.Add(this.gbClientList);
            this.Controls.Add(this.gbActions);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Live Stream Recorder";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.gbClientList.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.olvClients)).EndInit();
            this.gbActions.ResumeLayout(false);
            this.menuNotify.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbClientList;
        private BrightIdeasSoftware.FastObjectListView olvClients;
        private BrightIdeasSoftware.OLVColumn colChannelName;
        private BrightIdeasSoftware.OLVColumn colChannelURL;
        private BrightIdeasSoftware.OLVColumn colStatus;
        private BrightIdeasSoftware.OLVColumn colMediaInfo;
        private BrightIdeasSoftware.OLVColumn colLastDataTime;
        private System.Windows.Forms.GroupBox gbActions;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.RichTextBox richLogBox;
        private System.Windows.Forms.Timer timerRefreshInfo;
        private System.Windows.Forms.ContextMenuStrip menuNotify;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.NotifyIcon notifyIconMain;
    }
}

