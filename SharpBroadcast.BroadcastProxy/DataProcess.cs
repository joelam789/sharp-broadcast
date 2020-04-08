using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using SharpNetwork.Core;

namespace SharpBroadcast.BroadcastProxy
{
    public class HttpMessage
    {
        public const int WEB_MSG_BUF_CODE = -1;
        public const int WEB_MSG_DATA_CODE = -2;
        public const int WEB_MSG_TASK_CODE = -3;

        public const int STATE_WAIT_FOR_HEADER = 0;
        public const int STATE_WAIT_FOR_BODY = 1;
        public const int STATE_READY = 2;

        public const int MSG_TYPE_UNDEFINED = 0;
        public const int MSG_TYPE_HTTP_HEADER = 1;
        public const int MSG_TYPE_STRING = 2;
        public const int MSG_TYPE_BINARY = 3;

        public const string MSG_KEY_HTTP_HEADER = "RequestHeader";
        public const string MSG_KEY_CHANNEL = "RequestChannel";
        public const string MSG_KEY_PATH = "RequestPath";
        

        public enum HttpStatusCode
        {
            // for a full list of status codes, see..
            // https://en.wikipedia.org/wiki/List_of_HTTP_status_codes

            Continue = 100,

            Ok = 200,
            Created = 201,
            Accepted = 202,
            MovedPermanently = 301,
            Found = 302,
            NotModified = 304,
            BadRequest = 400,
            Forbidden = 403,
            NotFound = 404,
            MethodNotAllowed = 405,
            InternalServerError = 500
        }

        public int ReceivingState { get; set; }

        public int ContentSize { get; set; }
        public int MessageType { get; set; }

        //public Uri Url { get; set; }

        public string RequestUrl { get; set; }
        public string RequestMethod { get; set; }

        public int StatusCode { get; set; }
        public string ReasonPhrase { get; set; }

        public string ProtocolVersion { get; set; }

        public string MessageContent { get; set; }
        public byte[] RawContent { get; set; }

        public HttpMessage()
        {
            ReceivingState = STATE_WAIT_FOR_HEADER;

            ContentSize = 0;
            MessageType = 0;

            StatusCode = 200;
            ReasonPhrase = "OK";

            ProtocolVersion = "";

            RequestUrl = "/";
            RequestMethod = "";

            MessageContent = "";
            RawContent = null;
        }

        public HttpMessage(string msgContent)
            : this()
        {
            MessageType = MSG_TYPE_STRING;
            MessageContent = msgContent;

            ProtocolVersion = "HTTP/1.0";
        }

        public HttpMessage(string reqUrl, string reqMethod, string msgContent)
            : this(msgContent)
        {
            RequestUrl = reqUrl;
            RequestMethod = reqMethod;
        }

        public HttpMessage(int statusCode, string reasonPhrase, string msgContent)
            : this(msgContent)
        {
            StatusCode = statusCode;
            ReasonPhrase = reasonPhrase;
        }

        public HttpMessage(byte[] msgContent)
            : this()
        {
            MessageType = MSG_TYPE_BINARY;
            RawContent = msgContent;
            ContentSize = msgContent.Length;

            ProtocolVersion = "HTTP/1.0";
        }

        public HttpMessage(byte[] msgContent, int msgSize)
            : this(msgContent)
        {
            ContentSize = msgSize;
        }

        public HttpMessage(int statusCode, string reasonPhrase, byte[] msgContent)
            : this(msgContent)
        {
            StatusCode = statusCode;
            ReasonPhrase = reasonPhrase;
        }

        public bool IsString()
        {
            return MessageType == MSG_TYPE_STRING;
        }

        public bool IsBinary()
        {
            return MessageType == MSG_TYPE_BINARY;
        }

