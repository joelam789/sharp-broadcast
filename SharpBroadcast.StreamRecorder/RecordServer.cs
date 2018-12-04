using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;


namespace SharpBroadcast.StreamRecorder
{
    public class RecordServer
    {
        private List<MediaClient> m_Clients = new List<MediaClient>();
        private RecordConfig m_Config = new RecordConfig();

        private CommandServer m_CommandServer = null;

        private Timer m_AutoKeepOnlineTimer = null;

        public void Stop()
        {
            if (m_CommandServer != null)
            {
                try
                {
                    m_CommandServer.Stop();
                }
                catch { }
                m_CommandServer = null;
            }

            lock (m_Clients)
            {
                foreach (var client in m_Clients) client.Close();
                m_Clients.Clear();
            }

            if (m_AutoKeepOnlineTimer != null)
            {
                m_AutoKeepOnlineTimer.Dispose();
                m_AutoKeepOnlineTimer = null;
            }
        }

        public void Start()
        {
            Stop();

            ConfigurationManager.RefreshSection("appSettings");
            ConfigurationManager.RefreshSection("channels");

            var appSettings = ConfigurationManager.AppSettings;

            if (appSettings.AllKeys.Contains("Converter")) m_Config.Converter = appSettings["Converter"];
            if (appSettings.AllKeys.Contains("RemoteCallbackURL")) m_Config.Callback = appSettings["RemoteCallbackURL"];

            if (appSettings.AllKeys.Contains("StreamDataFolder")) m_Config.StreamDataFolder = appSettings["StreamDataFolder"];
            if (appSettings.AllKeys.Contains("RecordFileFolder")) m_Config.RecordFileFolder = appSettings["RecordFileFolder"];
            if (appSettings.AllKeys.Contains("RecordContentType")) m_Config.RecordContentType = appSettings["RecordContentType"];

            if (appSettings.AllKeys.Contains("MustCreateOutputFile")) m_Config.MustCreateOutputFile = Convert.ToInt32(appSettings["MustCreateOutputFile"].ToString());

            if (appSettings.AllKeys.Contains("VideoStartOffset")) m_Config.VideoStartOffset = Convert.ToDecimal(appSettings["VideoStartOffset"].ToString());
            if (appSettings.AllKeys.Contains("AudioStartOffset")) m_Config.AudioStartOffset = Convert.ToDecimal(appSettings["AudioStartOffset"].ToString());
            m_Config.VideoStartOffset = Decimal.Round(m_Config.VideoStartOffset, 2);
            m_Config.AudioStartOffset = Decimal.Round(m_Config.AudioStartOffset, 2);

            if (appSettings.AllKeys.Contains("MaxCacheSize"))
            {
                var cacheSize = appSettings["MaxCacheSize"].ToString().ToLower();
                if (cacheSize.Last() == 'k') m_Config.MaxCacheSize = Convert.ToInt32(cacheSize.Substring(0, cacheSize.Length - 1)) * 1024;
                else if (cacheSize.Last() == 'm') m_Config.MaxCacheSize = Convert.ToInt32(cacheSize.Substring(0, cacheSize.Length - 1)) * 1024 * 1024;
                else m_Config.MaxCacheSize = Convert.ToInt32(cacheSize);
            }
            if (appSettings.AllKeys.Contains("MaxRecordSize"))
            {
                var recordSize = appSettings["MaxRecordSize"].ToString().ToLower();
                if (recordSize.Last() == 'k') m_Config.MaxRecordSize = Convert.ToInt32(recordSize.Substring(0, recordSize.Length - 1)) * 1024;
                else if (recordSize.Last() == 'm') m_Config.MaxRecordSize = Convert.ToInt32(recordSize.Substring(0, recordSize.Length - 1)) * 1024 * 1024;
                else m_Config.MaxRecordSize = Convert.ToInt32(recordSize);
            }

            string streamDataFolder = Path.GetFullPath(m_Config.StreamDataFolder);
            string recordFileFolder = Path.GetFullPath(m_Config.RecordFileFolder);

            List<string> whitelist = new List<string>();
            if (appSettings.AllKeys.Contains("Whitelist"))
            {
                var list = appSettings["Whitelist"].ToString().Split(',');
                foreach (var item in list) whitelist.Add(item.Trim());
            }

            string allowOrigin = "";
            if (appSettings.AllKeys.Contains("AllowOrigin")) allowOrigin = appSettings["AllowOrigin"].ToString();

            int httpPort = 9009;
            if (appSettings.AllKeys.Contains("ServerPort")) httpPort = Convert.ToInt32(appSettings["ServerPort"].ToString());
            if (httpPort > 0)
            {
                try
                {
                    m_CommandServer = new CommandServer(this, httpPort, whitelist, allowOrigin);
                    if (!m_CommandServer.Start())
                    {
                        CommonLog.Error("Failed to start command server on port: " + httpPort);
                        try { m_CommandServer.Stop(); }
                        catch { }
                        m_CommandServer = null;
                    }
                    else
                    {
                        for (int i = 0; i < 20; i++)
                        {
                            if (!m_CommandServer.IsWorking()) Thread.Sleep(50);
                            else break;
                        }
                        if (m_CommandServer.IsWorking())
                        {
                            CommonLog.Info("Started command server on port: " + httpPort);
                        }
                    }
                }
                catch { }
            }

            try
            {
                if (!Directory.Exists(streamDataFolder)) Directory.CreateDirectory(streamDataFolder);
            }
            catch (Exception ex)
            {
                CommonLog.Error("Failed to create StreamDataFolder: " + streamDataFolder);
                CommonLog.Error(ex.Message);
                m_Config.StreamDataFolder = "";
            }

            try
            {
                if (!Directory.Exists(recordFileFolder)) Directory.CreateDirectory(recordFileFolder);
            }
            catch (Exception ex)
            {
                CommonLog.Error("Failed to create RecordFileFolder: " + recordFileFolder);
                CommonLog.Error(ex.Message);
                m_Config.RecordFileFolder = "";
            }

            var channelSettings = (NameValueCollection)ConfigurationManager.GetSection("channels");
            var allKeys = channelSettings.AllKeys;

            lock (m_Clients)
            {
                m_Clients.Clear();
                foreach (var key in allKeys)
                {
                    if (key.Trim().Length <= 0) continue;
                    string url = channelSettings[key];
                    m_Clients.Add(new MediaClient(key, url, m_Config));
                }
            }

            m_AutoKeepOnlineTimer = new Timer(TryToKeepClientsOnline, m_Clients, 500, 1000 * 15);
        }

