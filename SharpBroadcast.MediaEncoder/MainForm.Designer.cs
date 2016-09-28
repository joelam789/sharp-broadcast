namespace SharpBroadcast.MediaEncoder
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
            this.gbMediaSource = new System.Windows.Forms.GroupBox();
            this.cbbMics = new System.Windows.Forms.ComboBox();
            this.edtUrlSource = new System.Windows.Forms.TextBox();
            this.cbbCams = new System.Windows.Forms.ComboBox();
            this.rbtnFromUrl = new System.Windows.Forms.RadioButton();
            this.rbtnFromDevice = new System.Windows.Forms.RadioButton();
            this.gbLog = new System.Windows.Forms.GroupBox();
            this.mmLogger = new System.Windows.Forms.RichTextBox();
            this.gbAction = new System.Windows.Forms.GroupBox();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.menuNotify = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.notifyIconMain = new System.Windows.Forms.NotifyIcon(this.components);
            this.notifyIconStop = new System.Windows.Forms.NotifyIcon(this.components);
            this.notifyIconStart = new System.Windows.Forms.NotifyIcon(this.components);
            this.gbVideoTask = new System.Windows.Forms.GroupBox();
            this.btnSaveVideoTasks = new System.Windows.Forms.Button();
            this.btnReloadVideoTasks = new System.Windows.Forms.Button();
            this.btnEditVideoTask = new System.Windows.Forms.Button();
            this.btnDeleteVideoTask = new System.Windows.Forms.Button();
            this.btnAddVideoTask = new System.Windows.Forms.Button();
            this.olvVideoTasks = new BrightIdeasSoftware.FastObjectListView();
            this.colVideoType = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colResolution = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colFps = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colBitrate = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colPixelFormat = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colExtraParam = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colServerAddress = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colChannelName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.gbAudioTask = new System.Windows.Forms.GroupBox();
            this.btnSaveAudioTasks = new System.Windows.Forms.Button();
            this.btnReloadAudioTasks = new System.Windows.Forms.Button();
            this.btnEditAudioTask = new System.Windows.Forms.Button();
            this.btnDeleteAudioTask = new System.Windows.Forms.Button();
            this.btnAddAudioTask = new System.Windows.Forms.Button();
            this.olvAudioTasks = new BrightIdeasSoftware.FastObjectListView();
            this.colAudioType = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colAudioBitrate = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colAudioExtraParam = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colAudioServerAddress = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.colAudioChannelName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.edtVideoOption = new System.Windows.Forms.TextBox();
            this.edtAudioOption = new System.Windows.Forms.TextBox();
            this.gbMediaSource.SuspendLayout();
            this.gbLog.SuspendLayout();
            this.gbAction.SuspendLayout();
            this.menuNotify.SuspendLayout();
            this.gbVideoTask.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvVideoTasks)).BeginInit();
            this.gbAudioTask.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.olvAudioTasks)).BeginInit();
            this.SuspendLayout();
            // 
            // gbMediaSource
            // 
            this.gbMediaSource.Controls.Add(this.edtAudioOption);
            this.gbMediaSource.Controls.Add(this.edtVideoOption);
            this.gbMediaSource.Controls.Add(this.cbbMics);
            this.gbMediaSource.Controls.Add(this.edtUrlSource);
            this.gbMediaSource.Controls.Add(this.cbbCams);
            this.gbMediaSource.Controls.Add(this.rbtnFromUrl);
            this.gbMediaSource.Controls.Add(this.rbtnFromDevice);
            this.gbMediaSource.Location = new System.Drawing.Point(12, 12);
            this.gbMediaSource.Name = "gbMediaSource";
            this.gbMediaSource.Size = new System.Drawing.Size(400, 173);
            this.gbMediaSource.TabIndex = 0;
            this.gbMediaSource.TabStop = false;
            this.gbMediaSource.Text = "Media Source";
            // 
            // cbbMics
            // 
            this.cbbMics.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbMics.FormattingEnabled = true;
            this.cbbMics.Location = new System.Drawing.Point(208, 43);
            this.cbbMics.Name = "cbbMics";
            this.cbbMics.Size = new System.Drawing.Size(180, 20);
            this.cbbMics.TabIndex = 4;
            // 
            // edtUrlSource
            // 
            this.edtUrlSource.Location = new System.Drawing.Point(18, 136);
            this.edtUrlSource.Name = "edtUrlSource";
            this.edtUrlSource.Size = new System.Drawing.Size(370, 22);
            this.edtUrlSource.TabIndex = 3;
            this.edtUrlSource.Text = "rtmp://live.hkstv.hk.lxdns.com/live/hks";
            // 
            // cbbCams
            // 
            this.cbbCams.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbCams.FormattingEnabled = true;
            this.cbbCams.Location = new System.Drawing.Point(18, 43);
            this.cbbCams.Name = "cbbCams";
            this.cbbCams.Size = new System.Drawing.Size(180, 20);
            this.cbbCams.TabIndex = 2;
            // 
            // rbtnFromUrl
            // 
            this.rbtnFromUrl.AutoSize = true;
            this.rbtnFromUrl.Location = new System.Drawing.Point(18, 114);
            this.rbtnFromUrl.Name = "rbtnFromUrl";
            this.rbtnFromUrl.Size = new System.Drawing.Size(74, 16);
            this.rbtnFromUrl.TabIndex = 1;
            this.rbtnFromUrl.Text = "From URL";
            this.rbtnFromUrl.UseVisualStyleBackColor = true;
            this.rbtnFromUrl.CheckedChanged += new System.EventHandler(this.rbtnFromUrl_CheckedChanged);
            // 
            // rbtnFromDevice
            // 
            this.rbtnFromDevice.AutoSize = true;
            this.rbtnFromDevice.Checked = true;
            this.rbtnFromDevice.Location = new System.Drawing.Point(18, 21);
            this.rbtnFromDevice.Name = "rbtnFromDevice";
            this.rbtnFromDevice.Size = new System.Drawing.Size(83, 16);
            this.rbtnFromDevice.TabIndex = 0;
            this.rbtnFromDevice.TabStop = true;
            this.rbtnFromDevice.Text = "From Device";
            this.rbtnFromDevice.UseVisualStyleBackColor = true;
            this.rbtnFromDevice.CheckedChanged += new System.EventHandler(this.rbtnFromDevice_CheckedChanged);
            // 
            // gbLog
            // 
            this.gbLog.Controls.Add(this.mmLogger);
            this.gbLog.Location = new System.Drawing.Point(12, 284);
            this.gbLog.Name = "gbLog";
            this.gbLog.Size = new System.Drawing.Size(986, 230);
            this.gbLog.TabIndex = 3;
            this.gbLog.TabStop = false;
            this.gbLog.Text = "Log";
            // 
            // mmLogger
            // 
            this.mmLogger.Location = new System.Drawing.Point(8, 21);
            this.mmLogger.Name = "mmLogger";
            this.mmLogger.Size = new System.Drawing.Size(971, 200);
            this.mmLogger.TabIndex = 0;
            this.mmLogger.Text = "";
            // 
            // gbAction
            // 
            this.gbAction.Controls.Add(this.btnStop);
            this.gbAction.Controls.Add(this.btnStart);
            this.gbAction.Location = new System.Drawing.Point(12, 191);
            this.gbAction.Name = "gbAction";
            this.gbAction.Size = new System.Drawing.Size(400, 87);
            this.gbAction.TabIndex = 4;
            this.gbAction.TabStop = false;
            this.gbAction.Text = "Action";
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(208, 21);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(180, 50);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "STOP";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(18, 21);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(180, 50);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "START";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
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
            // notifyIconMain
            // 
            this.notifyIconMain.ContextMenuStrip = this.menuNotify;
            this.notifyIconMain.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIconMain.Icon")));
            this.notifyIconMain.Tag = "Stopped";
            this.notifyIconMain.Text = "Stopped";
            this.notifyIconMain.Visible = true;
            this.notifyIconMain.DoubleClick += new System.EventHandler(this.notifyIconMain_DoubleClick);
            // 
            // notifyIconStop
            // 
            this.notifyIconStop.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIconStop.Icon")));
            this.notifyIconStop.Tag = "Stopped";
            this.notifyIconStop.Text = "Stopped";
            // 
            // notifyIconStart
            // 
            this.notifyIconStart.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIconStart.Icon")));
            this.notifyIconStart.Tag = "Encoding";
            this.notifyIconStart.Text = "Encoding";
            // 
            // gbVideoTask
            // 
            this.gbVideoTask.Controls.Add(this.btnSaveVideoTasks);
            this.gbVideoTask.Controls.Add(this.btnReloadVideoTasks);
            this.gbVideoTask.Controls.Add(this.btnEditVideoTask);
            this.gbVideoTask.Controls.Add(this.btnDeleteVideoTask);
            this.gbVideoTask.Controls.Add(this.btnAddVideoTask);
            this.gbVideoTask.Controls.Add(this.olvVideoTasks);
            this.gbVideoTask.Location = new System.Drawing.Point(418, 12);
            this.gbVideoTask.Name = "gbVideoTask";
            this.gbVideoTask.Size = new System.Drawing.Size(580, 130);
            this.gbVideoTask.TabIndex = 6;
            this.gbVideoTask.TabStop = false;
            this.gbVideoTask.Text = "Video Publish Tasks";
            // 
            // btnSaveVideoTasks
            // 
            this.btnSaveVideoTasks.Location = new System.Drawing.Point(87, 14);
            this.btnSaveVideoTasks.Name = "btnSaveVideoTasks";
            this.btnSaveVideoTasks.Size = new System.Drawing.Size(75, 23);
            this.btnSaveVideoTasks.TabIndex = 13;
            this.btnSaveVideoTasks.Text = "Save";
            this.btnSaveVideoTasks.UseVisualStyleBackColor = true;
            this.btnSaveVideoTasks.Click += new System.EventHandler(this.btnSaveVideoTasks_Click);
            // 
            // btnReloadVideoTasks
            // 
            this.btnReloadVideoTasks.Location = new System.Drawing.Point(6, 14);
            this.btnReloadVideoTasks.Name = "btnReloadVideoTasks";
            this.btnReloadVideoTasks.Size = new System.Drawing.Size(75, 23);
            this.btnReloadVideoTasks.TabIndex = 12;
            this.btnReloadVideoTasks.Text = "Reload";
            this.btnReloadVideoTasks.UseVisualStyleBackColor = true;
            this.btnReloadVideoTasks.Click += new System.EventHandler(this.btnReloadVideoTasks_Click);
            // 
            // btnEditVideoTask
            // 
            this.btnEditVideoTask.Location = new System.Drawing.Point(498, 14);
            this.btnEditVideoTask.Name = "btnEditVideoTask";
            this.btnEditVideoTask.Size = new System.Drawing.Size(75, 23);
            this.btnEditVideoTask.TabIndex = 11;
            this.btnEditVideoTask.Text = "Edit";
            this.btnEditVideoTask.UseVisualStyleBackColor = true;
            this.btnEditVideoTask.Click += new System.EventHandler(this.btnEditVideoTask_Click);
            // 
            // btnDeleteVideoTask
            // 
            this.btnDeleteVideoTask.Location = new System.Drawing.Point(417, 14);
            this.btnDeleteVideoTask.Name = "btnDeleteVideoTask";
            this.btnDeleteVideoTask.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteVideoTask.TabIndex = 10;
            this.btnDeleteVideoTask.Text = "Delete";
            this.btnDeleteVideoTask.UseVisualStyleBackColor = true;
            this.btnDeleteVideoTask.Click += new System.EventHandler(this.btnDeleteVideoTask_Click);
            // 
            // btnAddVideoTask
            // 
            this.btnAddVideoTask.Location = new System.Drawing.Point(336, 14);
            this.btnAddVideoTask.Name = "btnAddVideoTask";
            this.btnAddVideoTask.Size = new System.Drawing.Size(75, 23);
            this.btnAddVideoTask.TabIndex = 9;
            this.btnAddVideoTask.Text = "Add";
            this.btnAddVideoTask.UseVisualStyleBackColor = true;
            this.btnAddVideoTask.Click += new System.EventHandler(this.btnAddVideoTask_Click);
            // 
            // olvVideoTasks
            // 
            this.olvVideoTasks.AllColumns.Add(this.colVideoType);
            this.olvVideoTasks.AllColumns.Add(this.colResolution);
            this.olvVideoTasks.AllColumns.Add(this.colFps);
            this.olvVideoTasks.AllColumns.Add(this.colBitrate);
            this.olvVideoTasks.AllColumns.Add(this.colPixelFormat);
            this.olvVideoTasks.AllColumns.Add(this.colExtraParam);
            this.olvVideoTasks.AllColumns.Add(this.colServerAddress);
            this.olvVideoTasks.AllColumns.Add(this.colChannelName);
            this.olvVideoTasks.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colVideoType,
            this.colResolution,
            this.colFps,
            this.colBitrate,
            this.colPixelFormat,
            this.colExtraParam,
            this.colServerAddress,
            this.colChannelName});
            this.olvVideoTasks.FullRowSelect = true;
            this.olvVideoTasks.HasCollapsibleGroups = false;
            this.olvVideoTasks.Location = new System.Drawing.Point(6, 43);
            this.olvVideoTasks.Name = "olvVideoTasks";
            this.olvVideoTasks.ShowGroups = false;
            this.olvVideoTasks.Size = new System.Drawing.Size(567, 81);
            this.olvVideoTasks.TabIndex = 8;
            this.olvVideoTasks.UseCompatibleStateImageBehavior = false;
            this.olvVideoTasks.View = System.Windows.Forms.View.Details;
            this.olvVideoTasks.VirtualMode = true;
            this.olvVideoTasks.DoubleClick += new System.EventHandler(this.olvVideoTasks_DoubleClick);
            // 
            // colVideoType
            // 
            this.colVideoType.AspectName = "VideoType";
            this.colVideoType.Text = "Type";
            this.colVideoType.Width = 50;
            // 
            // colResolution
            // 
            this.colResolution.AspectName = "Resolution";
            this.colResolution.Text = "Resolution";
            this.colResolution.Width = 70;
            // 
            // colFps
            // 
            this.colFps.AspectName = "FPS";
            this.colFps.Text = "FPS";
            this.colFps.Width = 35;
            // 
            // colBitrate
            // 
            this.colBitrate.AspectName = "Bitrate";
            this.colBitrate.Text = "Bitrate";
            this.colBitrate.Width = 50;
            // 
            // colPixelFormat
            // 
            this.colPixelFormat.AspectName = "PixelFormat";
            this.colPixelFormat.Text = "Pixel Format";
            this.colPixelFormat.Width = 80;
            // 
            // colExtraParam
            // 
            this.colExtraParam.AspectName = "ExtraParam";
            this.colExtraParam.Text = "Extra";
            this.colExtraParam.Width = 40;
            // 
            // colServerAddress
            // 
            this.colServerAddress.AspectName = "ServerAddress";
            this.colServerAddress.Text = "Server";
            this.colServerAddress.Width = 140;
            // 
            // colChannelName
            // 
            this.colChannelName.AspectName = "ChannelName";
            this.colChannelName.Text = "Channel";
            this.colChannelName.Width = 80;
            // 
            // gbAudioTask
            // 
            this.gbAudioTask.Controls.Add(this.btnSaveAudioTasks);
            this.gbAudioTask.Controls.Add(this.btnReloadAudioTasks);
            this.gbAudioTask.Controls.Add(this.btnEditAudioTask);
            this.gbAudioTask.Controls.Add(this.btnDeleteAudioTask);
            this.gbAudioTask.Controls.Add(this.btnAddAudioTask);
            this.gbAudioTask.Controls.Add(this.olvAudioTasks);
            this.gbAudioTask.Location = new System.Drawing.Point(418, 148);
            this.gbAudioTask.Name = "gbAudioTask";
            this.gbAudioTask.Size = new System.Drawing.Size(580, 130);
            this.gbAudioTask.TabIndex = 7;
            this.gbAudioTask.TabStop = false;
            this.gbAudioTask.Text = "Audio Publish Tasks";
            // 
            // btnSaveAudioTasks
            // 
            this.btnSaveAudioTasks.Location = new System.Drawing.Point(87, 14);
            this.btnSaveAudioTasks.Name = "btnSaveAudioTasks";
            this.btnSaveAudioTasks.Size = new System.Drawing.Size(75, 23);
            this.btnSaveAudioTasks.TabIndex = 15;
            this.btnSaveAudioTasks.Text = "Save";
            this.btnSaveAudioTasks.UseVisualStyleBackColor = true;
            this.btnSaveAudioTasks.Click += new System.EventHandler(this.btnSaveAudioTasks_Click);
            // 
            // btnReloadAudioTasks
            // 
            this.btnReloadAudioTasks.Location = new System.Drawing.Point(6, 14);
            this.btnReloadAudioTasks.Name = "btnReloadAudioTasks";
            this.btnReloadAudioTasks.Size = new System.Drawing.Size(75, 23);
            this.btnReloadAudioTasks.TabIndex = 14;
            this.btnReloadAudioTasks.Text = "Reload";
            this.btnReloadAudioTasks.UseVisualStyleBackColor = true;
            this.btnReloadAudioTasks.Click += new System.EventHandler(this.btnReloadAudioTasks_Click);
            // 
            // btnEditAudioTask
            // 
            this.btnEditAudioTask.Location = new System.Drawing.Point(498, 14);
            this.btnEditAudioTask.Name = "btnEditAudioTask";
            this.btnEditAudioTask.Size = new System.Drawing.Size(75, 23);
            this.btnEditAudioTask.TabIndex = 11;
            this.btnEditAudioTask.Text = "Edit";
            this.btnEditAudioTask.UseVisualStyleBackColor = true;
            this.btnEditAudioTask.Click += new System.EventHandler(this.btnEditAudioTask_Click);
            // 
            // btnDeleteAudioTask
            // 
            this.btnDeleteAudioTask.Location = new System.Drawing.Point(417, 14);
            this.btnDeleteAudioTask.Name = "btnDeleteAudioTask";
            this.btnDeleteAudioTask.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteAudioTask.TabIndex = 10;
            this.btnDeleteAudioTask.Text = "Delete";
            this.btnDeleteAudioTask.UseVisualStyleBackColor = true;
            this.btnDeleteAudioTask.Click += new System.EventHandler(this.btnDeleteAudioTask_Click);
            // 
            // btnAddAudioTask
            // 
            this.btnAddAudioTask.Location = new System.Drawing.Point(336, 14);
            this.btnAddAudioTask.Name = "btnAddAudioTask";
            this.btnAddAudioTask.Size = new System.Drawing.Size(75, 23);
            this.btnAddAudioTask.TabIndex = 9;
            this.btnAddAudioTask.Text = "Add";
            this.btnAddAudioTask.UseVisualStyleBackColor = true;
            this.btnAddAudioTask.Click += new System.EventHandler(this.btnAddAudioTask_Click);
            // 
            // olvAudioTasks
            // 
            this.olvAudioTasks.AllColumns.Add(this.colAudioType);
            this.olvAudioTasks.AllColumns.Add(this.colAudioBitrate);
            this.olvAudioTasks.AllColumns.Add(this.colAudioExtraParam);
            this.olvAudioTasks.AllColumns.Add(this.colAudioServerAddress);
            this.olvAudioTasks.AllColumns.Add(this.colAudioChannelName);
            this.olvAudioTasks.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colAudioType,
            this.colAudioBitrate,
            this.colAudioExtraParam,
            this.colAudioServerAddress,
            this.colAudioChannelName});
            this.olvAudioTasks.FullRowSelect = true;
            this.olvAudioTasks.HasCollapsibleGroups = false;
            this.olvAudioTasks.Location = new System.Drawing.Point(6, 43);
            this.olvAudioTasks.Name = "olvAudioTasks";
            this.olvAudioTasks.ShowGroups = false;
            this.olvAudioTasks.Size = new System.Drawing.Size(567, 81);
            this.olvAudioTasks.TabIndex = 8;
            this.olvAudioTasks.UseCompatibleStateImageBehavior = false;
            this.olvAudioTasks.View = System.Windows.Forms.View.Details;
            this.olvAudioTasks.VirtualMode = true;
            this.olvAudioTasks.DoubleClick += new System.EventHandler(this.olvAudioTasks_DoubleClick);
            // 
            // colAudioType
            // 
            this.colAudioType.AspectName = "AudioType";
            this.colAudioType.Text = "Type";
            this.colAudioType.Width = 80;
            // 
            // colAudioBitrate
            // 
            this.colAudioBitrate.AspectName = "Bitrate";
            this.colAudioBitrate.Text = "Bitrate";
            this.colAudioBitrate.Width = 80;
            // 
            // colAudioExtraParam
            // 
            this.colAudioExtraParam.AspectName = "ExtraParam";
            this.colAudioExtraParam.Text = "Extra";
            this.colAudioExtraParam.Width = 100;
            // 
            // colAudioServerAddress
            // 
            this.colAudioServerAddress.AspectName = "ServerAddress";
            this.colAudioServerAddress.Text = "Server";
            this.colAudioServerAddress.Width = 150;
            // 
            // colAudioChannelName
            // 
            this.colAudioChannelName.AspectName = "ChannelName";
            this.colAudioChannelName.Text = "Channel";
            this.colAudioChannelName.Width = 100;
            // 
            // edtVideoOption
            // 
            this.edtVideoOption.Location = new System.Drawing.Point(18, 69);
            this.edtVideoOption.Name = "edtVideoOption";
            this.edtVideoOption.Size = new System.Drawing.Size(180, 22);
            this.edtVideoOption.TabIndex = 5;
            // 
            // edtAudioOption
            // 
            this.edtAudioOption.Location = new System.Drawing.Point(208, 69);
            this.edtAudioOption.Name = "edtAudioOption";
            this.edtAudioOption.Size = new System.Drawing.Size(180, 22);
            this.edtAudioOption.TabIndex = 6;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1004, 521);
            this.Controls.Add(this.gbAudioTask);
            this.Controls.Add(this.gbVideoTask);
            this.Controls.Add(this.gbAction);
            this.Controls.Add(this.gbLog);
            this.Controls.Add(this.gbMediaSource);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Media Encoder";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.gbMediaSource.ResumeLayout(false);
            this.gbMediaSource.PerformLayout();
            this.gbLog.ResumeLayout(false);
            this.gbAction.ResumeLayout(false);
            this.menuNotify.ResumeLayout(false);
            this.gbVideoTask.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.olvVideoTasks)).EndInit();
            this.gbAudioTask.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.olvAudioTasks)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbMediaSource;
        private System.Windows.Forms.RadioButton rbtnFromUrl;
        private System.Windows.Forms.RadioButton rbtnFromDevice;
        private System.Windows.Forms.ComboBox cbbCams;
        private System.Windows.Forms.TextBox edtUrlSource;
        private System.Windows.Forms.GroupBox gbLog;
        private System.Windows.Forms.RichTextBox mmLogger;
        private System.Windows.Forms.GroupBox gbAction;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.ContextMenuStrip menuNotify;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.NotifyIcon notifyIconMain;
        private System.Windows.Forms.NotifyIcon notifyIconStop;
        private System.Windows.Forms.NotifyIcon notifyIconStart;
        private System.Windows.Forms.ComboBox cbbMics;
        private System.Windows.Forms.GroupBox gbVideoTask;
        private BrightIdeasSoftware.FastObjectListView olvVideoTasks;
        private BrightIdeasSoftware.OLVColumn colVideoType;
        private BrightIdeasSoftware.OLVColumn colResolution;
        private BrightIdeasSoftware.OLVColumn colBitrate;
        private BrightIdeasSoftware.OLVColumn colPixelFormat;
        private BrightIdeasSoftware.OLVColumn colServerAddress;
        private BrightIdeasSoftware.OLVColumn colChannelName;
        private System.Windows.Forms.Button btnEditVideoTask;
        private System.Windows.Forms.Button btnDeleteVideoTask;
        private System.Windows.Forms.Button btnAddVideoTask;
        private System.Windows.Forms.GroupBox gbAudioTask;
        private System.Windows.Forms.Button btnEditAudioTask;
        private System.Windows.Forms.Button btnDeleteAudioTask;
        private System.Windows.Forms.Button btnAddAudioTask;
        private BrightIdeasSoftware.FastObjectListView olvAudioTasks;
        private BrightIdeasSoftware.OLVColumn colAudioType;
        private BrightIdeasSoftware.OLVColumn colAudioServerAddress;
        private BrightIdeasSoftware.OLVColumn colAudioChannelName;
        private BrightIdeasSoftware.OLVColumn colExtraParam;
        private BrightIdeasSoftware.OLVColumn colAudioBitrate;
        private BrightIdeasSoftware.OLVColumn colAudioExtraParam;
        private BrightIdeasSoftware.OLVColumn colFps;
        private System.Windows.Forms.Button btnSaveVideoTasks;
        private System.Windows.Forms.Button btnReloadVideoTasks;
        private System.Windows.Forms.Button btnSaveAudioTasks;
        private System.Windows.Forms.Button btnReloadAudioTasks;
        private System.Windows.Forms.TextBox edtVideoOption;
        private System.Windows.Forms.TextBox edtAudioOption;
    }
}

