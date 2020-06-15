using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using SharpNetwork.Core;
using SharpNetwork.SimpleWebSocket;

namespace SharpBroadcast.Framework
{
    public class HttpSourceMediaServer : IMediaServer
    {
        public const int DEFAULT_INPUT_QUEUE_SIZE = 32;
        public const int DEFAULT_INPUT_BUFFER_SIZE = 8;
        public const int DEFAULT_OUTPUT_QUEUE_SIZE = 256;
        public const int DEFAULT_OUTPUT_BUFFER_SIZE = 256;
        
        protected string m_ServerName = "";

        protected string m_InputIp = "0.0.0.0"; // any ip
        protected int m_InputPort = 9210;
        protected string m_OutputIp = "0.0.0.0"; // any ip
        protected int m_OutputPort = 9220;

        protected string m_CertFile = "";
        protected string m_CertKey = "";

        protected List<string> m_InputWhiteList = new List<string>();

        protected Dictionary<string, List<object>> m_WebClients = new Dictionary<string, List<object>>();

        protected Dictionary<string, MediaChannelState> m_States = new Dictionary<string, MediaChannelState>();

        protected Dictionary<string, int> m_ChannelInputQueueLengths = null;

        protected HttpListener m_InputServer = null;
        protected Thread m_InputListenerThread = null;

        protected WebSocketServer m_OutputServer = null;

        protected IClientValidator m_ClientValidator = null;

        private Timer m_Timer = null;

        private IServerLogger m_Logger = null;

        private MediaResourceManager m_ResourceManager = null;

        public int InputQueueSize { get; set; }
        public int InputBufferSize { get; set; }

        public int OutputQueueSize { get; set; }
        public int OutputBufferSize { get; set; }
        public int OutputSocketBufferSize { get; set; }

        public string InputIp { get { return m_InputIp; } }
        public int InputPort { get { return m_InputPort; } }
        public string OutputIp { get { return m_OutputIp; } }
        public int OutputPort { get { return m_OutputPort; } }

        public string ServerName { get { return m_ServerName; } }

        public IServerLogger Logger { get { return m_Logger; } }

