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
            this.edtAudioUrlSource = new System.Windows.Forms.TextBox();
            this.edtAudioOption = new System.Windows.Forms.TextBox();
            this.edtVideoOption = new System.Windows.Forms.TextBox();
            this.cbbMics = new System.Windows.Forms.ComboBox();
            this.edtVideoUrlSource = new System.Windows.Forms.TextBox();
            this.cbbCams = new System.Windows.Forms.ComboBox();
            this.rbtnFromUrl = new System.Windows.Forms.RadioButton();
            this.rbtnFromDevice = new System.Windows.Forms.RadioButton();
            this.gbLog = new System.Windows.Forms.GroupBox();
            this.mmAudioLogger = new System.Windows.Forms.RichTextBox();
            this.mmVideoLogger = new System.Windows.Forms.RichTextBox();
            this.gbAction = new System.Windows.Forms.GroupBox();
            this.lblRestartInterval = new System.Windows.Forms.Label();
            this.ckbAutoRestart = new System.Windows.Forms.CheckBox();
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
            this.ttDevice = new System.Windows.Forms.ToolTip(this.components);
            this.timerAutoSet = new System.Windows.Forms.Timer(this.components);
            this.timerRestartVideo = new System.Windows.Forms.Timer(this.components);
            this.timerRestartAudio = new System.Windows.Forms.Timer(this.components);
            this.timerCheckRecvTimeout = new System.Windows.Forms.Timer(this.components);
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
            this.gbMediaSource.Controls.Add(this.edtAudioUrlSource);
            this.gbMediaSource.Controls.Add(this.edtAudioOption);
            this.gbMediaSource.Controls.Add(this.edtVideoOption);
            this.gbMediaSource.Controls.Add(this.cbbMics);
            this.gbMediaSource.Controls.Add(this.edtVideoUrlSource);
            this.gbMediaSource.Controls.Add(this.cbbCams);
            this.gbMediaSource.Controls.Add(this.rbtnFromUrl);
            this.gbMediaSource.Controls.Add(this.rbtnFromDevice);
            this.gbMediaSource.Enabled = false;
            this.gbMediaSource.Location = new System.Drawing.Point(12, 13);
            this.gbMediaSource.Name = "gbMediaSource";
            this.gbMediaSource.Size = new System.Drawing.Size(400, 190);
            this.gbMediaSource.TabIndex = 0;
            this.gbMediaSource.TabStop = false;
            this.gbMediaSource.Text = "Media Source";
            // 
            // edtAudioUrlSource
            // 
            this.edtAudioUrlSource.Location = new System.Drawing.Point(18, 159);
            this.edtAudioUrlSource.Name = "edtAudioUrlSource";
            this.edtAudioUrlSource.Size = new System.Drawing.Size(370, 20);
            this.edtAudioUrlSource.TabIndex = 7;
            // 
            // edtAudioOption
            // 
            this.edtAudioOption.Location = new System.Drawing.Point(208, 70);
            this.edtAudioOption.Name = "edtAudioOption";
            this.edtAudioOption.Size = new System.Drawing.Size(180, 20);
            this.edtAudioOption.TabIndex = 6;
            this.ttDevice.SetToolTip(this.edtAudioOption, "parameters for the device above");
            // 
            // edtVideoOption
            // 
            this.edtVideoOption.Location = new System.Drawing.Point(18, 70);
            this.edtVideoOption.Name = "edtVideoOption";
            this.edtVideoOption.Size = new System.Drawing.Size(180, 20);
            this.edtVideoOption.TabIndex = 5;
            this.ttDevice.SetToolTip(this.edtVideoOption, "parameters for the device above");
            // 
            // cbbMics
            // 
            this.cbbMics.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbMics.FormattingEnabled = true;
            this.cbbMics.Location = new System.Drawing.Point(208, 43);
            this.cbbMics.Name = "cbbMics";
            this.cbbMics.Size = new System.Drawing.Size(180, 21);
            this.cbbMics.TabIndex = 4;
            // 
            // edtVideoUrlSource
            // 
            this.edtVideoUrlSource.Location = new System.Drawing.Point(18, 130);
            this.edtVideoUrlSource.Name = "edtVideoUrlSource";
            this.edtVideoUrlSource.Size = new System.Drawing.Size(370, 20);
            this.edtVideoUrlSource.TabIndex = 3;
            this.edtVideoUrlSource.Text = "rtmp://live.hkstv.hk.lxdns.com/live/hks";
            // 
            // cbbCams
            // 
            this.cbbCams.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbCams.FormattingEnabled = true;
            this.cbbCams.Location = new System.Drawing.Point(18, 43);
            this.cbbCams.Name = "cbbCams";
            this.cbbCams.Size = new System.Drawing.Size(180, 21);
            this.cbbCams.TabIndex = 2;
            // 
            // rbtnFromUrl
            // 
            this.rbtnFromUrl.AutoSize = true;
            this.rbtnFromUrl.Location = new System.Drawing.Point(18, 108);
            this.rbtnFromUrl.Name = "rbtnFromUrl";
            this.rbtnFromUrl.Size = new System.Drawing.Size(73, 17);
            this.rbtnFromUrl.TabIndex = 1;
            this.rbtnFromUrl.Text = "From URL";
            this.rbtnFromUrl.UseVisualStyleBackColor = true;
            this.rbtnFromUrl.CheckedChanged += new System.EventHandler(this.rbtnFromUrl_CheckedChanged);
            // 
            // rbtnFromDevice
            // 
            this.rbtnFromDevice.AutoSize = true;
            this.rbtnFromDevice.Checked = true;
            this.rbtnFromDevice.Location = new System.Drawing.Point(18, 23);
            this.rbtnFromDevice.Name = "rbtnFromDevice";
            this.rbtnFromDevice.Size = new System.Drawing.Size(85, 17);
            this.rbtnFromDevice.TabIndex = 0;
            this.rbtnFromDevice.TabStop = true;
            this.rbtnFromDevice.Text = "From Device";
            this.rbtnFromDevice.UseVisualStyleBackColor = true;
            this.rbtnFromDevice.CheckedChanged += new System.EventHandler(this.rbtnFromDevice_CheckedChanged);
            // 
            // gbLog
            // 
            this.gbLog.Controls.Add(this.mmAudioLogger);
            this.gbLog.Controls.Add(this.mmVideoLogger);
            this.gbLog.Location = new System.Drawing.Point(12, 308);
            this.gbLog.Name = "gbLog";
            this.gbLog.Size = new System.Drawing.Size(986, 401);
            this.gbLog.TabIndex = 3;
            this.gbLog.TabStop = false;
            this.gbLog.Text = "Log";
            // 
            // mmAudioLogger
            // 
            this.mmAudioLogger.Location = new System.Drawing.Point(6, 211);
            this.mmAudioLogger.Name = "mmAudioLogger";
            this.mmAudioLogger.Size = new System.Drawing.Size(974, 184);
            this.mmAudioLogger.TabIndex = 1;
            this.mmAudioLogger.Text = "";
            // 
            // mmVideoLogger
            // 
            this.mmVideoLogger.Location = new System.Drawing.Point(6, 21);
            this.mmVideoLogger.Name = "mmVideoLogger";
            this.mmVideoLogger.Size = new System.Drawing.Size(974, 184);
            this.mmVideoLogger.TabIndex = 0;
            this.mmVideoLogger.Text = "";
            // 
            // gbAction
            // 
            this.gbAction.Controls.Add(this.lblRestartInterval);
            this.gbAction.Controls.Add(this.ckbAutoRestart);
            this.gbAction.Controls.Add(this.btnStop);
            this.gbAction.Controls.Add(this.btnStart);
            this.gbAction.Enabled = false;
            this.gbAction.Location = new System.Drawing.Point(12, 209);
            this.gbAction.Name = "gbAction";
            this.gbAction.Size = new System.Drawing.Size(400, 92);
            this.gbAction.TabIndex = 4;
            this.gbAction.TabStop = false;
            this.gbAction.Text = "Action";
            // 
            // lblRestartInterval
            // 
            this.lblRestartInterval.AutoSize = true;
            this.lblRestartInterval.Location = new System.Drawing.Point(102, 24);
            this.lblRestartInterval.Name = "lblRestartInterval";
            this.lblRestartInterval.Size = new System.Drawing.Size(173, 13);
            this.lblRestartInterval.TabIndex = 5;
            this.lblRestartInterval.Text = "(restart in 5 seconds once stopped)";
            // 
            // ckbAutoRestart
            // 
            this.ckbAutoRestart.AutoSize = true;
            this.ckbAutoRestart.Location = new System.Drawing.Point(18, 23);
            this.ckbAutoRestart.Name = "ckbAutoRestart";
            this.ckbAutoRestart.Size = new System.Drawing.Size(80, 17);
            this.ckbAutoRestart.TabIndex = 2;
            this.ckbAutoRestart.Text = "Auto restart";
            this.ckbAutoRestart.UseVisualStyleBackColor = true;
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(208, 43);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(180, 43);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "STOP";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(18, 43);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(180, 43);
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
            this.menuNotify.Size = new System.Drawing.Size(104, 48);
            this.menuNotify.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuNotify_ItemClicked);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(103, 22);
            this.toolStripMenuItem1.Tag = "1";
            this.toolStripMenuItem1.Text = "Show";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(103, 22);
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
            this.gbVideoTask.Location = new System.Drawing.Point(418, 13);
            this.gbVideoTask.Name = "gbVideoTask";
            this.gbVideoTask.Size = new System.Drawing.Size(580, 141);
            this.gbVideoTask.TabIndex = 6;
            this.gbVideoTask.TabStop = false;
            this.gbVideoTask.Text = "Video Publish Tasks";
            // 
            // btnSaveVideoTasks
            // 
            this.btnSaveVideoTasks.Location = new System.Drawing.Point(87, 15);
            this.btnSaveVideoTasks.Name = "btnSaveVideoTasks";
            this.btnSaveVideoTasks.Size = new System.Drawing.Size(75, 25);
            this.btnSaveVideoTasks.TabIndex = 13;
            this.btnSaveVideoTasks.Text = "Save";
            this.btnSaveVideoTasks.UseVisualStyleBackColor = true;
            this.btnSaveVideoTasks.Click += new System.EventHandler(this.btnSaveVideoTasks_Click);
            // 
            // btnReloadVideoTasks
            // 
            this.btnReloadVideoTasks.Location = new System.Drawing.Point(6, 15);
            this.btnReloadVideoTasks.Name = "btnReloadVideoTasks";
            this.btnReloadVideoTasks.Size = new System.Drawing.Size(75, 25);
            this.btnReloadVideoTasks.TabIndex = 12;
            this.btnReloadVideoTasks.Text = "Reload";
            this.btnReloadVideoTasks.UseVisualStyleBackColor = true;
            this.btnReloadVideoTasks.Click += new System.EventHandler(this.btnReloadVideoTasks_Click);
            // 
            // btnEditVideoTask
            // 
            this.btnEditVideoTask.Location = new System.Drawing.Point(498, 15);
            this.btnEditVideoTask.Name = "btnEditVideoTask";
            this.btnEditVideoTask.Size = new System.Drawing.Size(75, 25);
            this.btnEditVideoTask.TabIndex = 11;
            this.btnEditVideoTask.Text = "Edit";
            this.btnEditVideoTask.UseVisualStyleBackColor = true;
            this.btnEditVideoTask.Click += new System.EventHandler(this.btnEditVideoTask_Click);
            // 
            // btnDeleteVideoTask
            // 
            this.btnDeleteVideoTask.Location = new System.Drawing.Point(417, 15);
            this.btnDeleteVideoTask.Name = "btnDeleteVideoTask";
            this.btnDeleteVideoTask.Size = new System.Drawing.Size(75, 25);
            this.btnDeleteVideoTask.TabIndex = 10;
            this.btnDeleteVideoTask.Text = "Delete";
            this.btnDeleteVideoTask.UseVisualStyleBackColor = true;
            this.btnDeleteVideoTask.Click += new System.EventHandler(this.btnDeleteVideoTask_Click);
            // 
            // btnAddVideoTask
            // 
            this.btnAddVideoTask.Location = new System.Drawing.Point(336, 15);
            this.btnAddVideoTask.Name = "btnAddVideoTask";
            this.btnAddVideoTask.Size = new System.Drawing.Size(75, 25);
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
            this.olvVideoTasks.HideSelection = false;
            this.olvVideoTasks.Location = new System.Drawing.Point(6, 47);
            this.olvVideoTasks.Name = "olvVideoTasks";
            this.olvVideoTasks.ShowGroups = false;
            this.olvVideoTasks.Size = new System.Drawing.Size(567, 87);
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
            this.gbAudioTask.Location = new System.Drawing.Point(418, 160);
            this.gbAudioTask.Name = "gbAudioTask";
            this.gbAudioTask.Size = new System.Drawing.Size(580, 141);
            this.gbAudioTask.TabIndex = 7;
            this.gbAudioTask.TabStop = false;
            this.gbAudioTask.Text = "Audio Publish Tasks";
            // 
            // btnSaveAudioTasks
            // 
            this.btnSaveAudioTasks.Location = new System.Drawing.Point(87, 15);
            this.btnSaveAudioTasks.Name = "btnSaveAudioTasks";
            this.btnSaveAudioTasks.Size = new System.Drawing.Size(75, 25);
            this.btnSaveAudioTasks.TabIndex = 15;
            this.btnSaveAudioTasks.Text = "Save";
            this.btnSaveAudioTasks.UseVisualStyleBackColor = true;
            this.btnSaveAudioTasks.Click += new System.EventHandler(this.btnSaveAudioTasks_Click);
            // 
            // btnReloadAudioTasks
            // 
            this.btnReloadAudioTasks.Location = new System.Drawing.Point(6, 15);
            this.btnReloadAudioTasks.Name = "btnReloadAudioTasks";
            this.btnReloadAudioTasks.Size = new System.Drawing.Size(75, 25);
            this.btnReloadAudioTasks.TabIndex = 14;
            this.btnReloadAudioTasks.Text = "Reload";
            this.btnReloadAudioTasks.UseVisualStyleBackColor = true;
            this.btnReloadAudioTasks.Click += new System.EventHandler(this.btnReloadAudioTasks_Click);
            // 
            // btnEditAudioTask
            // 
            this.btnEditAudioTask.Location = new System.Drawing.Point(498, 15);
            this.btnEditAudioTask.Name = "btnEditAudioTask";
            this.btnEditAudioTask.Size = new System.Drawing.Size(75, 25);
            this.btnEditAudioTask.TabIndex = 11;
            this.btnEditAudioTask.Text = "Edit";
            this.btnEditAudioTask.UseVisualStyleBackColor = true;
            this.btnEditAudioTask.Click += new System.EventHandler(this.btnEditAudioTask_Click);
            // 
            // btnDeleteAudioTask
            // 
            this.btnDeleteAudioTask.Location = new System.Drawing.Point(417, 15);
            this.btnDeleteAudioTask.Name = "btnDeleteAudioTask";
            this.btnDeleteAudioTask.Size = new System.Drawing.Size(75, 25);
            this.btnDeleteAudioTask.TabIndex = 10;
            this.btnDeleteAudioTask.Text = "Delete";
            this.btnDeleteAudioTask.UseVisualStyleBackColor = true;
            this.btnDeleteAudioTask.Click += new System.EventHandler(this.btnDeleteAudioTask_Click);
            // 
            // btnAddAudioTask
            // 
            this.btnAddAudioTask.Location = new System.Drawing.Point(336, 15);
            this.btnAddAudioTask.Name = "btnAddAudioTask";
            this.btnAddAudioTask.Size = new System.Drawing.Size(75, 25);
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
            this.olvAudioTasks.HideSelection = false;
            this.olvAudioTasks.Location = new System.Drawing.Point(6, 47);
            this.olvAudioTasks.Name = "olvAudioTasks";
            this.olvAudioTasks.ShowGroups = false;
            this.olvAudioTasks.Size = new System.Drawing.Size(567, 87);
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
            // timerAutoSet
            // 
            this.timerAutoSet.Interval = 500;
            this.timerAutoSet.Tick += new System.EventHandler(this.timerAutoSet_Tick);
            // 
            // timerRestartVideo
            // 
            this.timerRestartVideo.Tick += new System.EventHandler(this.timerRestartVideo_Tick);
            // 
            // timerRestartAudio
            // 
            this.timerRestartAudio.Tick += new System.EventHandler(this.timerRestartAudio_Tick);
            // 
            // timerCheckRecvTimeout
            // 
            this.timerCheckRecvTimeout.Enabled = true;
            this.timerCheckRecvTimeout.Interval = 1000;
            this.timerCheckRecvTimeout.Tick += new System.EventHandler(this.timerCheckRecvTimeout_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1004, 716);
            this.Controls.Add(this.gbAudioTask);
            this.Controls.Add(this.gbVideoTask);
            this.Controls.Add(this.gbAction);
            this.Controls.Add(this.gbLog);
            this.Controls.Add(this.gbMediaSource);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Media Encoder";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.gbMediaSource.ResumeLayout(false);
            this.gbMediaSource.PerformLayout();
            this.gbLog.ResumeLayout(false);
            this.gbAction.ResumeLayout(false);
            this.gbAction.PerformLayout();
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
        private System.Windows.Forms.TextBox edtVideoUrlSource;
        private System.Windows.Forms.GroupBox gbLog;
        private System.Windows.Forms.RichTextBox mmVideoLogger;
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
        private System.Windows.Forms.ToolTip ttDevice;
        private System.Windows.Forms.Timer timerAutoSet;
        private System.Windows.Forms.RichTextBox mmAudioLogger;
        private System.Windows.Forms.CheckBox ckbAutoRestart;
        private System.Windows.Forms.Label lblRestartInterval;
        private System.Windows.Forms.Timer timerRestartVideo;
        private System.Windows.Forms.Timer timerRestartAudio;
        private System.Windows.Forms.TextBox edtAudioUrlSource;
        private System.Windows.Forms.Timer timerCheckRecvTimeout;
    }
}

