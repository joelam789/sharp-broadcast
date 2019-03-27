using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpBroadcast.Framework
{
    public class MediaChannel
    {
        protected List<byte> m_WelcomeData = new List<byte>();
        protected string m_WelcomeText = "";

        protected int m_MaxInputQueueSize = 0;

        //protected IServerLogger m_Logger = null;

        protected List<string> m_InputUrls = new List<string>();

        public string ChannelName { get; private set; }

        public MediaDispatcher Dispatcher { get; private set; }

        public List<ChannelServerItem> OutputList { get; private set; }

        public MediaChannel(string channelName)
        {
            ChannelName = channelName;
            OutputList = new List<ChannelServerItem>();
            Dispatcher = new MediaDispatcher(OutputList);
        }

        public void AddInputUrl(string url)
        {
            if (String.IsNullOrEmpty(url)) return;
            lock (m_InputUrls)
            {
                if (!m_InputUrls.Contains(url)) m_InputUrls.Add(url);
            }
        }

        public void RemoveInputUrl(string url)
        {
            if (String.IsNullOrEmpty(url)) return;
            lock (m_InputUrls)
            {
                if (m_InputUrls.Contains(url)) m_InputUrls.Remove(url);
                if (m_InputUrls.Count <= 0) // clear buffers
                {
                    BufferData item;
                    while (Dispatcher.Buffers.TryDequeue(out item))
                    {
                        // do nothing
                    }
                    //if (Dispatcher.Buffers.Count <= 0 && m_Logger != null) m_Logger.Info("Channel #" + ChannelName + "# buffer cleared ");
                }
            }
        }

        public List<string> GetInputUrls()
        {
            List<string> urls = new List<string>();
            lock (m_InputUrls)
            {
                urls.AddRange(m_InputUrls);
            }
            return urls;
        }

        public bool AddServer(IMediaServer server)
        {
            if (Dispatcher.IsWorking()) return false; // coz we will not lock OutputList

            if (server != null)
            {
                //m_Logger = server.Logger;

                int inputQueueLength = server.GetChannelInputQueueLength(ChannelName);
                if (inputQueueLength > 0)
                {
                    m_MaxInputQueueSize = inputQueueLength;
                    //server.Logger.Info("Max InputQueueLength of [" + ChannelName + "] is set to " + m_MaxInputQueueSize);
                }
                else
                {
                    inputQueueLength = server.InputQueueSize;
                    if (inputQueueLength > 0)
                    {
                        if (m_MaxInputQueueSize <= 0)
                        {
                            m_MaxInputQueueSize = server.InputQueueSize;
                            //server.Logger.Info("Max InputQueueLength of [" + ChannelName + "] is set to " + m_MaxInputQueueSize);
                        }
                        else
                        {
                            if (m_MaxInputQueueSize > server.InputQueueSize)
                            {
                                m_MaxInputQueueSize = server.InputQueueSize; // just get the small one
                                //server.Logger.Info("Max InputQueueLength of [" + ChannelName + "] is set to " + m_MaxInputQueueSize);
                            }
                        }
                    }
                }
            }
            OutputList.Add(new ChannelServerItem(server, server.GetClients(ChannelName)));
            return true;
        }

        public void Process(BufferData buffer)
        {
            if (!Dispatcher.IsWorking()) return;
            if (m_MaxInputQueueSize <= 0 || Dispatcher.Buffers.Count <= m_MaxInputQueueSize)
            {
                Dispatcher.Buffers.Enqueue(buffer);
                Dispatcher.Process();
            }
        }

        public void Start()
        {
            Dispatcher.Start();
        }

        public void Stop()
        {
            Dispatcher.Stop();
        }

        public bool IsWorking()
        {
            return Dispatcher.IsWorking();
        }

        public byte[] GetWelcomeData()
        {
            byte[] result = null;
            if (m_WelcomeData.Count <= 0) return result;
            lock (m_WelcomeData)
            {
                result = m_WelcomeData.ToArray();
            }
            return result;
        }

        public void SetWelcomeData(byte[] data)
        {
            if (data == null) return;
            lock (m_WelcomeData)
            {
                m_WelcomeData.Clear();
                m_WelcomeData.AddRange(data);
            }
        }

        public string GetWelcomeText()
        {
            string result = "";
            if (m_WelcomeText == null || m_WelcomeText.Length <= 0) return result;
            lock (m_WelcomeText)
            {
                result = String.Copy(m_WelcomeText);
            }
            return result;
        }

        public void SetWelcomeText(string text)
        {
            if (text == null) return;
            lock (m_WelcomeText)
            {
                if (text.Length > 0 && m_WelcomeText != null && m_WelcomeText.Length > 0) m_WelcomeText += "|" + text;
                else m_WelcomeText = String.Copy(text);
            }
        }

        public void RemoveWelcomeText(string text)
        {
            if (text == null || text.Length <= 0) return;
            lock (m_WelcomeText)
            {
                string removing = text.Trim();
                if (removing.Length > 0 && m_WelcomeText != null && m_WelcomeText.Length > 0)
                {
                    bool found = false;
                    string newtext = "";
                    var lines = m_WelcomeText.Split('|');
                    foreach (var line in lines)
                    {
                        if (!found && line == removing)
                        {
                            found = true;
                            continue;
                        }
                        else
                        {
                            if (newtext.Length > 0) newtext += "|" + line;
                            else newtext = line;
                        }
                    }
                    m_WelcomeText = newtext;
                }
            }
        }
    }

    public class ChannelServerItem
    {
        public IMediaServer Server { get; private set; }
        public List<object> Clients { get; private set; }

        public ChannelServerItem(IMediaServer server, List<object> clients)
        {
            Server = server;
            Clients = clients;
        }
    }

    public class ChannelClientItem
    {
        public IMediaServer Server { get; private set; }
        public object Client { get; private set; }

        public ChannelClientItem(IMediaServer server, object client)
        {
            Server = server;
            Client = client;
        }
    }


    public class MediaChannelState
    {
        public string ChannelName { get; set; }

        public string AddressInfo { get; set; }

        public string ServerInfo { get; set; }

        public string MediaInfo { get; set; }

        public int ClientCount { get; set; }

        public int SourceCount { get; set; }

        public MediaChannelState()
        {
            ChannelName = "";
            AddressInfo = "";
            ServerInfo = "";
            MediaInfo = "";
            ClientCount = 0;
            SourceCount = 0;
        }

        public MediaChannelState(MediaChannelState src)
        {
            ChannelName = src.ChannelName;
            AddressInfo = src.AddressInfo;
            ServerInfo = src.ServerInfo;
            MediaInfo = src.MediaInfo;
            ClientCount = src.ClientCount;
            SourceCount = src.SourceCount;
        }
    }
}
