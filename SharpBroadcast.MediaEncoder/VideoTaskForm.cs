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
    public partial class VideoTaskForm : Form
    {
        private VideoOutputTask m_Task = new VideoOutputTask();

        public VideoTaskForm()
        {
            InitializeComponent();
        }

        public VideoOutputTask GetVideoOutputTask()
        {
            return m_Task;
        }

        public void LoadFromTask(VideoOutputTask task)
        {
            if (task.VideoType == "mpeg") rbtMpeg1.Checked = true;
            if (task.VideoType == "h264") rbtH264.Checked = true;
            for (var i = 0; i < cbbResolution.Items.Count; i++)
            {
                if (cbbResolution.Items[i].ToString() == task.Resolution)
                {
                    cbbResolution.SelectedIndex = i;
                    break;
                }
            }
            nudFps.Value = task.FPS;
            nudBitrate.Value = task.Bitrate;
            edtPixelFormat.Text = task.PixelFormat;
            edtExtraParam.Text = task.ExtraParam;
            edtChannelName.Text = task.ChannelName;
            edtVideoServerUrl.Text = task.ServerAddress;
        }

        public VideoOutputTask SaveToTask(VideoOutputTask videoOutputTask = null)
        {
            VideoOutputTask task = videoOutputTask;
            if (task == null) task = m_Task;

            if (rbtMpeg1.Checked) task.VideoType = "mpeg";
            if (rbtH264.Checked) task.VideoType = "h264";
            if (cbbResolution.SelectedIndex >= 0)
                task.Resolution = cbbResolution.Items[cbbResolution.SelectedIndex].ToString();

            task.FPS = Convert.ToInt32(nudFps.Value);
            task.Bitrate = Convert.ToInt32(nudBitrate.Value);
            task.PixelFormat = edtPixelFormat.Text;
            task.ExtraParam = edtExtraParam.Text;
            task.ChannelName = edtChannelName.Text;
            task.ServerAddress = edtVideoServerUrl.Text;

            return task;
        }

        private void VideoTaskForm_Load(object sender, EventArgs e)
        {
            if (cbbResolution.SelectedIndex < 0)
            {
                if (cbbResolution.Items.Count > 0) cbbResolution.SelectedIndex = 0;
            }
        }

        private void rbtH264_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtH264.Checked)
            {
                if (edtPixelFormat.Text == "")
                {
                    edtPixelFormat.Text = "yuv420p";
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SaveToTask();
            this.DialogResult = DialogResult.OK;
        }


    }

    
}
