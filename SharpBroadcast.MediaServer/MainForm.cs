using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Newtonsoft.Json;
using SharpBroadcast.Framework;

namespace SharpBroadcast.MediaServer
{
    public partial class MainForm : Form
    {
        private MediaResourceManager m_MediaResourceManager = new MediaResourceManager();

        public MainForm()
        {
            InitializeComponent();
        }

        public static DialogResult InputBox(string title, string promptText, ref string value)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }

        private void Log(string text)
        {
            mmLogger.AppendText("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + text + Environment.NewLine);
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

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                m_MediaResourceManager.LoadHandlers();

                var appSettings = ConfigurationManager.AppSettings;

                string allAvailableChannels = "";
                if (appSettings.AllKeys.Contains("AvailableChannels")) allAvailableChannels = appSettings["AvailableChannels"];
                if (allAvailableChannels.Length > 0) m_MediaResourceManager.SetAvailableChannelNames(allAvailableChannels);

                string remoteValidationURL = "";
                if (appSettings.AllKeys.Contains("RemoteValidationURL")) remoteValidationURL = appSettings["RemoteValidationURL"];

                Dictionary<string, int> channelInputQueueLengths = null;
                var channelInputQueueLengthSetting = ConfigurationManager.GetSection("channel-input-queue-lengths") as NameValueCollection;
                if (channelInputQueueLengthSetting != null)
                {
                    channelInputQueueLengths = new Dictionary<string, int>();
                    var channelKeys = channelInputQueueLengthSetting.AllKeys;
                    foreach (var key in channelKeys)
                    {
                        int len = 0;
                        if (Int32.TryParse(channelInputQueueLengthSetting[key], out len))
                        {
                            if (!channelInputQueueLengths.ContainsKey(key)) channelInputQueueLengths.Add(key, len);
                        }
                    }
                }

                var mediaServerSettings = (NameValueCollection)ConfigurationManager.GetSection("media-servers");
                var allKeys = mediaServerSettings.AllKeys;

                foreach (var key in allKeys)
                {
                    string json = mediaServerSettings[key];
                    ServerSetting setting = JsonConvert.DeserializeObject<ServerSetting>(json);
                    HttpSourceMediaServer mediaServer = new HttpSourceMediaServer(key, m_MediaResourceManager, CommonLog.GetLogger(),
                        setting.InputIp, setting.InputPort, setting.OutputIp, setting.OutputPort, setting.InputWhitelist, setting.CertFile, setting.CertKey, channelInputQueueLengths);
                    mediaServer.InputQueueSize = setting.InputQueueSize;
                    mediaServer.InputBufferSize = setting.InputBufferSize;
                    mediaServer.OutputQueueSize = setting.OutputQueueSize;
                    mediaServer.OutputBufferSize = setting.OutputBufferSize;
                    mediaServer.OutputSocketBufferSize = setting.OutputSocketBufferSize;
                    mediaServer.SetClientValidator(new MediaClientValidator(CommonLog.GetLogger(), remoteValidationURL));
                }

                if (allAvailableChannels.Length > 0) LogMsg("Available Channel Names: " + allAvailableChannels);
                else LogMsg("Any channel name would be accepted");

                if (remoteValidationURL.Length > 0) LogMsg("Remote Validation URL: " + remoteValidationURL);
                else LogMsg("Remote validation has not been set (Any connection would be accepted)");

                var startedServerCount = 0;
                var servers = m_MediaResourceManager.GetServerList();
                foreach (var server in servers)
                {
                    if (server.Start())
                    {
                        startedServerCount++;
                        if (startedServerCount == 1) // just show the first server's whitelist ...
                        {
                            List<string> list = server.GetSourceWhitelist();
                            listWhitelist.Items.Clear();
                            foreach (var item in list) listWhitelist.Items.Add(item.ToString());
                        }
                        LogMsg(server.ServerName + " is working on port " + server.InputPort + " (input) and port " + server.OutputPort + " (output) ... ");

                    }
                    else LogMsg("Failed to start " + server.ServerName + "! ports: " + server.InputPort + " , " + server.OutputPort);
                }

                timerRefreshInfo.Enabled = true;
                timerRefreshInfo.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to quit ?", this.Text, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                timerRefreshInfo.Enabled = false;
                m_MediaResourceManager.Clear();
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

        private void timerRefreshInfo_Tick(object sender, EventArgs e)
        {
            if (!this.Visible) return;
            List<MediaChannelState> states = new List<MediaChannelState>();

            var servers = m_MediaResourceManager.GetServerList();
            foreach (var item in servers)
            {
                if (item.IsWorking()) states.AddRange(item.GetChannelStates());
            }

            olvClients.SetObjects(states);
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
                    Invoke((Action)(() =>
                    {
                        this.Close();
                    }));
                });
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string address = "";
            if (InputBox("Add new IP to whitelist", "IP Address", ref address) == DialogResult.OK)
            {
                if (address.Trim().Length > 0)
                {
                    listWhitelist.Items.Add(address.Trim());

                    List<string> list = new List<string>();
                    foreach (var item in listWhitelist.Items) list.Add(item.ToString());
                    list.Add(address.Trim());

                    var servers = m_MediaResourceManager.GetServerList();
                    foreach (var item in servers)
                    {
                        item.SetSourceWhitelist(list);
                    }
                }
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            string address = "";
            if (listWhitelist.SelectedIndex >= 0) address = listWhitelist.Items[listWhitelist.SelectedIndex].ToString();
            if (address.Length > 0)
            {
                if (MessageBox.Show("Are you sure you want to remove IP " + address + " from the whilelist ?",
                    this.Text, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    listWhitelist.Items.RemoveAt(listWhitelist.SelectedIndex);

                    List<string> list = new List<string>();
                    foreach (var item in listWhitelist.Items) list.Add(item.ToString());

                    var servers = m_MediaResourceManager.GetServerList();
                    foreach (var item in servers)
                    {
                        item.SetSourceWhitelist(list);
                    }
                }
            }
        }
    }
}