        public static Stack<Object> GetSessionBuffer(Session session, bool needCheck = false)
        {
            Stack<Object> result = null;
            Dictionary<int, object> attrMap = session.GetAttributes();

            if (needCheck)
            {
                lock (attrMap)
                {
                    if (attrMap.ContainsKey(WEB_MSG_BUF_CODE))
                    {
                        result = attrMap[WEB_MSG_BUF_CODE] as Stack<Object>;
                        if (result == null) attrMap.Remove(WEB_MSG_BUF_CODE);
                    }
                    if (result == null)
                    {
                        result = new Stack<Object>();
                        attrMap.Add(WEB_MSG_BUF_CODE, result);
                    }
                }
            }
            else
            {
                result = attrMap[WEB_MSG_BUF_CODE] as Stack<Object>;
            }

            return result;
        }

        public static Dictionary<string, object> GetSessionData(Session session, bool needCheck = false)
        {
            Dictionary<string, object> result = null;
            Dictionary<int, object> attrMap = session.GetAttributes();

            if (needCheck)
            {
                lock (attrMap)
                {
                    if (attrMap.ContainsKey(WEB_MSG_DATA_CODE))
                    {
                        result = attrMap[WEB_MSG_DATA_CODE] as Dictionary<string, object>;
                        if (result == null) attrMap.Remove(WEB_MSG_DATA_CODE);
                    }
                    if (result == null)
                    {
                        result = new Dictionary<string, object>();
                        attrMap.Add(WEB_MSG_DATA_CODE, result);
                    }
                }
            }
            else
            {
                result = attrMap[WEB_MSG_DATA_CODE] as Dictionary<string, object>;
            }

            return result;
        }

        public static void SetSessionData(Session session, string dataName, object dataValue)
        {
            Dictionary<string, object> dataMap = GetSessionData(session);
            if (dataMap.ContainsKey(dataName)) dataMap.Remove(dataName);
            dataMap.Add(dataName, dataValue);
        }

        public static object GetSessionData(Session session, string dataName)
        {
            Dictionary<string, object> dataMap = GetSessionData(session);
            if (dataMap.ContainsKey(dataName)) return dataMap[dataName];
            else return null;
        }

        public static void Send(Session session, string str)
        {
            var msg = new HttpMessage(str);
            session.Send(msg);
        }


    }


    public class ServerDataCodec : INetworkFilter
    {
        private int m_MaxMsgSize = 1024 * 1024 * 8; // 8M

        public void Encode(Session session, Object message, MemoryStream stream)
        {
            if (message is MemoryStream)
            {
                MemoryStream msg = message as MemoryStream;
                msg.WriteTo(stream);
            }
        }

        private string GetChannelFromPath(string path)
        {
            var parts = path.Split('/');

            if (parts != null && parts.Length > 0)
            {
                foreach (var part in parts)
                {
                    var item = part.Trim();
                    if (item.Length > 0) return item;
                }
            }

            return "";
        }

