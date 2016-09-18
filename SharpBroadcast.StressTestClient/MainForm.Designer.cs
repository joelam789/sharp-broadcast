namespace SharpBroadcast.StressTestClient
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
            this.gbSetting = new System.Windows.Forms.GroupBox();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.nudClientCount = new System.Windows.Forms.NumericUpDown();
            this.lblClientCount = new System.Windows.Forms.Label();
            this.edtServerUrl = new System.Windows.Forms.TextBox();
            this.lblServerUrl = new System.Windows.Forms.Label();
            this.gbClientList = new System.Windows.Forms.GroupBox();
            this.olvClients = new BrightIdeasSoftware.FastObjectListView();
            this.colClientID = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colActive = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colChannel = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colReceivedBytes = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.timerRefresh = new System.Windows.Forms.Timer(this.components);
            this.gbLog = new System.Windows.Forms.GroupBox();
            this.mmLogger = new System.Windows.Forms.RichTextBox();
            this.gbSetting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudClientCount)).BeginInit();
            this.gbClientList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvClients)).BeginInit();
            this.gbLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbSetting
            // 
            this.gbSetting.Controls.Add(this.btnStop);
            this.gbSetting.Controls.Add(this.btnStart);
            this.gbSetting.Controls.Add(this.nudClientCount);
            this.gbSetting.Controls.Add(this.lblClientCount);
            this.gbSetting.Controls.Add(this.edtServerUrl);
            this.gbSetting.Controls.Add(this.lblServerUrl);
            this.gbSetting.Location = new System.Drawing.Point(12, 12);
            this.gbSetting.Name = "gbSetting";
            this.gbSetting.Size = new System.Drawing.Size(600, 58);
            this.gbSetting.TabIndex = 0;
            this.gbSetting.TabStop = false;
            this.gbSetting.Text = "Settings";
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(507, 21);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 5;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(426, 21);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 4;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // nudClientCount
            // 
            this.nudClientCount.Location = new System.Drawing.Point(341, 21);
            this.nudClientCount.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nudClientCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudClientCount.Name = "nudClientCount";
            this.nudClientCount.Size = new System.Drawing.Size(60, 22);
            this.nudClientCount.TabIndex = 3;
            this.nudClientCount.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // lblClientCount
            // 
            this.lblClientCount.AutoSize = true;
            this.lblClientCount.Location = new System.Drawing.Point(273, 24);
            this.lblClientCount.Name = "lblClientCount";
            this.lblClientCount.Size = new System.Drawing.Size(62, 12);
            this.lblClientCount.TabIndex = 2;
            this.lblClientCount.Text = "ClientCount";
            // 
            // edtServerUrl
            // 
            this.edtServerUrl.Location = new System.Drawing.Point(74, 21);
            this.edtServerUrl.Name = "edtServerUrl";
            this.edtServerUrl.Size = new System.Drawing.Size(180, 22);
            this.edtServerUrl.TabIndex = 1;
            this.edtServerUrl.Text = "ws://127.0.0.1:9320/test";
            // 
            // lblServerUrl
            // 
            this.lblServerUrl.AutoSize = true;
            this.lblServerUrl.Location = new System.Drawing.Point(18, 24);
            this.lblServerUrl.Name = "lblServerUrl";
            this.lblServerUrl.Size = new System.Drawing.Size(50, 12);
            this.lblServerUrl.TabIndex = 0;
            this.lblServerUrl.Text = "ServerUrl";
            // 
            // gbClientList
            // 
            this.gbClientList.Controls.Add(this.olvClients);
            this.gbClientList.Location = new System.Drawing.Point(12, 76);
            this.gbClientList.Name = "gbClientList";
            this.gbClientList.Size = new System.Drawing.Size(600, 298);
            this.gbClientList.TabIndex = 1;
            this.gbClientList.TabStop = false;
            this.gbClientList.Text = "Clients";
            // 
            // olvClients
            // 
            this.olvClients.AllColumns.Add(this.colClientID);
            this.olvClients.AllColumns.Add(this.colActive);
            this.olvClients.AllColumns.Add(this.colChannel);
            this.olvClients.AllColumns.Add(this.colReceivedBytes);
            this.olvClients.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colClientID,
            this.colActive,
            this.colChannel,
            this.colReceivedBytes});
            this.olvClients.FullRowSelect = true;
            this.olvClients.HasCollapsibleGroups = false;
            this.olvClients.Location = new System.Drawing.Point(6, 21);
            this.olvClients.Name = "olvClients";
            this.olvClients.ShowGroups = false;
            this.olvClients.Size = new System.Drawing.Size(588, 268);
            this.olvClients.TabIndex = 3;
            this.olvClients.UseCompatibleStateImageBehavior = false;
            this.olvClients.View = System.Windows.Forms.View.Details;
            this.olvClients.VirtualMode = true;
            // 
            // colClientID
            // 
            this.colClientID.AspectName = "ClientID";
            this.colClientID.Text = "Client ID";
            this.colClientID.Width = 120;
            // 
            // colActive
            // 
            this.colActive.AspectName = "Active";
            this.colActive.Text = "Active";
            this.colActive.Width = 100;
            // 
            // colChannel
            // 
            this.colChannel.AspectName = "Channel";
            this.colChannel.Text = "Channel";
            this.colChannel.Width = 120;
            // 
            // colReceivedBytes
            // 
            this.colReceivedBytes.AspectName = "ReceivedBytes";
            this.colReceivedBytes.Text = "Received Bytes";
            this.colReceivedBytes.Width = 200;
            // 
            // timerRefresh
            // 
            this.timerRefresh.Interval = 3000;
            this.timerRefresh.Tick += new System.EventHandler(this.timerRefresh_Tick);
            // 
            // gbLog
            // 
            this.gbLog.Controls.Add(this.mmLogger);
            this.gbLog.Location = new System.Drawing.Point(12, 380);
            this.gbLog.Name = "gbLog";
            this.gbLog.Size = new System.Drawing.Size(600, 140);
            this.gbLog.TabIndex = 2;
            this.gbLog.TabStop = false;
            this.gbLog.Text = "Log";
            // 
            // mmLogger
            // 
            this.mmLogger.Location = new System.Drawing.Point(6, 21);
            this.mmLogger.Name = "mmLogger";
            this.mmLogger.Size = new System.Drawing.Size(588, 113);
            this.mmLogger.TabIndex = 0;
            this.mmLogger.Text = "";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 532);
            this.Controls.Add(this.gbLog);
            this.Controls.Add(this.gbClientList);
            this.Controls.Add(this.gbSetting);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Live Streaming Stress Test Client";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.gbSetting.ResumeLayout(false);
            this.gbSetting.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudClientCount)).EndInit();
            this.gbClientList.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.olvClients)).EndInit();
            this.gbLog.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbSetting;
        private System.Windows.Forms.Label lblServerUrl;
        private System.Windows.Forms.TextBox edtServerUrl;
        private System.Windows.Forms.Label lblClientCount;
        private System.Windows.Forms.NumericUpDown nudClientCount;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.GroupBox gbClientList;
        private BrightIdeasSoftware.FastObjectListView olvClients;
        private BrightIdeasSoftware.OLVColumn colClientID;
        private BrightIdeasSoftware.OLVColumn colActive;
        private BrightIdeasSoftware.OLVColumn colChannel;
        private BrightIdeasSoftware.OLVColumn colReceivedBytes;
        private System.Windows.Forms.Timer timerRefresh;
        private System.Windows.Forms.GroupBox gbLog;
        private System.Windows.Forms.RichTextBox mmLogger;
    }
}

