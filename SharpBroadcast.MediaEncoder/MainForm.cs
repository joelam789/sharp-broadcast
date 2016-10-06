using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Newtonsoft.Json;

namespace SharpBroadcast.MediaEncoder
{
    public partial class MainForm : Form
    {
        private ProcessIoWrapper m_ProcessIoWrapper = null;

        private bool m_UpdatedUI4Start = false;

        private string m_EncoderAAC = "";

        private VideoOutputTaskGroup m_VideoOutputTaskGroup = new VideoOutputTaskGroup();
        private AudioOutputTaskGroup m_AudioOutputTaskGroup = new AudioOutputTaskGroup();

        public MainForm()
        {
            InitializeComponent();
        }

        public string RunCmd(string exec, string args)
        {
            string result = "";

            ProcessStartInfo pinfo = new ProcessStartInfo(exec, args);
            pinfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
            pinfo.UseShellExecute = false;
            pinfo.CreateNoWindow = true;
            pinfo.RedirectStandardOutput = false;
            pinfo.RedirectStandardError = true;
            //pinfo.StandardOutputEncoding = Encoding.UTF8;
            pinfo.StandardErrorEncoding = Encoding.UTF8;

            using (Process process = Process.Start(pinfo))
            {
                //using (StreamReader reader = process.StandardOutput)
                //{
                //    result += reader.ReadToEnd();
                //}

                using (StreamReader reader = process.StandardError)
                {
                    result += reader.ReadToEnd();
                }
            }
            return result;
        }

        public int GetDevices(List<string> videoList, List<string> audioList)
        {
            videoList.Clear();
            audioList.Clear();

            string ret = RunCmd("ffmpeg", "-f dshow -list_devices true -i dummy");
            var lines = ret.Split('\n');
            bool foundVideoInfo = false;
            bool foundAudioInfo = false;
            foreach (var line in lines)
            {
                string info = line.Trim();
                if (info.Contains("[dshow @"))
                {
                    if (!foundVideoInfo)
                    {
                        if (info.Contains("DirectShow video devices"))
                        {
                            foundVideoInfo = true;
                            continue;
                        }
                    }
                    else
                    {
                        if (!foundAudioInfo)
                        {
                            if (info.Contains("DirectShow audio devices"))
                            {
                                foundAudioInfo = true;
                                continue;
                            }

                            if (info.Contains("Alternative name"))
                            {
                                continue;
                            }
                            else
                            {
                                int begin = info.IndexOf('"');
                                int end = info.LastIndexOf('"');
                                if (end > begin && begin > 0)
                                {
                                    string deviceName = info.Substring(begin + 1, end - begin - 1).Trim();
                                    if (deviceName != null && deviceName.Length > 0) videoList.Add(deviceName);
                                }
                            }
                        }
                        else
                        {
                            if (info.Contains("Alternative name"))
                            {
                                continue;
                            }
                            else
                            {
                                int begin = info.IndexOf('"');
                                int end = info.LastIndexOf('"');
                                if (end > begin && begin > 0)
                                {
                                    string deviceName = info.Substring(begin + 1, end - begin - 1).Trim();
                                    if (deviceName != null && deviceName.Length > 0) audioList.Add(deviceName);
                                }
                            }
                        }
                    }
                }

                if (info.Contains("Immediate exit requested")) break;
            }

            return videoList.Count + audioList.Count;

        }

        private void StopRunningProcess()
        {
            try
            {
                if (m_ProcessIoWrapper != null) m_ProcessIoWrapper.StopRunningProcess();
            }
            catch { }
  
            m_ProcessIoWrapper = null;
        }

        private void Log(string text)
        {
            if (mmLogger.Lines.Length > 1024)
            {
                List<string> finalLines = mmLogger.Lines.ToList();
                finalLines.RemoveRange(0, 512);
                mmLogger.Lines = finalLines.ToArray();
            }

            //mmLogger.AppendText("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + text);
            mmLogger.AppendText(text);
            mmLogger.SelectionStart = mmLogger.Text.Length;
            mmLogger.ScrollToCaret();
        }

        public void LogMsg(string msg)
        {
            BeginInvoke((Action)(() =>
            {
                Log(msg);
            }));
        }