        // please know that function Decode() should be called in single thread
        public bool Decode(Session session, MemoryStream stream, List<Object> output)
        {
            bool isNew = false;
            HttpMessage netMsg = null;

            bool hasKey = false;
            Stack<Object> stack = HttpMessage.GetSessionBuffer(session);
            if (stack.Count > 0)
            {
                hasKey = true;
                netMsg = (HttpMessage)stack.Peek();
            }
            if (netMsg == null)
            {
                isNew = true;
                netMsg = new HttpMessage();
                //netMsg.MessageType = HttpMessage.MSG_TYPE_STRING;
                if (hasKey) stack.Pop();
                stack.Push(netMsg);
            }

            if (isNew)
            {
                if (netMsg != null)
                {
                    netMsg.ReceivingState = HttpMessage.STATE_WAIT_FOR_BODY;
                    netMsg.MessageType = HttpMessage.MSG_TYPE_HTTP_HEADER;
                }
            }

            int total = 0;

            if (netMsg.ReceivingState == HttpMessage.STATE_WAIT_FOR_BODY)
            {
                if (netMsg.MessageType == HttpMessage.MSG_TYPE_HTTP_HEADER)
                {
                    long orgpos = stream.Position;
                    long msglen = stream.Length - stream.Position;

                    Byte[] bytes = new Byte[msglen];
                    stream.Read(bytes, 0, bytes.Length);

                    bool found = false;

                    int curpos = 0;
                    int maxpos = bytes.Length - 1;
                    int checkedlen = 0;
                    while (curpos <= maxpos && !found)
                    {
                        if (bytes[curpos] == '\r')
                        {
                            if (curpos + 1 <= maxpos && bytes[curpos + 1] == '\n')
                            {
                                if (curpos + 2 <= maxpos && bytes[curpos + 2] == '\r')
                                {
                                    if (curpos + 3 <= maxpos && bytes[curpos + 3] == '\n')
                                    {
                                        found = true;
                                        checkedlen = curpos + 3 + 1;
                                    }
                                }
                            }
                        }

                        curpos++;
                    }

                    if (found)
                    {
                        Encoding encode = Encoding.UTF8;
                        string headerContent = encode.GetString(bytes, 0, checkedlen);

                        HttpMessage.SetSessionData(session, HttpMessage.MSG_KEY_HTTP_HEADER, headerContent.Substring(0, headerContent.Length - 2));

                        string[] headerLines = headerContent.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < headerLines.Length; i++)
                        {
                            var line = headerLines[i];
                            if (i == 0)
                            {
                                string[] tokens = line.Trim().Split(' ');
                                if (tokens.Length != 3)
                                {
                                    //throw new Exception("invalid http request line");
                                    session.Close();
                                    return false;
                                }
                                netMsg.RequestMethod = tokens[0].ToUpper();
                                //netMsg.Url = new Uri(tokens[1], UriKind.Absolute);
                                //netMsg.RequestUrl = netMsg.Url.LocalPath;
                                netMsg.RequestUrl = Uri.UnescapeDataString(tokens[1]); // "HttpUtility.UrlDecode()" should be better... but need System.Web
                                netMsg.ProtocolVersion = tokens[2];

                                HttpMessage.SetSessionData(session, HttpMessage.MSG_KEY_PATH, netMsg.RequestUrl);
                                HttpMessage.SetSessionData(session, HttpMessage.MSG_KEY_CHANNEL, GetChannelFromPath(netMsg.RequestUrl));

                                break;
                            }
                        }

                        stream.Position = orgpos + checkedlen - 2;

                        netMsg.ReceivingState = HttpMessage.STATE_READY;
                        if (stack.Count > 0) stack.Pop();

                        netMsg = new HttpMessage();
                        netMsg.MessageType = HttpMessage.MSG_TYPE_BINARY;
                        netMsg.ReceivingState = HttpMessage.STATE_WAIT_FOR_HEADER;
                        stack.Push(netMsg);
                    }
                    else
                    {
                        if (curpos > m_MaxMsgSize) session.Close();
                        else stream.Position = orgpos;
                        return false;
                    }
                }

                if (netMsg.ReceivingState == HttpMessage.STATE_WAIT_FOR_BODY
                    && stream.Length - stream.Position >= netMsg.ContentSize)
                {
                    Byte[] bytes = new Byte[netMsg.ContentSize];
                    stream.Read(bytes, 0, netMsg.ContentSize);

                    if (netMsg.MessageType == HttpMessage.MSG_TYPE_BINARY)
                    {
                        netMsg.RawContent = bytes;
                        netMsg.ContentSize = bytes.Length;
                    }

                    output.Add(netMsg);
                    total++;

                    netMsg.ReceivingState = HttpMessage.STATE_READY;
                    if (stack.Count > 0) stack.Pop();

                    netMsg = new HttpMessage();
                    netMsg.MessageType = HttpMessage.MSG_TYPE_BINARY;
                    netMsg.ReceivingState = HttpMessage.STATE_WAIT_FOR_HEADER;
                    stack.Push(netMsg);
                }

            }

