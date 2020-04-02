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
        private ProcessIoWrapper m_VideoProcess = null;
        private ProcessIoWrapper m_AudioProcess = null;

        private bool m_UpdatedUI4Start = false;

        private string m_EncoderAAC = "";

        private string m_DefaultVideoDeviceName = "";
        private string m_DefaultAudioDeviceName = "";

        private string m_VideoArgs = "";
        private string m_AudioArgs = "";

        private VideoOutputTaskGroup m_VideoOutputTaskGroup = new VideoOutputTaskGroup();
        private AudioOutputTaskGroup m_AudioOutputTaskGroup = new AudioOutputTaskGroup();

        private bool m_NeedToStopAll = false;

        private DateTime m_LastVideoTime = DateTime.Now;
        private DateTime m_LastAudioTime = DateTime.Now;

        private int m_MaxRecvIdleSeconds = 0;


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

        public void StopAll()
        {
            m_NeedToStopAll = true;

            timerRestartVideo.Stop();
            timerRestartVideo.Enabled = false;

            timerRestartAudio.Stop();
            timerRestartAudio.Enabled = false;

            StopRunningProcesses();
        }

        private void StopRunningProcesses()
        {
            try
            {
                if (m_VideoProcess != null)
                {
                    m_VideoProcess.WriteStandardInput("q\n");
                    Thread.Sleep(200);
                }
            }
            catch { }

            try
            {
                
                if (m_VideoProcess != null) m_VideoProcess.StopRunningProcess();
            }
            catch { }

            m_VideoProcess = null;

            try
            {
                if (m_AudioProcess != null)
                {
                    m_AudioProcess.WriteStandardInput("q\n");
                    Thread.Sleep(200);
                }
            }
            catch { }

            try
            {
                if (m_AudioProcess != null) m_AudioProcess.StopRunningProcess();
            }
            catch { }

            m_AudioProcess = null;
        }

        private void Log4Video(string text)
        {
            try
            {
                if (mmVideoLogger.Lines.Length > 1024)
                {
                    List<string> finalLines = mmVideoLogger.Lines.ToList();
                    finalLines.RemoveRange(0, 512);
                    mmVideoLogger.Lines = finalLines.ToArray();
                }

                //mmLogger.AppendText("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + text);
                mmVideoLogger.AppendText(text);
                mmVideoLogger.SelectionStart = mmVideoLogger.Text.Length;
                mmVideoLogger.ScrollToCaret();

            }
            catch { } // just want to make it more robust
        }

        public void LogVideoMsg(string msg)
        {
            BeginInvoke((Action)(() =>
            {
                Log4Video(msg);
            }));
        }

        private void Log4Audio(string text)
        {
            try
            {
                if (mmAudioLogger.Lines.Length > 1024)
                {
                    List<string> finalLines = mmAudioLogger.Lines.ToList();
                    finalLines.RemoveRange(0, 512);
                    mmAudioLogger.Lines = finalLines.ToArray();
                }

                //mmLogger.AppendText("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + text);
                mmAudioLogger.AppendText(text);
                mmAudioLogger.SelectionStart = mmAudioLogger.Text.Length;
                mmAudioLogger.ScrollToCaret();
            }
            catch { } // just want to make it more robust
            
        }

        public void LogAudioMsg(string msg)
        {
            BeginInvoke((Action)(() =>
            {
                Log4Audio(msg);
            }));
        }

        public List<string> GenInputPart()
        {
            List<string> inputList = new List<string>();

            if (rbtnFromDevice.Checked)
            {
                if (cbbCams.SelectedIndex < 0 && cbbMics.SelectedIndex < 0)
                {
                    MessageBox.Show("Please select a device.");
                    return inputList;
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

                    //input = CommandGenerator.GenInputPart(videoDevice, audioDevice, videoOptions, audioOptions);

                    var videoInput = CommandGenerator.GenInputPart(videoDevice, "", videoOptions, "");
                    var audioInput = CommandGenerator.GenInputPart("", audioDevice, "", audioOptions);

                    inputList.Add(videoInput.Trim());
                    inputList.Add(audioInput.Trim());
                }
            }

            if (rbtnFromUrl.Checked)
            {
                if (edtVideoUrlSource.Text.Trim().Length <= 0 && edtAudioUrlSource.Text.Trim().Length <= 0)
                {
                    MessageBox.Show("Please input the URL source.");
                    return inputList;
                }
                else
                {
                    if (edtVideoUrlSource.Text.Trim().Length > 0)
                    {
                        var input = CommandGenerator.GenInputPart(edtVideoUrlSource.Text.Trim());
                        inputList.Add(input.Trim());
                    }
                    else inputList.Add("");
                    if (edtAudioUrlSource.Text.Trim().Length > 0)
                    {
                        var input = CommandGenerator.GenInputPart(edtAudioUrlSource.Text.Trim());
                        inputList.Add(input.Trim());
                    }
                }
            }

            return inputList;

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

        private void OnVideoStderrTextRead(string text)
        {
            m_LastVideoTime = DateTime.Now;
            LogVideoMsg(text);

            
        }

        private void OnVideoProcessExited()
        {
            if (m_VideoProcess != null && m_VideoProcess.HasExited())
            {
                if (ckbAutoRestart.Checked && timerRestartVideo.Interval >= 1000)
                {
                    BeginInvoke((Action)(() =>
                    {
                        timerRestartVideo.Enabled = true;
                        timerRestartVideo.Start();
                    }));
                    
                    LogVideoMsg("\n");
                    LogVideoMsg("Video process will be restarted in " + (timerRestartVideo.Interval / 1000) + " second(s) ... \n");
                    return;
                }
            }

            if ((m_VideoProcess == null || m_VideoProcess.HasExited())
                && (m_AudioProcess == null || m_AudioProcess.HasExited()))
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
                            btnStop.Enabled = true;

                            notifyIconMain.Icon = notifyIconStop.Icon;
                            notifyIconMain.Text = this.Text + " (" + notifyIconStop.Text + ")";
                        }
                    }
                    catch { }
                }));
            }
        }

        private void OnAudioStderrTextRead(string text)
        {
            m_LastAudioTime = DateTime.Now;
            LogAudioMsg(text);
        }

        private void OnAudioProcessExited()
        {
            if (m_AudioProcess != null && m_AudioProcess.HasExited())
            {
                if (ckbAutoRestart.Checked && timerRestartAudio.Interval >= 1000)
                {
                    BeginInvoke((Action)(() =>
                    {
                        timerRestartAudio.Enabled = true;
                        timerRestartAudio.Start();
                    }));

                    LogAudioMsg("\n");
                    LogAudioMsg("Audio process will be restarted in " + (timerRestartAudio.Interval / 1000) + " second(s) ... \n");
                    return;
                }
            }

            if ((m_VideoProcess == null || m_VideoProcess.HasExited())
                && (m_AudioProcess == null || m_AudioProcess.HasExited()))
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
                            btnStop.Enabled = true;

                            notifyIconMain.Icon = notifyIconStop.Icon;
                            notifyIconMain.Text = this.Text + " (" + notifyIconStop.Text + ")";
                        }
                    }
                    catch { }
                }));
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            rbtnFromDevice.Checked = false;
            cbbCams.Enabled = false;
            cbbMics.Enabled = false;
            rbtnFromUrl.Checked = true;
            edtVideoUrlSource.Enabled = true;
            edtAudioUrlSource.Enabled = true;

            btnStart.Enabled = true;
            btnStop.Enabled = true;

            bool needAudio = false;

            var appSettings = ConfigurationManager.AppSettings;
            var allKeys = appSettings.AllKeys;

            if (allKeys.Contains("EncoderAAC")) m_EncoderAAC = appSettings["EncoderAAC"];

            if (allKeys.Contains("MaxRecvIdleSeconds"))
            {
                int maxIdleTime = 0;
                if (Int32.TryParse(appSettings["MaxRecvIdleSeconds"].ToString(), out maxIdleTime))
                    m_MaxRecvIdleSeconds = maxIdleTime > 0 ? maxIdleTime : 0;
            }

            int interval = 0;
            if (allKeys.Contains("AutoRestartInterval")) int.TryParse(appSettings["AutoRestartInterval"], out interval);
            if (interval > 0)
            {
                timerRestartVideo.Interval = 1000 * interval;
                timerRestartAudio.Interval = 1000 * interval;
                ckbAutoRestart.Checked = true;
                ckbAutoRestart.Enabled = true;

                if (interval <= 1) lblRestartInterval.Text = "(restart in 1 second once stopped)";
                else lblRestartInterval.Text = "(restart in " + interval + " seconds once stopped)";
            }
            else
            {
                timerRestartVideo.Interval = 100;
                timerRestartAudio.Interval = 100;
                ckbAutoRestart.Checked = false;
                ckbAutoRestart.Enabled = false;

                lblRestartInterval.Text = "(disabled)";
            }

            if (allKeys.Contains("VideoDeviceParam")) edtVideoOption.Text = appSettings["VideoDeviceParam"];
            if (allKeys.Contains("AudioDeviceParam")) edtAudioOption.Text = appSettings["AudioDeviceParam"];

            if (allKeys.Contains("VideoDeviceName")) m_DefaultVideoDeviceName = appSettings["VideoDeviceName"];
            if (allKeys.Contains("AudioDeviceName")) m_DefaultAudioDeviceName = appSettings["AudioDeviceName"];

            if (allKeys.Contains("VideoPublishTasks"))
                m_VideoOutputTaskGroup = JsonConvert.DeserializeObject<VideoOutputTaskGroup>(appSettings["VideoPublishTasks"].ToString());

            if (allKeys.Contains("AudioPublishTasks"))
                m_AudioOutputTaskGroup = JsonConvert.DeserializeObject<AudioOutputTaskGroup>(appSettings["AudioPublishTasks"].ToString());

            if (allKeys.Contains("VideoUrlSource")) edtVideoUrlSource.Text = appSettings["VideoUrlSource"];
            if (allKeys.Contains("AudioUrlSource")) edtAudioUrlSource.Text = appSettings["AudioUrlSource"];

            if (allKeys.Contains("EnableAudio")) needAudio = Convert.ToBoolean(appSettings["EnableAudio"]);

            needAudio = true; // always enable audio ...

            cbbMics.Visible = needAudio;
            gbAudioTask.Enabled = needAudio;

            if (!needAudio)
            {
                cbbCams.Width = edtVideoUrlSource.Width;
                edtAudioUrlSource.Text = "";
                edtAudioUrlSource.Enabled = false;
                edtAudioUrlSource.Visible = false;
            }

            olvVideoTasks.SetObjects(m_VideoOutputTaskGroup.Tasks);
            olvAudioTasks.SetObjects(m_AudioOutputTaskGroup.Tasks);

            notifyIconMain.Icon = notifyIconStop.Icon;
            notifyIconMain.Text = this.Text + " (" + notifyIconStop.Text + ")";

            if (m_DefaultVideoDeviceName.Length > 0 || m_DefaultAudioDeviceName.Length > 0)
            {
                gbAction.Enabled = false;
                gbMediaSource.Enabled = false;
                timerAutoSet.Enabled = true;
            }
            else
            {
                gbAction.Enabled = true;
                gbMediaSource.Enabled = true;
                timerAutoSet.Enabled = false;
            }

            m_UpdatedUI4Start = true;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to quit ?", this.Text, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                StopRunningProcesses();
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            StopAll();
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
            if (!gbMediaSource.Enabled) return;

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
            edtVideoUrlSource.Enabled = rbtnFromUrl.Checked;
            edtAudioUrlSource.Enabled = rbtnFromUrl.Checked;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (m_NeedToStopAll) return;

            if (ckbAutoRestart.Enabled && !ckbAutoRestart.Checked)
            {
                if (MessageBox.Show("Do you want to enable 'auto-restart' at the same time ?", this.Text, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    ckbAutoRestart.Checked = true;
                }
            }

            List<string> input = new List<string>();

            string outputVideo = "";
            string outputAudio = "";

            input = GenInputPart();
            if (input.Count <= 0) return;

            outputVideo = CommandGenerator.GenVideoOutputPart(m_VideoOutputTaskGroup.Tasks);
            outputAudio = CommandGenerator.GenAudioOutputPart(m_AudioOutputTaskGroup.Tasks, m_EncoderAAC);

            if (outputVideo.Length <= 0 && outputAudio.Length <= 0) return;

            if (input.Count <= 1)
            {
                m_VideoArgs = input[0] + " " + outputVideo + " " + outputAudio;
                m_AudioArgs = "";
            }
            else
            {
                var mpegItemCount = 0;
                foreach (var taskItem in m_VideoOutputTaskGroup.Tasks)
                {
                    if (taskItem.VideoType == "mpeg") mpegItemCount++;
                }
                if (mpegItemCount > 0 && mpegItemCount == m_VideoOutputTaskGroup.Tasks.Count)
                {
                    m_VideoArgs = input[0] + " " + input[1] + " " + outputVideo;
                }
                else
                {
                    m_VideoArgs = outputVideo.Trim().Length <= 0 ? "" : (input[0].Trim().Length > 0 ? input[0] + " " + outputVideo : "");
                }
                
                m_AudioArgs = outputAudio.Trim().Length <= 0 ? "" : (input[1].Trim().Length > 0 ? input[1] + " " + outputAudio : "");
            }

            m_VideoArgs = m_VideoArgs.Trim();
            m_AudioArgs = m_AudioArgs.Trim();

            //MessageBox.Show(args);
            Console.WriteLine(m_VideoArgs);
            Console.WriteLine(m_AudioArgs);

            gbMediaSource.Enabled = false;
            gbVideoTask.Enabled = false;
            gbAudioTask.Enabled = false;

            btnStart.Enabled = false;

            m_UpdatedUI4Start = false;


            try
            {
                StopRunningProcesses();

                mmVideoLogger.Clear();
                mmAudioLogger.Clear();

                if (m_VideoArgs.Length > 0)
                {
                    if (m_VideoArgs.Contains("-f nut pipe:1"))
                    {
                        // in this case, Process.Kill() will just kill "cmd", but not "ffmpeg" ...
                        m_VideoProcess = new ProcessIoWrapper("cmd", "/C ffmpeg " + m_VideoArgs, ProcessIoWrapper.FLAG_INPUT | ProcessIoWrapper.FLAG_ERROR);
                    }
                    else
                    {
                        m_VideoProcess = new ProcessIoWrapper("ffmpeg", m_VideoArgs, ProcessIoWrapper.FLAG_INPUT | ProcessIoWrapper.FLAG_ERROR);
                    }

                    m_LastVideoTime = DateTime.Now;

                    //m_ProcessIoWrapper.OnStandardOutputTextRead = new Action<string>((text) => { OnStderrTextRead(text); });
                    m_VideoProcess.OnStandardErrorTextRead = new Action<string>((text) => { OnVideoStderrTextRead(text); });
                    m_VideoProcess.OnProcessExited = new Action(() => { OnVideoProcessExited(); });
                    m_VideoProcess.StartProcess();
                }

                if (m_AudioArgs.Length > 0)
                {
                    if (m_AudioArgs.Contains("-f nut pipe:1"))
                    {
                        // in this case, Process.Kill() will just kill "cmd", but not "ffmpeg" ...
                        m_AudioProcess = new ProcessIoWrapper("cmd", "/C ffmpeg " + m_AudioArgs, ProcessIoWrapper.FLAG_INPUT | ProcessIoWrapper.FLAG_ERROR);
                    }
                    else
                    {
                        m_AudioProcess = new ProcessIoWrapper("ffmpeg", m_AudioArgs, ProcessIoWrapper.FLAG_INPUT | ProcessIoWrapper.FLAG_ERROR);
                    }

                    m_LastAudioTime = DateTime.Now;

                    //m_ProcessIoWrapper.OnStandardOutputTextRead = new Action<string>((text) => { OnStderrTextRead(text); });
                    m_AudioProcess.OnStandardErrorTextRead = new Action<string>((text) => { OnAudioStderrTextRead(text); });
                    m_AudioProcess.OnProcessExited = new Action(() => { OnAudioProcessExited(); });
                    m_AudioProcess.StartProcess();
                }

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

            if (ckbAutoRestart.Enabled) ckbAutoRestart.Checked = false;

            if (m_VideoProcess != null)
            {
                m_VideoProcess.WriteStandardInput("q\n");
                //Thread.Sleep(500);
            }

            if (m_AudioProcess != null)
            {
                m_AudioProcess.WriteStandardInput("q\n");
                //Thread.Sleep(500);
            }

            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(500);
            })
            .ContinueWith((x) =>
            {
                if ((m_VideoProcess != null && !m_VideoProcess.HasExited())
                    || (m_AudioProcess != null && !m_AudioProcess.HasExited()))
                {
                    StopRunningProcesses();

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

            btnStop.Enabled = true;
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

        private void timerAutoSet_Tick(object sender, EventArgs e)
        {
            timerAutoSet.Enabled = false;

            int idxAudio = -1;
            int idxVideo = -1;

            List<string> videoList = new List<string>();
            List<string> audioList = new List<string>();

            try
            {
                gbMediaSource.Enabled = false;
                gbAction.Enabled = false;

                GetDevices(videoList, audioList);

                if (m_DefaultVideoDeviceName.Length > 0 && videoList.Count > 0) idxVideo = videoList.IndexOf(m_DefaultVideoDeviceName);
                if (m_DefaultAudioDeviceName.Length > 0 && audioList.Count > 0) idxAudio = audioList.IndexOf(m_DefaultAudioDeviceName);

                if (idxVideo >= 0 || idxAudio >= 0) rbtnFromDevice.Checked = true;
            }
            finally
            {
                gbAction.Enabled = true;
                gbMediaSource.Enabled = true;
            }

            if (idxVideo >= 0 || idxAudio >= 0)
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

                foreach (var item in videoList) cbbCams.Items.Add(item);

                if (cbbMics.Visible)
                {
                    foreach (var item in audioList) cbbMics.Items.Add(item);
                }

                if (idxVideo >= 0 && cbbCams.Enabled)
                {
                    cbbCams.SelectedIndex = idxVideo;
                    cbbCams.Text = cbbCams.Items[cbbCams.SelectedIndex].ToString();
                }
                if (idxAudio >= 0 && cbbMics.Enabled)
                {
                    cbbMics.SelectedIndex = idxAudio;
                    cbbMics.Text = cbbMics.Items[cbbMics.SelectedIndex].ToString();
                }
            }
        }

        private void timerRestartVideo_Tick(object sender, EventArgs e)
        {
            timerRestartVideo.Stop();
            timerRestartVideo.Enabled = false;

            if (m_NeedToStopAll) return;

            if (!ckbAutoRestart.Checked || timerRestartVideo.Interval < 1000)
            {
                LogVideoMsg("Cancelled video restart.\n");
                return;
            }
            if (m_VideoProcess != null && !m_VideoProcess.HasExited()) return;

            if (m_VideoArgs.Length > 0)
            {
                try
                {
                    if (m_VideoProcess != null) m_VideoProcess.StopRunningProcess();
                }
                catch { }

                if (m_VideoArgs.Contains("-f nut pipe:1"))
                {
                    // in this case, Process.Kill() will just kill "cmd", but not "ffmpeg" ...
                    m_VideoProcess = new ProcessIoWrapper("cmd", "/C ffmpeg " + m_VideoArgs, ProcessIoWrapper.FLAG_INPUT | ProcessIoWrapper.FLAG_ERROR);
                }
                else
                {
                    m_VideoProcess = new ProcessIoWrapper("ffmpeg", m_VideoArgs, ProcessIoWrapper.FLAG_INPUT | ProcessIoWrapper.FLAG_ERROR);
                }

                m_LastVideoTime = DateTime.Now;

                //m_ProcessIoWrapper.OnStandardOutputTextRead = new Action<string>((text) => { OnStderrTextRead(text); });
                m_VideoProcess.OnStandardErrorTextRead = new Action<string>((text) => { OnVideoStderrTextRead(text); });
                m_VideoProcess.OnProcessExited = new Action(() => { OnVideoProcessExited(); });
                m_VideoProcess.StartProcess();
            }
        }

        private void timerRestartAudio_Tick(object sender, EventArgs e)
        {
            timerRestartAudio.Stop();
            timerRestartAudio.Enabled = false;

            if (m_NeedToStopAll) return;

            if (!ckbAutoRestart.Checked || timerRestartAudio.Interval < 1000)
            {
                LogAudioMsg("Cancelled audio restart.\n");
                return;
            }
            if (m_AudioProcess != null && !m_AudioProcess.HasExited()) return;

            if (m_AudioArgs.Length > 0)
            {
                try
                {
                    if (m_AudioProcess != null) m_AudioProcess.StopRunningProcess();
                }
                catch { }

                if (m_AudioArgs.Contains("-f nut pipe:1"))
                {
                    // in this case, Process.Kill() will just kill "cmd", but not "ffmpeg" ...
                    m_AudioProcess = new ProcessIoWrapper("cmd", "/C ffmpeg " + m_AudioArgs, ProcessIoWrapper.FLAG_INPUT | ProcessIoWrapper.FLAG_ERROR);
                }
                else
                {
                    m_AudioProcess = new ProcessIoWrapper("ffmpeg", m_AudioArgs, ProcessIoWrapper.FLAG_INPUT | ProcessIoWrapper.FLAG_ERROR);
                }

                m_LastAudioTime = DateTime.Now;

                //m_ProcessIoWrapper.OnStandardOutputTextRead = new Action<string>((text) => { OnStderrTextRead(text); });
                m_AudioProcess.OnStandardErrorTextRead = new Action<string>((text) => { OnAudioStderrTextRead(text); });
                m_AudioProcess.OnProcessExited = new Action(() => { OnAudioProcessExited(); });
                m_AudioProcess.StartProcess();
            }
        }

        private void timerCheckRecvTimeout_Tick(object sender, EventArgs e)
        {
            BeginInvoke((Action)(() =>
            {
                DateTime currentTime = DateTime.Now;

                var maxIdleTime = m_MaxRecvIdleSeconds;
                if (maxIdleTime <= 0) return;

                try
                {
                    if (btnStart.Enabled == false && btnStop.Enabled == true)
                    {
                        var seconds = (currentTime - m_LastVideoTime).TotalSeconds;
                        if (seconds > maxIdleTime)
                        {
                            try
                            {
                                if (m_VideoProcess != null)
                                {
                                    m_VideoProcess.WriteStandardInput("q\n");
                                    Thread.Sleep(200);
                                }
                            }
                            catch { }

                            try
                            {

                                if (m_VideoProcess != null) m_VideoProcess.StopRunningProcess();
                            }
                            catch { }

                            m_VideoProcess = null;

                            m_LastVideoTime = DateTime.Now;
                        }
                    }

                }
                catch { }

                try
                {
                    if (btnStart.Enabled == false && btnStop.Enabled == true)
                    {
                        var seconds = (currentTime - m_LastAudioTime).TotalSeconds;
                        if (seconds > maxIdleTime)
                        {
                            try
                            {
                                if (m_AudioProcess != null)
                                {
                                    m_AudioProcess.WriteStandardInput("q\n");
                                    Thread.Sleep(200);
                                }
                            }
                            catch { }

                            try
                            {
                                if (m_AudioProcess != null) m_AudioProcess.StopRunningProcess();
                            }
                            catch { }

                            m_AudioProcess = null;

                            m_LastAudioTime = DateTime.Now;
                        }
                    }

                }
                catch { }


            }));

                
        }
    }
}
