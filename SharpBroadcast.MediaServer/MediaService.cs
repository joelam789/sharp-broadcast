using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

using Newtonsoft.Json;
using SharpBroadcast.Framework;

namespace SharpBroadcast.MediaServer
{
    partial class MediaService : ServiceBase
    {
        private MediaResourceManager m_MediaResourceManager = new MediaResourceManager();

        public MediaService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: Add code here to start your service.

            m_MediaResourceManager.Clear();
            m_MediaResourceManager.LoadHandlers();

            ConfigurationManager.RefreshSection("appSettings");
            ConfigurationManager.RefreshSection("media-servers");

            CommonLog.Info("=== Media Server is starting ===");

            var appSettings = ConfigurationManager.AppSettings;

            string allAvailableChannels = "";
            if (appSettings.AllKeys.Contains("AvailableChannels")) allAvailableChannels = appSettings["AvailableChannels"];
            if (allAvailableChannels.Length > 0) m_MediaResourceManager.SetAvailableChannelNames(allAvailableChannels);

            string remoteValidationURL = "";
            if (appSettings.AllKeys.Contains("RemoteValidationURL")) remoteValidationURL = appSettings["RemoteValidationURL"];

            var mediaServerSettings = (NameValueCollection)ConfigurationManager.GetSection("media-servers");
            var allKeys = mediaServerSettings.AllKeys;

            foreach (var key in allKeys)
            {
                string json = mediaServerSettings[key];
                ServerSetting setting = JsonConvert.DeserializeObject<ServerSetting>(json);
                HttpSourceMediaServer mediaServer = new HttpSourceMediaServer(key, m_MediaResourceManager, CommonLog.GetLogger(), 
                    setting.InputPort, setting.OutputPort, setting.InputWhitelist, setting.CertFile, setting.CertKey);
                mediaServer.InputBufferSize = setting.InputBufferSize;
                mediaServer.OutputQueueSize = setting.OutputQueueSize;
                mediaServer.OutputBufferSize = setting.OutputBufferSize;
                mediaServer.OutputSocketBufferSize = setting.OutputSocketBufferSize;
                mediaServer.SetClientValidator(new MediaClientValidator(CommonLog.GetLogger(), remoteValidationURL));
            }

            var servers = m_MediaResourceManager.GetServerList();
            foreach (var item in servers)
            {
                if (item.Start())
                    CommonLog.Info("Media Server is working on port " + item.InputPort
                        + " (input) and port " + item.OutputPort + " (output) ... ");
            }
        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.

            m_MediaResourceManager.Clear();

            CommonLog.Info("=== Media Server stopped ===");
        }
    }
}