        public HttpSourceMediaServer(string serverName, MediaResourceManager resourceManager, IServerLogger logger = null,
                            string inputIp = "", int inputPort = -1, string outputIp = "", int outputPort = -1, 
                            List<string> inputWhiteList = null, string certFile = "", string certKey = "", Dictionary<string, int> channelInputQueueLengths = null)
        {
            InputQueueSize = DEFAULT_INPUT_QUEUE_SIZE;
            InputBufferSize = DEFAULT_INPUT_BUFFER_SIZE;

            OutputQueueSize = DEFAULT_OUTPUT_QUEUE_SIZE;
            OutputBufferSize = DEFAULT_OUTPUT_BUFFER_SIZE;
            OutputSocketBufferSize = 0;  // use default value

            m_ServerName = serverName;

            m_CertFile = certFile;
            m_CertKey = certKey;

            if (logger != null) m_Logger = logger;
            else m_Logger = new ConsoleLogger();

            m_ResourceManager = resourceManager;
            m_ResourceManager.AddServer(this);

            if (inputIp.Length > 0) m_InputIp = inputIp;
            if (outputIp.Length > 0) m_OutputIp = outputIp;

            if (inputPort >= 0) m_InputPort = inputPort;
            if (outputPort >= 0) m_OutputPort = outputPort;

            if (inputWhiteList != null) m_InputWhiteList.AddRange(inputWhiteList);
            if (m_InputWhiteList.Count <= 0) m_InputWhiteList.Add("127.0.0.1");

            if (channelInputQueueLengths != null) m_ChannelInputQueueLengths = new Dictionary<string, int>(channelInputQueueLengths);
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

        private void StartAutoUpdaeStates(int interval = 1000 * 5)
        {
            if (m_Timer != null) StopAutoUpdaeStates();
            if (m_Timer == null) m_Timer = new Timer(AutoUpdaeStates, this, 200, interval);
        }

        private void StopAutoUpdaeStates()
        {
            if (m_Timer != null)
            {
                m_Timer.Dispose();
                m_Timer = null;
            }
        }

        private void AutoUpdaeStates(object param)
        {
            lock (m_States)
            {
                lock (m_WebClients)
                {
                    foreach (var item in m_States)
                    {
                        if (m_WebClients.ContainsKey(item.Key))
                        {
                            item.Value.ClientCount = m_WebClients[item.Key].Count;
                        }
                    }
                }
            }
        }

        public void SetSSL(string certFile, string certKey)
        {
            m_CertFile = certFile;
            m_CertKey = certKey;
        }

        public MediaChannel GetChannel(string channelName)
        {
            return m_ResourceManager.GetChannel(channelName, m_ResourceManager.IsAvailableChannelName(channelName));
        }

        public int GetChannelInputQueueLength(string channelName)
        {
            if (m_ChannelInputQueueLengths != null && m_ChannelInputQueueLengths.ContainsKey(channelName))
                return m_ChannelInputQueueLengths[channelName];

            return 0;
        }

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

        public List<MediaChannelState> GetChannelStates()
        {
            List<MediaChannelState> states = new List<MediaChannelState>();
            lock (m_States)
            {
                foreach (var item in m_States)
                {
                    MediaChannelState state = new MediaChannelState(item.Value);
                    states.Add(state);
                }
            }
            return states;
        }

        public void UpdateState(string channelName, MediaChannelState state)
        {
            // update state
            lock (m_States)
            {
                int streams = 1;
                if (m_States.ContainsKey(channelName))
                {
                    streams += m_States[channelName].SourceCount;
                    m_States.Remove(channelName); // refresh it
                }
                state.ServerInfo = ServerName + "(" + InputIp + ":" + InputPort + "/" + OutputIp + ":" + OutputPort + ")";
                state.SourceCount = streams;
                m_States.Add(channelName, state);
            }
        }

        public int RemoveState(string channelName)
        {
            int left = 0;
            lock (m_States)
            {
                if (m_States.ContainsKey(channelName))
                {
                    left = m_States[channelName].SourceCount - 1;
                    if (left <= 0)
                    {
                        left = 0;
                        m_States.Remove(channelName);
                    }
                    else
                    {
                        m_States[channelName].SourceCount = left;
                    }
                }
            }
            return left;
        }

        public void SetClientValidator(IClientValidator validator)
        {
            m_ClientValidator = validator;
        }

        public virtual void RemoveClient(object client)
        {
            try
            {
                if (client != null && client is Session)
                {
                    Session session = client as Session;

                    string requestPath = "";
                    var sessionData = WebMessage.GetSessionData(session, true); // make sure session data is ok
                    if (sessionData.ContainsKey("Path")) requestPath = sessionData["Path"].ToString();

                    if (requestPath == null || requestPath.Length <= 0) return;

                    List<string> paramList = new List<string>();
                    string[] parts = requestPath.Split('/');
                    foreach (var part in parts)
                    {
                        if (part.Trim().Length > 0)
                        {
                            paramList.Add(part.Trim());
                        }
                    }
                    if (paramList.Count <= 0) return;

                    string sourceName = paramList.First();
                    if (sourceName != null && sourceName.Length > 0)
                    {
                        List<object> clients = null;
                        lock (m_WebClients)
                        {
                            if (m_WebClients.ContainsKey(sourceName))
                            {
                                clients = m_WebClients[sourceName];
                            }
                        }
                        if (clients != null)
                        {
                            lock (clients)
                            {
                                clients.Remove(session);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Error("Failed to remove client: " + ex.Message);
            }

        }

        public virtual void ValidateClient(object client)
        {
            Session session = client as Session;
            if (session == null) return;

            Task.Factory.StartNew(() =>
            {
                try
                {
                    string target = "";
                    string reqpath = WebMessage.GetSessionData(session, "Path").ToString();

                    if (m_ClientValidator != null)
                    {
                        target = m_ClientValidator.Validate(session.GetRemoteIp(), reqpath);
                    }
                    else
                    {
                        List<string> paramList = new List<string>();
                        string[] parts = reqpath.Split('/');
                        foreach (var part in parts)
                        {
                            if (part.Trim().Length > 0)
                            {
                                paramList.Add(part.Trim());
                            }
                        }
                        if (paramList.Count > 0) target = paramList.First();
                    }
                    
                    if (target != null && target.Length > 0)
                    {
                        bool isOK = true;
                        try
                        {
                            
                            byte[] welcomeData = null;
                            string welcomeText = "";

                            var channel = m_ResourceManager.GetChannel(target, false);
                            if (channel != null)
                            {
                                welcomeData = channel.GetWelcomeData();
                                welcomeText = channel.GetWelcomeText();
                            }
                            if (welcomeData != null && welcomeData.Length > 0) session.Send(new WebMessage(welcomeData));
                            if (welcomeText != null && welcomeText.Length > 0) session.Send(new WebMessage(welcomeText));

                        }
                        catch { isOK = false; }

                        if (isOK)
                        {
                            List<object> clients = null;
                            int channelClientCount = 0;
                            lock (m_WebClients)
                            {
                                if (m_WebClients.ContainsKey(target))
                                {
                                    clients = m_WebClients[target];
                                }
                            }
                            if (clients != null)
                            {
                                lock (clients)
                                {
                                    clients.Add(session);
                                    channelClientCount = clients.Count;
                                }
                            }

                            if (m_InputPort <= 0)
                            {
                                // update state
                                lock (m_States)
                                {
                                    if (!m_States.ContainsKey(target))
                                    {
                                        var channelState = new MediaChannelState();
                                        channelState.ChannelName = target;
                                        channelState.ClientCount = channelClientCount;
                                        channelState.ServerInfo = m_ServerName + "(" + m_InputIp + ":" + m_InputPort + "/" + m_OutputIp + ":" + m_OutputPort + ")";
                                        m_States.Add(target, channelState);
                                    }
                                }
                            }
                        }

                    }
                    else
                    {
                        session.Close(); // just close it if failed to validate
                    }

                }
                catch (Exception ex)
                {
                    Logger.Error("Validation error: " + ex.Message);
                }
            });

        }

        public List<object> GetClients(string channelName)
        {
            // get clients
            List<object> clients = null;
            lock (m_WebClients)
            {
                if (m_WebClients.ContainsKey(channelName)) clients = m_WebClients[channelName];
                else
                {
                    clients = new List<object>();
                    m_WebClients.Add(channelName, clients);
                }
            }
            return clients;
        }

        public bool Start(string inputIp = "", int inputPort = -1, string outputIp = "", int outputPort = -1, List<string> inputWhiteList = null)
        {
            if (m_InputServer != null || m_OutputServer != null) Stop();

            if (inputIp.Length > 0) m_InputIp = inputIp;
            if (outputIp.Length > 0) m_OutputIp = outputIp;

            if (inputPort >= 0) m_InputPort = inputPort;
            if (outputPort >= 0) m_OutputPort = outputPort;

            lock (m_InputWhiteList)
            {
                if (inputWhiteList != null)
                {
                    m_InputWhiteList.Clear();
                    m_InputWhiteList.AddRange(inputWhiteList);
                }
                if (m_InputWhiteList.Count <= 0) m_InputWhiteList.Add("127.0.0.1");
            }

            bool isServerOK = true;

            if (isServerOK && m_InputServer == null && m_InputPort > 0)
            {
                isServerOK = false;

                try
                {
                    m_InputServer = new HttpListener();

                    if (m_InputIp.Length > 0 && m_InputIp != "0.0.0.0")
                    {
                        var uri = new Uri("http://" + m_InputIp + ":" + m_InputPort, UriKind.Absolute);
                        m_InputServer.Prefixes.Add(uri.AbsoluteUri);
                    }
                    else
                    {
                        m_InputServer.Prefixes.Add(String.Format(@"http://+:{0}/", m_InputPort));
                    }

                    m_InputServer.Start();
                    m_InputListenerThread = new Thread(HandleInputRequests);
                    m_InputListenerThread.Start();

                    isServerOK = true;

                }
                catch (Exception ex)
                {
                    try
                    {
                        Stop();
                    }
                    catch { }
                    m_InputServer = null;
                    Logger.Error("HTTP listening error: " + ex.Message);
                }
            }

            if (isServerOK && m_OutputServer == null && m_OutputPort > 0)
            {
                isServerOK = false;

                try
                {
                    m_OutputServer = new WebSocketServer(this, m_CertFile, m_CertKey);

                    //m_OutputServer.ActualServer.SetIdleTime(Session.IO_BOTH, 3 * 60); // set max idle time to 3 mins

                    if (m_OutputIp.Length > 0 && m_OutputIp != "0.0.0.0")
                    {
                        m_OutputServer.ActualServer.Start(m_OutputIp, m_OutputPort);
                    }
                    else
                    {
                        m_OutputServer.ActualServer.Start(m_OutputPort);
                    }
                    
                    isServerOK = true;
                }
                catch (Exception ex)
                {
                    try
                    {
                        Stop();
                    }
                    catch { }
                    m_OutputServer = null;
                    Logger.Error("WebSocket listening error: " + ex.Message);
                }
            }

            isServerOK = isServerOK && IsWorking();

            if (isServerOK) StartAutoUpdaeStates();
            else
            {
                try
                {
                    Stop();
                }
                catch { }
                m_InputServer = null;
                m_OutputServer = null;
                Logger.Error("Failed to start server.");
            }

            return isServerOK;
        }

        private void HandleInputRequests()
        {
            while (m_InputServer.IsListening)
            {
                try
                {
                    var context = m_InputServer.BeginGetContext(new AsyncCallback(InputListenerCallback), m_InputServer);
                    context.AsyncWaitHandle.WaitOne();
                }
                catch { }
            }
        }

        private void InputListenerCallback(IAsyncResult ar)
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
                Logger.Error("HTTP context error: " + ex.Message);
                return;
            }

            bool isValidAddress = false; // must set whitelist ...
            if (m_InputWhiteList.Count > 0 && remoteIp.Length > 0)
            {
                lock (m_InputWhiteList)
                {
                    foreach (string pattern in m_InputWhiteList)
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
                if (context != null && remoteIp.Length > 0 && isValidAddress && context.Request.Url.Segments.Length > 1)
                {
                    //Logger.Info("Accepted Input from [" + remoteIp + "] - " + context.Request.Url.ToString());
                    Thread thread = new Thread(ProcessInputData);
                    thread.Start(context);
                }
                else if (context != null)
                {
                    context.Response.Abort(); // make sure the connection is closed
                }
            }
            catch (Exception ex)
            {
                Logger.Error("HTTP process error: " + ex.Message);

                try
                {
                    if (context != null) context.Response.Abort(); // make sure the connection is closed
                }
                catch { }
            }
        }

        protected virtual void ProcessInputData(object obj)
        {
            HttpListenerContext ctx = obj as HttpListenerContext;
            if (ctx == null) return;

            string remoteIp = "";
            string sourceUrl = "";
            try
            {
                remoteIp = ctx.Request.RemoteEndPoint.Address.ToString();
                sourceUrl = "[" + remoteIp + "] - " + ctx.Request.Url.ToString();
            }
            catch (Exception ex)
            {
                Logger.Error("HTTP context error: " + ex.Message);
            }

            IMediaHandler handler = null;
            List<MediaChannel> channels = new List<MediaChannel>();

            var urlSegments = ctx.Request.Url.Segments;

            string channelNames = "";
            if (urlSegments.Length > 1) channelNames = urlSegments[1].Replace("/", "");

            if (channelNames.Length > 0)
            {
                var names = channelNames.Split(',');
                foreach (var channelName in names)
                {
                    var channel = GetChannel(channelName.Trim());

                    if (channel != null) channel.AddInputUrl(sourceUrl);
                    if (channel != null)
                    {
                        var srcUrls = channel.GetInputUrls();
                        Logger.Info("Channel #" + channelName + "# input source updated >> ");
                        foreach (var srcUrl in srcUrls) Logger.Info("#" + channelName + "# - " + srcUrl);
                    }

                    if (channel != null && channel.IsWorking() == false) channel.Start();
                    if (channel != null) channels.Add(channel);

                    Thread.Sleep(50);
                }
            }

            if (channels.Count > 0)
            {
                string typeinfo = "";
                if (urlSegments.Length > 2) typeinfo = urlSegments[2].Replace("/", "");
                if (typeinfo.Length > 0) handler = m_ResourceManager.GetHandler(typeinfo);
            }

            if (channels.Count > 0 && handler != null)
            {
                string mediainfo = "";
                if (urlSegments.Length > 3) mediainfo = urlSegments[3].Replace("/", "");
                if (mediainfo.Length > 0) mediainfo = handler.GetMediaType() + "(" + mediainfo + ")";
                else mediainfo = handler.GetMediaType();

                foreach (var channel in channels)
                {
                    MediaChannelState state = new MediaChannelState();
                    state.ChannelName = channel.ChannelName;
                    state.AddressInfo = ctx.Request.RemoteEndPoint.ToString();
                    state.MediaInfo = mediainfo;
                    state.ClientCount = 0;

                    UpdateState(channel.ChannelName, state);
                }

                // should ensure there is at least a try-catch block inside the process function
                handler.HandleInput(this, channels, ctx.Request.InputStream, mediainfo);

                foreach (var channel in channels)
                {
                    //channel.SetWelcomeText("");
                    channel.RemoveWelcomeText(mediainfo);
                    channel.SetWelcomeData(new byte[0]);

                    channel.RemoveInputUrl(sourceUrl);
                    if (channel != null)
                    {
                        var srcUrls = channel.GetInputUrls();
                        if (srcUrls.Count > 0)
                        {
                            Logger.Info("Channel #" + channel.ChannelName + "# input source updated >> ");
                            foreach (var srcUrl in srcUrls) Logger.Info("#" + channel.ChannelName + "# - " + srcUrl);
                        }
                        else
                        {
                            Logger.Info("Channel #" + channel.ChannelName + "# input source dropped.");
                        }
                        
                    }

                    RemoveState(channel.ChannelName);

                    Thread.Sleep(50);
                }
            }

            try
            {
                if (ctx != null) ctx.Response.Abort(); // make sure the connection is closed
            }
            catch { }
        }

        public void Stop()
        {
            try
            {
                if (m_InputServer != null && m_InputServer.IsListening)
                {
                    m_InputServer.Stop();
                    if (m_InputListenerThread != null)
                    {
                        m_InputListenerThread.Join();
                        m_InputListenerThread = null;
                    }
                    m_InputServer.Close();
                }
                if (m_InputListenerThread != null)
                {
                    m_InputListenerThread.Join();
                    m_InputListenerThread = null;
                }
                m_InputServer = null;
            }
            catch { }

            try
            {
                if (m_OutputServer != null)
                {
                    m_OutputServer.ActualServer.Stop();
                }
                m_OutputServer = null;
            }
            catch { }

            lock (m_WebClients)
            {
                foreach (var item in m_WebClients)
                {
                    List<object> clients = item.Value;
                    lock (clients)
                    {
                        clients.Clear();
                    }
                }
                m_WebClients.Clear();
            }

            StopAutoUpdaeStates();
        }

        public virtual bool IsWorking()
        {
            return (m_InputServer != null && m_InputServer.IsListening)
                || (m_OutputServer != null && m_OutputServer.ActualServer.GetState() > 0);
        }

    }
}
