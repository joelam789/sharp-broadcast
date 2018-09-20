using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SharpBroadcast.BroadcastProxy
{
    public class Proxy
    {
        Dictionary<int, BroadcastServer> m_Servers = new Dictionary<int, BroadcastServer>();

        public void Start()
        {
            Stop();

            ConfigurationManager.RefreshSection("appSettings");

            var proxySettings = new Dictionary<int, List<string>>();
            var keys = ConfigurationManager.AppSettings.AllKeys;
            foreach (var key in keys)
            {
                int port = 0;
                if (Int32.TryParse(key, out port))
                {
                    try
                    {
                        var list = new List<string>();
                        var targets = ConfigurationManager.AppSettings[key].ToString().Split(',');
                        foreach (var target in targets) list.Add(target.Trim());
                        proxySettings.Add(port, list);
                    }
                    catch (Exception ex)
                    {
                        CommonLog.Error("Failed to load config for listening port " + port);
                        CommonLog.Error(ex.ToString());
                    }
                }
            }

            lock (m_Servers)
            {
                foreach (var item in proxySettings)
                {
                    var targets = item.Value;
                    var server = new BroadcastServer();
                    server.LoadTargets(targets);
                    if (server.Start(item.Key))
                    {
                        m_Servers.Add(item.Key, server);
                        CommonLog.Info("Added proxy [" + item.Key + "] => " + String.Join(", ", targets.ToArray()));
                    }
                }
            }
            
        }

        public void Stop()
        {
            lock (m_Servers)
            {
                foreach (var item in m_Servers)
                {
                    try
                    {
                        if (item.Value != null) item.Value.Stop();
                    }
                    catch { }
                }
                m_Servers.Clear();
            }
        }
    }
}