        public string GenInputPart()
        {
            string input = "";

            if (rbtnFromDevice.Checked)
            {
                if (cbbCams.SelectedIndex < 0 && cbbMics.SelectedIndex < 0)
                {
                    MessageBox.Show("Please select a device.");
                    return "";
                }
                else
                {
                    string videoDevice = (cbbCams.Visible && cbbCams.Enabled && cbbCams.SelectedIndex >= 0) 
                                            ? cbbCams.Items[cbbCams.SelectedIndex].ToString() : "";
                    string audioDevice = (cbbMics.Visible && cbbMics.Enabled && cbbMics.SelectedIndex >= 0) 
                                            ? cbbMics.Items[cbbMics.SelectedIndex].ToString() : "";

                    string videoOptions = (videoDevice.Length > 0 && edtVideoOption.Enabled)
                                            ? edtVideoOption.Text : "";
                    string audioOptions = (audioDevice.Length > 0 && edtAudioOption.Enabled)
                                            ? edtAudioOption.Text : "";

                    input = CommandGenerator.GenInputPart(videoDevice, audioDevice, videoOptions, audioOptions);
                }
            }

            if (rbtnFromUrl.Checked)
            {
                if (edtUrlSource.Text.Trim().Length <= 0)
                {
                    MessageBox.Show("Please input the URL source.");
                    return "";
                }
                else
                {
                    input = CommandGenerator.GenInputPart(edtUrlSource.Text.Trim());
                }
            }

            return input;

        }

        public void ReloadVideoTasks()
        {
            ConfigurationManager.RefreshSection("appSettings");

            var appSettings = ConfigurationManager.AppSettings;
            var allKeys = appSettings.AllKeys;

            if (allKeys.Contains("VideoPublishTasks"))
                m_VideoOutputTaskGroup = JsonConvert.DeserializeObject<VideoOutputTaskGroup>(appSettings["VideoPublishTasks"].ToString());

            olvVideoTasks.SetObjects(m_VideoOutputTaskGroup.Tasks);
            olvVideoTasks.Refresh();
        }

        public void SaveVideoTasks()
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configuration.AppSettings.Settings["VideoPublishTasks"].Value = JsonConvert.SerializeObject(m_VideoOutputTaskGroup);
            configuration.Save(ConfigurationSaveMode.Full, true);
            ConfigurationManager.RefreshSection("appSettings");
        }

        public void ReloadAudioTasks()
        {
            ConfigurationManager.RefreshSection("appSettings");

            var appSettings = ConfigurationManager.AppSettings;
            var allKeys = appSettings.AllKeys;

            if (allKeys.Contains("AudioPublishTasks"))
                m_AudioOutputTaskGroup = JsonConvert.DeserializeObject<AudioOutputTaskGroup>(appSettings["AudioPublishTasks"].ToString());

            olvAudioTasks.SetObjects(m_AudioOutputTaskGroup.Tasks);
            olvAudioTasks.Refresh();
        }

