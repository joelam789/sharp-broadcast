using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SharpBroadcast.StreamRecorder
{
    public partial class MainForm : Form
    {
        private RecordServer m_Server = new RecordServer();

        public MainForm()
        {
            InitializeComponent();
        }

        public List<string> GetSelectedChannelNames()
        {
            List<string> channels = new List<string>();
            if (olvClients.SelectedObjects != null && olvClients.SelectedObjects.Count > 0)
            {
                foreach (var obj in olvClients.SelectedObjects)
                {
                    channels.Add((obj as ClientInfo).ChannelName);
                }
            }
            return channels;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            CommonLog.SetGuiControl(this, richLogBox);
            try
            {
                m_Server.Start();

                timerRefreshInfo.Enabled = true;
                timerRefreshInfo.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to quit ?", this.Text, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                timerRefreshInfo.Enabled = false;
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }
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

        private void timerRefreshInfo_Tick(object sender, EventArgs e)
        {
            if (!Visible) return;

            List<ClientInfo> list = m_Server.GetClientInfoList();
            olvClients.SetObjects(list);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            List<string> channels = GetSelectedChannelNames();
            foreach (var channel in channels)
            {
                var client = m_Server.GetClient(channel);
                if (client != null) client.Record();
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            List<string> channels = GetSelectedChannelNames();
            foreach (var channel in channels)
            {
                var client = m_Server.GetClient(channel);
                if (client != null) client.Export();
            }
        }

        private void notifyIconMain_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

    }
}
