namespace SharpBroadcast.MediaEncoder
{
    partial class VideoTaskForm
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
            this.gbVideoSetting = new System.Windows.Forms.GroupBox();
            this.btnAttach = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.nudFps = new System.Windows.Forms.NumericUpDown();
            this.lblFps = new System.Windows.Forms.Label();
            this.edtExtraParam = new System.Windows.Forms.TextBox();
            this.lblExtraParam = new System.Windows.Forms.Label();
            this.edtPixelFormat = new System.Windows.Forms.TextBox();
            this.lblPixelFormat = new System.Windows.Forms.Label();
            this.lblBitrateUnit = new System.Windows.Forms.Label();
            this.lblVideoType = new System.Windows.Forms.Label();
            this.rbtH264 = new System.Windows.Forms.RadioButton();
            this.rbtMpeg1 = new System.Windows.Forms.RadioButton();
            this.nudBitrate = new System.Windows.Forms.NumericUpDown();
            this.lblBitrate = new System.Windows.Forms.Label();
            this.cbbResolution = new System.Windows.Forms.ComboBox();
            this.lblResolution = new System.Windows.Forms.Label();
            this.gbServerInfo = new System.Windows.Forms.GroupBox();
            this.edtVideoServerUrl = new System.Windows.Forms.TextBox();
            this.lblServerAddress = new System.Windows.Forms.Label();
            this.edtChannelName = new System.Windows.Forms.TextBox();
            this.lblChannelName = new System.Windows.Forms.Label();
            this.gbAction = new System.Windows.Forms.GroupBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.rbtRtmp = new System.Windows.Forms.RadioButton();
            this.gbVideoSetting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFps)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBitrate)).BeginInit();
            this.gbServerInfo.SuspendLayout();
            this.gbAction.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbVideoSetting
            // 
            this.gbVideoSetting.Controls.Add(this.rbtRtmp);
            this.gbVideoSetting.Controls.Add(this.btnAttach);
            this.gbVideoSetting.Controls.Add(this.btnRefresh);
            this.gbVideoSetting.Controls.Add(this.nudFps);
            this.gbVideoSetting.Controls.Add(this.lblFps);
            this.gbVideoSetting.Controls.Add(this.edtExtraParam);
            this.gbVideoSetting.Controls.Add(this.lblExtraParam);
            this.gbVideoSetting.Controls.Add(this.edtPixelFormat);
            this.gbVideoSetting.Controls.Add(this.lblPixelFormat);
            this.gbVideoSetting.Controls.Add(this.lblBitrateUnit);
            this.gbVideoSetting.Controls.Add(this.lblVideoType);
            this.gbVideoSetting.Controls.Add(this.rbtH264);
            this.gbVideoSetting.Controls.Add(this.rbtMpeg1);
            this.gbVideoSetting.Controls.Add(this.nudBitrate);
            this.gbVideoSetting.Controls.Add(this.lblBitrate);
            this.gbVideoSetting.Controls.Add(this.cbbResolution);
            this.gbVideoSetting.Controls.Add(this.lblResolution);
            this.gbVideoSetting.Location = new System.Drawing.Point(12, 13);
            this.gbVideoSetting.Name = "gbVideoSetting";
            this.gbVideoSetting.Size = new System.Drawing.Size(355, 209);
            this.gbVideoSetting.TabIndex = 2;
            this.gbVideoSetting.TabStop = false;
            this.gbVideoSetting.Text = "Video";
            // 
            // btnAttach
            // 
            this.btnAttach.Location = new System.Drawing.Point(258, 169);
            this.btnAttach.Name = "btnAttach";
            this.btnAttach.Size = new System.Drawing.Size(75, 25);
            this.btnAttach.TabIndex = 16;
            this.btnAttach.Text = "Attach audio";
            this.btnAttach.UseVisualStyleBackColor = true;
            this.btnAttach.Click += new System.EventHandler(this.btnAttach_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(258, 46);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(50, 25);
            this.btnRefresh.TabIndex = 15;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // nudFps
            // 
            this.nudFps.Location = new System.Drawing.Point(130, 78);
            this.nudFps.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.nudFps.Name = "nudFps";
            this.nudFps.Size = new System.Drawing.Size(120, 20);
            this.nudFps.TabIndex = 14;
            this.nudFps.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // lblFps
            // 
            this.lblFps.AutoSize = true;
            this.lblFps.Location = new System.Drawing.Point(99, 80);
            this.lblFps.Name = "lblFps";
            this.lblFps.Size = new System.Drawing.Size(27, 13);
            this.lblFps.TabIndex = 13;
            this.lblFps.Text = "FPS";
            // 
            // edtExtraParam
            // 
            this.edtExtraParam.Location = new System.Drawing.Point(130, 169);
            this.edtExtraParam.Name = "edtExtraParam";
            this.edtExtraParam.Size = new System.Drawing.Size(120, 20);
            this.edtExtraParam.TabIndex = 12;
            // 
            // lblExtraParam
            // 
            this.lblExtraParam.AutoSize = true;
            this.lblExtraParam.Location = new System.Drawing.Point(60, 172);
            this.lblExtraParam.Name = "lblExtraParam";
            this.lblExtraParam.Size = new System.Drawing.Size(64, 13);
            this.lblExtraParam.TabIndex = 11;
            this.lblExtraParam.Text = "Extra Param";
            // 
            // edtPixelFormat
            // 
            this.edtPixelFormat.Location = new System.Drawing.Point(130, 139);
            this.edtPixelFormat.Name = "edtPixelFormat";
            this.edtPixelFormat.Size = new System.Drawing.Size(120, 20);
            this.edtPixelFormat.TabIndex = 10;
            // 
            // lblPixelFormat
            // 
            this.lblPixelFormat.AutoSize = true;
            this.lblPixelFormat.Location = new System.Drawing.Point(60, 142);
            this.lblPixelFormat.Name = "lblPixelFormat";
            this.lblPixelFormat.Size = new System.Drawing.Size(64, 13);
            this.lblPixelFormat.TabIndex = 9;
            this.lblPixelFormat.Text = "Pixel Format";
            // 
            // lblBitrateUnit
            // 
            this.lblBitrateUnit.AutoSize = true;
            this.lblBitrateUnit.Location = new System.Drawing.Point(262, 111);
            this.lblBitrateUnit.Name = "lblBitrateUnit";
            this.lblBitrateUnit.Size = new System.Drawing.Size(34, 13);
            this.lblBitrateUnit.TabIndex = 8;
            this.lblBitrateUnit.Text = "kbit/s";
            // 
            // lblVideoType
            // 
            this.lblVideoType.AutoSize = true;
            this.lblVideoType.Location = new System.Drawing.Point(64, 23);
            this.lblVideoType.Name = "lblVideoType";
            this.lblVideoType.Size = new System.Drawing.Size(61, 13);
            this.lblVideoType.TabIndex = 7;
            this.lblVideoType.Text = "Video Type";
            // 
            // rbtH264
            // 
            this.rbtH264.AutoSize = true;
            this.rbtH264.Location = new System.Drawing.Point(198, 23);
            this.rbtH264.Name = "rbtH264";
            this.rbtH264.Size = new System.Drawing.Size(54, 17);
            this.rbtH264.TabIndex = 6;
            this.rbtH264.Text = "H.264";
            this.rbtH264.UseVisualStyleBackColor = true;
            this.rbtH264.CheckedChanged += new System.EventHandler(this.rbtH264_CheckedChanged);
            // 
            // rbtMpeg1
            // 
            this.rbtMpeg1.AutoSize = true;
            this.rbtMpeg1.Checked = true;
            this.rbtMpeg1.Location = new System.Drawing.Point(132, 23);
            this.rbtMpeg1.Name = "rbtMpeg1";
            this.rbtMpeg1.Size = new System.Drawing.Size(62, 17);
            this.rbtMpeg1.TabIndex = 5;
            this.rbtMpeg1.TabStop = true;
            this.rbtMpeg1.Text = "MPEG1";
            this.rbtMpeg1.UseVisualStyleBackColor = true;
            // 
            // nudBitrate
            // 
            this.nudBitrate.Location = new System.Drawing.Point(130, 108);
            this.nudBitrate.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudBitrate.Name = "nudBitrate";
            this.nudBitrate.Size = new System.Drawing.Size(120, 20);
            this.nudBitrate.TabIndex = 3;
            this.nudBitrate.Value = new decimal(new int[] {
            512,
            0,
            0,
            0});
            // 
            // lblBitrate
            // 
            this.lblBitrate.AutoSize = true;
            this.lblBitrate.Location = new System.Drawing.Point(88, 111);
            this.lblBitrate.Name = "lblBitrate";
            this.lblBitrate.Size = new System.Drawing.Size(37, 13);
            this.lblBitrate.TabIndex = 2;
            this.lblBitrate.Text = "Bitrate";
            // 
            // cbbResolution
            // 
            this.cbbResolution.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbResolution.FormattingEnabled = true;
            this.cbbResolution.Items.AddRange(new object[] {
            "256x144",
            "320x240",
            "384x216",
            "512x288",
            "640x360",
            "640x480",
            "768x432",
            "768x576",
            "800x450",
            "800x600",
            "869x504",
            "1024x576",
            "1024x768",
            "1280x720",
            "1280x960"});
            this.cbbResolution.Location = new System.Drawing.Point(130, 47);
            this.cbbResolution.Name = "cbbResolution";
            this.cbbResolution.Size = new System.Drawing.Size(120, 21);
            this.cbbResolution.TabIndex = 1;
            // 
            // lblResolution
            // 
            this.lblResolution.AutoSize = true;
            this.lblResolution.Location = new System.Drawing.Point(69, 50);
            this.lblResolution.Name = "lblResolution";
            this.lblResolution.Size = new System.Drawing.Size(57, 13);
            this.lblResolution.TabIndex = 0;
            this.lblResolution.Text = "Resolution";
            // 
            // gbServerInfo
            // 
            this.gbServerInfo.Controls.Add(this.edtVideoServerUrl);
            this.gbServerInfo.Controls.Add(this.lblServerAddress);
            this.gbServerInfo.Controls.Add(this.edtChannelName);
            this.gbServerInfo.Controls.Add(this.lblChannelName);
            this.gbServerInfo.Location = new System.Drawing.Point(12, 229);
            this.gbServerInfo.Name = "gbServerInfo";
            this.gbServerInfo.Size = new System.Drawing.Size(355, 98);
            this.gbServerInfo.TabIndex = 3;
            this.gbServerInfo.TabStop = false;
            this.gbServerInfo.Text = "Server";
            // 
            // edtVideoServerUrl
            // 
            this.edtVideoServerUrl.Location = new System.Drawing.Point(87, 57);
            this.edtVideoServerUrl.Name = "edtVideoServerUrl";
            this.edtVideoServerUrl.Size = new System.Drawing.Size(245, 20);
            this.edtVideoServerUrl.TabIndex = 7;
            this.edtVideoServerUrl.Text = "http://127.0.0.1:9310";
            // 
            // lblServerAddress
            // 
            this.lblServerAddress.AutoSize = true;
            this.lblServerAddress.Location = new System.Drawing.Point(5, 61);
            this.lblServerAddress.Name = "lblServerAddress";
            this.lblServerAddress.Size = new System.Drawing.Size(79, 13);
            this.lblServerAddress.TabIndex = 6;
            this.lblServerAddress.Text = "Server Address";
            // 
            // edtChannelName
            // 
            this.edtChannelName.Location = new System.Drawing.Point(86, 23);
            this.edtChannelName.Name = "edtChannelName";
            this.edtChannelName.Size = new System.Drawing.Size(246, 20);
            this.edtChannelName.TabIndex = 5;
            this.edtChannelName.Text = "test-video";
            // 
            // lblChannelName
            // 
            this.lblChannelName.AutoSize = true;
            this.lblChannelName.Location = new System.Drawing.Point(6, 26);
            this.lblChannelName.Name = "lblChannelName";
            this.lblChannelName.Size = new System.Drawing.Size(77, 13);
            this.lblChannelName.TabIndex = 4;
            this.lblChannelName.Text = "Channel Name";
            // 
            // gbAction
            // 
            this.gbAction.Controls.Add(this.btnCancel);
            this.gbAction.Controls.Add(this.btnOK);
            this.gbAction.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.gbAction.Location = new System.Drawing.Point(0, 332);
            this.gbAction.Name = "gbAction";
            this.gbAction.Size = new System.Drawing.Size(384, 59);
            this.gbAction.TabIndex = 4;
            this.gbAction.TabStop = false;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(292, 21);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 25);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(198, 21);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 25);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // rbtRtmp
            // 
            this.rbtRtmp.AutoSize = true;
            this.rbtRtmp.Location = new System.Drawing.Point(258, 23);
            this.rbtRtmp.Name = "rbtRtmp";
            this.rbtRtmp.Size = new System.Drawing.Size(56, 17);
            this.rbtRtmp.TabIndex = 17;
            this.rbtRtmp.Text = "RTMP";
            this.rbtRtmp.UseVisualStyleBackColor = true;
            // 
            // VideoTaskForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 391);
            this.Controls.Add(this.gbAction);
            this.Controls.Add(this.gbServerInfo);
            this.Controls.Add(this.gbVideoSetting);
            this.Name = "VideoTaskForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Video Task";
            this.Load += new System.EventHandler(this.VideoTaskForm_Load);
            this.gbVideoSetting.ResumeLayout(false);
            this.gbVideoSetting.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFps)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBitrate)).EndInit();
            this.gbServerInfo.ResumeLayout(false);
            this.gbServerInfo.PerformLayout();
            this.gbAction.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbVideoSetting;
        private System.Windows.Forms.RadioButton rbtH264;
        private System.Windows.Forms.RadioButton rbtMpeg1;
        private System.Windows.Forms.NumericUpDown nudBitrate;
        private System.Windows.Forms.Label lblBitrate;
        private System.Windows.Forms.ComboBox cbbResolution;
        private System.Windows.Forms.Label lblResolution;
        private System.Windows.Forms.GroupBox gbServerInfo;
        private System.Windows.Forms.TextBox edtVideoServerUrl;
        private System.Windows.Forms.Label lblServerAddress;
        private System.Windows.Forms.TextBox edtChannelName;
        private System.Windows.Forms.Label lblChannelName;
        private System.Windows.Forms.GroupBox gbAction;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lblVideoType;
        private System.Windows.Forms.Label lblBitrateUnit;
        private System.Windows.Forms.Label lblPixelFormat;
        private System.Windows.Forms.TextBox edtPixelFormat;
        private System.Windows.Forms.TextBox edtExtraParam;
        private System.Windows.Forms.Label lblExtraParam;
        private System.Windows.Forms.NumericUpDown nudFps;
        private System.Windows.Forms.Label lblFps;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnAttach;
        private System.Windows.Forms.RadioButton rbtRtmp;
    }
}