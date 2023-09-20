using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SharpBroadcast.Framework
{
    public class CommandServer
    {
        private HttpListener m_HttpServer = null;
        private Thread m_HttpListenerThread = null;

        private MediaResourceManager m_MediaResourceManager = null;

        private IServerLogger m_Logger = null;

        private ExpandoObjectConverter m_CommonConverter = new ExpandoObjectConverter();

        private string m_Ip = "";
        private int m_Port = 9010;

        private string m_AllowOrigin = "";

        private List<string> m_WhiteList = new List<string>();

        public CommandServer(MediaResourceManager mediaResourceManager, IServerLogger logger = null,
            string ip = null, int port = 0, List<string> whiteList = null, string allowOrigin = null)
        {
            if (ip != null) m_Ip = ip;
            if (port > 0) m_Port = port;
            if (whiteList != null) m_WhiteList.AddRange(whiteList);
            if (m_WhiteList.Count <= 0) m_WhiteList.Add("127.0.0.1");
            if (allowOrigin != null) m_AllowOrigin = allowOrigin.Trim();

            m_MediaResourceManager = mediaResourceManager;

            if (logger != null) m_Logger = logger;
            else m_Logger = new ConsoleLogger();
        }

        public static bool Match(string pattern1, string pattern2)
        {
            if (pattern1 == pattern2) return true;
            if (pattern1 == "*" || pattern2 == "*") return true;
            if (pattern1.Length == 0) return pattern2.Length == 0;
            else if (pattern2.Length == 0) return false;

            if (pattern1[0] == '*')
            {
                for (int i = 0; i < pattern2.Length; i++)
                {
                    if (Match(pattern1.Substring(1), pattern2.Substring(i)))
                        return true;
                }
                return false;
            }
            else if (pattern2[0] == '*')
            {
                for (int i = 0; i < pattern1.Length; i++)
                {
                    if (Match(pattern1.Substring(i), pattern2.Substring(1)))
                        return true;
                }
                return false;
            }
            else return pattern1[0] == pattern2[0] && Match(pattern1.Substring(1), pattern2.Substring(1));

        }

        public List<string> GetWhitelist()
        {
            List<string> list = new List<string>();
            lock (m_WhiteList)
            {
                list.AddRange(m_WhiteList);
            }
            return list;
        }

        public void SetWhitelist(List<string> list)
        {
            lock (m_WhiteList)
            {
                m_WhiteList.Clear();
                m_WhiteList.AddRange(list);
            }
        }


        public bool Start()
        {
            if (m_HttpServer != null) Stop();

            if (m_HttpServer == null)
            {
                try
                {
                    m_HttpServer = new HttpListener();

                    if (m_Ip.Length > 0 && m_Ip != "0.0.0.0")
                    {
                        var uri = new Uri("http://" + m_Ip + ":" + m_Port, UriKind.Absolute);
                        m_HttpServer.Prefixes.Add(uri.AbsoluteUri);
                    }
                    else
                    {
                        m_HttpServer.Prefixes.Add(String.Format(@"http://+:{0}/", m_Port));
                    }

                    m_HttpServer.Start();
                    m_HttpListenerThread = new Thread(HandleHttpRequests);
                    m_HttpListenerThread.Start();

                    return true;
                }
                catch (Exception ex)
                {
                    try
                    {
                        Stop();
                    }
                    catch { }
                    m_HttpServer = null;
                    m_Logger.Error("HTTP start error: " + ex.Message);
                }
            }
            return false;
        }

        private void HandleHttpRequests()
        {
            while (m_HttpServer.IsListening)
            {
                try
                {
                    var context = m_HttpServer.BeginGetContext(new AsyncCallback(HttpListenerCallback), m_HttpServer);
                    context.AsyncWaitHandle.WaitOne();
                }
                catch { }
            }
        }

        private void HttpListenerCallback(IAsyncResult ar)
        {
            HttpListener listener = null;
            HttpListenerContext context = null;

            string remoteIp = "";

            try
            {
                listener = ar.AsyncState as HttpListener;
                context = listener.EndGetContext(ar);
                remoteIp = context.Request.RemoteEndPoint.Address.ToString();
            }
            catch (Exception ex)
            {
                m_Logger.Error("HTTP context error: " + ex.Message);
                return;
            }

            bool isValidAddress = false; // must set whitelist ...
            if (m_WhiteList.Count > 0 && remoteIp.Length > 0)
            {
                lock (m_WhiteList)
                {
                    foreach (string pattern in m_WhiteList)
                    {
                        if (Match(pattern, remoteIp))
                        {
                            isValidAddress = true;
                            break;
                        }
                    }
                }
            }


            try
            {
                if (context != null)
                {
                    if (m_AllowOrigin != null && m_AllowOrigin.Length > 0)
                    {
                        try
                        {
                            context.Response.AppendHeader("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");
                            context.Response.AppendHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS, PUT, HEAD, DELETE, CONNECT");
                            context.Response.AppendHeader("Access-Control-Allow-Origin", m_AllowOrigin);
                        }
                        catch { }
                    }
                }

                bool isRequestOK = false;
                if (context != null)
                {
                    isRequestOK = isValidAddress;
                }

                if (isRequestOK)
                {
                    using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
                    {
                        string message = reader.ReadToEnd();

                        if (message != null && message.Length > 0)
                        {
                            IDictionary<string, object> msg = null;
                            try
                            {
                                msg = JsonConvert.DeserializeObject<ExpandoObject>(message, m_CommonConverter) as IDictionary<string, object>;
                            }
                            catch { }

                            if (msg == null || msg.Keys.Count <= 0) return;

                            Dictionary<string, string> reqestParams = new Dictionary<string, string>();
                            foreach (var item in msg) reqestParams.Add(item.Key.Trim(), item.Value.ToString().Trim());

                            ProcessRequest(context, reqestParams);

                        }
                    }

                }
                else
                {
                    if (!string.IsNullOrEmpty(remoteIp))
                    {
                        m_Logger.Error("Invalid Request IP: " + remoteIp);
                    }

                    if (context != null)
                    {
                        byte[] buffer = Encoding.UTF8.GetBytes("Invalid IP Address");
                        context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                m_Logger.Error("HTTP process error: " + ex.Message);
            }
            finally
            {
                try
                {
                    if (context != null)
                    {
                        context.Response.OutputStream.Close();
                    }
                }
                catch { }
            }
        }

        private void ProcessRequest(HttpListenerContext context, Dictionary<string, string> reqestParams)
        {
            if (m_MediaResourceManager == null) return;

            if (!reqestParams.ContainsKey("Command")) return;

            m_Logger.Info("Request: " + JsonConvert.SerializeObject(reqestParams));

            if (reqestParams["Command"] == "SetMute")
            {
                if (!reqestParams.ContainsKey("Value")) return;
                if (reqestParams["Value"] == "1") m_MediaResourceManager.Mute = true;
                else m_MediaResourceManager.Mute = false;

                byte[] buffer = Encoding.UTF8.GetBytes("OK");
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            }
        }

        public void Stop()
        {
            try
            {
                if (m_HttpServer != null && m_HttpServer.IsListening)
                {
                    m_HttpServer.Stop();
                }
            }
            catch { }

            try
            {
                if (m_HttpListenerThread != null)
                {
                    m_HttpListenerThread.Join();
                }
            }
            catch { }

            m_HttpListenerThread = null;

            try
            {
                if (m_HttpServer != null)
                {
                    m_HttpServer.Close();
                }
            }
            catch { }

            m_HttpServer = null;
        }

        public bool IsWorking()
        {
            return m_HttpServer != null && m_HttpServer.IsListening;
        }

        public int GetPort()
        {
            return m_Port;
        }
    }
}
