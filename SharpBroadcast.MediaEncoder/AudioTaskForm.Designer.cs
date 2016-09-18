namespace SharpBroadcast.MediaEncoder
{
    partial class AudioTaskForm
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
            this.gbAction = new System.Windows.Forms.GroupBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.gbServerInfo = new System.Windows.Forms.GroupBox();
            this.edtVideoServerUrl = new System.Windows.Forms.TextBox();
            this.lblServerAddress = new System.Windows.Forms.Label();
            this.edtChannelName = new System.Windows.Forms.TextBox();
            this.lblChannelName = new System.Windows.Forms.Label();
            this.gbAudio = new System.Windows.Forms.GroupBox();
            this.edtExtraParam = new System.Windows.Forms.TextBox();
            this.lblExtraParam = new System.Windows.Forms.Label();
            this.lblBitrateUnit = new System.Windows.Forms.Label();
            this.lblAudioType = new System.Windows.Forms.Label();
            this.nudBitrate = new System.Windows.Forms.NumericUpDown();
            this.lblBitrate = new System.Windows.Forms.Label();
            this.rbtOpus = new System.Windows.Forms.RadioButton();
            this.rbtAac = new System.Windows.Forms.RadioButton();
            this.rbtMp3 = new System.Windows.Forms.RadioButton();
            this.gbAction.SuspendLayout();
            this.gbServerInfo.SuspendLayout();
            this.gbAudio.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBitrate)).BeginInit();
            this.SuspendLayout();
            // 
            // gbAction
            // 
            this.gbAction.Controls.Add(this.btnCancel);
            this.gbAction.Controls.Add(this.btnOK);
            this.gbAction.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.gbAction.Location = new System.Drawing.Point(0, 283);
            this.gbAction.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbAction.Name = "gbAction";
            this.gbAction.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbAction.Size = new System.Drawing.Size(512, 68);
            this.gbAction.TabIndex = 6;
            this.gbAction.TabStop = false;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(389, 26);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 29);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(263, 26);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(100, 29);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // gbServerInfo
            // 
            this.gbServerInfo.Controls.Add(this.edtVideoServerUrl);
            this.gbServerInfo.Controls.Add(this.lblServerAddress);
            this.gbServerInfo.Controls.Add(this.edtChannelName);
            this.gbServerInfo.Controls.Add(this.lblChannelName);
            this.gbServerInfo.Location = new System.Drawing.Point(16, 164);
            this.gbServerInfo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbServerInfo.Name = "gbServerInfo";
            this.gbServerInfo.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbServerInfo.Size = new System.Drawing.Size(473, 112);
            this.gbServerInfo.TabIndex = 5;
            this.gbServerInfo.TabStop = false;
            this.gbServerInfo.Text = "Server";
            // 
            // edtVideoServerUrl
            // 
            this.edtVideoServerUrl.Location = new System.Drawing.Point(116, 66);
            this.edtVideoServerUrl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.edtVideoServerUrl.Name = "edtVideoServerUrl";
            this.edtVideoServerUrl.Size = new System.Drawing.Size(325, 25);
            this.edtVideoServerUrl.TabIndex = 7;
            this.edtVideoServerUrl.Text = "http://127.0.0.1:9210";
            // 
            // lblServerAddress
            // 
            this.lblServerAddress.AutoSize = true;
            this.lblServerAddress.Location = new System.Drawing.Point(7, 70);
            this.lblServerAddress.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblServerAddress.Name = "lblServerAddress";
            this.lblServerAddress.Size = new System.Drawing.Size(93, 15);
            this.lblServerAddress.TabIndex = 6;
            this.lblServerAddress.Text = "Server Address";
            // 
            // edtChannelName
            // 
            this.edtChannelName.Location = new System.Drawing.Point(115, 26);
            this.edtChannelName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.edtChannelName.Name = "edtChannelName";
            this.edtChannelName.Size = new System.Drawing.Size(327, 25);
            this.edtChannelName.TabIndex = 5;
            this.edtChannelName.Text = "test-audio";
            // 
            // lblChannelName
            // 
            this.lblChannelName.AutoSize = true;
            this.lblChannelName.Location = new System.Drawing.Point(8, 30);
            this.lblChannelName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblChannelName.Name = "lblChannelName";
            this.lblChannelName.Size = new System.Drawing.Size(90, 15);
            this.lblChannelName.TabIndex = 4;
            this.lblChannelName.Text = "Channel Name";
            // 
            // gbAudio
            // 
            this.gbAudio.Controls.Add(this.edtExtraParam);
            this.gbAudio.Controls.Add(this.lblExtraParam);
            this.gbAudio.Controls.Add(this.lblBitrateUnit);
            this.gbAudio.Controls.Add(this.lblAudioType);
            this.gbAudio.Controls.Add(this.nudBitrate);
            this.gbAudio.Controls.Add(this.lblBitrate);
            this.gbAudio.Controls.Add(this.rbtOpus);
            this.gbAudio.Controls.Add(this.rbtAac);
            this.gbAudio.Controls.Add(this.rbtMp3);
            this.gbAudio.Location = new System.Drawing.Point(16, 19);
            this.gbAudio.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbAudio.Name = "gbAudio";
            this.gbAudio.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gbAudio.Size = new System.Drawing.Size(473, 138);
            this.gbAudio.TabIndex = 7;
            this.gbAudio.TabStop = false;
            this.gbAudio.Text = "Audio";
            // 
            // edtExtraParam
            // 
            this.edtExtraParam.Location = new System.Drawing.Point(149, 92);
            this.edtExtraParam.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.edtExtraParam.Name = "edtExtraParam";
            this.edtExtraParam.Size = new System.Drawing.Size(159, 25);
            this.edtExtraParam.TabIndex = 14;
            // 
            // lblExtraParam
            // 
            this.lblExtraParam.AutoSize = true;
            this.lblExtraParam.Location = new System.Drawing.Point(59, 96);
            this.lblExtraParam.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblExtraParam.Name = "lblExtraParam";
            this.lblExtraParam.Size = new System.Drawing.Size(78, 15);
            this.lblExtraParam.TabIndex = 13;
            this.lblExtraParam.Text = "Extra Param";
            // 
            // lblBitrateUnit
            // 
            this.lblBitrateUnit.AutoSize = true;
            this.lblBitrateUnit.Location = new System.Drawing.Point(317, 59);
            this.lblBitrateUnit.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBitrateUnit.Name = "lblBitrateUnit";
            this.lblBitrateUnit.Size = new System.Drawing.Size(43, 15);
            this.lblBitrateUnit.TabIndex = 12;
            this.lblBitrateUnit.Text = "kbits/s";
            // 
            // lblAudioType
            // 
            this.lblAudioType.AutoSize = true;
            this.lblAudioType.Location = new System.Drawing.Point(60, 29);
            this.lblAudioType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAudioType.Name = "lblAudioType";
            this.lblAudioType.Size = new System.Drawing.Size(75, 15);
            this.lblAudioType.TabIndex = 11;
            this.lblAudioType.Text = "Audio Type";
            // 
            // nudBitrate
            // 
            this.nudBitrate.Location = new System.Drawing.Point(149, 56);
            this.nudBitrate.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.nudBitrate.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudBitrate.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudBitrate.Name = "nudBitrate";
            this.nudBitrate.Size = new System.Drawing.Size(160, 25);
            this.nudBitrate.TabIndex = 10;
            this.nudBitrate.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            // 
            // lblBitrate
            // 
            this.lblBitrate.AutoSize = true;
            this.lblBitrate.Location = new System.Drawing.Point(93, 59);
            this.lblBitrate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBitrate.Name = "lblBitrate";
            this.lblBitrate.Size = new System.Drawing.Size(45, 15);
            this.lblBitrate.TabIndex = 9;
            this.lblBitrate.Text = "Bitrate";
            // 
            // rbtOpus
            // 
            this.rbtOpus.AutoSize = true;
            this.rbtOpus.Location = new System.Drawing.Point(325, 26);
            this.rbtOpus.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rbtOpus.Name = "rbtOpus";
            this.rbtOpus.Size = new System.Drawing.Size(131, 19);
            this.rbtOpus.TabIndex = 2;
            this.rbtOpus.Text = "OPUS (OggOpus)";
            this.rbtOpus.UseVisualStyleBackColor = true;
            // 
            // rbtAac
            // 
            this.rbtAac.AutoSize = true;
            this.rbtAac.Location = new System.Drawing.Point(213, 26);
            this.rbtAac.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rbtAac.Name = "rbtAac";
            this.rbtAac.Size = new System.Drawing.Size(104, 19);
            this.rbtAac.TabIndex = 1;
            this.rbtAac.Text = "AAC(ADTS)";
            this.rbtAac.UseVisualStyleBackColor = true;
            // 
            // rbtMp3
            // 
            this.rbtMp3.AutoSize = true;
            this.rbtMp3.Checked = true;
            this.rbtMp3.Location = new System.Drawing.Point(149, 26);
            this.rbtMp3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rbtMp3.Name = "rbtMp3";
            this.rbtMp3.Size = new System.Drawing.Size(56, 19);
            this.rbtMp3.TabIndex = 0;
            this.rbtMp3.TabStop = true;
            this.rbtMp3.Text = "MP3";
            this.rbtMp3.UseVisualStyleBackColor = true;
            // 
            // AudioTaskForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(512, 351);
            this.Controls.Add(this.gbAudio);
            this.Controls.Add(this.gbAction);
            this.Controls.Add(this.gbServerInfo);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "AudioTaskForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Audio Task";
            this.Load += new System.EventHandler(this.AudioTaskForm_Load);
            this.gbAction.ResumeLayout(false);
            this.gbServerInfo.ResumeLayout(false);
            this.gbServerInfo.PerformLayout();
            this.gbAudio.ResumeLayout(false);
            this.gbAudio.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBitrate)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbAction;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.GroupBox gbServerInfo;
        private System.Windows.Forms.TextBox edtVideoServerUrl;
        private System.Windows.Forms.Label lblServerAddress;
        private System.Windows.Forms.TextBox edtChannelName;
        private System.Windows.Forms.Label lblChannelName;
        private System.Windows.Forms.GroupBox gbAudio;
        private System.Windows.Forms.RadioButton rbtOpus;
        private System.Windows.Forms.RadioButton rbtAac;
        private System.Windows.Forms.RadioButton rbtMp3;
        private System.Windows.Forms.Label lblBitrateUnit;
        private System.Windows.Forms.Label lblAudioType;
        private System.Windows.Forms.NumericUpDown nudBitrate;
        private System.Windows.Forms.Label lblBitrate;
        private System.Windows.Forms.TextBox edtExtraParam;
        private System.Windows.Forms.Label lblExtraParam;
    }
}