        public void SaveAudioTasks()
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configuration.AppSettings.Settings["AudioPublishTasks"].Value = JsonConvert.SerializeObject(m_AudioOutputTaskGroup);
            configuration.Save(ConfigurationSaveMode.Full, true);
            ConfigurationManager.RefreshSection("appSettings");
        }

        //private void OnStdoutTextRead(string text)
        //{
        //    LogMsg(text);
        //}

        private void OnStderrTextRead(string text)
        {
            LogMsg(text);
        }

        private void OnProcessExited()
        {
            BeginInvoke((Action)(() =>
            {
                try
                {
                    if (!m_UpdatedUI4Start)
                    {
                        m_UpdatedUI4Start = true;

                        gbMediaSource.Enabled = true;
                        gbVideoTask.Enabled = true;
                        gbAudioTask.Enabled = true;

                        btnStart.Enabled = true;
                        btnStop.Enabled = false;

                        notifyIconMain.Icon = notifyIconStop.Icon;
                        notifyIconMain.Text = this.Text + " (" + notifyIconStop.Text + ")";
                    }
                }
                catch { }
            }));
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            rbtnFromDevice.Checked = false;
            cbbCams.Enabled = false;
            cbbMics.Enabled = false;
            rbtnFromUrl.Checked = true;
            edtUrlSource.Enabled = true;

            btnStart.Enabled = true;
            btnStop.Enabled = false;

            bool needAudio = false;

            var appSettings = ConfigurationManager.AppSettings;
            var allKeys = appSettings.AllKeys;

            if (allKeys.Contains("EncoderAAC")) m_EncoderAAC = appSettings["EncoderAAC"];

            if (allKeys.Contains("VideoDeviceParam")) edtVideoOption.Text = appSettings["VideoDeviceParam"];
            if (allKeys.Contains("AudioDeviceParam")) edtAudioOption.Text = appSettings["AudioDeviceParam"];

            if (allKeys.Contains("VideoPublishTasks"))
                m_VideoOutputTaskGroup = JsonConvert.DeserializeObject<VideoOutputTaskGroup>(appSettings["VideoPublishTasks"].ToString());

            if (allKeys.Contains("AudioPublishTasks"))
                m_AudioOutputTaskGroup = JsonConvert.DeserializeObject<AudioOutputTaskGroup>(appSettings["AudioPublishTasks"].ToString());

            if (allKeys.Contains("EnableAudio")) needAudio = Convert.ToBoolean(appSettings["EnableAudio"]);

            needAudio = true; // always enable audio ...

            cbbMics.Visible = needAudio;
            gbAudioTask.Enabled = needAudio;

            if (!needAudio)
            {
                cbbCams.Width = edtUrlSource.Width;
            }

            if (allKeys.Contains("UrlSource")) edtUrlSource.Text = appSettings["UrlSource"];

            olvVideoTasks.SetObjects(m_VideoOutputTaskGroup.Tasks);
            olvAudioTasks.SetObjects(m_AudioOutputTaskGroup.Tasks);

            notifyIconMain.Icon = notifyIconStop.Icon;
            notifyIconMain.Text = this.Text + " (" + notifyIconStop.Text + ")";

            m_UpdatedUI4Start = true;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to quit ?", this.Text, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                StopRunningProcess();
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
            }
        }

        private void rbtnFromDevice_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnFromDevice.Checked)
            {
                cbbCams.Enabled = true;
                cbbCams.Items.Clear();

                if (cbbMics.Visible)
                {
                    cbbMics.Enabled = true;
                    cbbMics.Items.Clear();
                }

                edtVideoOption.Enabled = cbbCams.Enabled;
                edtAudioOption.Enabled = cbbMics.Enabled;

                List<string> videoList = new List<string>();
                List<string> audioList = new List<string>();

                GetDevices(videoList, audioList);

                foreach (var item in videoList) cbbCams.Items.Add(item);

                if (cbbMics.Visible)
                {
                    foreach (var item in audioList) cbbMics.Items.Add(item);
                }
            }
            else
            {
                cbbCams.Enabled = false;
                cbbCams.Items.Clear();

                if (cbbMics.Visible)
                {
                    cbbMics.Enabled = false;
                    cbbMics.Items.Clear();
                }

                edtVideoOption.Enabled = cbbCams.Enabled;
                edtAudioOption.Enabled = cbbMics.Enabled;
            }
        }

        private void rbtnFromUrl_CheckedChanged(object sender, EventArgs e)
        {
            edtUrlSource.Enabled = rbtnFromUrl.Checked;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            string input = "";

            string outputVideo = "";
            string outputAudio = "";

            input = GenInputPart();
            if (input.Length <= 0) return;

            outputVideo = CommandGenerator.GenVideoOutputPart(m_VideoOutputTaskGroup.Tasks);
            outputAudio = CommandGenerator.GenAudioOutputPart(m_AudioOutputTaskGroup.Tasks, m_EncoderAAC);

            if (outputVideo.Length <= 0 && outputAudio.Length <= 0) return;

            string args = input + " " + outputVideo + " " + outputAudio;

            //MessageBox.Show(args);
            Console.WriteLine(args);

            gbMediaSource.Enabled = false;
            gbVideoTask.Enabled = false;
            gbAudioTask.Enabled = false;

            btnStart.Enabled = false;

            m_UpdatedUI4Start = false;


            try
            {
                StopRunningProcess();

                mmLogger.Clear();

                if (args.Contains("-f nut pipe:1"))
                {
                    m_ProcessIoWrapper = new ProcessIoWrapper("cmd", "/C ffmpeg " + args, ProcessIoWrapper.FLAG_INPUT | ProcessIoWrapper.FLAG_ERROR);
                }
                else
                {
                    m_ProcessIoWrapper = new ProcessIoWrapper("ffmpeg", args, ProcessIoWrapper.FLAG_INPUT | ProcessIoWrapper.FLAG_ERROR);
                }
                
                //m_ProcessIoWrapper.OnStandardOutputTextRead = new Action<string>((text) => { OnStderrTextRead(text); });
                m_ProcessIoWrapper.OnStandardErrorTextRead = new Action<string>((text) => { OnStderrTextRead(text); });
                m_ProcessIoWrapper.OnProcessExited = new Action(() => { OnProcessExited(); });
                m_ProcessIoWrapper.StartProcess();

                btnStop.Enabled = true;

                notifyIconMain.Icon = notifyIconStart.Icon;
                notifyIconMain.Text = this.Text + " (" + notifyIconStart.Text + ")";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                m_UpdatedUI4Start = true;
                gbMediaSource.Enabled = true;
                gbVideoTask.Enabled = true;
                gbAudioTask.Enabled = true;
                btnStart.Enabled = true;
            }

        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (!btnStop.Enabled) return;

            btnStop.Enabled = false;

            if (m_ProcessIoWrapper != null)
            {
                m_ProcessIoWrapper.WriteStandardInput("q\n");
                //Thread.Sleep(500);
            }

            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(500);
            })
            .ContinueWith((x) =>
            {
                if (m_ProcessIoWrapper != null && !m_ProcessIoWrapper.HasExited())
                {
                    StopRunningProcess();

                    Task.Factory.StartNew(() =>
                    {
                        Thread.Sleep(500);
                    })
                    .ContinueWith((y) =>
                    {
                        BeginInvoke((Action)(() =>
                        {
                            if (!m_UpdatedUI4Start)
                            {
                                m_UpdatedUI4Start = true;
                                gbMediaSource.Enabled = true;
                                gbVideoTask.Enabled = true;
                                gbAudioTask.Enabled = true;
                                btnStart.Enabled = true;

                                notifyIconMain.Icon = notifyIconStop.Icon;
                                notifyIconMain.Text = this.Text + " (" + notifyIconStop.Text + ")";
                            }
                        }));
                    });
                }
                else
                {
                    BeginInvoke((Action)(() =>
                    {
                        if (!m_UpdatedUI4Start)
                        {
                            m_UpdatedUI4Start = true;
                            gbMediaSource.Enabled = true;
                            gbVideoTask.Enabled = true;
                            gbAudioTask.Enabled = true;
                            btnStart.Enabled = true;

                            notifyIconMain.Icon = notifyIconStop.Icon;
                            notifyIconMain.Text = this.Text + " (" + notifyIconStop.Text + ")";
                        }
                    }));
                }
            });

        }

        private void notifyIconMain_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void menuNotify_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Tag.ToString() == "1")
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
            }
            else if (e.ClickedItem.Tag.ToString() == "2")
            {
                Task.Factory.StartNew(() =>
                {
                    BeginInvoke((Action)(() =>
                    {
                        this.Close();
                    }));
                });
                
            }
        }

        private void btnAddVideoTask_Click(object sender, EventArgs e)
        {
            var videoForm = new VideoTaskForm();
            if (videoForm.ShowDialog() == DialogResult.OK)
            {
                m_VideoOutputTaskGroup.Tasks.Add(new VideoOutputTask(videoForm.GetVideoOutputTask()));
                olvVideoTasks.SetObjects(m_VideoOutputTaskGroup.Tasks);
                olvVideoTasks.Refresh();
            }
        }

        private void btnDeleteVideoTask_Click(object sender, EventArgs e)
        {
            if (olvVideoTasks.SelectedItem != null)
            {
                if (MessageBox.Show("Are you sure you want to delete the selected video publish task ?", this.Text, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    m_VideoOutputTaskGroup.Tasks.Remove(olvVideoTasks.SelectedItem.RowObject as VideoOutputTask);
                    olvVideoTasks.SetObjects(m_VideoOutputTaskGroup.Tasks);
                    olvVideoTasks.Refresh();
                }
            }
        }

        private void btnEditVideoTask_Click(object sender, EventArgs e)
        {
            if (olvVideoTasks.SelectedItem != null)
            {
                var videoForm = new VideoTaskForm();
                var currentItem = olvVideoTasks.SelectedItem.RowObject as VideoOutputTask;
                videoForm.LoadFromTask(currentItem);
                if (videoForm.ShowDialog() == DialogResult.OK)
                {
                    var idx = m_VideoOutputTaskGroup.Tasks.IndexOf(currentItem);
                    if (idx >= 0) m_VideoOutputTaskGroup.Tasks[idx] = new VideoOutputTask(videoForm.GetVideoOutputTask());
                    else m_VideoOutputTaskGroup.Tasks.Add(new VideoOutputTask(videoForm.GetVideoOutputTask()));
                    olvVideoTasks.SetObjects(m_VideoOutputTaskGroup.Tasks);
                    olvVideoTasks.Refresh();
                }
            }
        }

        private void btnAddAudioTask_Click(object sender, EventArgs e)
        {
            var audioForm = new AudioTaskForm();
            if (audioForm.ShowDialog() == DialogResult.OK)
            {
                m_AudioOutputTaskGroup.Tasks.Add(new AudioOutputTask(audioForm.GetAudioOutputTask()));
                olvAudioTasks.SetObjects(m_AudioOutputTaskGroup.Tasks);
                olvAudioTasks.Refresh();
            }
        }

        private void btnDeleteAudioTask_Click(object sender, EventArgs e)
        {
            if (olvAudioTasks.SelectedItem != null)
            {
                if (MessageBox.Show("Are you sure you want to delete the selected audio publish task ?", this.Text, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    m_AudioOutputTaskGroup.Tasks.Remove(olvAudioTasks.SelectedItem.RowObject as AudioOutputTask);
                    olvAudioTasks.SetObjects(m_AudioOutputTaskGroup.Tasks);
                    olvAudioTasks.Refresh();
                }
            }
        }

        private void btnEditAudioTask_Click(object sender, EventArgs e)
        {
            if (olvAudioTasks.SelectedItem != null)
            {
                var audioForm = new AudioTaskForm();
                var currentItem = olvAudioTasks.SelectedItem.RowObject as AudioOutputTask;
                audioForm.LoadFromTask(currentItem);
                if (audioForm.ShowDialog() == DialogResult.OK)
                {
                    var idx = m_AudioOutputTaskGroup.Tasks.IndexOf(currentItem);
                    if (idx >= 0) m_AudioOutputTaskGroup.Tasks[idx] = new AudioOutputTask(audioForm.GetAudioOutputTask());
                    else m_AudioOutputTaskGroup.Tasks.Add(new AudioOutputTask(audioForm.GetAudioOutputTask()));
                    olvAudioTasks.SetObjects(m_AudioOutputTaskGroup.Tasks);
                    olvAudioTasks.Refresh();
                }
            }
        }

        private void olvVideoTasks_DoubleClick(object sender, EventArgs e)
        {
            btnEditVideoTask.PerformClick();
        }

        private void olvAudioTasks_DoubleClick(object sender, EventArgs e)
        {
            btnEditAudioTask.PerformClick();
        }

        private void btnReloadVideoTasks_Click(object sender, EventArgs e)
        {
            ReloadVideoTasks();
        }

        private void btnSaveVideoTasks_Click(object sender, EventArgs e)
        {
            SaveVideoTasks();
            MessageBox.Show("Saved all video publish tasks successfully");
        }

        private void btnReloadAudioTasks_Click(object sender, EventArgs e)
        {
            ReloadAudioTasks();
        }

        private void btnSaveAudioTasks_Click(object sender, EventArgs e)
        {
            SaveAudioTasks();
            MessageBox.Show("Saved all audio publish tasks successfully");
        }
        
    }
}
