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

        List<string> m_InputWhiteList = new List<string>();

        public List<string> GetSourceWhitelist()
        {
            List<string> list = new List<string>();
            lock (m_InputWhiteList)
            {
                list.AddRange(m_InputWhiteList);
            }
            return list;
        }

        public void SetSourceWhitelist(List<string> list)
        {
            lock (m_InputWhiteList)
            {
                m_InputWhiteList.Clear();
                m_InputWhiteList.AddRange(list);
            }
        }

        public void Start()
        {
            Stop();

            ConfigurationManager.RefreshSection("appSettings");

            var proxySettings = new Dictionary<int, Dictionary<string, List<string>>>();
            var keys = ConfigurationManager.AppSettings.AllKeys;
            var inputWhiteList = new List<string>();

            var maxRecvIdleSeconds = 0;

            foreach (var key in keys)
            {
                if (key == "InputWhitelist")
                {
                    var ips = ConfigurationManager.AppSettings[key].ToString().Split(',');
                    foreach (var ip in ips) inputWhiteList.Add(ip.Trim());
                    continue;
                }

                if (key == "MaxRecvIdleSeconds")
                {
                    int maxIdleTime = 0;
                    if (Int32.TryParse(ConfigurationManager.AppSettings[key].ToString(), out maxIdleTime))
                        maxRecvIdleSeconds = maxIdleTime > 0 ? maxIdleTime : 0;
                    continue;
                }

                var mainParts = key.Split('/');
                if (mainParts == null || mainParts.Length < 2) continue;

                int port = 0;
                if (Int32.TryParse(mainParts[0], out port))
                {
                    try
                    {
                        var reqPath = mainParts[1].Trim();

                        var list = new List<string>();
                        var targets = ConfigurationManager.AppSettings[key].ToString().Split(',');
                        foreach (var target in targets) list.Add(target.Trim());

                        Dictionary<string, List<string>> clients = null;
                        if (proxySettings.ContainsKey(port)) clients = proxySettings[port];
                        else
                        {
                            clients = new Dictionary<string, List<string>>();
                            proxySettings.Add(port, clients);
                        }

                        if (clients != null)
                        {
                            if (clients.ContainsKey(reqPath)) clients.Remove(reqPath);
                            clients.Add(reqPath, list);
                        }
                    }
                    catch (Exception ex)
                    {
                        CommonLog.Error("Failed to load config for listening port " + port);
                        CommonLog.Error(ex.ToString());
                    }
                }
            }

            CommonLog.Info("Max receiving idle seconds: " + maxRecvIdleSeconds);

            lock (m_InputWhiteList)
            {
                if (inputWhiteList != null)
                {
                    m_InputWhiteList.Clear();
                    m_InputWhiteList.AddRange(inputWhiteList);
                }
                if (m_InputWhiteList.Count <= 0) m_InputWhiteList.Add("127.0.0.1");
            }

            var ipWhitelist = GetSourceWhitelist();
            CommonLog.Info("Input source whitelist: " + String.Join(", ", ipWhitelist.ToArray()));

            lock (m_Servers)
            {
                foreach (var item in proxySettings)
                {
                    var targets = item.Value;
                    var server = new BroadcastServer(item.Key, maxRecvIdleSeconds);
                    server.LoadTargets(targets);
                    if (server.Start(ipWhitelist))
                    {
                        m_Servers.Add(item.Key, server);
                        foreach (var target in targets)
                        {
                            CommonLog.Info("Added proxy [" + item.Key + "/" + target.Key + "] => " + String.Join(", ", target.Value.ToArray()));
                        }
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
