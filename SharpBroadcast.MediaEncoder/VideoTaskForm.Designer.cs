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
            this.gbVideoSetting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFps)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBitrate)).BeginInit();
            this.gbServerInfo.SuspendLayout();
            this.gbAction.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbVideoSetting
            // 
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
            this.gbVideoSetting.Location = new System.Drawing.Point(12, 12);
            this.gbVideoSetting.Name = "gbVideoSetting";
            this.gbVideoSetting.Size = new System.Drawing.Size(355, 193);
            this.gbVideoSetting.TabIndex = 2;
            this.gbVideoSetting.TabStop = false;
            this.gbVideoSetting.Text = "Video";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(258, 41);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(50, 23);
            this.btnRefresh.TabIndex = 15;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // nudFps
            // 
            this.nudFps.Location = new System.Drawing.Point(130, 72);
            this.nudFps.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.nudFps.Name = "nudFps";
            this.nudFps.Size = new System.Drawing.Size(120, 22);
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
            this.lblFps.Location = new System.Drawing.Point(99, 74);
            this.lblFps.Name = "lblFps";
            this.lblFps.Size = new System.Drawing.Size(23, 12);
            this.lblFps.TabIndex = 13;
            this.lblFps.Text = "FPS";
            // 
            // edtExtraParam
            // 
            this.edtExtraParam.Location = new System.Drawing.Point(130, 156);
            this.edtExtraParam.Name = "edtExtraParam";
            this.edtExtraParam.Size = new System.Drawing.Size(120, 22);
            this.edtExtraParam.TabIndex = 12;
            // 
            // lblExtraParam
            // 
            this.lblExtraParam.AutoSize = true;
            this.lblExtraParam.Location = new System.Drawing.Point(60, 159);
            this.lblExtraParam.Name = "lblExtraParam";
            this.lblExtraParam.Size = new System.Drawing.Size(62, 12);
            this.lblExtraParam.TabIndex = 11;
            this.lblExtraParam.Text = "Extra Param";
            // 
            // edtPixelFormat
            // 
            this.edtPixelFormat.Location = new System.Drawing.Point(130, 128);
            this.edtPixelFormat.Name = "edtPixelFormat";
            this.edtPixelFormat.Size = new System.Drawing.Size(120, 22);
            this.edtPixelFormat.TabIndex = 10;
            // 
            // lblPixelFormat
            // 
            this.lblPixelFormat.AutoSize = true;
            this.lblPixelFormat.Location = new System.Drawing.Point(60, 131);
            this.lblPixelFormat.Name = "lblPixelFormat";
            this.lblPixelFormat.Size = new System.Drawing.Size(64, 12);
            this.lblPixelFormat.TabIndex = 9;
            this.lblPixelFormat.Text = "Pixel Format";
            // 
            // lblBitrateUnit
            // 
            this.lblBitrateUnit.AutoSize = true;
            this.lblBitrateUnit.Location = new System.Drawing.Point(262, 102);
            this.lblBitrateUnit.Name = "lblBitrateUnit";
            this.lblBitrateUnit.Size = new System.Drawing.Size(30, 12);
            this.lblBitrateUnit.TabIndex = 8;
            this.lblBitrateUnit.Text = "kbit/s";
            // 
            // lblVideoType
            // 
            this.lblVideoType.AutoSize = true;
            this.lblVideoType.Location = new System.Drawing.Point(64, 21);
            this.lblVideoType.Name = "lblVideoType";
            this.lblVideoType.Size = new System.Drawing.Size(60, 12);
            this.lblVideoType.TabIndex = 7;
            this.lblVideoType.Text = "Video Type";
            // 
            // rbtH264
            // 
            this.rbtH264.AutoSize = true;
            this.rbtH264.Location = new System.Drawing.Point(198, 21);
            this.rbtH264.Name = "rbtH264";
            this.rbtH264.Size = new System.Drawing.Size(52, 16);
            this.rbtH264.TabIndex = 6;
            this.rbtH264.Text = "H.264";
            this.rbtH264.UseVisualStyleBackColor = true;
            this.rbtH264.CheckedChanged += new System.EventHandler(this.rbtH264_CheckedChanged);
            // 
            // rbtMpeg1
            // 
            this.rbtMpeg1.AutoSize = true;
            this.rbtMpeg1.Checked = true;
            this.rbtMpeg1.Location = new System.Drawing.Point(132, 21);
            this.rbtMpeg1.Name = "rbtMpeg1";
            this.rbtMpeg1.Size = new System.Drawing.Size(60, 16);
            this.rbtMpeg1.TabIndex = 5;
            this.rbtMpeg1.TabStop = true;
            this.rbtMpeg1.Text = "MPEG1";
            this.rbtMpeg1.UseVisualStyleBackColor = true;
            // 
            // nudBitrate
            // 
            this.nudBitrate.Location = new System.Drawing.Point(130, 100);
            this.nudBitrate.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudBitrate.Name = "nudBitrate";
            this.nudBitrate.Size = new System.Drawing.Size(120, 22);
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
            this.lblBitrate.Location = new System.Drawing.Point(88, 102);
            this.lblBitrate.Name = "lblBitrate";
            this.lblBitrate.Size = new System.Drawing.Size(36, 12);
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
            this.cbbResolution.Location = new System.Drawing.Point(130, 43);
            this.cbbResolution.Name = "cbbResolution";
            this.cbbResolution.Size = new System.Drawing.Size(120, 20);
            this.cbbResolution.TabIndex = 1;
            // 
            // lblResolution
            // 
            this.lblResolution.AutoSize = true;
            this.lblResolution.Location = new System.Drawing.Point(69, 46);
            this.lblResolution.Name = "lblResolution";
            this.lblResolution.Size = new System.Drawing.Size(55, 12);
            this.lblResolution.TabIndex = 0;
            this.lblResolution.Text = "Resolution";
            // 
            // gbServerInfo
            // 
            this.gbServerInfo.Controls.Add(this.edtVideoServerUrl);
            this.gbServerInfo.Controls.Add(this.lblServerAddress);
            this.gbServerInfo.Controls.Add(this.edtChannelName);
            this.gbServerInfo.Controls.Add(this.lblChannelName);
            this.gbServerInfo.Location = new System.Drawing.Point(12, 211);
            this.gbServerInfo.Name = "gbServerInfo";
            this.gbServerInfo.Size = new System.Drawing.Size(355, 90);
            this.gbServerInfo.TabIndex = 3;
            this.gbServerInfo.TabStop = false;
            this.gbServerInfo.Text = "Server";
            // 
            // edtVideoServerUrl
            // 
            this.edtVideoServerUrl.Location = new System.Drawing.Point(87, 53);
            this.edtVideoServerUrl.Name = "edtVideoServerUrl";
            this.edtVideoServerUrl.Size = new System.Drawing.Size(245, 22);
            this.edtVideoServerUrl.TabIndex = 7;
            this.edtVideoServerUrl.Text = "http://127.0.0.1:9310";
            // 
            // lblServerAddress
            // 
            this.lblServerAddress.AutoSize = true;
            this.lblServerAddress.Location = new System.Drawing.Point(5, 56);
            this.lblServerAddress.Name = "lblServerAddress";
            this.lblServerAddress.Size = new System.Drawing.Size(75, 12);
            this.lblServerAddress.TabIndex = 6;
            this.lblServerAddress.Text = "Server Address";
            // 
            // edtChannelName
            // 
            this.edtChannelName.Location = new System.Drawing.Point(86, 21);
            this.edtChannelName.Name = "edtChannelName";
            this.edtChannelName.Size = new System.Drawing.Size(246, 22);
            this.edtChannelName.TabIndex = 5;
            this.edtChannelName.Text = "test-video";
            // 
            // lblChannelName
            // 
            this.lblChannelName.AutoSize = true;
            this.lblChannelName.Location = new System.Drawing.Point(6, 24);
            this.lblChannelName.Name = "lblChannelName";
            this.lblChannelName.Size = new System.Drawing.Size(74, 12);
            this.lblChannelName.TabIndex = 4;
            this.lblChannelName.Text = "Channel Name";
            // 
            // gbAction
            // 
            this.gbAction.Controls.Add(this.btnCancel);
            this.gbAction.Controls.Add(this.btnOK);
            this.gbAction.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.gbAction.Location = new System.Drawing.Point(0, 307);
            this.gbAction.Name = "gbAction";
            this.gbAction.Size = new System.Drawing.Size(384, 54);
            this.gbAction.TabIndex = 4;
            this.gbAction.TabStop = false;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(292, 19);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(198, 19);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // VideoTaskForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 361);
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
    }
}