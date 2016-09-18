using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SharpBroadcast.MediaEncoder
{
    public partial class AudioTaskForm : Form
    {
        private AudioOutputTask m_Task = new AudioOutputTask();

        public AudioTaskForm()
        {
            InitializeComponent();
        }

        public AudioOutputTask GetAudioOutputTask()
        {
            return m_Task;
        }

        public void LoadFromTask(AudioOutputTask task)
        {
            if (task.AudioType == "mp3") rbtMp3.Checked = true;
            if (task.AudioType == "aac") rbtAac.Checked = true;
            if (task.AudioType == "opus") rbtOpus.Checked = true;

            nudBitrate.Value = task.Bitrate;
            edtExtraParam.Text = task.ExtraParam;
            edtChannelName.Text = task.ChannelName;
            edtVideoServerUrl.Text = task.ServerAddress;
        }

        public AudioOutputTask SaveToTask(AudioOutputTask audioOutputTask = null)
        {
            AudioOutputTask task = audioOutputTask;
            if (task == null) task = m_Task;

            if (rbtMp3.Checked) task.AudioType = "mp3";
            if (rbtAac.Checked) task.AudioType = "aac";
            if (rbtOpus.Checked) task.AudioType = "opus";

            task.Bitrate = Convert.ToInt32(nudBitrate.Value);
            task.ExtraParam = edtExtraParam.Text;
            task.ChannelName = edtChannelName.Text;
            task.ServerAddress = edtVideoServerUrl.Text;

            return task;
        }

        private void AudioTaskForm_Load(object sender, EventArgs e)
        {

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SaveToTask();
            this.DialogResult = DialogResult.OK;
        }
    }

    
}