            while (netMsg.MessageType == HttpMessage.MSG_TYPE_BINARY 
                    && netMsg.ReceivingState == HttpMessage.STATE_WAIT_FOR_HEADER)
            {
                long orgpos = stream.Position;
                long msglen = stream.Length - stream.Position;

                Byte[] bytes = new Byte[msglen];
                stream.Read(bytes, 0, bytes.Length);

                bool found = false;
                bool foundFirst = false;
                bool foundSecond = false;

                string hexstr = "";

                int curpos = 0;
                int maxpos = bytes.Length - 1;
                //int checkedlen = 0;
                while (curpos <= maxpos && !found)
                {
                    if (bytes[curpos] == '\r')
                    {
                        if (curpos + 1 <= maxpos && bytes[curpos + 1] == '\n')
                        {
                            if (!foundFirst)
                            {
                                curpos++;
                                foundFirst = true;
                            }
                            else if (!foundSecond)
                            {
                                curpos++;
                                foundSecond = true;
                            }

                            if (foundFirst && foundSecond) found = true;
                        }
                    }
                    else
                    {
                        if (foundFirst && !foundSecond) hexstr += Convert.ToChar(bytes[curpos]);
                    }

                    curpos++;
                }

                if (found)
                {
                    hexstr = hexstr.Trim();
                    //CommonLog.Info("Chunk Length: 0x" + hexstr);
                    netMsg.ContentSize = Convert.ToInt32("0x" + hexstr, 16);

                    netMsg.ReceivingState = HttpMessage.STATE_WAIT_FOR_BODY;

                    stream.Position = orgpos + curpos;
                }
                else
                {
                    if (curpos > m_MaxMsgSize) session.Close();
                    else stream.Position = orgpos;
                    return false;
                }

                if (netMsg.ReceivingState == HttpMessage.STATE_WAIT_FOR_BODY)
                {
                    if (stream.Length - stream.Position >= netMsg.ContentSize)
                    {
                        Byte[] dataBytes = new Byte[netMsg.ContentSize];
                        stream.Read(dataBytes, 0, netMsg.ContentSize);

                        netMsg.RawContent = dataBytes;
                        netMsg.ReceivingState = HttpMessage.STATE_READY;

                        output.Add(netMsg);
                        total++;

                        netMsg = new HttpMessage();
                        netMsg.MessageType = HttpMessage.MSG_TYPE_BINARY;
                        netMsg.ReceivingState = HttpMessage.STATE_WAIT_FOR_HEADER;
                        if (stack.Count > 0) stack.Pop();
                        stack.Push(netMsg);
                        continue;
                    }

                }
            }

            if (total > 0 && stream.Length - stream.Position <= 0) return true;

            if (netMsg.ReceivingState != HttpMessage.STATE_WAIT_FOR_HEADER
                    && netMsg.ReceivingState != HttpMessage.STATE_WAIT_FOR_BODY
                    && netMsg.ReceivingState != HttpMessage.STATE_READY)
            {
                session.Close();
            }

