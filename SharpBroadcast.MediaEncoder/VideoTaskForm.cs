using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Newtonsoft.Json;

namespace SharpBroadcast.MediaEncoder
{
    public partial class VideoTaskForm : Form
    {
        private VideoOutputTask m_Task = new VideoOutputTask();
        private VideoResolutionOptionGroup m_Resolution = new VideoResolutionOptionGroup();

        public VideoTaskForm()
        {
            InitializeComponent();
            ReloadResolutionOptions();
        }

        public VideoOutputTask GetVideoOutputTask()
        {
            return m_Task;
        }

        public void ReloadResolutionOptions()
        {
            ConfigurationManager.RefreshSection("appSettings");

            var appSettings = ConfigurationManager.AppSettings;
            var allKeys = appSettings.AllKeys;

            if (allKeys.Contains("ResolutionOptions"))
            {
                m_Resolution = JsonConvert.DeserializeObject<VideoResolutionOptionGroup>(appSettings["ResolutionOptions"].ToString());
                cbbResolution.Items.Clear();
                foreach (var item in m_Resolution.Options) cbbResolution.Items.Add(item.Trim());
            }
        }

        public void LoadFromTask(VideoOutputTask task)
        {
            if (task.VideoType == "mpeg") rbtMpeg1.Checked = true;
            if (task.VideoType == "h264") rbtH264.Checked = true;
            if (task.VideoType == "rtmp") rbtRtmp.Checked = true;
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

            btnAttach.Visible = rbtMpeg1.Checked;
        }

        public VideoOutputTask SaveToTask(VideoOutputTask videoOutputTask = null)
        {
            VideoOutputTask task = videoOutputTask;
            if (task == null) task = m_Task;

            if (rbtMpeg1.Checked) task.VideoType = "mpeg";
            if (rbtH264.Checked) task.VideoType = "h264";
            if (rbtRtmp.Checked) task.VideoType = "rtmp";
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
            btnAttach.Visible = rbtMpeg1.Checked;
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
            btnAttach.Visible = rbtMpeg1.Checked;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SaveToTask();
            this.DialogResult = DialogResult.OK;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            string currentResolution = "";

            if (cbbResolution.SelectedIndex >= 0)
                currentResolution = cbbResolution.Items[cbbResolution.SelectedIndex].ToString();

            ReloadResolutionOptions();

            if (currentResolution.Length > 0)
            {
                for (var i = 0; i < cbbResolution.Items.Count; i++)
                {
                    if (cbbResolution.Items[i].ToString() == currentResolution)
                    {
                        cbbResolution.SelectedIndex = i;
                        break;
                    }
                }
            }
            
        }

        private void btnAttach_Click(object sender, EventArgs e)
        {
            if (rbtMpeg1.Checked && !edtExtraParam.Text.ToLower().Contains("mp2"))
            {
                edtExtraParam.Text += " -codec:a mp2 -b:a 128k ";
            }
        }


    }

    public class VideoResolutionOptionGroup
    {
        public List<string> Options { get; set; }

        public VideoResolutionOptionGroup()
        {
            Options = new List<string>();
        }
    }

    
}
