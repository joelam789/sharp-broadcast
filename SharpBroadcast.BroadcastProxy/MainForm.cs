using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SharpBroadcast.BroadcastProxy
{
    public partial class MainForm : Form
    {
        Proxy m_Proxy = new Proxy();

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            CommonLog.SetGuiControl(this, mmLog);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            CommonLog.Info("=== " + Program.SVC_NAME + " is starting ===");

            m_Proxy.Start();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            m_Proxy.Stop();

            CommonLog.Info("=== " + Program.SVC_NAME + " stopped ===");
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_Proxy.Stop();
        }
    }
}