            return false;
        }
    }

    public class ClientDataCodec : INetworkFilter
    {
        public const int CR = 13; // <US-ASCII CR, carriage return (13)>
        public const int LF = 10; // <US-ASCII LF, linefeed (10)>

        public void Encode(Session session, Object message, MemoryStream stream)
        {
            if (message is HttpMessage)
            {
                HttpMessage msg = message as HttpMessage;
                if (msg.IsString())
                {
                    Encoding encode = Encoding.UTF8;
                    Byte[] bytes = null;

                    if (msg.MessageContent.Length > 0)
                        bytes = encode.GetBytes(msg.MessageContent);

                    if (bytes == null) bytes = new byte[0];

                    BinaryWriter writer = new BinaryWriter(stream);

                    stream.Write(bytes, 0, bytes.Length);
                }
                else if (msg.IsBinary())
                {
                    Encoding encode = Encoding.UTF8;
                    Byte[] bytes = msg.RawContent;
                    if (bytes == null) bytes = new byte[0];

                    int len = bytes.Length;
                    string hexValue = len.ToString("X");
                    var hexBytes = encode.GetBytes(hexValue);

                    BinaryWriter writer = new BinaryWriter(stream);

                    stream.WriteByte(CR & 0xFF);
                    stream.WriteByte(LF & 0xFF);
                    stream.Write(hexBytes, 0, hexBytes.Length);
                    stream.WriteByte(CR & 0xFF);
                    stream.WriteByte(LF & 0xFF);
                    stream.Write(bytes, 0, bytes.Length);
                }
            }
        }

        // please know that function Decode() should be called in single thread
        public bool Decode(Session session, MemoryStream stream, List<Object> output)
        {
            if (stream != null && stream.Length > 0)
            {
                MemoryStream outputStream = new MemoryStream();
                stream.WriteTo(outputStream);
                output.Add(outputStream);
            }
            return true;
        }
    }

    public interface IBroadcastServer
    {
        void ProcessIncomingData(Session session, HttpMessage msg);

        void ReconnectTargets(string channel);

        List<string> GetSourceWhitelist();
    }

    public class ClientNetworkEventHandler : CommonNetworkEventHandler
    {
        IBroadcastServer m_Server = null;

        //Random m_Random = new Random();
        //string m_DataFileName = "";

        public ClientNetworkEventHandler(IBroadcastServer server)
        {
            m_Server = server;
        }

        public override void OnConnect(Session session)
        {
            base.OnConnect(session);
            session.FitReadQueueAction = Session.ACT_KEEP_OLD;
            session.FitWriteQueueAction = Session.ACT_KEEP_OLD;

            if (session != null) session.UserData = null;

            //m_DataFileName = AppDomain.CurrentDomain.BaseDirectory + "/"
            //                    + DateTime.Now.ToString("yyMMddHHmmss") + "_" + session.GetId() + "_" + m_Random.Next(10, 100) + ".data";
        }

        public override void OnDisconnect(Session session)
        {
            if (session != null) session.UserData = null;
            base.OnDisconnect(session);
        }

        public override int OnReceive(Session session, Object data)
        {
            MemoryStream stream = data as MemoryStream;

            /*
            if (stream != null)
            using (var file = File.Exists(m_DataFileName) ? File.Open(m_DataFileName, FileMode.Append) 
                                : File.Open(m_DataFileName, FileMode.CreateNew))
            {
                using (var bw = new BinaryWriter(file))
                {
                    MemoryStream content = new MemoryStream();
                    stream.WriteTo(content);
                    var bytes = content.GetBuffer();
                    bw.Write(bytes);
                }
            }
            */

            return base.OnReceive(session, data);
        }
    }

    public class ServerNetworkEventHandler : CommonNetworkEventHandler
    {
        IBroadcastServer m_Server = null;

        //Random m_Random = new Random();
        //string m_DataFileName = "";

        public ServerNetworkEventHandler(IBroadcastServer server)
        {
            m_Server = server;
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

        public override void OnConnect(Session session)
        {
            base.OnConnect(session);

            var remoteIp = session.GetRemoteIp();
            var inputWhiteList = m_Server.GetSourceWhitelist();

            bool isValidAddress = false; // must set whitelist ...
            if (inputWhiteList.Count > 0 && remoteIp.Length > 0)
            {
                foreach (string pattern in inputWhiteList)
                {
                    if (Match(pattern, remoteIp))
                    {
                        isValidAddress = true;
                        break;
                    }
                }
            }

            if (!isValidAddress)
            {
                CommonLog.Error("Stream source IP is not on white list: " + remoteIp);
                session.Close();
                return;
            }

            session.FitReadQueueAction = Session.ACT_KEEP_OLD;
            session.FitWriteQueueAction = Session.ACT_KEEP_OLD;

            //m_DataFileName = AppDomain.CurrentDomain.BaseDirectory + "/" 
            //                    + DateTime.Now.ToString("yyMMddHHmmss") + "_" + m_Random.Next(10, 100) + ".data";

            HttpMessage.GetSessionBuffer(session, true);
            HttpMessage.GetSessionData(session, true);

            HttpMessage.SetSessionData(session, HttpMessage.MSG_KEY_HTTP_HEADER, "");
            HttpMessage.SetSessionData(session, HttpMessage.MSG_KEY_CHANNEL, "");
            HttpMessage.SetSessionData(session, HttpMessage.MSG_KEY_PATH, "");
        }

        public override void OnDisconnect(Session session)
        {
            if (m_Server != null)
            {
                string channel = session == null ? "" : HttpMessage.GetSessionData(session, HttpMessage.MSG_KEY_CHANNEL).ToString();
                if (channel != null && channel.Length > 0) m_Server.ReconnectTargets(channel);
            }
            base.OnDisconnect(session);
        }

        public override void OnIdle(Session session, Int32 optype)
        {
            if (session != null && optype == Session.IO_RECEIVE) session.Close(false);
            else base.OnIdle(session, optype);
        }

        public override int OnReceive(Session session, Object data)
        {
            HttpMessage msg = data as HttpMessage;
            if (msg != null && m_Server != null) m_Server.ProcessIncomingData(session, msg);

            /*
            using (var file = File.Exists(m_DataFileName) ? File.Open(m_DataFileName, FileMode.Append) 
                                : File.Open(m_DataFileName, FileMode.CreateNew))
            {
                using (var bw = new BinaryWriter(file))
                {
                    MemoryStream content = new MemoryStream();
                    stream.WriteTo(content);
                    var bytes = content.GetBuffer();
                    bw.Write(bytes);
                }
            }
            */

            return base.OnReceive(session, data);
        }
    }

    public class BroadcastServer: IBroadcastServer
    {
        Server m_Server = new Server();
        Dictionary<string, Dictionary<string, Client>> m_Targets = new Dictionary<string, Dictionary<string, Client>>();

        Timer m_Timer = null;
        int m_CheckInterval = 3; // in seconds
        int m_MaxRecvIdleSeconds = 0;

        List<string> m_InputWhiteList = new List<string>();

        int m_Port = 9810;

        public BroadcastServer(int port, int maxRecvIdleSeconds = 0, int checkInterval = 0)
        {
            m_Server.SetIoFilter(new ServerDataCodec());
            m_Server.SetIoHandler(new ServerNetworkEventHandler(this));

            if (port > 0) m_Port = port;
            if (checkInterval > 0) m_CheckInterval = checkInterval;
            if (maxRecvIdleSeconds > 0) m_MaxRecvIdleSeconds = maxRecvIdleSeconds;
        }

        public void LoadTargets(Dictionary<string, List<string>> urlsMap)
        {
            var oldTargets = m_Targets;

            foreach (var item in oldTargets)
            {
                foreach (var subItem in item.Value)
                {
                    try
                    {
                        if (subItem.Value != null)
                        {
                            subItem.Value.SetClientId(0);
                            subItem.Value.Disconnect(true);
                        }
                    }
                    catch { }
                }
            }

            var newTargets = new Dictionary<string, Dictionary<string, Client>>();

            int count = 0;
            var paths = urlsMap.Keys;
            foreach (string path in paths)
            {
                Dictionary<string, Client> clients = null;
                if (newTargets.ContainsKey(path)) clients = newTargets[path];
                else
                {
                    clients = new Dictionary<string, Client>();
                    newTargets.Add(path, clients);
                }

                var urls = urlsMap[path];
                foreach (var url in urls)
                {
                    try
                    {
                        if (url != null && url.Length > 0)
                        {
                            count++;
                            var address = url.StartsWith("http://") ? url : "http://" + url;
                            var uri = new Uri(address, UriKind.Absolute);
                            var client = new Client();
                            client.SetClientId(count);
                            client.SetIoFilter(new ClientDataCodec());
                            client.SetIoHandler(new ClientNetworkEventHandler(this));
                            client.Connect(uri.Host, uri.Port);
                            clients.Add(url, client);
                        }
                    }
                    catch (Exception ex)
                    {
                        CommonLog.Error("Failed to load target URL: " + m_Port + "/" + path + " => " + url);
                        CommonLog.Error(ex.ToString());
                    }
                }
                
            }

            m_Targets = newTargets;
        }

        public void ProcessIncomingData(Session session, HttpMessage msg)
        {
            //CommonLog.Info("stream length: " + stream.Length);

            string httpHeader = null;

            var currentTargets = m_Targets;

            string channel = session == null ? "" : HttpMessage.GetSessionData(session, HttpMessage.MSG_KEY_CHANNEL).ToString();
            if (channel == null || channel.Length <= 0)
            {
                if (session != null) session.Close(false);
                return;
            }

            var targets = currentTargets.ContainsKey(channel) ? currentTargets[channel] : null;
            if (targets == null || targets.Count <= 0)
            {
                if (session != null) session.Close(false);
                return;
            }

            foreach (var item in targets)
            {
                try
                {
                    if (item.Value != null && item.Value.GetClientId() > 0)
                    {
                        var clientSession = item.Value.GetSession();
                        if (clientSession != null && clientSession.UserData == null)
                        {
                            if (httpHeader == null)
                            {
                                if (session == null) httpHeader = "";
                                else httpHeader = HttpMessage.GetSessionData(session, HttpMessage.MSG_KEY_HTTP_HEADER).ToString();
                            }
                            if (httpHeader != null && httpHeader.Length > 0)
                            {
                                item.Value.Send(new HttpMessage(httpHeader));
                                clientSession.UserData = "header";
                            }
                        }
                        item.Value.Send(msg);
                    }
                }
                catch { }
            }
        }

        public void ReconnectTargets(string channel)
        {
            var currentTargets = m_Targets;
            if (channel == null || channel.Length <= 0) return;

            var targets = currentTargets.ContainsKey(channel) ? currentTargets[channel] : null;
            if (targets == null || targets.Count <= 0) return;

            foreach (var item in targets)
            {
                try
                {
                    if (item.Value != null)
                    {
                        item.Value.Disconnect(true);
                    }
                }
                catch { }
            }
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

        public bool Start(List<string> inputWhiteList = null)
        {
            Stop();

            lock (m_InputWhiteList)
            {
                if (inputWhiteList != null)
                {
                    m_InputWhiteList.Clear();
                    m_InputWhiteList.AddRange(inputWhiteList);
                }
                if (m_InputWhiteList.Count <= 0) m_InputWhiteList.Add("127.0.0.1");
            }

            bool isServerOK = false;

            try
            {
                if (m_Server != null)
                {
                    if (m_MaxRecvIdleSeconds > 0) m_Server.SetIdleTime(Session.IO_RECEIVE, m_MaxRecvIdleSeconds);
                    m_Server.Start(m_Port);
                    isServerOK = true;
                }
            }
            catch (Exception ex)
            {
                CommonLog.Error("Failed to listen on port: " + m_Port);
                CommonLog.Error(ex.ToString());
            }

            m_Timer = new Timer(TryToKeepTargetsOnline, m_Targets, 1000, 1000 * m_CheckInterval);

            return isServerOK;
        }

        public void Stop()
        {
            try
            {
                if (m_Timer != null)
                {
                    m_Timer.Dispose();
                    m_Timer = null;
                }
            }
            catch { }

            try
            {
                if (m_Server != null)
                {
                    m_Server.Stop();
                }
            }
            catch { }
        }

        private void TryToKeepTargetsOnline(Object config)
        {
            var targets = m_Targets;

            foreach (var mainItem in targets)
            {
                foreach (var item in mainItem.Value)
                {
                    try
                    {
                        if (item.Key != null && item.Key.Length > 0
                            && item.Value != null && item.Value.GetClientId() > 0)
                        {
                            if (item.Value.GetState() < 0)
                            {
                                var address = item.Key.StartsWith("http://") ? item.Key : "http://" + item.Key;
                                var uri = new Uri(address, UriKind.Absolute);
                                item.Value.Connect(uri.Host, uri.Port);
                            }
                        }
                    }
                    catch { }
                }
            }
        }
    }

}
