using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using WebSocket4Net;

namespace SharpBroadcast.StressTestClient
{
    public partial class MainForm : Form, ISimpleLogger
    {
        private Dictionary<string, MediaClient> m_ClientMap = new Dictionary<string, MediaClient>();

        public MainForm()
        {
            InitializeComponent();

            // always ignore certificate validation
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
        }

        private void Log(string text)
        {
            mmLogger.AppendText("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + text + Environment.NewLine);
            mmLogger.SelectionStart = mmLogger.Text.Length;
            mmLogger.ScrollToCaret();
        }

        public void LogMsg(string text)
        {
            BeginInvoke((Action)(() =>
            {
                Log(text);
            }));
        }

        public void StopClients()
        {
            lock (m_ClientMap)
            {
                foreach (var item in m_ClientMap)
                {
                    item.Value.Close();
                }
                m_ClientMap.Clear();
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            btnStop.Enabled = false;

            timerRefresh.Enabled = true;
            timerRefresh.Start();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            timerRefresh.Enabled = false;
            timerRefresh.Stop();

            StopClients();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!btnStart.Enabled) return;

            int count = Convert.ToInt32(nudClientCount.Value);
            string url = edtServerUrl.Text.Trim();
            if(count <= 0 || url.Length <= 0) return;

            btnStart.Enabled = false;
            btnStop.Enabled = true;

            List<MediaClient> list = new List<MediaClient>();

            StopClients();
            lock (m_ClientMap)
            {
                for (int i = 0; i < count; i++)
                {
                    MediaClient client = new MediaClient(this, url);
                    m_ClientMap.Add(client.Info.ClientID, client);
                    list.Add(client);
                }
            }

            Task.Factory.StartNew(() =>
            {
                foreach (var item in list)
                {
                    item.Open();
                    Thread.Sleep(10);
                }
            });

        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (!btnStop.Enabled) return;

            StopClients();

            olvClients.SetObjects(new List<ClientInfo>());

            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }

        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            Dictionary<string, MediaClient> clients = null;

            lock (m_ClientMap)
            {
                clients = new Dictionary<string, MediaClient>(m_ClientMap);
            }

            if (clients != null && clients.Count > 0)
            {
                List<ClientInfo> list = new List<ClientInfo>();
                foreach (object obj in olvClients.Objects)
                {
                    ClientInfo info = obj as ClientInfo;
                    if (info != null)
                    {
                        info.ReceivedBytes = clients[info.ClientID].Info.ReceivedBytes;
                        list.Add(info);
                    }
                }
                if (list.Count <= 0)
                {
                    foreach (var item in clients) list.Add(item.Value.Info);
                    olvClients.SetObjects(list);

                }
                else olvClients.UpdateObjects(list);
            }
        }
    }

    public class ClientInfo
    {
        public string ClientID { get; set; }
        public bool Active { get; set; }
        public string Channel { get; set; }
        public int ReceivedBytes { get; set; }
    }

    public class MediaClient
    {
        private static string m_ClientID = "0";
        private ISimpleLogger m_Logger = null;

        public string ServerURL { get; private set; }
        public WebSocket Socket { get; private set; }
        public ClientInfo Info { get; private set; }

        public MediaClient(ISimpleLogger logger, string url)
        {
            string clientId = m_ClientID;

            lock (m_ClientID)
            {
                int cid = Convert.ToInt32(m_ClientID);
                cid++;
                m_ClientID = cid.ToString();
                clientId = m_ClientID;
            }

            Info = new ClientInfo()
            {
                ClientID = clientId,
                Active = false,
                Channel = "",
                ReceivedBytes = 0
            };

            m_Logger = logger;

            ServerURL = url;

            string path = url.Substring(url.LastIndexOf('/') + 1).Trim();
            Info.Channel = path;

            Socket = new WebSocket(ServerURL);
            Socket.AllowUnstrustedCertificate = true;
            Socket.Closed += new EventHandler(WhenClosed);
            Socket.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(WhenError);
            Socket.DataReceived += new EventHandler<DataReceivedEventArgs>(WhenDataReceived);
            Socket.Opened += new EventHandler(WhenOpened);

        }

        public void Open()
        {
            Socket.Open();
        }

        public void Close()
        {
            try
            {
                Socket.Close();
            }
            catch { }
        }

        private void WhenOpened(object sender, EventArgs e)
        {
            Info.Active = true;
        }

        private void WhenError(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            //Console.WriteLine("Socket Error: " + e.Exception.Message);
        }

        private void WhenClosed(object sender, EventArgs e)
        {
            Info.Active = false;
        }

        private void WhenDataReceived(object sender, DataReceivedEventArgs e)
        {
            Info.ReceivedBytes += e.Data.Length;
        }
    }

    public interface ISimpleLogger
    {
        void LogMsg(string text); 
    }
}