        private void TryToKeepClientsOnline(Object clients)
        {
            List<MediaClient> list = clients as List<MediaClient>;
            if (list == null) return;
            lock (list)
            {
                var currentTime = DateTime.Now;
                List<MediaClient> badlist = new List<MediaClient>();
                List<MediaClient> newlist = new List<MediaClient>();
                List<MediaClient> idlelist = new List<MediaClient>();
                foreach (var client in list)
                {
                    if (client.Info.ErrorTimes > 5)
                    {
                        badlist.Add(client);
                    }
                    else if (client.Info.Status == "closed")
                    {
                        client.Open();
                    }
                    else if (client.Info.Status == "opened")
                    {
                        if (currentTime.Subtract(client.Info.LastActiveTime).TotalSeconds > 60)
                        {
                            idlelist.Add(client);
                        }
                    }
                }
                if (idlelist.Count > 0)
                {
                    foreach (var client in idlelist)
                    {
                        client.Close();
                    }
                }
                if (badlist.Count > 0)
                {
                    foreach (var client in badlist)
                    {
                        newlist.Add(new MediaClient(client.ChannelName, client.ChannelURL, m_Config));
                        client.Close();
                        list.Remove(client);
                    }
                }
                if (newlist.Count > 0)
                {
                    foreach (var client in newlist)
                    {
                        list.Add(client);
                    }
                    foreach (var client in newlist)
                    {
                        if (client.Info.Status == "closed") client.Open();
                    }
                }
            }
        }

        public MediaClient GetClient(string channelName)
        {
            MediaClient result = null;
            lock (m_Clients)
            {
                foreach (var client in m_Clients)
                {
                    if (client.ChannelName == channelName)
                    {
                        result = client;
                        break;
                    }
                }
            }
            return result;
        }

        public List<ClientInfo> GetClientInfoList()
        {
            List<ClientInfo> list = new List<ClientInfo>();
            lock (m_Clients)
            {
                foreach (var client in m_Clients)
                {
                    list.Add(client.Info);
                }
            }
            return list;
        }
    }
}
